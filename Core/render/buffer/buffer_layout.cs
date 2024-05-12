
namespace Core.render {

    using OpenTK.Graphics.OpenGL4;

    public sealed class Buffer_Layout {

        public Buffer_Layout() { }

        public List<Buffer_Element> get_buffer_elements() => _elements;
        public int get_stride() => _stride;

        public Buffer_Layout add<T>(int count, bool normalized = false) where T : struct {

            if(!_typemap.TryGetValue(typeof(T), out var vertex_type))
                throw new NotSupportedException($"type {typeof(T)} is not supported.");

            _stride += Util.get_size_of_VertexAttribPointerType(vertex_type) * count;
            _elements.Add(new Buffer_Element { type = vertex_type, count = count, normalized = normalized });
            return this;
        }

        private List<Buffer_Element> _elements = new();
        private int _stride = 0;
        private readonly Dictionary<Type, VertexAttribPointerType> _typemap =
            new Dictionary<Type, VertexAttribPointerType> {

                { typeof(byte), (VertexAttribPointerType.UnsignedByte) },
                { typeof(sbyte), (VertexAttribPointerType.Byte) },
                { typeof(short), (VertexAttribPointerType.Short) },
                { typeof(ushort), (VertexAttribPointerType.UnsignedShort) },
                { typeof(int), (VertexAttribPointerType.Int) },
                { typeof(uint), (VertexAttribPointerType.UnsignedInt) },
                { typeof(float), (VertexAttribPointerType.Float) },
                { typeof(double), (VertexAttribPointerType.Double) },
            };


    }

}
