
using Assets.Scripts.Core.Model;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Scenes.GameScene
{
    public interface IStorage
    {
        Dictionary<Mineral, Double> GetStorage();

    }
}