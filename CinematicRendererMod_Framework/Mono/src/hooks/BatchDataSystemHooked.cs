using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Rendering;

namespace LemmyModFramework.hooks
{
    public class BatchDataSystemHooked
    {

        public static bool Prefix_GetLevelOfDetail(ref float __result, BatchDataSystem __instance, float levelOfDetail, IGameCameraController cameraController)
        {
            if (cameraController != null)
                levelOfDetail *= (float)(1.0 - 1.0 / (2.0 + 0.0099999997764825821 * (double)cameraController.zoom));

            __result = levelOfDetail;

            return false;
        }
    }
}
 