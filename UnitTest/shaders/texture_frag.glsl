#version 330 core

in vec2 tex_coord;
out vec4 fragColor;

uniform sampler2D u_texture[5];

void main() {

    fragColor = texture(u_texture[0], tex_coord);
}