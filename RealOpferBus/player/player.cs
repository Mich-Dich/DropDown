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
            add_collider(new collider(Collision_Shape.Circle) { blocking = false })
                //.Set_Offset(new transform(Vector2.Zero, new Vector2(-10)))
                .Set_Mobility(mobility.DYNAMIC);

            movementSpeed = 300.0f;
            this.health = 100; // Initialize health from the base class

            levelFloor = playerSpawn.Y * 1.01f;
        }

        public override void hit(hitData hit)
        {
            if (hit.hitObject.collider.type == Collision_Type.enemy_bullet)
            {
                this.health -= (int)hit.hitObject.collider.damage; // Use base class health property

                game.instance.activeMap.remove_game_object(hit.hitObject);

                if (this.health <= 0) // Use base class health property
                {
                    game.instance.activeMap.remove_game_object(this);
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
                game.instance.activeMap.add_game_object(proj);
                lastShootTime = game_time.total;
            }
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

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