using Core.manager.texture;
using Core.renderer;

namespace Core.manager {

    public class resource_manager {

        private static resource_manager _instance = null;
        private static readonly object _loc = new();
        private IDictionary<string, texture_2d> _texture_cache = new Dictionary<string, texture_2d>();

        public static resource_manager instance {
            get {
                lock(_loc) {

                    if (_instance == null)
                        _instance = new resource_manager();

                    return _instance;
                }
            }
        }

        // =============================================== textures =============================================== 

        public texture_2d load_texture(string texture_name) {

            _texture_cache.TryGetValue(texture_name, out var value);

            if (value != null)
                return value;

            value = texture_manager.load("assets/" + texture_name);
            _texture_cache.Add(texture_name, value);
            return value;
        }

        public int get_number_of_textures() {

            return texture_manager.texture_cursor;
        }
    }
}
