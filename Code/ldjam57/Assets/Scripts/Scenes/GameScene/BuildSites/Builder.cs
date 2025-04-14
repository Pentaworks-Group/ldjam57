using Assets.Scripts.Core.Model;
using GameFrame.Core.Math;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Scenes.GameScene
{
    public abstract class Builder : MonoBehaviour
    {
        public InventoryItem inventoryItem;

        //public Builder(InventoryItem inventoryItem)
        //{
        //    this.inventoryItem = inventoryItem;
        //}

        abstract public List<(Point2, Direction)> GetPossibleBuildSites();

        abstract public BuildSiteBehaviour GenerateBuildSite(Point2 pos, Direction dir);
    }
}
