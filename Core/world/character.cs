
namespace Core.world {

    using Box2DX.Common;
    using Core.Controllers;
    using Core.Controllers.ai;
    using Core.physics;
    using Core.render;
    using ImGuiNET;
    using OpenTK.Mathematics;

    public class Character : Game_Object {

        public float movement_speed { get; set; } = 100.0f;
        public float movement_speed_max { get; set; } = 100.0f;
        public float movement_force { get; set; } = 100000.0f;
        public float movement_force_max { get; set; } = 100000.0f;
        public int health { get; set; } = 100;
        public int health_max { get; set; } = 100;



        public Character() {

            // this.set_sprite(new visual.sprite(resource_manager.get_texture("./assets/textures/Spaceship/Spaceship.png")));
        }

        public void Set_Controller(I_Controller controller) {

            this.controller = controller;
            controller.character = this;
        }

        public void set_animation(Animation animation) {

            if(sprite != null)
                sprite.animation = animation;

            sprite.animation.Play();
        }

        /// <summary>
        /// Sets the velocity of the character.
        /// </summary>
        /// <param name="new_velocity">The new velocity to set for the character.</param>
        public void Set_Velocity(Vector2 new_velocity) {

            if(this.collider != null)
                this.collider.velocity = new_velocity;
        }

        public void Add_Linear_Velocity(Vec2 add_velocity) {

            if(this.collider != null && this.collider.body != null) 
                this.collider.body.SetLinearVelocity(this.collider.body.GetLinearVelocity() + add_velocity);
        }

        public void Set_Velocity(Vec2 new_velocity) {

            if(this.collider != null && this.collider.body != null) 
                this.collider.body.SetLinearVelocity(new_velocity);
        }

        public void add_force(Vec2 force) {

            if(this.collider != null && this.collider.body != null) 
                this.collider.body.ApplyForce(force, Vec2.Zero);
        }

        public void Update_position() {

            Vec2 pos = this.collider.body.GetPosition();
            this.transform.position = (pos.X, pos.Y);
        }

        public override void Hit(hitData hit) {

            Console.WriteLine($"character [{this}] was hit");
        }
        
        public void rotate_to_move_dir() {

            Vec2 movement_dir = collider.body.GetLinearVelocity();
            movement_dir.Normalize();
            float angleRadians = (float)System.Math.Atan2(movement_dir.X, movement_dir.Y);
            transform.rotation = -angleRadians + float.Pi;
        }

        public void rotate_to_vector(Vec2 dir) {

            float angleRadians = (float)System.Math.Atan2(dir.X, dir.Y);
            transform.rotation = -angleRadians + float.Pi;
        }

        public void rotate_to_vector(Vector2 dir, float lerp_t = 0.1f) {

            float angleRadians = (float)System.Math.Atan2(dir.X, dir.Y);
            transform.rotation = Util.Lerp(transform.rotation, -angleRadians + float.Pi, lerp_t);
        }

        public void Display_Healthbar() {

            ImGuiWindowFlags window_flags = ImGuiWindowFlags.NoDecoration
                | ImGuiWindowFlags.NoDocking
                | ImGuiWindowFlags.AlwaysAutoResize
                | ImGuiWindowFlags.NoSavedSettings
                | ImGuiWindowFlags.NoFocusOnAppearing
                | ImGuiWindowFlags.NoNav
                | ImGuiWindowFlags.NoMove;

            ImGui.SetNextWindowPos(new System.Numerics.Vector2(665, 350));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new System.Numerics.Vector2(0));
            ImGui.Begin("test_helth_bar", window_flags);
            ImGui.ProgressBar(0.8f, new System.Numerics.Vector2(60, 5), string.Empty);
            ImGui.End();
            ImGui.PopStyleVar();
        }

        // ================================================== private ==================================================

        private I_Controller? controller;

    }
}
