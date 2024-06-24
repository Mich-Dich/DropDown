
namespace DropDown.player {

    using Box2DX.Common;
    using Core;
    using Core.Controllers.player;
    using Core.defaults;
    using Core.util;
    using Core.world;
    using DropDown.spells;
    using OpenTK.Mathematics;

    public class PC_Default : Player_Controller {

        public Action move { get; set; }
        public Action look { get; set; }
        public Action sprint { get; set; }
        public Action interact { get; set; }
        public Action pause { get; set; }

        private float time_stap = 0;

        public PC_Default(Character character)
            : base (character, null) {

            actions.Clear();

            move = new Action(
                "move",
                (uint)Action_ModefierFlags.auto_reset,
                false,
                ActionType.VEC_2D,
                0f,
                new List<KeyBindingDetail> {

                    new(Key_Code.W, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_2 | KeyModefierFlags.negate),
                    new(Key_Code.S, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_2),
                    new(Key_Code.D, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_1),
                    new(Key_Code.A, ResetFlags.reset_on_key_up, TriggerFlags.key_down, KeyModefierFlags.axis_1 | KeyModefierFlags.negate),
                });
            AddInputAction(move);


            look = new Action(
                "look",
                (uint)Action_ModefierFlags.none,
                false,
                ActionType.VEC_1D,
                0f,
                new List<KeyBindingDetail> {

                    new(Key_Code.MouseWheelY, ResetFlags.reset_on_key_move_up, TriggerFlags.mouse_pos_and_neg),
                });
            AddInputAction(look);


            sprint = new Action(
                "shoot",
                (uint)Action_ModefierFlags.none,
                false,
                ActionType.BOOL,
                0f,
                new List<KeyBindingDetail> {

                    new(Key_Code.LeftShift, ResetFlags.reset_on_key_up, TriggerFlags.key_down),
                    new(Key_Code.RightShift, ResetFlags.reset_on_key_up, TriggerFlags.key_down),
                });
            AddInputAction(sprint);


            pause = new Action(
                "shoot",
                (uint)Action_ModefierFlags.none,
                false,
                ActionType.BOOL,
                0f,
                new List<KeyBindingDetail> {

                    new(Key_Code.P, ResetFlags.reset_on_key_down, TriggerFlags.key_down | TriggerFlags.key_up),
                    new(Key_Code.Pause, ResetFlags.reset_on_key_down, TriggerFlags.key_down),
                });
            AddInputAction(pause);

            
            interact = new Action(
                "shoot",
                (uint)Action_ModefierFlags.auto_reset,
                false,
                ActionType.BOOL,
                0f,
                new List<KeyBindingDetail> {

                    new(Key_Code.LeftMouseButton, ResetFlags.reset_on_key_down, TriggerFlags.key_down),
                });
            AddInputAction(interact);


            //Game.Instance.camera.Add_Zoom_Offset(0.2f);
            //Game.Instance.camera.zoom_offset = 0.2f;
        }

        protected override void Update(float deltaTime) {

            float total_speed = character.movement_speed;
            if((bool)sprint.GetValue()) 
                total_speed += sprint_speed;

            // simple movement
            if(move.X != 0 || move.Y != 0) {

                Vector2 direction = Vector2.NormalizeFast((Vector2)move.GetValue());
                character.Add_Linear_Velocity(new Vec2(direction.X, direction.Y) * total_speed * deltaTime);
            }

            // camera follows player
            Game.Instance.camera.transform.position = character.transform.position;
            
            // look at mouse
            character.rotate_to_vector(Game.Instance.Get_Mouse_Relative_Pos());
            
            // set zoom
            Game.Instance.camera.Add_Zoom_Offset((float)look.GetValue() / 50);

            if((bool)pause.GetValue())
                Game.Instance.pause(true);

            if ((bool)interact.GetValue()) {

                // mele attack
                List<Game_Object> intersected_game_objects = new List<Game_Object>();
                character.perception_check(ref intersected_game_objects, (float.Pi/2),  16, 2, 60, true, 1.5f);
                //Console.WriteLine($"hit objects count: {intersected_game_objects.Count}");
                foreach(var obj in intersected_game_objects) {

                    if(obj is Character intersected_character)
                        intersected_character.apply_damage(20);
                }
                ((CH_player)character).play_swing_sound(intersected_game_objects.Count > 0);


                // spawn projectily
                if((time_stap + projectile_data.cooldown.current) < Game_Time.total) {

                    Console.WriteLine($"cooldown: {projectile_data.cooldown.current}");

                    Vector2 proj_rot = util.vector_from_angle(character.transform.rotation);
                    Vector2 pooj_pos = character.transform.position + (proj_rot*110);
                    try {
                        var projectile = (Projectile)Activator.CreateInstance(ProjectileType, pooj_pos, proj_rot);
                        Game.Instance.get_active_map().Add_Game_Object(projectile);
                        time_stap = Game_Time.total;
                        Console.WriteLine($"setting time spamp {time_stap} cooldown: {projectile_data.cooldown.current}");
                    }
                    catch {
                        Console.WriteLine($"Exeption thrown when adding projectile");
                    }
                }

            }
        }

        private readonly float sprint_speed = 350.0f;
        private Type ProjectileType { get; set; } = typeof(P_base);

    }
}
