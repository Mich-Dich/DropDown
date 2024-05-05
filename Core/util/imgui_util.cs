using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Core.util {

    public static class imgui_util {
    
    
        public static void begin_default_table(string label, bool display_lable = false) {

            if(display_lable)
                ImGui.Text(label);

            ImGuiTableFlags flags = ImGuiTableFlags.Resizable;
            ImGui.BeginTable(label, 2, flags);
            ImGui.TableSetupColumn("##one", ImGuiTableColumnFlags.NoHeaderLabel);
            ImGui.TableSetupColumn("##two", ImGuiTableColumnFlags.NoHeaderLabel | ImGuiTableColumnFlags.NoResize);

        }

        public static void end_default_table() {

            ImGui.EndTable();
        }

        public static void title(string label, bool giant = false) {

            if (giant)
                ImGui.PushFont(imgui_fonts.fonts["giant"]);
            else
                ImGui.PushFont(imgui_fonts.fonts["regular_big"]);
            ImGui.Text(label);
            ImGui.PopFont();
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

        public static void add_table_spacing() {

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.TableSetColumnIndex(1);
            ImGui.Spacing();
            ImGui.Spacing();
        }

        public static void shift_cursor_pos(float shift_x = 0, float shift_y = 0) {

            var current_pos = ImGui.GetCursorPos();
            ImGui.SetCursorPos(current_pos + new System.Numerics.Vector2(shift_x, shift_y));
        }

    }
}
