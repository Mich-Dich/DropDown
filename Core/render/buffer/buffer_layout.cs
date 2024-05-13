namespace Core.render
{
    using OpenTK.Graphics.OpenGL4;

    public sealed class Buffer_Layout
    {
        public Buffer_Layout()
        {
        }

        public List<Buffer_Element> get_buffer_elements() => this.elements;

        public int get_stride() => this.stride;

        public Buffer_Layout add<T>(int count, bool normalized = false)
            where T : struct
        {
            if (!this.typemap.TryGetValue(typeof(T), out var vertex_type))
            {
                throw new NotSupportedException($"type {typeof(T)} is not supported.");
            }

            this.stride += Util.Get_Size_Of_VertexAttribPointerType(vertex_type) * count;
            this.elements.Add(new Buffer_Element { type = vertex_type, count = count, normalized = normalized });
            return this;
        }

        private readonly List<Buffer_Element> elements = new ();
        private readonly Dictionary<Type, VertexAttribPointerType> typemap =
            new ()
            {
                { typeof(byte), VertexAttribPointerType.UnsignedByte },
                { typeof(sbyte), VertexAttribPointerType.Byte },
                { typeof(short), VertexAttribPointerType.Short },
                { typeof(ushort), VertexAttribPointerType.UnsignedShort },
                { typeof(int), VertexAttribPointerType.Int },
                { typeof(uint), VertexAttribPointerType.UnsignedInt },
                { typeof(float), VertexAttribPointerType.Float },
                { typeof(double), VertexAttribPointerType.Double },
            };

        private int stride = 0;
    }
}
