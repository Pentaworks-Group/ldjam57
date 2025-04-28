using System;

using GameFrame.Core.Math;

using UnityEngine;
namespace Assets.Scripts.Scenes.GameScene
{
    public abstract class TileBehaviour : MonoBehaviour
    {
        protected WorldBehaviour worldBehaviour;
        protected bool digable = true;

        [SerializeField]
        private GameObject progressBarGameObject;
        [SerializeField]
        private ProgressBarBehaviour progressBarBehaviour;

        public Boolean IsProgressBarVisible { get; private set; }

        public void Init(WorldBehaviour worldBehaviour)
        {
            this.worldBehaviour = worldBehaviour;
        }

        public abstract Point2 GetPosition();

        public bool IsDigable() { return digable; }

        protected virtual void UpdateProgressBar(Single level)
        {
            if (this.progressBarBehaviour != null)
            {
                var clamped = Mathf.Clamp01(level);

                progressBarBehaviour.SetValue(clamped);
            }
        }

        protected virtual void ShowProgessBar()
        {
            if (this.progressBarGameObject != null)
            {
                if (!this.progressBarGameObject.activeSelf)
                {
                    this.IsProgressBarVisible = true;
                    this.progressBarGameObject.SetActive(true);
                }
            }
        }

        protected virtual void HideProgressBar()
        {
            if (this.progressBarGameObject != null)
            {
                if (this.progressBarGameObject.activeSelf)
                {
                    this.IsProgressBarVisible = false;
                    this.progressBarGameObject.SetActive(false);
                }
            }
        }
    }
}
