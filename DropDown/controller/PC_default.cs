﻿using Core.controllers;
using Core.input;

namespace DropDown {

    public class PC_default : player_controller {

        public PC_default() {

            this.actions.Clear();

            add_input_action(new action(
                "move",
                false,
                action_type.VEC_2D,
                0f,
                new List<key_details> {

                    key_details.create(key_code.W, 0, 0),
                    key_details.create(key_code.S, 0, 0),
                    key_details.create(key_code.A, 0, 0),
                    key_details.create(key_code.D, 0, 0),
                }));

            add_input_action(new action(
                "look",
                false,
                action_type.VEC_2D,
                0f,
                new List<key_details> {

                    key_details.create(key_code.CursorPositionX, 0, 0),
                    key_details.create(key_code.CursorPositionY, 0, 0),
                }));

        }

    }
}
