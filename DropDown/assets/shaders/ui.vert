#version 330

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoord;

out vec2 TexCoord;

uniform mat4 transform;
uniform vec4 texCoordRange;

void main()
{
    gl_Position = transform * vec4(aPosition, 1.0);
    TexCoord = aTexCoord * (texCoordRange.zw - texCoordRange.xy) + texCoordRange.xy;
}