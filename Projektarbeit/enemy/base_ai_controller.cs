namespace Hell.enemy {

    using Core;
    using Core.Controllers.ai;
    using Core.util;
    using Core.world;
    using Core.render;
    using OpenTK.Mathematics;
    using System;

    public class Base_AI_Controller : AI_Controller {

        public Base_AI_Controller(Character character) 
            : base(character) {

            Set_Statup_State(typeof(EnterScreen));
        }
    }

    public class EnterScreen : I_AI_State {

        public bool Exit(AI_Controller aI_Controller) { return true; }
        public bool Enter(AI_Controller aI_Controller) {

            aI_Controller.character.sprite.set_animation("assets/animation/small_bug/idle_01.png", 16, 10, true, false, 30, true);
            return true;
        }

        public Type Execute(AI_Controller aI_Controller) {

            // Move the enemy onto the screen from the top
            aI_Controller.character.add_force(new Box2DX.Common.Vec2(0, -1) * aI_Controller.character.movement_force * Game_Time.delta);

            // Once the enemy is fully on screen, switch to the MoveInPattern state
            if (aI_Controller.character.transform.position.Y < Game.Instance.window.ClientSize.Y - aI_Controller.character.transform.size.Y / 2) {
                return typeof(MoveInPattern);
            }

            return typeof(EnterScreen);
        }
    }

    public class MoveInPattern : I_AI_State {

        public bool Exit(AI_Controller aI_Controller) { return true; }
        public bool Enter(AI_Controller aI_Controller) {

            aI_Controller.character.sprite.set_animation("assets/animation/small_bug/walk.png", 8, 4, true, false, 80, true);
            return true;
        }

        public Type Execute(AI_Controller aI_Controller) {

            // Move the enemy in a pattern (e.g., a sine wave)
            float x = (float)Math.Sin(Game_Time.total) * aI_Controller.character.movement_speed_max * Game_Time.delta;
            float y = -aI_Controller.character.movement_speed * Game_Time.delta;
            aI_Controller.character.add_force(new Box2DX.Common.Vec2(x, y) * aI_Controller.character.movement_force * Game_Time.delta);

            // If the enemy has moved off the bottom of the screen, switch to the ExitScreen state
            if (aI_Controller.character.transform.position.Y < -aI_Controller.character.transform.size.Y / 2) {
                return typeof(ExitScreen);
            }

            // Periodically switch to the Shoot state to fire bullets
            if (Game_Time.total % 1 < 0.01) { // Adjust the 1 to change how often the enemy shoots
                return typeof(Shoot);
            }

            return typeof(MoveInPattern);
        }
    }

    public class Shoot : I_AI_State {

        public bool Exit(AI_Controller aI_Controller) { return true; }
        public bool Enter(AI_Controller aI_Controller) {

            aI_Controller.character.sprite.set_animation("assets/animation/small_bug/attack_01.png", 8, 3, true, false, 30, true);
            return true;
        }

        public Type Execute(AI_Controller aI_Controller) {

            // Fire a bullet
            ((Base_Enemy)aI_Controller.character).FireBullet();

            // Immediately switch back to the MoveInPattern state
            return typeof(MoveInPattern);
        }
    }

    public class ExitScreen : I_AI_State {

        public bool Exit(AI_Controller aI_Controller) { return true; }
        public bool Enter(AI_Controller aI_Controller) {

            aI_Controller.character.sprite.set_animation("assets/animation/small_bug/idle_01.png", 16, 10, true, false, 30, true);
            return true;
        }

        public Type Execute(AI_Controller aI_Controller) {

            // Move the enemy off the screen and then remove it
            aI_Controller.character.add_force(new Box2DX.Common.Vec2(0, -1) * aI_Controller.character.movement_force * Game_Time.delta);

            if (aI_Controller.character.transform.position.Y < -aI_Controller.character.transform.size.Y) {
                Game.Instance.get_active_map().Remove_Game_Object(aI_Controller.character);
            }

            return typeof(ExitScreen);
        }
    }
}