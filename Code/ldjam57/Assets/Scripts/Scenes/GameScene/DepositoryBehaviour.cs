using System;

using Assets.Scripts.Core.Model;

using GameFrame.Core.Extensions;

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

        private Boolean addDirection = true;

        public void OnClicked()
        {
            if (addDirection)
            {
                AddMineral(5000);

                var factor = depository.Value / depository.Capacity;

                if (factor > 0.9d)
                {
                    addDirection = false;
                }
            }
            else
            {
                SubtractMineral(5000);

                var factor = depository.Value / depository.Capacity;

                if (factor < 0.05)
                {
                    addDirection = true;
                }
            }
        }

        public void AddMineral(Double amount)
        {
            this.depository.Value += amount;
            UpdateLevel();
        }

        public void SubtractMineral(Double amount)
        {
            this.depository.Value -= amount;

            if (this.depository.Value < 0)
            {
                this.depository.Value = 0;
            }

            UpdateLevel();
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
