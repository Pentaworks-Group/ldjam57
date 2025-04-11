using Assets.Scripts.Core.Model;
using GameFrame.Core.Math;
using System.Collections.Generic;

namespace Assets.Scripts.Scenes.GameScene
{
    public abstract class Builder
    {
        public InventoryItem inventoryItem;

        //public Builder(InventoryItem inventoryItem)
        //{
        //    this.inventoryItem = inventoryItem;
        //}

        abstract public List<(Point2, Direction)> GetPossibleBuildSites();

        abstract public List<Direction> GetPossiblBuildDirections(Point2 position);

    }
}
