#version 330 core

in vec2 vTexCoord;
in vec4 vColor; // Base color from C# (XPOrbType, etc.)
out vec4 FragColor;

uniform float time;

// 2D simplex noise for organic, flowing energy effects
vec3 mod289(vec3 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
vec2 mod289(vec2 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
vec3 permute(vec3 x) { return mod289(((x*34.0)+1.0)*x); }

float snoise(vec2 v) {
    const vec4 C = vec4(0.211324865405187,  // (3.0-sqrt(3.0))/6.0
                        0.366025403784439,  // 0.5*(sqrt(3.0)-1.0)
                       -0.577350269189626,  // -1.0 + 2.0 * C.x
                        0.024390243902439); // 1.0 / 41.0
    vec2 i  = floor(v + dot(v, C.yy));
    vec2 x0 = v -   i + dot(i, C.xx);
    vec2 i1 = (x0.x > x0.y) ? vec2(1.0, 0.0) : vec2(0.0, 1.0);
    vec4 x12 = x0.xyxy + C.xxzz;
    x12.xy -= i1;
    i = mod289(i);
    vec3 p = permute(permute(i.y + vec3(0.0, i1.y, 1.0)) + i.x + vec3(0.0, i1.x, 1.0));
    vec3 m = max(0.5 - vec3(dot(x0,x0), dot(x12.xy,x12.xy), dot(x12.zw,x12.zw)), 0.0);
    m = m*m;
    m = m*m;
    vec3 x = 2.0 * fract(p * C.www) - 1.0;
    vec3 h = abs(x) - 0.5;
    vec3 ox = floor(x + 0.5);
    vec3 a0 = x - ox;
    m *= 1.79284291400159 - 0.85373472095314 * (a0*a0 + h*h);
    vec3 g;
    g.x  = a0.x  * x0.x  + h.x  * x0.y;
    g.yz = a0.yz * x12.xz + h.yz * x12.yw;
    return 130.0 * dot(m, g);
}

void main()
{
    // 1. Calculate distance from center (0.0 to 0.5)
    vec2 uv = vTexCoord - 0.5;
    float dist = length(uv);

    // If outside the particle radius, discard immediately.
    if (dist > 0.5) {
        discard;
    }

    // 2. Define Radii for the different layers of the effect
    float coreRadius = 0.25;
    float rimFalloff = 0.04;
    float glowRadius = 0.48;

    // 3. Create animated internal energy
    // We use two layers of scrolling noise for a dynamic, wispy look.
    vec2 motion1 = vec2(time * 0.1, time * -0.05);
    vec2 motion2 = vec2(time * -0.04, time * 0.08);
    float noise1 = snoise((uv + 0.5) * 6.0 + motion1);
    float noise2 = snoise((uv + 0.5) * 4.0 + motion2);
    float energy = pow(abs(noise1 + noise2), 2.0);

    // 4. Build the particle from the inside out

    // --- The Core: A solid, vibrant center.
    // It contains the animated energy.
    vec3 coreColor = vColor.rgb + (energy * vColor.rgb * 0.5);
    float coreMask = 1.0 - smoothstep(coreRadius - 0.02, coreRadius, dist);

    // --- The Rim Light: An intense, bright edge for high contrast.
    // This is the key to making the particle "pop" against any background.
    vec3 rimColor = min(vColor.rgb * 1.5 + vec3(0.5), vec3(1.0));
    float rimMask = smoothstep(coreRadius - rimFalloff, coreRadius, dist) - smoothstep(coreRadius, coreRadius + rimFalloff, dist);

    // --- The Glow: A soft, colored aura that fades out.
    vec3 glowColor = vColor.rgb * 0.8;
    float glowMask = 1.0 - smoothstep(glowRadius - 0.2, glowRadius, dist);
    
    // 5. Combine layers and calculate final Color and Alpha
    
    // The alpha is a combination of the glow and core masks. It's opaque in the center and fades out.
    float finalAlpha = (glowMask * 0.5 + coreMask * 0.5) * vColor.a;

    // Start with the soft glow color
    vec3 finalColor = glowColor;
    
    // Blend the brighter core color on top of the glow
    finalColor = mix(finalColor, coreColor, coreMask);
    
    // Add the intense rim light on top of everything.
    finalColor = mix(finalColor, rimColor, rimMask);

    // Prevent fully transparent pixels from being written
    if (finalAlpha < 0.01) {
        discard;
    }

    FragColor = vec4(finalColor, finalAlpha);
}
