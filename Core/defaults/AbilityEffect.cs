namespace Core.defaults {
    using Core.world;
    using Core.render;
    using OpenTK.Mathematics;

    public class AbilityEffect : Game_Object
    {
        public string SpriteSheet { get; set; }
        public float Scale { get; set; }
        public Animation Animation { get; set; }
        public AbilityEffect(string spriteSheetPath, float scale, int columns, int rows, int fps = 30, bool loop = true) : base()
        {
            string directoryPath = Path.GetDirectoryName(spriteSheetPath);
            string fileName = Path.GetFileName(spriteSheetPath);

            if (string.IsNullOrEmpty(fileName))
            {
                string[] files = Directory.GetFiles(directoryPath, "*.png");
                if (files.Length > 0)
                {
                    fileName = Path.GetFileName(files[0]);
                }
            }

            string fullSpriteSheetPath = Path.Combine(directoryPath, fileName);

            this.SpriteSheet = fullSpriteSheetPath;
            this.transform.size = new Vector2(this.transform.size.X * scale, this.transform.size.Y * scale);

            Texture texture = new(fullSpriteSheetPath);
            Sprite sprite = new(texture);
            SpriteBatch spriteBatch = new(directoryPath);

            this.Animation = new Animation(sprite, spriteBatch, fps, loop);
            SetSprite(fullSpriteSheetPath);

            set_animation(fullSpriteSheetPath, columns, rows, fps, loop);
        }

        public void SetSprite(string spriteSheetPath)
        {
            Sprite sprite = new(new Texture(spriteSheetPath));
            this.Set_Sprite(sprite);
        }

        public void set_animation(string animationDataPath, int numOfColumns, int numOfRows, int fps, bool loop) {
            if(sprite != null) {
                Texture textureAtlas = new(animationDataPath);
                sprite.animation = new Animation(sprite, textureAtlas, numOfColumns, numOfRows, fps, loop);
            }

            sprite.animation.Play();
            Console.WriteLine("Set animation for Effect");
        }
    }
}