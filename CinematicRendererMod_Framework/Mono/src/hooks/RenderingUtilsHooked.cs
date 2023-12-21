using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine.Rendering;

namespace LemmyModFramework.hooks
{
    public class RenderingUtilsHooked
    {
        public static bool CalculateLodParameters(ref float4 __result, float lodFactor, LODParameters lodParameters)
        {
            float w = 1f / math.tan(math.radians(lodParameters.fieldOfView * 0.5f));
            //   lodFactor *= 1540f * w;
            lodFactor *= 540f * w;
            __result = new float4(lodFactor, (float)(1.0 / ((double)lodFactor * (double)lodFactor)), w + 1f, w);

            return false;
        }
    }
}
