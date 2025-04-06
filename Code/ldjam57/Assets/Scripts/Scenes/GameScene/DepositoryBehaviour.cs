using System;

using Assets.Scripts.Core.Model;

using GameFrame.Core.Extensions;

using UnityEditor.UIElements;

using UnityEngine;

namespace Assets.Scripts.Scenes.GameScene
{
    public class DepositoryBehaviour : MonoBehaviour, IClickable
    {
        private WorldBehaviour worldBehaviour;
        private Depository depository;

        private SpriteRenderer levelRenderer;
        
        public void Init(WorldBehaviour worldBehaviour, Depository depository)
        {
            this.worldBehaviour = worldBehaviour;
            this.depository = depository;

            levelRenderer = transform.Find("Stash").GetComponent<SpriteRenderer>();

            levelRenderer.color = depository.Mineral.Color.ToUnity();
        }

        public void OnClicked()
        {
            // Open Shop?
        }

        /// <summary>
        /// Adds given amount to deposit.
        /// </summary>
        /// <param name="amount">amount to be added</param>
        /// <param name="overflow">Overflow if amount exceeds capacity</param>
        /// <returns>True of successful, false if capacity is exceeded. Returns the <paramref name="overflow"/> if false.</returns>
        public Boolean AddMineral(Double amount, out Double overflow)
        {
            var result = true;

            overflow = 0;

            this.depository.Value += amount;

            if (this.depository.Value > this.depository.Capacity)
            {
                overflow = this.depository.Value - this.depository.Capacity;
                this.depository.Value = this.depository.Capacity;

                result = false;
            }

            UpdateLevel();

            return result;
        }

        /// <summary>
        /// Subtracts the given amount from deposit.
        /// </summary>
        /// <param name="amount">Value to be subtracted</param>
        /// <returns>True if subtract was successful, otherwise false.</returns>
        public Boolean SubtractMineral(Double amount)
        {
            if (amount > this.depository.Value)
            {
                this.depository.Value -= amount;
                UpdateLevel();

                return true;
            }

            return false;
        }

        private void UpdateLevel()
        {
            var level = depository.Value / depository.Capacity;

            var spriteName = "";

            if (level > 0.9)
            {
                spriteName = "Depository_Ore_Level_4";
            }
            else if (level > 0.8)
            {
                spriteName = "Depository_Ore_Level_3";
            }
            else if (level > 0.6)
            {
                spriteName = "Depository_Ore_Level_2";
            }
            else if (level > 0.4)
            {
                spriteName = "Depository_Ore_Level_1";
            }
            else if (level > 0.2)
            {
                spriteName = "Depository_Ore_Level_0";
            }

            var sprite = default(Sprite);

            Debug.Log(String.Format("level: {0} - {1}", level, spriteName));

            if (spriteName.HasValue())
            {
                sprite = GameFrame.Base.Resources.Manager.Sprites.Get(spriteName);
            }

            this.levelRenderer.sprite = sprite;
        }
    }
}
