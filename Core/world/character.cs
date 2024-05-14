
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
        public float health { get; set; } = 100;
        public float health_max { get; set; } = 100;

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

        public override void Hit(hitData hit) {

            Console.WriteLine($"character [{this}] was hit");
        }
        
        public void perception_check(ref List<Type> intersected_game_objects, int num_of_rays = 6, float angle = float.Pi, float look_distance = 800) {

            float angle_per_ray = angle / (float)(num_of_rays-1);
            for(int x = 0; x < num_of_rays; x++) {

                var look_dir = Util.vector_from_angle(transform.rotation - rotation_offset - (angle/2) + (angle_per_ray * x));
                Vector2 start = transform.position + (look_dir * (transform.size.X/2));
                Vector2 end = start + (look_dir * look_distance);

                if(!Game.Instance.get_active_map().ray_cast(start, end, out Box2DX.Common.Vec2 intersection_point, out float distance, out var buffer, true, 0))
                    continue;
                    
                if(buffer != null)
                    intersected_game_objects.Add(buffer.GetType());
            }

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
        private float Lerp(float a, float b, float t)
        {
            return (1 - t) * a + t * b;
        }
        private I_Controller? controller;

    }
}
