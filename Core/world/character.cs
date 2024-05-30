
namespace Core.world {

    using Box2DX.Common;
    using Core.controllers;
    using Core.Controllers.ai;
    using Core.defaults;
    using Core.physics;
    using Core.render;
    using Core.util;
    using ImGuiNET;
    using OpenTK.Mathematics;

    public class Character : Game_Object {

        public float movement_speed { get; set; } = 100.0f;
        public float movement_speed_max { get; set; } = 100.0f;
        public float movement_force { get; set; } = 100000.0f;
        public float movement_force_max { get; set; } = 100000.0f;
        public float auto_heal_amout { get; set; } = 5;
        public float health {
            get { return _health; }
            set {
                _health = value;
                UpdateHealthRatio();
            }
        }
        public float HealthRatio { get; private set; }
        public bool auto_remove_on_death = false;
        public bool IsDead { get; set; } = false;
        public float health_max {
            get { return _health_max; }
            set {
                _health_max = value;
                UpdateHealthRatio();
            }
        }
        public bool Invincible { get; set; } = false;
        public Action death_callback { get; set; }
        public Ability Ability { get; set; }
        public float abilityLastUsedTime;

        private List<PowerUp> all_power_ups = new List<PowerUp>();
        private float _health;
        private float _health_max;

        public Character() {
            _health = 100;
            _health_max = 100;
            transform.mobility = Mobility.DYNAMIC;
        }

        // ---------------------------------------------------------------------------------------------------------------
        // controller
        // ---------------------------------------------------------------------------------------------------------------

        public void Set_Controller(AI_Controller controller) 
        {
            this.controller = controller;
            controller.characters.Add(this);
        }

        // ---------------------------------------------------------------------------------------------------------------
        // default setters/getters
        // ---------------------------------------------------------------------------------------------------------------
        public void set_animation(Animation animation) {

            if(sprite != null)
                sprite.animation = animation;

            sprite.animation.Play();
        }

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

        public Vec2 Get_Velocity() {
            if(this.collider != null && this.collider.body != null)
                return this.collider.body.GetLinearVelocity();
            return Vec2.Zero;
        }

        public void add_force(Vec2 force) {

            if(this.collider != null && this.collider.body != null)
                this.collider.body.ApplyForce(force, Vec2.Zero);
        }

        //public void force_set_position(Vec2 new_position, float angle = 0) {

        //    this.transform.position = util.convert_Vector(new_position);
        //    if(this.collider != null)
        //        if(this.collider.body != null)
        //            collider.body.SetXForm(new_position, angle);

        //    Console.WriteLine($"new position: {new_position.X}/{new_position.Y}");
        //}

        // ---------------------------------------------------------------------------------------------------------------
        // interaction
        // ---------------------------------------------------------------------------------------------------------------

        public override void Hit(hitData hit) { }

        public virtual void apply_damage(float damage)
        {
            if (!Invincible) {
                health -= damage;
                if(health <= 0 && death_callback != null)
                    death_callback();
            }
        }

        public void perception_check(ref List<Game_Object> intersected_game_objects, float check_direction = 0, int num_of_rays = 6, float angle = float.Pi, float look_distance = 800, bool display_debug = false, float display_duration = 1f) {

            float angle_per_ray = angle / (float)(num_of_rays-1);
            for(int x = 0; x < num_of_rays; x++) {

                var look_dir = Core.util.util.vector_from_angle(transform.rotation - rotation_offset + check_direction - (angle/2) + (angle_per_ray * x));
                Vector2 start = transform.position + (look_dir * (transform.size.X/2));
                Vector2 end = start + (look_dir * look_distance);

                if(!Game.Instance.get_active_map().ray_cast(start, end, out Box2DX.Common.Vec2 intersection_point, out float distance, out var buffer, display_debug, display_duration))
                    continue;

                if(buffer != null)
                    if(!intersected_game_objects.Contains(buffer))
                        intersected_game_objects.Add(buffer);
            }

        }

        // ---------------------------------------------------------------------------------------------------------------
        // power up
        // ---------------------------------------------------------------------------------------------------------------

        public void add_power_up(PowerUp power_up) {
            if(all_power_ups.Contains(power_up))
                return;

            all_power_ups.Add(power_up);
            power_up.activation(this);
        }

