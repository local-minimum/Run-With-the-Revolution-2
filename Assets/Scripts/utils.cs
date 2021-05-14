using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RwRUtils
{
    public static class Extensions
    {
        public static LayerMask AsLayerMask(this int layer)
        {
            return (1 << layer);
        }

        public static bool HasLayer(this LayerMask mask, int layer)
        {
            return (mask & (1 << layer)) != 0;
        }
    }
}