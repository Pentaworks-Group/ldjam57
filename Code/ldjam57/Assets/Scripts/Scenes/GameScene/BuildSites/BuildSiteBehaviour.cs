using Assets.Scripts.Core.Model;
using GameFrame.Core.Math;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.Scenes.GameScene
{
    abstract public class BuildSiteBehaviour : MonoBehaviour, IClickable
    {
        protected BuildSiteManagerBehaviour buildSiteManagerBehaviour;
        protected InventoryItem inventoryItem;
        protected Direction direction;
        protected Point2 position;


        abstract protected void BuildSite();

        public void Init(BuildSiteManagerBehaviour buildSiteManagerBehaviour, Point2 position, InventoryItem inventoryItem, Direction direction)
        {
            this.buildSiteManagerBehaviour = buildSiteManagerBehaviour;
            this.inventoryItem = inventoryItem;
            this.position = position;
            this.direction = direction;
            UpdatePosition();
        }

        public void OnClicked()
        {
            inventoryItem.Amount -= 1;
            BuildSite();
        }


        public void UpdatePosition()
        {
            var uLocaleScale = transform.localScale;
            var uRotation = transform.rotation;
            UnityEngine.Vector3 uPosition;
            UnityEngine.Vector3 uScale;
            if (direction == Direction.Right)
            {
                uScale = new UnityEngine.Vector3(uLocaleScale.x, uLocaleScale.y / 2, uLocaleScale.z);
                uPosition = buildSiteManagerBehaviour.worldBehaviour.GetUnityVector(position, transform.position.z, xOffset: (-(1 - uScale.y) / 2) * .9f);
                uRotation *= Quaternion.Euler(0, 0, -90);
            }
            else if (direction == Direction.Left)
            {
                uScale = new UnityEngine.Vector3(uLocaleScale.x, uLocaleScale.y / 2, uLocaleScale.z);
                uPosition = buildSiteManagerBehaviour.worldBehaviour.GetUnityVector(position, transform.position.z, xOffset: ((1 - uScale.y) / 2) * .9f);
                uRotation *= Quaternion.Euler(0, 0, 90);
            }
            else
            {
                uScale = new UnityEngine.Vector3(uLocaleScale.x, uLocaleScale.y / 2, uLocaleScale.z);
                uPosition = buildSiteManagerBehaviour.worldBehaviour.GetUnityVector(position, transform.position.z, yOffset: (-(1 - uScale.y) / 2) * .9f);
                uRotation *= Quaternion.Euler(0, 0, 180);

            }
            transform.rotation = uRotation;
            transform.position = uPosition;
            transform.localScale = uScale;
        }


    }
}
