#version 330 core

in vec2 vTexCoord;
in vec4 vColor;
out vec4 FragColor;

void main()
{
    float dist = length(vTexCoord - vec2(0.5));
    float glow = exp(-dist * 6.0);
    glow *= 3.0;
    glow = clamp(glow, 0.0, 1.0);
    vec3 finalColor = vColor.rgb * glow;
    float alpha = glow;
    FragColor = vec4(finalColor, alpha);

    if (dist > 0.5)
        discard;
}
