using Core.defaults;
using System.Numerics;
using Core.UI;
using Projektarbeit.characters.player.abilities;
using Core.util;
using System.Collections.Generic;

namespace Projektarbeit.UI.SkillTrees
{
    public class AbilityUpgradeDialog
    {
        public bool IsOpen { get; private set; }
        private List<UIElement> dialogElements = new List<UIElement>();
        private Ability currentAbility;
        private bool needsUpdate = true;

        public AbilityUpgradeDialog() { IsOpen = false; }
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
            float dialogHeight = 400f;
            Vector2 dialogPos = new Vector2((windowSize.X - dialogWidth) / 2, (windowSize.Y - dialogHeight) / 2);
            dialogElements.Add(new Background(new Vector4(0, 0, 0, 0.7f)));
            dialogElements.Add(new Card(dialogPos, new Vector2(dialogWidth, dialogHeight), "Upgrade Ability")
            {
                BackgroundColor = new Vector4(0.1f, 0.1f, 0.2f, 0.98f),
                GradientColor = new Vector4(0.15f, 0.1f, 0.25f, 0.98f),
                BorderColor = new Vector4(0.4f, 0.3f, 0.6f, 1.0f),
                BorderRadius = 20.0f,
                ShadowOffset = 8.0f,
                UseGradient = true
            });
            dialogElements.Add(new Text(dialogPos + new Vector2(dialogWidth / 2, 30), currentAbility.Name.ToUpper(), new Vector4(1.0f, 0.7f, 0.3f, 1.0f), 2.5f, TextAlign.Center)
            {
                UseShadow = true, ShadowOffset = new Vector2(2, 2), ShadowColor = new Vector4(0, 0, 0, 0.7f), UseGradient = true, GradientColor = new Vector4(1.0f, 0.9f, 0.5f, 1.0f), IsBold = true, LetterSpacing = 2.0f
            });
            dialogElements.Add(new Text(dialogPos + new Vector2(dialogWidth / 2, 80), currentAbility.Description, new Vector4(0.9f, 0.9f, 0.95f, 1.0f), 1.1f, TextAlign.Center)
            {
                UseShadow = true, ShadowOffset = new Vector2(1, 1), ShadowColor = new Vector4(0, 0, 0, 0.5f)
            });
            dialogElements.Add(new Text(dialogPos + new Vector2(dialogWidth / 2, 130), $"Level: {currentAbility.Level}", new Vector4(0.8f, 1.0f, 0.8f, 1.0f), 1.3f, TextAlign.Center)
            {
                UseShadow = true, ShadowOffset = new Vector2(1, 1), ShadowColor = new Vector4(0, 0, 0, 0.5f), IsBold = true
            });
            dialogElements.Add(new Text(dialogPos + new Vector2(dialogWidth / 2, 170), $"Upgrade Cost: {currentAbility.UnlockCost}", new Vector4(0.8f, 0.8f, 1.0f, 1.0f), 1.3f, TextAlign.Center)
            {
                UseShadow = true, ShadowOffset = new Vector2(1, 1), ShadowColor = new Vector4(0, 0, 0, 0.5f), IsBold = true
            });
            float buttonY = dialogPos.Y + dialogHeight - 70f;
            float buttonWidth = 110f;
            float buttonHeight = 45f;
            dialogElements.Add(new Button(new Vector2(dialogPos.X + 20f, buttonY), new Vector2(buttonWidth, buttonHeight), "CANCEL", () => Close(), () => { },
                new Vector4(0.4f, 0.2f, 0.6f, 1.0f), new Vector4(0.5f, 0.3f, 0.7f, 1.0f), new Vector4(0.3f, 0.1f, 0.5f, 1.0f),
                new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f), false)
            {
                BorderRadius = 10.0f, UseGradient = true, UseShadow = true, ShadowOffset = 3.0f, AnimationSpeed = 0.2f
            });
            dialogElements.Add(new Button(new Vector2(dialogPos.X + dialogWidth / 2 - buttonWidth / 2, buttonY), new Vector2(buttonWidth, buttonHeight), "UPGRADE", () => UpgradeAbility(), () => { },
                Core.Game.Instance.GameState.Currency >= currentAbility.UnlockCost ? new Vector4(0.2f, 0.8f, 0.3f, 1.0f) : new Vector4(0.5f, 0.5f, 0.5f, 1.0f),
                Core.Game.Instance.GameState.Currency >= currentAbility.UnlockCost ? new Vector4(0.3f, 0.9f, 0.4f, 1.0f) : new Vector4(0.5f, 0.5f, 0.5f, 1.0f),
                Core.Game.Instance.GameState.Currency >= currentAbility.UnlockCost ? new Vector4(0.1f, 0.7f, 0.2f, 1.0f) : new Vector4(0.5f, 0.5f, 0.5f, 1.0f),
                new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f), false)
            {
                BorderRadius = 10.0f, UseGradient = true, UseShadow = true, ShadowOffset = 3.0f, AnimationSpeed = 0.2f
            });
            dialogElements.Add(new Button(new Vector2(dialogPos.X + dialogWidth - buttonWidth - 20f, buttonY), new Vector2(buttonWidth, buttonHeight), currentAbility.IsEquipped ? "UNEQUIP" : "EQUIP", () => ToggleEquip(), () => { },
                currentAbility.IsEquipped ? new Vector4(0.8f, 0.3f, 0.3f, 1.0f) : new Vector4(0.3f, 0.6f, 0.8f, 1.0f),
                currentAbility.IsEquipped ? new Vector4(0.9f, 0.4f, 0.4f, 1.0f) : new Vector4(0.4f, 0.7f, 0.9f, 1.0f),
                currentAbility.IsEquipped ? new Vector4(0.7f, 0.2f, 0.2f, 1.0f) : new Vector4(0.2f, 0.5f, 0.7f, 1.0f),
                new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f), false)
            {
                BorderRadius = 10.0f, UseGradient = true, UseShadow = true, ShadowOffset = 3.0f, AnimationSpeed = 0.2f
            });
        }

        private void UpgradeAbility()
        {
            if (currentAbility != null && Core.Game.Instance.GameState.Currency >= currentAbility.UnlockCost)
            {
                currentAbility.Upgrade();
                int index = Core.Game.Instance.GameState.Abilities.IndexOf(currentAbility);
                if (index != -1)
                {
                    Core.Game.Instance.GameState.Abilities[index] = currentAbility;
                    GameStateManager.SaveGameState(Core.Game.Instance.GameState, "save.json");
                }
                needsUpdate = true;
            }
        }
        private void ToggleEquip()
        {
            if (currentAbility != null)
            {
                currentAbility.ToggleEquip();
                needsUpdate = true;
            }
        }
    }
}