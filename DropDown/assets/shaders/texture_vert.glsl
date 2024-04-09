#version 330 core

layout (location = 0) in vec2 position;
layout (location = 1) in vec2 in_tex_coord;
layout (location = 2) in float texture_index;
out vec2 tex_coord;
out float tex_index;

uniform mat4 projection;
uniform mat4 model;

void main() {

    tex_index = texture_index;
    gl_Position = projection * model * vec4(position, 0, 1);
    tex_coord = in_tex_coord;
}