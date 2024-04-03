
namespace Core.renderer {

    public class shader_programm_source {

        public string vertex_shader_string;
        public string fragment_shader_string;

        public shader_programm_source(String vertex_shader_string, String fragment_shader_string) {

            this.vertex_shader_string = vertex_shader_string;
            this.fragment_shader_string = fragment_shader_string;
        }
    }
}
