using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropDown {
    internal class test_game : Core.Game {

        public test_game(String title, Int32 initalWindowWidth, Int32 initalWindowHeight) 
            : base(title, initalWindowWidth, initalWindowHeight) { }

        public override void StartGame() { }

        protected override void Init() { }

        protected override void Render(Single deltaTime) { }

        protected override void Render_Imgui(Single deltaTime) { }

        protected override void Shutdown() { }

        protected override void Update(Single deltaTime) { }
    }
}
