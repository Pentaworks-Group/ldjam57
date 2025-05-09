﻿using System;
using System.Collections.Generic;

namespace Assets.Scripts.Core.Definitons
{
    public class WorldDefinition
    {
        public Single? Seed { get; set; }
        public Int32? Width { get; set; }
        public HeadquartersDefinition Headquarters { get; set; }
        public List<MineralDefinition> Minerals { get; set; }
        public List<DepositoryDefinition> Depositories { get; set; }
    }
}
