#version 330 core

in vec2 vTexCoord;
in vec4 vColor;
out vec4 FragColor;

uniform float time; // Use this to animate or affect the noise

// A simple pseudo-random function based on vTexCoord
float pseudoRandom(vec2 co) {
    // Use time to vary noise over time (optional)
    co += time * 0.1; 
    return fract(sin(dot(co, vec2(12.9898,78.233))) * 43758.5453);
}

void main()
{
    float dist = length(vTexCoord - vec2(0.5));
    
    // Extend total radius beyond 0.5 for a halo
    float maxRadius = 0.6;
    
    if (dist > maxRadius) {
        discard;
    }

    // Generate some noise for breakup
    float noiseVal = pseudoRandom(vTexCoord * 10.0);
    // Add subtle distortion to dist based on noise
    dist += (noiseVal - 0.5) * 0.03;

    // Define base colors for gradient
    vec3 centerColor = vec3(0.0, 1.0, 1.0); // Cyan center
    vec3 edgeColor   = vec3(1.0, 0.0, 1.0); // Magenta edge

    float normalizedDist = dist / 0.5; 
    normalizedDist = clamp(normalizedDist, 0.0, 1.0);

    // Smooth transition for color
    float colorMix = smoothstep(0.0, 1.0, normalizedDist);
    vec3 baseColor = mix(centerColor, edgeColor, colorMix);

    // Incorporate the per-particle color
    baseColor *= vColor.rgb;

    // Alpha calculation
    float alpha;
    if (dist <= 0.5) {
        alpha = 1.0 - normalizedDist * 0.8;
    } else {
        // Halo zone fade-out
        float haloDist = (dist - 0.5) / (maxRadius - 0.5);
        alpha = exp(-haloDist * 5.0);
    }

    // Add slight alpha variation from noise for more breakup
    alpha *= (0.9 + noiseVal * 0.2);

    // Subtle rim highlight near the edge of the inner circle
    if (dist > 0.45 && dist < 0.5) {
        float rimStrength = 1.0 - smoothstep(0.45, 0.5, dist);
        baseColor += vec3(0.5, 0.5, 0.5) * rimStrength * 0.3;
    }

    FragColor = vec4(baseColor, alpha);
}
