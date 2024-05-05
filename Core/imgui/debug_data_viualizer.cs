using Core.util;
using ImGuiNET;
using System;
using System.Numerics;

namespace Core.imgui {

    public class debug_data_viualizer {

        public debug_data_viualizer() {

        }

        public void draw(debug_data debug_data) {

			ImGuiWindowFlags window_flags = ImGuiWindowFlags.NoDecoration 
				| ImGuiWindowFlags.NoDocking 
				| ImGuiWindowFlags.AlwaysAutoResize 
				| ImGuiWindowFlags.NoSavedSettings 
				| ImGuiWindowFlags.NoFocusOnAppearing 
				| ImGuiWindowFlags.NoNav 
				| ImGuiWindowFlags.NoMove;

			ImGui.SetNextWindowBgAlpha(0.8f);
			ImGui.SetNextWindowPos(new Vector2(20, 20));
			ImGui.Begin("debug_data_viualizer", window_flags);

			ImGui.Text("Game general-debugger");
			ImGui.Separator();

			imgui_util.begin_default_table("debug_data");
			imgui_util.add_table_row_string("draw calls", $"{debug_data.draw_calls_num}");
			imgui_util.add_table_row_string("collision checks", $"{debug_data.collision_checks_num}");

            imgui_util.end_default_table();


            ImGui.End();



			ImGui.SetNextWindowPos(new Vector2(665,350));
			ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0));
			ImGui.Begin("test_helth_bar", window_flags);
			ImGui.ProgressBar(0.8f, new Vector2(60, 5), "");
			ImGui.End();
			ImGui.PopStyleVar();


		}

        private bool show_window = true;

    }
}



/*
 

		ImGuiStyle& style = ImGui::GetStyle();
		ImVec2 current_item_spacing = style.ItemSpacing;
		flags |= ImGuiInputTextFlags_AllowTabInput;

		ImGui::TableNextRow();
		ImGui::TableSetColumnIndex(0);
		ImGui::Text("%s", label.data());

		ImGui::TableSetColumnIndex(1);

		if constexpr (std::is_same_v<T, bool>) {

			ImGui::Text("%s", util::bool_to_str(value));

*/