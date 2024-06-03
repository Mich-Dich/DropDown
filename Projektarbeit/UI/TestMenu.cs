using Core.UI;
using System.Numerics;

namespace Hell.UI {
    public class TestMenu : Menu {
        public TestMenu() {
            // Create a floating Text
            var floatingText = new Text(new Vector2(300, 300), "Floating Text");
            AddElement(floatingText);

            // Create a VerticalBox
            var verticalBox = new VerticalBox(new Vector2(400, 400), new Vector2(100, 200), new Vector2(10, 10));

            // Create two ProgressBars
            var progressBar1 = new ProgressBar(
                new Vector2(0, 0), 
                new Vector2(100, 10), 
                new Vector4(1, 0, 0, 1), 
                new Vector4(0, 0, 0, 1), 
                () => 0.5f, 
                0, 
                100, 
                false
            );

            var progressBar2 = new ProgressBar(
                new Vector2(0, 0), 
                new Vector2(100, 10), 
                new Vector4(0, 1, 0, 1), 
                new Vector4(0, 0, 0, 1), 
                () => 0.75f, 
                0, 
                100, 
                false
            );

            // Add ProgressBars to the VerticalBox
            verticalBox.AddElement(progressBar1);
            verticalBox.AddElement(progressBar2);

            // Add VerticalBox to the Menu
            AddElement(verticalBox);
        }
    }
}