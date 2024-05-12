using Core.game_objects;
using Core.physics;
using Core.physics.material;
using Core.util;
using Core.visual;
using Core.defaults;
using OpenTK.Mathematics;
using Core.input;
using Core;
using System.Collections.Generic;

namespace Hell
{
    public class player : character
    {
        private const float ShootCooldown = 0.3f;
        private float lastShootTime = -ShootCooldown;
        private float movementThreshold = 400;
        public Vector2 playerSpawn { get; set; } = new Vector2(480, 6080);
        private float levelFloor;

        public player()
        {
            transform.size = new Vector2(100);
            this.transform.position = playerSpawn;
            set_sprite(new sprite(resource_manager.get_texture("assets/textures/Angel-1/Angel-1.png")));
            add_collider(new collider(collision_shape.Circle) { Blocking = false })
                //.set_offset(new transform(Vector2.Zero, new Vector2(-10)))
                .set_mobility(mobility.DYNAMIC);

            movement_speed = 300.0f;
            this.health = 100; // Initialize health from the base class

            levelFloor = playerSpawn.Y * 1.01f;
        }

        public override void hit(hit_data hit)
        {
            if (hit.hit_object.collider.type == collision_type.enemy_bullet)
            {
                this.health -= (int)hit.hit_object.collider.damage; // Use base class health property

                game.instance.active_map.remove_game_object(hit.hit_object);

                if (this.health <= 0) // Use base class health property
                {
                    game.instance.active_map.remove_game_object(this);
                    Console.WriteLine("Player has died");
                }
            }
        }

        public void Shoot()
        {
            if (game_time.total - lastShootTime >= ShootCooldown)
            {
                Vector2 direction = -Vector2.UnitY;
                float speed = 1000f;

                var proj = new projectile((this.transform.position), direction, speed, 5f, new Vector2(20, 60), 25.0f);
                game.instance.active_map.add_game_object(proj);
                lastShootTime = game_time.total;
            }
        }

        public override void update(float delta_time)
        {
            base.update(delta_time);

            this.display_helthbar(this.health); // Use base class health property

            if (this.transform.position.X < playerSpawn.X - movementThreshold)
            {
                this.transform.position = new Vector2(playerSpawn.X - movementThreshold, this.transform.position.Y);
            }
            else if (this.transform.position.X > playerSpawn.X + movementThreshold)
            {
                this.transform.position = new Vector2(playerSpawn.X + movementThreshold, this.transform.position.Y);
            }

            if (this.transform.position.Y > levelFloor)
            {
                this.transform.position = new Vector2(this.transform.position.X, levelFloor);
            }

            //Console.WriteLine(this.health); // Use base class health property
        }
    }
}