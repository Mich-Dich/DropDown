namespace Core
{
    using ImGuiNET;
    using OpenTK.Graphics.OpenGL4;

    public static class Util
    {
        public static int Get_Size_Of_VertexAttribPointerType(VertexAttribPointerType attrib_type)
        {
            switch (attrib_type)
            {
                case VertexAttribPointerType.Byte:
                case VertexAttribPointerType.UnsignedByte:
                    return sizeof(byte);

                case VertexAttribPointerType.Short:
                case VertexAttribPointerType.UnsignedShort:
                    return sizeof(short);

                case VertexAttribPointerType.Int:
                    return sizeof(int);
                case VertexAttribPointerType.UnsignedInt:
                    return sizeof(uint);

                case VertexAttribPointerType.Float:
                    return sizeof(float);

                case VertexAttribPointerType.Double:
                    return sizeof(double);

                case VertexAttribPointerType.HalfFloat: // Assuming sizeof(short) for HalfFloat
                    return sizeof(short);

                case VertexAttribPointerType.Fixed: // Assuming sizeof(int) for Fixed
                    return sizeof(int);

                case VertexAttribPointerType.UnsignedInt2101010Rev: // Assuming sizeof(int) for UnsignedInt2101010Rev
                    return sizeof(int);

                case VertexAttribPointerType.UnsignedInt10F11F11FRev: // Assuming sizeof(int) for UnsignedInt10F11F11FRev
                    return sizeof(int);

                case VertexAttribPointerType.Int2101010Rev: // Assuming sizeof(int) for Int2101010Rev
                    return sizeof(int);

                default:
                    return 0;
            }
        }

        public static double Radians_To_Degree(double angle)
        {
            return angle * (180 / Math.PI);
        }

        public static double Degree_To_Radians(double angle)
        {
            return angle * (Math.PI / 180);
        }
    }

    public static class Imgui_Fonts
    {
        public static Dictionary<string, ImFontPtr> fonts = new ();
    }

    public sealed class RefWrapper<T>
    {
        private readonly List<T> list;
        private readonly int index;

        public RefWrapper(List<T> list, int index)
        {
            this.list = list;
            this.index = index;
        }

        public T Value
        {
            get { return this.list[this.index]; }
            set { this.list[this.index] = value; }
        }
    }

    public sealed class ResourceNotAssignedException : Exception
    {
        public ResourceNotAssignedException(string message)
            : base(message)
            {
        }
    }
}
