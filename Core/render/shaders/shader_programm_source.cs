﻿namespace Core.render.shaders {
    public sealed class Shader_Programm_Source {
        public string vertexShaderString;
        public string fragmentShaderString;

        public Shader_Programm_Source(string vertexShaderString, string fragmentShaderString) {
            this.vertexShaderString = vertexShaderString;
            this.fragmentShaderString = fragmentShaderString;
        }
    }
}
