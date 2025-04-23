using System;

using Assets.Scripts.Core.Persistence;
using Assets.Scripts.Extensions;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Prefabs.Menus
{
    public class SavedGameListSlotBehaviour : GameFrame.Core.UI.List.ListSlotBehaviour<SavedGamePreview>
    {
        private TMP_Text savedOnText;
        private TMP_Text timePlayedText;
        private TMP_Text depthText;
        private TMP_Text moneyText;

        public override void RudeAwake()
        {
            if (!transform.TryFindAndGetComponent("Content/SavedOnText", out savedOnText))
            {
                throw new System.Exception("Failed to get Text 'SavedOnText'!");
            }

            if (!transform.TryFindAndGetComponent("Content/TimePlayedText", out timePlayedText))
            {
                throw new System.Exception("Failed to get Text 'TimePlayedText'!");
            }

            if (!transform.TryFindAndGetComponent("Content/DepthText", out depthText))
            {
                throw new System.Exception("Failed to get Text 'DepthText'!");
            }

            if (!transform.TryFindAndGetComponent("Content/MoneyText", out moneyText))
            {
                throw new System.Exception("Failed to get Text 'MoneyText'!");
            }
        }

        public override void UpdateUI()
        {
            this.savedOnText.text = this.content.SavedOn.ToString("G");
            this.moneyText.text = this.content.Money.ToString("F2");
            this.depthText.text = this.content.Depth.ToString("F0");
            this.timePlayedText.text = TimeSpan.FromSeconds(this.content.TimeElapsed).ToString("hh\\:mm\\:ss");
        }
    }
}
