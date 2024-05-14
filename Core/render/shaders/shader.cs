
namespace Core.render.shaders {

    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;

    public sealed class Shader {

        // ============================================================================== public ==============================================================================

        public int programmId { get; private set; }

        public Shader(string vertShaderPath, string fragShaderPath, bool compile = true) {

            this.shaderProgrammSource = Shader.Parse_Shader(vertShaderPath, fragShaderPath);
            if(compile)
                this.Compile();
        }

        ~Shader() { GL.DeleteProgram(this.programmId); }

        public static Shader_Programm_Source Parse_Shader(string vertex_shader_path, string fragment_shader_path) {
            
            string vert_path = "assets/" + vertex_shader_path;
            if(!File.Exists(vert_path))
                throw new FileNotFoundException(vert_path);

            string frag_path = "assets/" + fragment_shader_path;
            if(!File.Exists(frag_path))
                throw new FileNotFoundException(frag_path);

            string vert_shader = File.ReadAllText(vert_path);
            string frag_shader = File.ReadAllText(frag_path);
            return new Shader_Programm_Source(vert_shader, frag_shader);
        }

        public bool Compile() {

            if(this.isCompiled) {

                Console.WriteLine("Shader already Compiled");
                return true;
            }

            if(this.shaderProgrammSource == null) {

                Console.WriteLine("[shader_programm_sorce] is null");
                return false;
            }

            int vert_shader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vert_shader, this.shaderProgrammSource.vertexShaderString);
            GL.CompileShader(vert_shader);
            GL.GetShader(vert_shader, ShaderParameter.CompileStatus, out var vert_shader_Compile_status);
            if(vert_shader_Compile_status != (int)All.True) {

                Console.WriteLine(GL.GetShaderInfoLog(vert_shader));
                return false;
            }

            int frag_shader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(frag_shader, this.shaderProgrammSource.fragmentShaderString);
            GL.CompileShader(frag_shader);
            GL.GetShader(frag_shader, ShaderParameter.CompileStatus, out var frag_shader_Compile_status);
            if(frag_shader_Compile_status != (int)All.True) {

                Console.WriteLine(GL.GetShaderInfoLog(frag_shader));
                return false;
            }

            this.programmId = GL.CreateProgram();
            GL.AttachShader(this.programmId, vert_shader);
            GL.AttachShader(this.programmId, frag_shader);
            GL.LinkProgram(this.programmId);

            // free unneeded resources
            GL.DetachShader(this.programmId, vert_shader);
            GL.DetachShader(this.programmId, frag_shader);
            GL.DeleteShader(vert_shader);
            GL.DeleteShader(frag_shader);

            GL.GetProgram(this.programmId, GetProgramParameterName.ActiveUniforms, out var total_uniforms);
            for(int x = 0; x < total_uniforms; x++) {

                string key = GL.GetActiveUniform(this.programmId, x, out _, out _);
                int location = GL.GetUniformLocation(this.programmId, key);
                this.uniforms.Add(key, location);
            }

            this.isCompiled = true;
            return true;
        }

        public void Use() {

            if(!this.isCompiled) {

                Console.WriteLine("Shader not Compiled");
                return;
            }

            GL.UseProgram(this.programmId);
        }

        public void SetUniform(string name, Vector3 value) {

            int location = GL.GetUniformLocation(this.programmId, name);
            GL.Uniform3(location, value);
        }

        public void Set_Uniform(string name, Vector4 value) {

            int location = GL.GetUniformLocation(this.programmId, name);
            GL.Uniform4(location, value);
        }

        public void Set_Matrix_4x4(string uniform_name, Matrix4 mat) {

            int location = GL.GetUniformLocation(this.programmId, uniform_name);
            GL.UniformMatrix4(location, 1, false, this.Get_Matrix4_Values(mat));
        }

        public float[] Get_Matrix4_Values(Matrix4 mat) {

            return new float[] {
                mat.M11, mat.M12, mat.M13, mat.M14,
                mat.M21, mat.M22, mat.M23, mat.M24,
                mat.M31, mat.M32, mat.M33, mat.M34,
                mat.M41, mat.M42, mat.M43, mat.M44,
            };
        }
        
        public int GetAttribLocation(string attribName) { return GL.GetAttribLocation(this.programmId, attribName); }

        public void Dispose() { GL.DeleteProgram(this.programmId);}

        public void Unbind() { GL.UseProgram(0); }

        public int Get_Uniform_Location(string name) => this.uniforms[name];

        // ============================================================================== private ==============================================================================

        private readonly IDictionary<string, int> uniforms = new Dictionary<string, int>();
        private Shader_Programm_Source shaderProgrammSource { get; }
        private bool isCompiled = false;
    }
}
