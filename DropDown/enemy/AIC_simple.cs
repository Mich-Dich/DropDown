
using System.Media;
using System;

namespace DropDown.enemy {

    using Core;
    using Core.Controllers.ai;
    using Core.util;
    using Core.world;
    using DropDown.player;
    using OpenTK.Mathematics;

    public class AIC_simple : AI_Controller {
        public AIC_simple(List<Character> characters) : base(characters) {

            if(characters.Count <= 0) {

                Console.WriteLine("Warning: No characters provided to AIC_simple.");
                return;
            }
            
            Character character = characters[0];
            get_state_machine().Set_Statup_State(typeof(idle));
            character.death_callback = () => {
            
                ((CH_player)Game.Instance.player).add_XP(((CH_base_NPC)character).XP);
                get_state_machine().force_set_state(typeof(death));
                character.health = 0;
                character.auto_heal_amout = 0;
                character.death_callback = () => { };
            };
        }
    }


    public class death : I_state<AI_Controller> {
        public bool exit(AI_Controller aiController) => true;

        public bool enter(AI_Controller aiController) {

            foreach(CH_base_NPC character in aiController.characters)
                character.set_animation_from_anim_data(character.death_anim);

            return true;
        }

        public Type execute(AI_Controller aiController, float deltaTime) => typeof(death);
    }

    public class idle : I_state<AI_Controller> {
        List<Game_Object> intersectedGameObjects = new List<Game_Object>();

        public bool exit(AI_Controller aiController) => true;

        public bool enter(AI_Controller aiController) {

            foreach(CH_base_NPC character in aiController.characters)
                character.set_animation_from_anim_data(character.idle_anim);
            
            return true;
        }

        public Type execute(AI_Controller aiController, float deltaTime) {

            if(Game.Instance.player.health < 0)
                return typeof(idle);

            foreach(CH_base_NPC character in aiController.characters) {
                intersectedGameObjects.Clear();
                character.perception_check(ref intersectedGameObjects, 0, character.ray_number, character.ray_cast_angle, character.ray_cast_range);
                
                if(intersectedGameObjects.Contains(Game.Instance.player))
                    return typeof(pursue_player);

                float playerDistance = (Game.Instance.player.transform.position - character.transform.position).LengthFast;
                if(playerDistance < character.auto_detection_range)
                    return typeof(pursue_player);
            }

            return typeof(idle);
        }
    }

    public class pursue_player : I_state<AI_Controller> {
        List<Game_Object> intersectedGameObjects = new List<Game_Object>();

        public bool exit(AI_Controller aiController) => true;

        public bool enter(AI_Controller aiController) {
            
            foreach(CH_base_NPC character in aiController.characters)
                character.set_animation_from_anim_data(character.walk_anim);
            
            return true;
        }

        public Type execute(AI_Controller aiController, float deltaTime) {
            
            foreach(CH_base_NPC character in aiController.characters) {
            
                Vector2 playerVec = Game.Instance.player.transform.position - character.transform.position;
                float playerDistance = playerVec.LengthFast;
                intersectedGameObjects.Clear();
                character.perception_check(ref intersectedGameObjects, 0, character.ray_number, character.ray_cast_angle, character.ray_cast_range);

                if(!intersectedGameObjects.Contains(Game.Instance.player) && playerDistance > character.auto_detection_range)
                    return typeof(idle);

                if(playerDistance < character.attack_range)
                    return typeof(attack_player);

                playerVec.NormalizeFast();
                character.Add_Linear_Velocity(new Box2DX.Common.Vec2(playerVec.X, playerVec.Y) * character.movement_speed * Game_Time.delta);
                character.rotate_to_vector_smooth(playerVec);
            }

            return typeof(pursue_player);
        }
    }

    public class attack_player : I_state<AI_Controller> {

        public bool exit(AI_Controller aiController) => true;

        public bool enter(AI_Controller aiController) {
            
            foreach(CH_base_NPC character in aiController.characters) {
                character.set_animation_from_anim_data(character.attack_anim);
                character.sprite.animation.add_animation_notification(21, () => {
                    var lookDir = util.vector_from_angle(character.transform.rotation - character.rotation_offset);
                    Vector2 start = character.transform.position + (lookDir * (character.transform.size.X / 2));
                    Vector2 end = start + (lookDir * (character.attack_range - (character.transform.size.X / 2)));

                    if(Game.Instance.get_active_map().ray_cast(start, end, out Box2DX.Common.Vec2 intersectionPoint, out float distance, out Game_Object intersectedGameObject, true, 0.5f)) {
                        if(intersectedGameObject is Character intersectedCharacter)
                            intersectedCharacter.apply_damage(character.damage);
                    }
                    character.play_attack_sound();
                });
            }
            
            return true;
        }

        public Type execute(AI_Controller aiController, float deltaTime) {

            if(Game.Instance.player.health < 0)
                return typeof(idle);

            foreach(CH_base_NPC character in aiController.characters) {
                
                Vector2 playerVec = Game.Instance.player.transform.position - character.transform.position;
                float playerDistance = playerVec.LengthFast;
                if(playerDistance > character.attack_range)
                    return typeof(pursue_player);

                playerVec.NormalizeFast();
                character.rotate_to_vector_smooth(playerVec);
            }

            return typeof(attack_player);
        }
    }
}
