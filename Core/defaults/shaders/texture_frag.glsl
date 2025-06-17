#version 330 core

in vec2 tex_coord;
out vec4 fragColor;

uniform sampler2D u_texture[5];
uniform vec4 u_tint = vec4(1.0, 1.0, 1.0, 1.0); // Default to white (no tint)

void main() {
    vec4 texColor = texture(u_texture[0], tex_coord);
    fragColor = texColor * u_tint;
}