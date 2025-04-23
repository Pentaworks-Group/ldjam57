using System;
using System.Collections.Generic;

using Assets.Scripts.Core.Model;

using GameFrame.Core.Math;

namespace Assets.Scripts.Scenes.GameScene
{
    public interface IStorage
    {
        public abstract Dictionary<String, MineralAmount> GetStorage();

        public Double GetCapacity();

        public Point2 GetPosition();

        protected void RegisterStorage();
        protected void UnRegisterStorage();

        public bool AllowsNewTypes()
        {
            return true;
        }

        public bool CanBeTakenFrom()
        {
            return true;
        }

        public bool CanBePutInto()
        {
            return true;
        }

        public int Priority()
        {
            return 0;
        }

        public void StorageChanged()
        {

        }
    }
}