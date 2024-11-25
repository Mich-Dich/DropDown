#version 330 core

layout(location = 0) in vec3 aVertexPosition;
layout(location = 1) in vec4 aPositionSize;
layout(location = 2) in vec4 aColor;

uniform mat4 u_Projection;

out vec2 vTexCoord;
out vec4 vColor;

void main()
{
    vec2 scaledPosition = aVertexPosition.xy * aPositionSize.z + aPositionSize.xy;

    gl_Position = u_Projection * vec4(scaledPosition, 0.0, 1.0);

    vTexCoord = aVertexPosition.xy + 0.5;
    vColor = aColor;
}
