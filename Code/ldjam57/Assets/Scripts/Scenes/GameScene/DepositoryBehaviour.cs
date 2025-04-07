using System;

using Assets.Scripts.Core.Model;

using GameFrame.Core.Extensions;
using TMPro;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Assets.Scripts.Scenes.GameScene
{
    public class DepositoryBehaviour : MonoBehaviour, IClickable
    {
        private WorldBehaviour worldBehaviour;
        private Depository depository;

        private SpriteRenderer levelRenderer;
        private GameObject popupMenu;

        [SerializeField]
        TextMeshProUGUI nameField;
        [SerializeField]
        TextMeshProUGUI storageField;
        [SerializeField]
        TextMeshProUGUI priceField;
        [SerializeField]
        Button sellButton;
        [SerializeField]
        ProgressBarBehaviour fillAmountBehaviour;
        
        public void Init(WorldBehaviour worldBehaviour, Depository depository)
        {
            this.worldBehaviour = worldBehaviour;
            this.depository = depository;

            levelRenderer = transform.Find("Stash").GetComponent<SpriteRenderer>();
            popupMenu = transform.Find("PopupMenu").gameObject;

            levelRenderer.color = depository.Mineral.Color.ToUnity();

            if (fillAmountBehaviour != null )
            {
                Debug.Log(depository.Mineral.Color.ToUnity());
                fillAmountBehaviour.setColor(depository.Mineral.Color.ToUnity());
                fillAmountBehaviour.SetValue(0);
            }

        }

        public void Update()
        {
            if (popupMenu != null && popupMenu.activeSelf) {
                updatePopupUI();
            }
        }

        public void OnClicked()
        {
            // Open Shop?
            if (popupMenu != null && !popupMenu.activeSelf)
            {
                popupMenu.SetActive(true);

                updatePopupUI();
            }
            else if (popupMenu != null) {
                popupMenu.SetActive(false);
            }
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

        public void Sell()
        {
            //TODO: right now just sell 1 (t)
            Debug.Log("Sell");
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

            if (fillAmountBehaviour != null)
            {
                fillAmountBehaviour.SetValue((float)level);
            }
        }

        private void updatePopupUI()
        {
            //Update UI
            nameField.SetText(depository.Mineral.Name);
            storageField.SetText(depository.Value.ToString("F1") + " t");
            priceField.SetText("$" + depository.Value.ToString("F1") + "/t"); //TODO

            if(depository.Value > 1)
            {
                sellButton.enabled = true;
            } else
            {
                sellButton.enabled = false;
            }
        }

    }
}
