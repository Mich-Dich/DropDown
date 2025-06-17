#version 330 core

in vec2 vTexCoord;
in vec4 vColor;
out vec4 FragColor;

uniform float time; // Use this to animate or affect the noise

// A simple pseudo-random function based on vTexCoord
float pseudoRandom(vec2 co) {
    co += time * 0.1; 
    return fract(sin(dot(co, vec2(12.9898,78.233))) * 43758.5453);
}

// Star-shaped sparkle function
float star(vec2 uv, float points, float sharpness, float size) {
    float angle = atan(uv.y - 0.5, uv.x - 0.5);
    float radius = length(uv - vec2(0.5));
    float star = pow(abs(cos(points * angle)), sharpness);
    return smoothstep(size, size - 0.04, radius) * star;
}

void main()
{
    float dist = length(vTexCoord - vec2(0.5));
    float maxRadius = 0.7;
    float coreRadius = 0.19;
    float glowRadius = 0.38;
    float rimRadius = 0.52;
    float rimSoftness = 0.16;

    // Core color pulse: deep blue <-> deep purple
    float pulse = 0.5 + 0.5 * sin(time * 2.7 + vTexCoord.x * 8.0);
    vec3 blue = vec3(0.13, 0.32, 0.85);
    vec3 purple = vec3(0.38, 0.13, 0.55);
    vec3 corePulseColor = mix(blue, purple, pulse);

    // Core: dark, saturated, pulsing
    float core = smoothstep(coreRadius, coreRadius - 0.07, dist);
    vec3 coreColor = mix(corePulseColor, vec3(0.25, 0.25, 0.35), 0.18);

    // Glow: thick, saturated, colored, pulsing (out of phase)
    float glowPulse = 0.5 + 0.5 * sin(time * 2.0 + vTexCoord.y * 7.0 + 2.0);
    vec3 glowColor = mix(blue, purple, 1.0 - glowPulse);
    float glow = smoothstep(glowRadius, coreRadius, dist);
    glowColor = mix(glowColor, vec3(0.18, 0.18, 0.25), 0.08);

    // Rim: very dark, colored (deep blue/purple)
    float rim = smoothstep(rimRadius, rimRadius - rimSoftness, dist);
    vec3 rimColor = mix(vec3(0.01, 0.01, 0.08), vec3(0.08, 0.01, 0.13), 0.5);

    // Outer fade
    float alpha = 1.0;
    if (dist > rimRadius) {
        float fade = 1.0 - smoothstep(rimRadius, maxRadius, dist);
        alpha *= fade;
    }
    if (dist > maxRadius) discard;

    // Twinkle: much larger, brighter, more frequent
    float sparkle = star(vTexCoord, 6.0, 10.0, 0.22 + 0.08 * sin(time * 2.0 + vTexCoord.y * 10.0));
    float sparkleChance = step(0.85, pseudoRandom(vTexCoord * 30.0 + time * 2.0));
    float sparkleIntensity = sparkle * sparkleChance * (0.8 + 0.2 * pulse);
    vec3 sparkleColor = mix(vec3(0.7, 0.7, 1.0), purple, 0.3);

    // Combine layers
    vec3 color = vec3(0.0);
    color += core * coreColor * 1.1;
    color += glow * glowColor * 1.0;
    color = mix(color, rimColor, rim);
    color += sparkleColor * sparkleIntensity;

    // Modulate by particle color (for XP, this is usually blue/purple)
    color *= vColor.rgb;
    alpha *= vColor.a;

    // Lower minimum visibility clamp for better contrast
    float minVis = 0.08;
    color = max(color, vec3(minVis));

    FragColor = vec4(color, alpha);
}
