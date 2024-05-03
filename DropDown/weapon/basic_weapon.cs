using Core.game_objects;
using Core.visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropDown.weapon {

    public /*abstract*/ class base_weapon : game_object {

        public base_weapon() {



        }

        public void set_sprite(sprite sprite) { this.weapon_sprite = sprite; }

        public void set_muzzle_flash(sprite sprite) { this.muzzle_flash = sprite; }

        // ============================== functions ============================== 

        public virtual void fire() {

            muzzle_flash?.animation.play();

        }

        public virtual void reload() {

        }

        public override void update() {
        
        }

        public override void draw() {

            this.muzzle_flash?.draw();
            base.draw();
        }

        // ============================== private ============================== 

        private int magazin_capazity = 42;
        private int fill_count = 42;

        private sprite      weapon_sprite;
        private sprite?     muzzle_flash;



    }
}
