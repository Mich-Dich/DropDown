#version 330 core

layout (location = 0) in vec2 position;
layout (location = 1) in vec2 in_tex_coord;
out vec2 tex_coord;

uniform mat4 projection;
uniform mat4 model;

void main() {

    gl_Position = projection * model * vec4(position, 0f, 1f);
    tex_coord = in_tex_coord;
}