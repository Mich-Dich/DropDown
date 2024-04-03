#version 330 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec4 color;

out vec4 vertColor;

void main() {

    gl_Position = vec4(position, 1f);
    vertColor = color;
}