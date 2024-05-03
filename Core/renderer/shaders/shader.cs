using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Core.renderer {

    public class shader {

        //  ============================================================================== public ============================================================================== 
        public int programm_id { get; private set; }

        public shader(string vert_shader_path, string frag_shader_path, bool compile = true) {
                        
            _shader_programm_source = shader.parse_shader(vert_shader_path, frag_shader_path);

            if(compile)
                this.compile();
        }

        ~shader() {

            GL.DeleteProgram(programm_id);
        }

        public bool compile() {

            if(_is_compiled) {
                
                Console.WriteLine("Shader already compiled");
                return true;
            }

            if(_shader_programm_source == null) {

                Console.WriteLine("[shader_programm_sorce] is null");
                return false;
            }

            int vert_shader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vert_shader, _shader_programm_source.vertex_shader_string);
            GL.CompileShader(vert_shader);
            GL.GetShader(vert_shader, ShaderParameter.CompileStatus, out var vert_shader_compile_status);
            if(vert_shader_compile_status != (int)All.True) {
                Console.WriteLine(GL.GetShaderInfoLog(vert_shader));
                return false;
            }

            int frag_shader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(frag_shader, _shader_programm_source.fragment_shader_string);
            GL.CompileShader(frag_shader);
            GL.GetShader(frag_shader, ShaderParameter.CompileStatus, out var frag_shader_compile_status);
            if(frag_shader_compile_status != (int)All.True) {
                Console.WriteLine(GL.GetShaderInfoLog(frag_shader));
                return false;
            }

            programm_id = GL.CreateProgram();
            GL.AttachShader(programm_id, vert_shader);
            GL.AttachShader(programm_id, frag_shader);
            GL.LinkProgram(programm_id);

            // free unneeded resources
            GL.DetachShader(programm_id, vert_shader);
            GL.DetachShader(programm_id, frag_shader);
            GL.DeleteShader(vert_shader);
            GL.DeleteShader(frag_shader);

            GL.GetProgram(programm_id, GetProgramParameterName.ActiveUniforms, out var total_uniforms);
            for (int x = 0; x < total_uniforms; x++) {

                string key = GL.GetActiveUniform(programm_id, x, out _, out _);
                int location = GL.GetUniformLocation(programm_id, key);
                _uniforms.Add(key, location);
            }

            _is_compiled = true;
            return true;
        }

        public void use() {

            if(!_is_compiled) {

                Console.WriteLine("Shader not compiled");
                return;
            }

            GL.UseProgram(programm_id);
        }
        
        public void SetUniform(string name, Vector3 value) {
            int location = GL.GetUniformLocation(programm_id, name);
            GL.Uniform3(location, value);
        }

        public void set_uniform(string name, Vector4 value) {
            int location = GL.GetUniformLocation(programm_id, name);
            GL.Uniform4(location, value);
        }

        public void set_matrix_4x4(string uniform_name, Matrix4 mat) {
            int location = GL.GetUniformLocation(programm_id, uniform_name);
            GL.UniformMatrix4(location, 1, false, get_matrix4_values(mat));
        }

        public int GetAttribLocation(string attribName) {
            return GL.GetAttribLocation(programm_id, attribName);
        }

        public float[] get_matrix4_values(Matrix4 mat) {

            return new float[] {
                mat.M11, mat.M12, mat.M13, mat.M14,
                mat.M21, mat.M22, mat.M23, mat.M24,
                mat.M31, mat.M32, mat.M33, mat.M34,
                mat.M41, mat.M42, mat.M43, mat.M44
            };
        }

        public static shader_programm_source parse_shader(string vertex_shader_path, string fragment_shader_path) {

            string vert_path = "assets/" + vertex_shader_path;
            if(!File.Exists(vert_path))
                throw new FileNotFoundException(vert_path);

            string frag_path = "assets/" + fragment_shader_path;
            if(!File.Exists(frag_path))
                throw new FileNotFoundException(frag_path);

            string vert_shader = File.ReadAllText(vert_path );
            string frag_shader = File.ReadAllText(frag_path);

            return new shader_programm_source(vert_shader, frag_shader );
        }

        public void dispose() {
            GL.DeleteProgram(programm_id);
        }

        public void unbind()
        {
            GL.UseProgram(0);
        }

        public int get_uniform_location(string name) => _uniforms[name];

        //  ============================================================================== private ============================================================================== 
        private readonly IDictionary<string, int> _uniforms = new Dictionary<string, int>();
        private shader_programm_source _shader_programm_source { get; }
        private bool _is_compiled = false;
    }
}
