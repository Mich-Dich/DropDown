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

        public static void add_table_row_string(string label, string value) {

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);

            ImGui.Text(label);

            ImGui.TableSetColumnIndex(1);

            ImGui.Text(value);
        }
    }
}
