using System;

using Assets.Scripts.Core.Model;

using TMPro;

using UnityEngine;

namespace Assets.Scripts.Scenes.GameScene
{
    public class MaxDepthBehaviour : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text text;

        private World world;
        private Int32 currentMaxDepth = -1;

        private void Awake()
        {
            Base.Core.Game.ExecuteAfterInstantation(Load);
        }

        private void Load()
        {
            if (Base.Core.Game.State != default)
            {
                this.world = Base.Core.Game.State.World;
            }
        }

        private void Update()
        {
            if (this.world != default)
            {
                if (currentMaxDepth < this.world.MaxDepth)
                {
                    currentMaxDepth = this.world.MaxDepth;
                    this.text.text = currentMaxDepth.ToString();
                }
            }
        }
    }
}
