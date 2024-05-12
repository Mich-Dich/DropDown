
namespace Core.util {

    using ImGuiNET;
    using System.Numerics;

    public static class Imgui_Util {

        /// <summary>
        /// Begins an ImGui table with better aperance settings.
        /// </summary>
        /// <param name="label">Label for the table.</param>
        /// <param name="display_lable">Whether to display the label as text.</param>
        public static void Begin_Table(string label, bool display_lable = false) {

            if(display_lable)
                ImGui.Text(label);

            ImGuiTableFlags flags = ImGuiTableFlags.Resizable;
            ImGui.BeginTable(label, 2, flags);
            ImGui.TableSetupColumn($"##one{label}", ImGuiTableColumnFlags.NoHeaderLabel);
            ImGui.TableSetupColumn($"##two{label}", ImGuiTableColumnFlags.NoHeaderLabel | ImGuiTableColumnFlags.NoResize);

        }

        /// <summary>
        /// Ends the current ImGui table.
        /// </summary>
        public static void End_Table() {

            ImGui.EndTable();
        }

        /// <summary>
        /// Adds a row to the current ImGui table with a label and value.
        /// </summary>
        /// <param name="label">Label for the row.</param>
        /// <param name="value">Value to display in the row.</param>
        public static void Add_Table_Row(string label, string value) {

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Text(label);
            ImGui.TableSetColumnIndex(1);
            ImGui.Text(value);
        }

        /// <summary>
        /// Adds a row to the current ImGui table with a label and a float value that can be edited.
        /// </summary>
        /// <param name="label">Label for the row.</param>
        /// <param name="value">Reference to the float value.</param>
        /// <param name="speed">Increment speed for editing the value.</param>
        /// <param name="min">Minimum value for the float.</param>
        /// <param name="max">Maximum value for the float.</param>
        public static void Add_Table_Row(string label, ref float value, float speed = 0.1f, float min = 0f, float max = 0f) {

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Text(label);
            ImGui.TableSetColumnIndex(1);
            ImGui.DragFloat($"##{label}", ref value, speed, min, max);
        }

        /// <summary>
        /// Adds a row to the current ImGui table with a label and an integer value that can be edited.
        /// </summary>
        /// <param name="label">Label for the row.</param>
        /// <param name="value">Reference to the integer value.</param>
        /// <param name="speed">Increment speed for editing the value.</param>
        /// <param name="min">Minimum value for the integer.</param>
        /// <param name="max">Maximum value for the integer.</param>
        public static void Add_Table_Row(string label, ref int value, float speed = 1, int min = 0, int max = 100) {

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Text(label);
            ImGui.TableSetColumnIndex(1);
            ImGui.DragInt($"##{label}", ref value, speed, min, max);
        }

        /// <summary>
        /// Adds a row to the current ImGui table with a label and an Action to execute.
        /// </summary>
        /// <param name="label">Label for the row.</param>
        /// <param name="lamda">Action to execute.</param>
        public static void Add_Table_Row(string label, Action lamda) {

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Text(label);
            ImGui.TableSetColumnIndex(1);
            lamda();
        }

        /// <summary>
        /// Adds spacing to the current ImGui table.
        /// </summary>
        /// <param name="amount">Amount of spacing rows to add.</param>
        public static void Add_Table_Spacing(int amout = 1) {

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            for (int x = 0; x < amout; x++)
                ImGui.Spacing();
            ImGui.TableSetColumnIndex(1);
            for (int x = 0; x < amout; x++)
                ImGui.Spacing();
        }

        /// <summary>
        /// Shifts the ImGui cursor position by the specified amount.
        /// </summary>
        /// <param name="shift_x">Amount to shift the cursor on the X-axis.</param>
        /// <param name="shift_y">Amount to shift the cursor on the Y-axis.</param>
        public static void Shift_Cursor_Pos(float shift_x = 0, float shift_y = 0) {

            var current_pos = ImGui.GetCursorPos();
            ImGui.SetCursorPos(current_pos + new System.Numerics.Vector2(shift_x, shift_y));
        }

        /// <summary>
        /// Displays a Title with specified font size.
        /// </summary>
        /// <param name="label">Label for the Title.</param>
        /// <param name="giant">Whether to Use a larger font size.</param>
        public static void Title(string label, bool giant = false) {

            if(giant)
                ImGui.PushFont(imgui_fonts.fonts["giant"]);
            else
                ImGui.PushFont(imgui_fonts.fonts["regular_big"]);
            ImGui.Text(label);
            ImGui.PopFont();
        }

        /// <summary>
        /// Displays a stylized progress bar with custom appearance.
        /// </summary>
        /// <param name="ratio">Ratio of progress (0.0f to 1.0f).</param>
        /// <param name="size">Size of the progress bar.</param>
        /// <param name="color">Color of the progress bar.</param>
        /// <param name="background_color">Background color of the progress bar.</param>
        /// <param name="length_of_mini_bar">Length ratio of the secondary bar.</param>
        /// <param name="height_of_mini_bar">Height ratio of the secondary bar.</param>
        /// <param name="slope">Slope of the triangle indicator.</param>
        public static void Progress_Bar_Stylised(float ratio, System.Numerics.Vector2? size = null, uint color = 4281591347, uint background_color = 4278190080, float length_of_mini_bar = 0.3f, float height_of_mini_bar = 0.3f, float slope = 1.0f) {

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
