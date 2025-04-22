using System;

using Assets.Scripts.Core.Persistence;

using TMPro;

using UnityEngine;

namespace Assets.Scripts.Prefabs.Menus
{
    public class SavedGameMenuBehaviour : MonoBehaviour
    {
        [SerializeField]
        private SavedGameListContainerBehaviour listContainer;

        [SerializeField]
        private TMP_Text savedOnText;

        [SerializeField]
        private TMP_Text startedOnText;
        
        [SerializeField]
        private TMP_Text balanceText;
        
        [SerializeField]
        private TMP_Text depthText;
        
        [SerializeField]
        private TMP_Text timeElapsedText;

        public void SaveGame()
        {
            Base.Core.Game.SaveGame();

            listContainer.UpdateList();
        }

        public void OnSlotSelected(SavedGamePreview selectedSlot)
        {
            DisplaySlot(selectedSlot);
        }

        private void DisplaySlot(SavedGamePreview selectedSlot)
        {
            this.savedOnText.text = selectedSlot.SavedOn.ToString("G");
            this.startedOnText.text = selectedSlot.StartedOn.ToString("G");
            this.balanceText.text = selectedSlot.Money.ToString("F2");
            this.depthText.text = selectedSlot.Depth.ToString("F0");
            this.timeElapsedText.text = TimeSpan.FromSeconds(selectedSlot.TimeElapsed).ToString("hh\\:mm\\:ss");
        }
    }
}
