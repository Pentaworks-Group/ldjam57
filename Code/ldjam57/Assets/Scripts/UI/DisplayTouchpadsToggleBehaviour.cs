using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class DisplayTouchpadsToggleBehaviour : MonoBehaviour
    {
        private Toggle toggle;

        private void Awake()
        {
            toggle = GetComponent<Toggle>();
            toggle.isOn = Base.Core.Game.Options.ShowTouchPads;

        }

        private void OnEnable()
        {
            toggle.isOn = Base.Core.Game.Options.ShowTouchPads;
        }

        public void SetNewValue()
        {
            Base.Core.Game.Options.ShowTouchPads = toggle.isOn;
        }
    }
}