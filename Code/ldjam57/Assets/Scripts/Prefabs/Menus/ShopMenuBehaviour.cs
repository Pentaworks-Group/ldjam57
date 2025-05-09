﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Prefabs.Menus
{    
    public class ShopMenuBehaviour : MonoBehaviour
    {
        public UnityEvent<Boolean> OnShopToggled = new UnityEvent<Boolean>();

        [SerializeField]
        private GameObject toggleArea;

        public Boolean IsOpen => toggleArea.activeSelf;

        public void ShowShop()
        {
            GameFrame.Base.Audio.Effects.Play("Bell");

            this.toggleArea.SetActive(true);
            this.OnShopToggled.Invoke(true);
        }

        public void HideShop()
        {
            GameFrame.Base.Audio.Effects.Play("Button");

            this.toggleArea.SetActive(false);
            this.OnShopToggled.Invoke(false);

        }
    }
}