        public void force_remove_power_up(PowerUp power_up) {
            if(!all_power_ups.Contains(power_up))
                return;

            power_up.deactivation(this);
            all_power_ups.Remove(power_up);
        }

        // ---------------------------------------------------------------------------------------------------------------
        // abilities
        // ---------------------------------------------------------------------------------------------------------------
        public void UseAbility() {
            var currentTime = Game_Time.total;
            if (currentTime - abilityLastUsedTime >= Ability.Cooldown) {
                Ability.Use(this);
                abilityLastUsedTime = currentTime;

                if (Ability.Effect != null) {
                    Ability.AddEffectToCharacter(this);
                    Ability.Effect.Animation.Play();
                }
            }
        }

        // ---------------------------------------------------------------------------------------------------------------
        // display
        // ---------------------------------------------------------------------------------------------------------------

        public void Display_Healthbar(System.Numerics.Vector2? display_size = null, System.Numerics.Vector2? pos_offset = null, System.Numerics.Vector2? padding = null, float rounding = 0.0f) {

            string UniqueId = $"Helthbar_for_character_{this.GetHashCode()}";
            if(display_size == null)
                display_size = new System.Numerics.Vector2(healthbar_width, healthbar_height);

            ImGuiWindowFlags window_flags = ImGuiWindowFlags.NoDecoration
                | ImGuiWindowFlags.NoDocking
                | ImGuiWindowFlags.AlwaysAutoResize
                | ImGuiWindowFlags.NoSavedSettings
                | ImGuiWindowFlags.NoFocusOnAppearing
                | ImGuiWindowFlags.NoNav
                | ImGuiWindowFlags.NoMove;

            System.Numerics.Vector2 position =
                Core.util.util.convert_Vector(Core.util.util.Convert_World_To_Screen_Coords(transform.position)) + (pos_offset?? System.Numerics.Vector2.Zero);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, padding ?? new System.Numerics.Vector2(4));

            if(rounding != 0.0f) {
                ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, rounding);
                ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, rounding);
            }

            ImGui.SetNextWindowPos(position, ImGuiCond.Always, new System.Numerics.Vector2(0.5f));
            ImGui.Begin(UniqueId, window_flags);

            Imgui_Util.Progress_Bar_Stylised(health / health_max,
                display_size,
                (health / health_max) > 0.3f ? healthbar_col_default : healthbar_col_almost_dead,
                healthbar_col_background,
                healthbar_length_of_mini_bar,
                healthbar_height_of_mini_bar,
                healthbar_slope);

            ImGui.End();
            ImGui.PopStyleVar();

            if(rounding != 0.0f)
                ImGui.PopStyleVar(2);

        }

        // ---------------------------------------------------------------------------------------------------------------
        // update
        // ---------------------------------------------------------------------------------------------------------------

        public void UpdateHealthRatio() {
            HealthRatio = health / health_max;
        }

        public override void Update(Single deltaTime) {
            base.Update(deltaTime);

            if(health < health_max)
                health += (auto_heal_amout * deltaTime);

            if(all_power_ups.Count >= 0) {
                List<PowerUp> power_ups_to_remove = new List<PowerUp>();

                foreach(var powerup in all_power_ups) {
                    if(Game_Time.total >= powerup.ActivationTime + powerup.Duration) {
                        powerup.deactivation(this);
                        power_ups_to_remove.Add(powerup);
                    }
                }

                foreach(var powerup in power_ups_to_remove)
                    all_power_ups.Remove(powerup);
            }

            UpdateHealthRatio();
        }

        // healthbar
        public float healthbar_width = 100;
        public float healthbar_height = 8;
        public float healthbar_length_of_mini_bar = 0f;
        public float healthbar_height_of_mini_bar= 0f;
        public float healthbar_slope= 0.35f;

        uint healthbar_col_default = 4291572531;         // BLUE    => 0.2f,    0.2f,   0.8f,   1f
        uint healthbar_col_almost_dead = 4281545702;     // RED     => 0.9f,    0.2f,   0.2f,   1f
        uint healthbar_col_background = 4278190080;      // BLACK   => 0f,      0f,     0f,     1f

        // ================================================== private ==================================================

        private I_Controller? controller;

    }
}
