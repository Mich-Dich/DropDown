using Core.game_objects;
using Core.physics;
using Core.physics.material;
using Core.util;
using Core.visual;
using Core.defaults;
using OpenTK.Mathematics;
using Core.input;

namespace Hell
{

    public class player : character
    {

        public player()
        {

            transform.size = new Vector2(100);
            this.transform.position = new Vector2(480, 6080);
            set_sprite(new sprite(resource_manager.get_texture("assets/textures/Angel-1/Angel-1.png")));
            add_collider(new collider(collision_shape.Circle) { Blocking = true })
                //.set_offset(new transform(Vector2.Zero, new Vector2(-10)))
                .set_mobility(mobility.DYNAMIC);

            movement_speed = 2000.0f;
        }


        public override void hit(hit_data hit) { }

    }

    /*
        public class player : character, I_character_actions {

            public float gravity { get; set; } = 150.0f;
            public float jumpForce { get; set; } = 100.0f;
            public bool isJumping { get; set; } = false;
            public float dashForce { get; set; } = 10.0f;

            public player() {
                this.transform.size = new Vector2(100);
                set_sprite(new sprite(resource_manager.get_texture("assets/textures/Spaceship/Spaceship.png", true)));
                add_collider(new collider(collision_shape.Circle)).set_mobility(mobility.DYNAMIC);

                this.movement_speed = 60.0f;
            }

            public override void hit(hit_data hit) {
                if (hit.hit_object.collider.type == collision_type.world) {
                    Console.WriteLine("Player landed");
                    isJumping = false;
                }
            }

            public void jump() {
                if (!isJumping) {
                    Vector2 newVelocity = this.collider.velocity;
                    newVelocity.Y -= jumpForce;
                    this.collider.velocity = newVelocity;
                    isJumping = true;
                }
            }

            public void dash() {
                Vector2 newVelocity = this.collider.velocity;
                newVelocity.X += this.dashForce;
                this.collider.velocity = newVelocity;
                isJumping = false;
            }

            public override void update(float delta_time) {
                base.update(delta_time);

                Vector2 newVelocity = this.collider.velocity;

                newVelocity.Y += gravity * delta_time;

                if (!isJumping) {
                    newVelocity.X *= 1 - this.collider.material.friction * 5;
                    applyGroundFriction();
                } else {
                    newVelocity.X *= 1 - this.collider.material.friction;
                }

                this.collider.velocity = newVelocity;
            }

            public void applyGroundFriction() {
                if (!isJumping && this.collider.velocity.Length > 0.1f) {
                    Vector2 frictionForce = -this.collider.velocity.Normalized() * this.collider.material.friction;
                    this.collider.velocity -= frictionForce;
                }
            }
        }

        */
}