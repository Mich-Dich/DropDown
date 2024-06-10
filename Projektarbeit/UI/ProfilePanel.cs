namespace Projektarbeit.UI
{
    using System.Numerics;
    using Core.UI;

    public class ProfilePanel : UIElement
    {
        public ProfilePanel(Vector2 position) : base(position, new Vector2(200, 120))
        {
            var xpBar = new ProgressBar(
                position + new Vector2(10, 10),
                new Vector2(200, 20),
                new Vector4(0, 1, 0, 1),
                new Vector4(0.1f, 0.1f, 0.1f, 1),
                () => { try { return Core.Game.Instance.GameState.GetXPProgress(); } catch { return 0; } },
                0,
                1,
                true
            );
            AddElement(xpBar);

            var levelText = new Text(
                position + new Vector2(110, 60),
                new Func<string>(() => 
                {
                    try 
                    {
                        return $"Level: {Core.Game.Instance.GameState.AccountLevel}";
                    } 
                    catch 
                    {
                        return "Level: Error";
                    }
                }),
                Vector4.One,
                1f,
                new Vector2(200, 50)
            );
            AddElement(levelText);

            var currencyText = new Text(
                position + new Vector2(200, 60),
                new Func<string>(() => 
                {
                    try 
                    {
                        return $"Currency: {Core.Game.Instance.GameState.Currency}";
                    } 
                    catch 
                    {
                        return "Currency: Error";
                    }
                }),
                Vector4.One,
                1f,
                new Vector2(200, 50)
            );
            AddElement(currencyText);
        }

        public override void Render()
        {
            foreach (var element in Elements)
            {
                element.Render();
            }
        }
    }
}