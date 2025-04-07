using Assets.Scripts.Prefabs.Menus;

using UnityEngine;

namespace Assets.Scripts.Scenes.GameScene
{
    public class HeadquarterBehaviour : MonoBehaviour, IClickable
    {
        [SerializeField]
        private ShopMenuBehaviour shop;

        public void OnClicked()
        {
            shop.ShowShop();
        }
    }
}