using Assets.Scripts.Core.Model;
using Assets.Scripts.Prefabs.Menus;
using Assets.Scripts.Scenes.GameScene;
using UnityEngine;

namespace Assets.Scripts.Scenes.GameScene
{
    public class HeadquarterBehaviour : MonoBehaviour, IClickable
    {
        [SerializeField]
        private ShopMenuBehaviour shop;

        public void OnClicked()
        {
            this.shop.ShowShop();
        }
    }
}