#version 330 core

layout(location = 0) in vec3 aVertexPosition;
layout(location = 1) in vec4 aPositionSize; // (x, y, size, unused)
layout(location = 2) in vec4 aColor;        // per-particle color

uniform mat4 projection;

out vec2 vTexCoord;
out vec4 vColor;

void main()
{
    // Scale the quad by aPositionSize.z and translate by aPositionSize.xy
    vec2 scaledPosition = aVertexPosition.xy * aPositionSize.z + aPositionSize.xy;

    gl_Position = projection * vec4(scaledPosition, 0.0, 1.0);

    // vTexCoord is used for generating our circular gradient in the fragment shader
    vTexCoord = aVertexPosition.xy + 0.5;
    vColor = aColor; // pass per-particle color
}
