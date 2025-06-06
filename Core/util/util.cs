﻿
namespace Core.util {

    using ImGuiNET;
    using OpenTK.Graphics.OpenGL4;
    using System;

    public static class util {

        public static int Get_Size_Of_VertexAttribPointerType(VertexAttribPointerType attrib_type) {
            switch(attrib_type) {
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

        public static double Radians_To_Degree(double angle) { return angle * (180 / Math.PI); }

        public static double Degree_To_Radians(double angle) { return angle * (Math.PI / 180); }

        public static float Lerp(float a, float b, float t) { return a + (b - a) * Math.Clamp(t, 0, 1); }

        public static float angle_from_vec(OpenTK.Mathematics.Vector2 dir) { return (float)Math.Atan2(dir.X, dir.Y); }

        public static float angle_from_vec(Box2DX.Common.Vec2 dir) { return (float)Math.Atan2(dir.X, dir.Y); }

        public static OpenTK.Mathematics.Vector2 vector_from_angle(float angle_radians) { return new OpenTK.Mathematics.Vector2((float)Math.Cos(angle_radians), (float)Math.Sin(angle_radians)); }

        public static OpenTK.Mathematics.Vector2 Convert_Screen_To_World_Coords(float x, float y) {

            OpenTK.Mathematics.Vector2 result = new ();
            OpenTK.Mathematics.Vector2 default_offset = new (17, 40);
            OpenTK.Mathematics.Vector2 mouse_position = new (x, y);

            result = Game.Instance.camera.transform.position * Game.Instance.camera.GetScale();
            result += mouse_position - Game.Instance.window.Size / 2;
            result += default_offset * (mouse_position / Game.Instance.window.Size);
            result /= Game.Instance.camera.GetScale();
            return result;
        }

        public static OpenTK.Mathematics.Vector2 Convert_World_To_Screen_Coords(OpenTK.Mathematics.Vector2 world_position) {

            OpenTK.Mathematics.Vector2 default_offset = new(17, 40);

            world_position *= Game.Instance.camera.GetScale();
            world_position -= Game.Instance.camera.transform.position * Game.Instance.camera.GetScale();
            world_position -= default_offset * (world_position / Game.Instance.window.Size);
            return world_position + Game.Instance.window.Size / 2;
        }

        public static OpenTK.Mathematics.Vector2 convert_Vector(System.Numerics.Vector2 vector) { return new OpenTK.Mathematics.Vector2(vector.X, vector.Y); }
        public static System.Numerics.Vector2 convert_Vector(OpenTK.Mathematics.Vector2 vector) { return new System.Numerics.Vector2(vector.X, vector.Y); }
        public static OpenTK.Mathematics.Vector2 convert_Vector(Box2DX.Common.Vec2 vector_box2d) { return new OpenTK.Mathematics.Vector2(vector_box2d.X, vector_box2d.Y); }
        public static Box2DX.Common.Vec2 convert_Vector_x(OpenTK.Mathematics.Vector2 vector) { return new Box2DX.Common.Vec2(vector.X, vector.Y); }

        // Function to generate a UInt64 value with specified density of bits flipped
        public static ulong generate_random_UInt64_with_density(double density, Random? random = null) {

            Random loc_random = random ?? new Random();
            ulong result = 0;
            for(int i = 0; i < 64; i++) {

                double randomValue = loc_random.NextDouble();
                if(randomValue < density)
                    result |= (ulong)1 << i;
            }
            return result;
        }
        
        public static int Count_Number_Of_Following_Ones(byte target, int index_of_first_one) {

            int count = 0;
            byte buffer = target;
            byte mask = 0x01; // Start with the least significant bit (LSB) mask
            mask <<= index_of_first_one;
            int buffer_index = index_of_first_one;

            // Iterate over each bit of the byte value
            while((buffer & mask) != 0 && buffer_index < 8) {
                count++; // Increment the count for each '1' encountered
                mask <<= 1; // Shift the mask to the left to check the next bit
            }
            return count;
        }


    }

    public static class Imgui_Fonts {
        public static Dictionary<string, ImFontPtr> fonts = new ();

        public static void LoadFont(string path, float size) {
            if (!File.Exists(path)) {
                throw new FileNotFoundException($"Font file not found: {path}");
            }

            ImFontPtr font = ImGui.GetIO().Fonts.AddFontFromFileTTF(path, size);
            Console.WriteLine(font);

            fonts[path + size.ToString()] = font;
        }
    }

    public sealed class ResourceNotAssignedException : Exception {

        public ResourceNotAssignedException(string message)
            : base(message) {
        }
    }
}
