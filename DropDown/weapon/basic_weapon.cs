
namespace DropDown.weapon
{

    using Core.world;
    using Core.render;

    public abstract class Base_Weapon : Game_Object {

        public Type projectile;

        protected Base_Weapon(Sprite weapon_sprite, Type projectile, Sprite? muzzle_flash, int magazin_capazity, int magazin_capazity_max) {

            Set_Sprite(weapon_sprite);
            _muzzle_flash = muzzle_flash;
            this.projectile = projectile;
            this.magazin_capazity = magazin_capazity;
            this.magazin_capazity_max = magazin_capazity_max;
        }

        public void set_muzzle_flash(Sprite sprite) { _muzzle_flash = sprite; }

        public void set_muzzle_flash(Animation animation) {

            if(_muzzle_flash == null)
                _muzzle_flash = new Sprite(animation);
            else 
                _muzzle_flash.animation = animation;
        }

        // ============================== functions ============================== 

        public virtual void fire() {

            if(magazin_capazity <= 0)
                return;

            _muzzle_flash?.animation?.Play();
            magazin_capazity--;


            // TODO: spawn projectile 

        }

        public int reload(int available_amount = 0) {

            int reload_amount = Math.Min(magazin_capazity_max-magazin_capazity, available_amount);
            magazin_capazity += reload_amount;
            return reload_amount;
        }

        public override void Draw() {

            base.Draw();
            _muzzle_flash?.Draw();
        }

        // ============================== private ============================== 

        private int magazin_capazity = 42;
        private readonly int magazin_capazity_max = 42;
        private Sprite?     _muzzle_flash;

    }
}
