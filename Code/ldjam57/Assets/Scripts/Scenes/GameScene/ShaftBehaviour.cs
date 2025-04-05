using UnityEngine;

namespace Assets.Scripts.Scenes.GameScene
{
    public class ShaftBehaviour : TileBehaviour, IClickable
    {
        private void Awake()
        {
            this.digable = false;
        }
       

        public void OnClicked()
        {
            
        }

    }
}
