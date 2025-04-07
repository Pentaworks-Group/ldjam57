using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI

{
	public class InvertAxisOptionsBehaviour : MonoBehaviour
	{
        private Toggle toggle;

        private void Awake()
        {
            toggle = GetComponent<Toggle>();
            toggle.isOn = Base.Core.Game.Options.InvertAxis;

        }

        private void OnEnable()
        {
            toggle.isOn = Base.Core.Game.Options.InvertAxis;
        }

        public void SetNewValue()
        {
            Base.Core.Game.Options.InvertAxis = toggle.isOn;
        }
    }
}
