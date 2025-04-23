using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameFrame.Core.Extensions;

namespace Assets.Scripts.Extensions
{
    public static class TransformExtensions
    {
        public static Boolean TryFindAndGetComponent<TComponent>(this UnityEngine.Transform transform, String path, out TComponent component) where TComponent : UnityEngine.Component
        {
            component = null;

            if (transform != null  && path.HasValue())
            {
                var child = transform.Find(path);

                if (child != null && child.TryGetComponent(out component))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
