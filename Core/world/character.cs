
namespace Core.world {
                                                                            
    using Box2DX.Common;
    using Core.controllers;
    using Core.physics;
    using Core.render;
    using ImGuiNET;
    using OpenTK.Mathematics;

    public class Character : Game_Object {

        public float    movementSpeed { get; set; } = 100.0f;
        public float    movementSpeed_max { get; set; } = 100.0f;
        public int      health { get; set; } = 100;
        public int      health_max { get; set; } = 100;

        /// <summary>
        /// Constructs a new Character instance.
        /// </summary>
        public Character() {

            //this.set_sprite(new visual.sprite(resource_manager.get_texture("./assets/textures/Spaceship/Spaceship.png")));
        }

        /// <summary>
        /// Sets the controller for this character.
        /// </summary>
        /// <param name="controller">The controller to assign to this character.</param>
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

        /// <summary>
        /// Adds to the current velocity of the character.
        /// </summary>
        /// <param name="new_velocity">The additional velocity to add to the character's current velocity.</param>
        public void Add_Linear_Velocity(Vec2 add_velocity) {
            
            if(this.collider != null && collider.body != null)
                collider.body.SetLinearVelocity(collider.body.GetLinearVelocity() + add_velocity);
        }

        public void Set_Velocity(Vec2 new_velocity) {

            if(this.collider != null && collider.body != null)
                collider.body.SetLinearVelocity(new_velocity);
        }

        public void add_force(Vec2 force) {

            if(this.collider != null && collider.body != null)
                collider.body.ApplyForce(force, Vec2.Zero);
        }
        
        public void Update_position() {

            Vec2 pos = this.collider.body.GetPosition();
            this.transform.position = (pos.X, pos.Y);
        }

        /// <summary>
        /// Handles the character being hit by an external force or attack.
        /// </summary>
        /// <param name="hit">Data representing the hit event.</param>
        public override void Hit(hitData hit) {

            Console.WriteLine($"character [{this}] was hit");
        }

        /// <summary>
        /// Displays the health bar of the character using ImGui.
        /// </summary>
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
            ImGui.ProgressBar(0.8f, new System.Numerics.Vector2(60, 5), "");
            ImGui.End();
            ImGui.PopStyleVar();
            
        }

        // ================================================== private ==================================================

        private I_Controller? controller;



    }
}
