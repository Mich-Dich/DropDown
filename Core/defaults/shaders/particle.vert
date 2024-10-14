#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec4 aOffsetSize;
layout(location = 2) in vec4 aColor;

out vec4 vColor;

void main()
{
    vec3 offset = aOffsetSize.xyz;
    float size = aOffsetSize.w;
    gl_Position = vec4(aPosition * size + offset, 1.0);
    vColor = aColor;
}