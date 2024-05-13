
namespace DropDown.weapon {

    using Core.world;
    using Core.render;

    /// <summary>
    /// Represents a base class for a weapon in the game.
    /// </summary>
    public abstract class Base_Weapon : Game_Object {

        public Type projectile;

        /// <summary>
        /// Initializes a new instance of the Base_Weapon class with the specified parameters.
        /// </summary>
        /// <param name="weapon_sprite">The sprite representing the weapon.</param>
        /// <param name="projectile">The type of projectile this weapon fires.</param>
        /// <param name="muzzle_flash">The sprite representing the muzzle flash when firing.</param>
        /// <param name="magazin_capazity">The current magazine capacity of the weapon.</param>
        /// <param name="magazin_capazity_max">The maximum magazine capacity of the weapon.</param>
        protected Base_Weapon(Sprite weapon_sprite, Type projectile, Sprite? muzzle_flash, Int32 magazin_capazity, Int32 magazin_capazity_max) {

            Set_Sprite(weapon_sprite);
            _muzzle_flash = muzzle_flash;
            this.projectile = projectile;
            this.magazin_capazity = magazin_capazity;
            this.magazin_capazity_max = magazin_capazity_max;
        }

        /// <summary>
        /// Sets the muzzle flash sprite for this weapon.
        /// </summary>
        /// <param name="sprite">The muzzle flash sprite.</param>
        public void set_muzzle_flash(Sprite sprite) { _muzzle_flash = sprite; }

        /// <summary>
        /// Sets the muzzle flash animation for this weapon.
        /// If no existing muzzle flash is set, creates a new Sprite using the animation.
        /// </summary>
        /// <param name="animation">The muzzle flash animation.</param>
        public void set_muzzle_flash(Animation animation) {

            if(_muzzle_flash == null)
                _muzzle_flash = new Sprite(animation);
            else 
                _muzzle_flash.animation = animation;
        }

        // ============================== functions ============================== 

        /// <summary>
        /// Fires the weapon, decrementing the magazine capacity and triggering muzzle flash animation.
        /// </summary>
        public virtual void fire() {

            if(magazin_capazity <= 0)
                return;

            _muzzle_flash?.animation?.Play();
            magazin_capazity--;


            // TODO: spawn projectile 

        }

        /// <summary>
        /// Reloads the weapon with the specified amount of ammo.
        /// </summary>
        /// <param name="available_amount">The amount of ammo available for reloading.</param>
        /// <returns>The number of ammo used for reloading.</returns>
        public int reload(int available_amount = 0) {

            int reload_amount = Math.Min(magazin_capazity_max-magazin_capazity, available_amount);
            magazin_capazity += reload_amount;
            return reload_amount;
        }

        /// <summary>
        /// Draws the weapon and its muzzle flash.
        /// </summary>
        public override void Draw() {

            base.Draw();
            _muzzle_flash?.Draw();
        }

        // ============================== private ============================== 

        private int magazin_capazity = 42;
        private int magazin_capazity_max = 42;
        private Sprite?     _muzzle_flash;

    }
}
