
using System;

namespace Assets.Scripts.Core
{
    public class PlayerOptions : GameFrame.Core.PlayerOptions
    {
        public Boolean ShowTouchPads { get; set; }

        public Boolean InvertAxis { get; set; }

        public Single ScrollSensivity { get; set; } = .5f;
        public Single ZoomSensivity { get; set; } = .5f;
    }
}
