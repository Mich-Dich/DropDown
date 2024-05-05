﻿using OpenTK.Graphics.OpenGL4;

namespace Core.renderer {

    public class parameter_buffer : i_buffer {

        public int id { get; }

        public parameter_buffer(float[] indecies, int stride) { 
            
            id = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ParameterBuffer, id);
            GL.BufferData(BufferTarget.ParameterBuffer, indecies.Length * stride, indecies, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ParameterBuffer, 0);
        }

        public void bind() {

            GL.BindBuffer(BufferTarget.ParameterBuffer, id);
        }

        public void unbind() {
            
            GL.BindBuffer(BufferTarget.ParameterBuffer, 0);
        }

    }
}