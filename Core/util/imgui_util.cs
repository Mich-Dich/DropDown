
namespace Core.util {

    using ImGuiNET;
    using System.Numerics;

    public static class Imgui_Util {
    
    
        public static void begin_default_table(string label, bool display_lable = false) {

            if(display_lable)
                ImGui.Text(label);

            ImGuiTableFlags flags = ImGuiTableFlags.Resizable;
            ImGui.BeginTable(label, 2, flags);
            ImGui.TableSetupColumn($"##one{label}", ImGuiTableColumnFlags.NoHeaderLabel);
            ImGui.TableSetupColumn($"##two{label}", ImGuiTableColumnFlags.NoHeaderLabel | ImGuiTableColumnFlags.NoResize);

        }

        public static void end_default_table() {

            ImGui.EndTable();
        }

        public static void add_table_row(string label, string value) {

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Text(label);
            ImGui.TableSetColumnIndex(1);
            ImGui.Text(value);
        }

        public static void add_table_row(string label, ref float value, float speed = 0.1f, float min = 0f, float max = 0f) {

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Text(label);
            ImGui.TableSetColumnIndex(1);
            ImGui.DragFloat($"##{label}", ref value, speed, min, max);
        }

        public static void add_table_row(string label, ref int value, float speed = 1, int min = 0, int max = 100) {

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Text(label);
            ImGui.TableSetColumnIndex(1);
            ImGui.DragInt($"##{label}", ref value, speed, min, max);
        }

        public static void add_table_row(string label, Action lamda) {

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Text(label);
            ImGui.TableSetColumnIndex(1);
            lamda();
        }

        public static void add_table_spacing(int amout = 1) {

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            for (int x = 0; x < amout; x++)
                ImGui.Spacing();
            ImGui.TableSetColumnIndex(1);
            for (int x = 0; x < amout; x++)
                ImGui.Spacing();
        }

        public static void shift_cursor_pos(float shift_x = 0, float shift_y = 0) {

            var current_pos = ImGui.GetCursorPos();
            ImGui.SetCursorPos(current_pos + new System.Numerics.Vector2(shift_x, shift_y));
        }

        public static void title(string label, bool giant = false) {

            if(giant)
                ImGui.PushFont(imgui_fonts.fonts["giant"]);
            else
                ImGui.PushFont(imgui_fonts.fonts["regular_big"]);
            ImGui.Text(label);
            ImGui.PopFont();
        }

        public static void progress_bar_stylised(float ratio, System.Numerics.Vector2? size = null, uint color = 4281591347, uint background_color = 4278190080, float length_of_mini_bar = 0.3f, float height_of_mini_bar = 0.3f, float slope = 1.0f) {

            ImDrawListPtr draw_list = ImGui.GetWindowDrawList();
            var current_pos = ImGui.GetCursorPos();
            var window_pos = ImGui.GetWindowPos() + ImGui.GetCursorPos();
            System.Numerics.Vector2 loc_size = size ?? new System.Numerics.Vector2(150, 20);

            ImGui.PushStyleColor(ImGuiCol.PlotHistogram, color);
            ImGui.ProgressBar(ratio, loc_size, "");
            ImGui.PopStyleColor();

            float partial_height = loc_size.Y * height_of_mini_bar;
            float partial_width = loc_size.X * length_of_mini_bar;

            var current_pos_after_bar = ImGui.GetCursorPos();

            draw_list.AddTriangleFilled(
                window_pos + new System.Numerics.Vector2(0, loc_size.Y),
                window_pos + new System.Numerics.Vector2(0, 0),
                window_pos + new System.Numerics.Vector2(loc_size.Y * slope, 0),
                background_color);
            draw_list.AddTriangleFilled(
                window_pos + new System.Numerics.Vector2(loc_size.X - partial_width - (partial_height * slope), loc_size.Y),
                window_pos + new System.Numerics.Vector2(loc_size.X - partial_width, loc_size.Y - partial_height),
                window_pos + new System.Numerics.Vector2(loc_size.X - partial_width, loc_size.Y), 
                background_color);
            draw_list.AddRectFilled(
                window_pos + new System.Numerics.Vector2(loc_size.X - partial_width, loc_size.Y - partial_height),
                window_pos + new System.Numerics.Vector2(loc_size.X, loc_size.Y),
                background_color);
            draw_list.AddTriangleFilled(
                window_pos + new System.Numerics.Vector2(loc_size.X - (loc_size.Y * slope), loc_size.Y),
                window_pos + new System.Numerics.Vector2(loc_size.X, 0),
                window_pos + new System.Numerics.Vector2(loc_size.X, loc_size.Y),
                background_color);

            ImGui.SetCursorPos(current_pos_after_bar);

        }

    }
}
