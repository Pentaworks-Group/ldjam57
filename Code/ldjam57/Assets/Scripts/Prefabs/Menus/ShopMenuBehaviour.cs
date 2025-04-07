using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Assets.Scripts.Prefabs.Menus
{    
    public class ShopMenuBehaviour : MonoBehaviour
    {
        [SerializeField]
        private GameObject toggleArea;

        public void ShowShop()
        {
            this.toggleArea.SetActive(true);
        }

        public void HideShop()
        {
            this.toggleArea.SetActive(false);
        }
    }
}
