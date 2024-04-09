#version 330 core

in vec2 tex_coord;
in float tex_index;
out vec4 fragColor;

uniform sampler2D u_texture[5];

void main() {

    int index = int(tex_index);
    fragColor = texture(u_texture[index], tex_coord);
}