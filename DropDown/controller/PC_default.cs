using Core.controllers;
using Core.input;
using Core.util;

namespace DropDown {

    public class PC_default : player_controller {


        public action move { get; set; }
        public action look { get; set; }

        public PC_default() {

            this.actions.Clear();

            move = new action(
                "move",
                false,
                action_type.VEC_2D,
                0f,
                new List<key_details> {

                    new key_details(key_code.W, 0, 0),
                    new key_details(key_code.S, 0, 0),
                    new key_details(key_code.A, 0, 0),
                    new key_details(key_code.D, 0, 0),
                });

            look = new action(
                "look",
                false,
                action_type.VEC_2D,
                0f,
                new List<key_details> {

                    new key_details(key_code.CursorPositionX, 0, 0),
                    new key_details(key_code.CursorPositionY, 0, 0),
                });
        }

        protected override void update(game_time delta_time) {

            move.get_value();


            Console.WriteLine("updating player_controller");
        }
    }
}
