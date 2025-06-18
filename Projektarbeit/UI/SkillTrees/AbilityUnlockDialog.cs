using Core.defaults;
using System.Numerics;
using Core.UI;
using System.Collections.Generic;

namespace Projektarbeit.UI.SkillTrees
{
    public class AbilityUnlockDialog
    {
        public bool IsOpen { get; private set; }
        private List<UIElement> dialogElements = new List<UIElement>();
        private Ability currentAbility;
        private bool needsUpdate = true;

        public AbilityUnlockDialog() { IsOpen = false; }

        public void Open() { IsOpen = true; }
        public void Close() { IsOpen = false; }

        public void Render(Ability ability)
        {
            if (!IsOpen || ability == null) return;
            currentAbility = ability;
            if (needsUpdate)
            {
                UpdateDialogContent();
                needsUpdate = false;
            }
            foreach (var el in dialogElements) el.Render();
        }

        private void UpdateDialogContent()
        {
            dialogElements.Clear();
            Vector2 windowSize = new Vector2(Core.Game.Instance.window.Size.X, Core.Game.Instance.window.Size.Y);
            float dialogWidth = 500f;
            float dialogHeight = 350f;
            Vector2 dialogPos = new Vector2((windowSize.X - dialogWidth) / 2, (windowSize.Y - dialogHeight) / 2);
            dialogElements.Add(new Background(new Vector4(0, 0, 0, 0.7f)));
            dialogElements.Add(new Card(dialogPos, new Vector2(dialogWidth, dialogHeight), "Unlock Ability")
            {
                BackgroundColor = new Vector4(0.1f, 0.1f, 0.2f, 0.98f),
                GradientColor = new Vector4(0.15f, 0.1f, 0.25f, 0.98f),
                BorderColor = new Vector4(0.4f, 0.3f, 0.6f, 1.0f),
                BorderRadius = 20.0f,
                ShadowOffset = 8.0f,
                UseGradient = true
            });
            dialogElements.Add(new Text(dialogPos + new Vector2(dialogWidth / 2, 40), currentAbility.Name.ToUpper(), new Vector4(1.0f, 0.7f, 0.3f, 1.0f), 2.5f, TextAlign.Center)
            {
                UseShadow = true, ShadowOffset = new Vector2(2, 2), ShadowColor = new Vector4(0, 0, 0, 0.7f), UseGradient = true, GradientColor = new Vector4(1.0f, 0.9f, 0.5f, 1.0f), IsBold = true, LetterSpacing = 2.0f
            });
            dialogElements.Add(new Text(dialogPos + new Vector2(dialogWidth / 2, 100), currentAbility.Description, new Vector4(0.9f, 0.9f, 0.95f, 1.0f), 1.2f, TextAlign.Center)
            {
                UseShadow = true, ShadowOffset = new Vector2(1, 1), ShadowColor = new Vector4(0, 0, 0, 0.5f)
            });
            dialogElements.Add(new Text(dialogPos + new Vector2(dialogWidth / 2, 160), $"Unlock Cost: {currentAbility.UnlockCost}", new Vector4(0.8f, 0.8f, 1.0f, 1.0f), 1.4f, TextAlign.Center)
            {
                UseShadow = true, ShadowOffset = new Vector2(1, 1), ShadowColor = new Vector4(0, 0, 0, 0.5f), IsBold = true
            });
            float buttonY = dialogPos.Y + dialogHeight - 80f;
            float buttonWidth = 120f;
            float buttonHeight = 50f;
            dialogElements.Add(new Button(new Vector2(dialogPos.X + 30f, buttonY), new Vector2(buttonWidth, buttonHeight), "CANCEL", () => Close(), () => { },
                new Vector4(0.4f, 0.2f, 0.6f, 1.0f), new Vector4(0.5f, 0.3f, 0.7f, 1.0f), new Vector4(0.3f, 0.1f, 0.5f, 1.0f),
                new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f), false)
            {
                BorderRadius = 12.0f, UseGradient = true, UseShadow = true, ShadowOffset = 3.0f, AnimationSpeed = 0.2f
            });
            dialogElements.Add(new Button(new Vector2(dialogPos.X + dialogWidth - buttonWidth - 30f, buttonY), new Vector2(buttonWidth, buttonHeight), "UNLOCK", () => UnlockAbility(), () => { },
                Core.Game.Instance.GameState.Currency >= currentAbility.UnlockCost ? new Vector4(0.2f, 0.8f, 0.3f, 1.0f) : new Vector4(0.5f, 0.5f, 0.5f, 1.0f),
                Core.Game.Instance.GameState.Currency >= currentAbility.UnlockCost ? new Vector4(0.3f, 0.9f, 0.4f, 1.0f) : new Vector4(0.5f, 0.5f, 0.5f, 1.0f),
                Core.Game.Instance.GameState.Currency >= currentAbility.UnlockCost ? new Vector4(0.1f, 0.7f, 0.2f, 1.0f) : new Vector4(0.5f, 0.5f, 0.5f, 1.0f),
                new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f), false)
            {
                BorderRadius = 12.0f, UseGradient = true, UseShadow = true, ShadowOffset = 3.0f, AnimationSpeed = 0.2f
            });
        }

        private void UnlockAbility()
        {
            if (currentAbility != null && Core.Game.Instance.GameState.Currency >= currentAbility.UnlockCost)
            {
                currentAbility.Unlock();
                needsUpdate = true;
                Close();
            }
        }
    }
}