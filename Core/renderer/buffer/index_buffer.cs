﻿using OpenTK.Graphics.OpenGL4;

namespace Core.renderer {

    public class index_buffer : i_buffer {

        public int id { get; }

        public index_buffer(uint[] indecies) { 
            
            id = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, id);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indecies.Length * sizeof(uint), indecies, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void bind() {

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, id);
        }

        public void unbind() {
            
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

    }
}
