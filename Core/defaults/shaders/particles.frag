#version 330 core

in vec2 vTexCoord;
in vec4 vColor; // Base color from C#
out vec4 FragColor;

uniform float time;
uniform int renderPass; // 0 = Detail Pass, 1 = Glow Pass

// 2D simplex noise (for organic, flowing energy)
vec3 mod289(vec3 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
vec2 mod289(vec2 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
vec3 permute(vec3 x) { return mod289(((x*34.0)+1.0)*x); }
float snoise(vec2 v) {
    const vec4 C = vec4(0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439);
    vec2 i = floor(v + dot(v, C.yy)), x0 = v - i + dot(i, C.xx);
    vec2 i1 = (x0.x > x0.y) ? vec2(1.0, 0.0) : vec2(0.0, 1.0);
    vec4 x12 = x0.xyxy + C.xxzz; x12.xy -= i1;
    i = mod289(i);
    vec3 p = permute(permute(i.y + vec3(0.0, i1.y, 1.0)) + i.x + vec3(0.0, i1.x, 1.0));
    vec3 m = max(0.5 - vec3(dot(x0,x0), dot(x12.xy,x12.xy), dot(x12.zw,x12.zw)), 0.0);
    m = m*m; m = m*m;
    vec3 x = 2.0 * fract(p * C.www) - 1.0, h = abs(x) - 0.5, ox = floor(x + 0.5), a0 = x - ox;
    m *= 1.79284291400159 - 0.85373472095314 * (a0*a0 + h*h);
    vec3 g; g.x = a0.x * x0.x + h.x * x0.y; g.yz = a0.yz * x12.xz + h.yz * x12.yw;
    return 130.0 * dot(m, g);
}

void main()
{
    vec2 uv = vTexCoord - 0.5;
    float dist = length(uv);

    if (dist > 0.5) discard;

    // ======================================================================
    //                           PASS 1: DETAIL & CORE BODY
    // ======================================================================
    if (renderPass == 0)
    {
        // 1. Create a single, very smooth falloff for the main body of the orb.
        // We use pow() to create a soft, non-linear curve instead of a hard edge.
        float falloff = 1.0 - dist / 0.45; // Goes from 1.0 at center to 0.0 at radius 0.45
        float bodyMask = pow(max(0.0, falloff), 2.0);

        // 2. Create the internal energy effect.
        // We make it more subtle and wispy.
        float angle = atan(uv.y, uv.x);
        float rays = pow(sin(angle * 6.0 - time * 2.0) * 0.5 + 0.5, 2.0); // Smoother rays
        vec2 motion = vec2(time * 0.2, time * -0.1);
        float wisps = snoise((uv + 0.5) * 5.0 + motion) * 0.5 + 0.5;
        float energy = rays * wisps * bodyMask;

        // 3. Define the final color for the core body.
        // The base color is brightened by the internal energy.
        vec3 litColor = vColor.rgb + (vColor.rgb * energy * 0.6);
        vec3 finalColor = litColor * bodyMask;

        // 4. The alpha is determined by the same soft mask.
        float finalAlpha = bodyMask * vColor.a;

        FragColor = vec4(finalColor, finalAlpha);
    }
    // ======================================================================
    //                           PASS 2: BLURRED GLOW
    // ======================================================================
    else // renderPass == 1
    {
        // 1. Create a wide and extremely soft falloff for the glow.
        // This is the key to the "blurred" effect.
        // The falloff is a gentle curve from the center to the absolute edge.
        float glowFalloff = 1.0 - dist / 0.5;
        float glowMask = pow(max(0.0, glowFalloff), 2.5); // Use a higher power for an even softer edge

        // 2. Add a subtle, slow-moving atmospheric noise to the glow.
        vec2 slowMotion = vec2(time * 0.02);
        float atmosphere = snoise(vTexCoord * 3.0 + slowMotion) * 0.5 + 0.5;
        glowMask *= mix(0.8, 1.2, atmosphere); // Varies the glow intensity slightly

        // 3. Final color is just the base color, as it will be added to the scene.
        vec3 finalColor = vColor.rgb;

        // 4. The alpha for the additive pass is determined by our soft glow mask.
        float finalAlpha = glowMask * 0.6 * vColor.a; // Multiplier controls glow strength

        FragColor = vec4(finalColor, finalAlpha);
    }
}