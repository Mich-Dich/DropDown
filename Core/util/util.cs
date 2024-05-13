
namespace Core {
    
    using ImGuiNET;
    using OpenTK.Graphics.OpenGL4;

    /// <summary>
    /// Contains utility methods for handling OpenGL vertex attribute pointer types and angle conversions.
    /// </summary>
    public static class Util {

        /// <summary>
        /// Retrieves the size of the specified VertexAttribPointerType in bytes.
        /// </summary>
        /// <param name="attrib_type">The VertexAttribPointerType to determine the size for.</param>
        /// <returns>The size of the VertexAttribPointerType in bytes.</returns>
        public static int Get_Size_Of_VertexAttribPointerType(VertexAttribPointerType attrib_type) {

            switch (attrib_type) {

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

                case VertexAttribPointerType.HalfFloat:                 // Assuming sizeof(short) for HalfFloat
                    return sizeof(short);

                case VertexAttribPointerType.Fixed:                     // Assuming sizeof(int) for Fixed
                    return sizeof(int);

                case VertexAttribPointerType.UnsignedInt2101010Rev:     // Assuming sizeof(int) for UnsignedInt2101010Rev
                    return sizeof(int);

                case VertexAttribPointerType.UnsignedInt10F11F11FRev:   // Assuming sizeof(int) for UnsignedInt10F11F11FRev
                    return sizeof(int);

                case VertexAttribPointerType.Int2101010Rev:             // Assuming sizeof(int) for Int2101010Rev
                    return sizeof(int);

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Converts an angle from radians to degrees.
        /// </summary>
        /// <param name="angle">Angle in radians.</param>
        /// <returns>Angle converted to degrees.</returns>
        public static double Radians_To_Degree(double angle) { return angle * (180 / Math.PI); }

        /// <summary>
        /// Converts an angle from degrees to radians.
        /// </summary>
        /// <param name="angle">Angle in degrees.</param>
        /// <returns>Angle converted to radians.</returns>
        public static double Degree_To_Radians(double angle) { return angle * (Math.PI / 180); }

    }

    /// <summary>
    /// Stores ImFontPtr instances for ImGui font management.
    /// </summary>
    public static class Imgui_Fonts {

        /// <summary>
        /// Dictionary to store ImGui fonts by name.
        /// </summary>
        public static Dictionary<string, ImFontPtr> fonts = new Dictionary<string, ImFontPtr>();
    }

    /// <summary>
    /// Provides a wrapper for referencing elements in a list by index.
    /// </summary>
    /// <typeparam name="T">Type of elements in the list.</typeparam>
    public sealed class RefWrapper<T> {

        private List<T> list;
        private int index;

        /// <summary>
        /// Constructs a RefWrapper instance with a list and index.
        /// </summary>
        /// <param name="list">The list to reference.</param>
        /// <param name="index">The index to reference in the list.</param>
        public RefWrapper(List<T> list, int index) {
            this.list = list;
            this.index = index;
        }

        /// <summary>
        /// Gets or sets the value at the referenced index in the list.
        /// </summary>
        public T Value {
            get { return list[index]; }
            set { list[index] = value; }
        }
    }

    /// <summary>
    /// Represents an exception thrown when a required resource is not assigned.
    /// </summary>
    public sealed class ResourceNotAssignedException : Exception {

        /// <summary>
        /// Constructs a ResourceNotAssignedException with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public ResourceNotAssignedException(string message)
            : base(message) {
        }
    }
}
