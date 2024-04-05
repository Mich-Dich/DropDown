using OpenTK.Graphics.OpenGL4;
using Core.util;

namespace Core.renderer {

    public class buffer_layout {

        private List<buffer_element> _elements = new();
        private int _stride = 0;

        public buffer_layout() { 
        
        }

        public List<buffer_element> get_buffer_elements() => _elements;
        public int get_stride() => _stride;

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

        public void add<T>(int count, bool normalized = false) where T : struct {

            if(_typemap.TryGetValue(typeof(T), out var vertex_type)) {

                _stride += utility.get_size_of_VertexAttribPointerType(vertex_type) * count;
                _elements.Add(new buffer_element { type = vertex_type, count = count, normalized = normalized });
            }

            else
                throw new NotSupportedException($"type {typeof(T)} is not supported.");

        }
    }
}

/*
sizeof(byte)
sizeof(sbyte)
sizeof(short)
sizeof(ushort)
sizeof(int)
sizeof(uint)
sizeof(float)
sizeof(double)
sizeof(short)
*/
