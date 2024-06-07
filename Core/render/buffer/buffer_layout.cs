namespace Core.render.buffer
{
    using Core.util;
    using OpenTK.Graphics.OpenGL4;

    public sealed class Buffer_Layout
    {

        public Buffer_Layout() { }

        public List<Buffer_Element> get_buffer_elements() => elements;

        public int get_stride() => stride;

        public Buffer_Layout add<T>(int count, bool normalized = false) where T : struct
        {

            if (!typemap.TryGetValue(typeof(T), out var vertex_type))
                throw new NotSupportedException($"type {typeof(T)} is not supported.");

            stride += util.Get_Size_Of_VertexAttribPointerType(vertex_type) * count;
            elements.Add(new Buffer_Element { type = vertex_type, count = count, normalized = normalized });
            return this;
        }

        private int stride = 0;
        private readonly List<Buffer_Element> elements = new();
        private readonly Dictionary<Type, VertexAttribPointerType> typemap =
            new() {

                { typeof(byte), VertexAttribPointerType.UnsignedByte },
                { typeof(sbyte), VertexAttribPointerType.Byte },
                { typeof(short), VertexAttribPointerType.Short },
                { typeof(ushort), VertexAttribPointerType.UnsignedShort },
                { typeof(int), VertexAttribPointerType.Int },
                { typeof(uint), VertexAttribPointerType.UnsignedInt },
                { typeof(float), VertexAttribPointerType.Float },
                { typeof(double), VertexAttribPointerType.Double },
            };
    }
}
