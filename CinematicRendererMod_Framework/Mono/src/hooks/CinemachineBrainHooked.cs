using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;

namespace LemmyModFramework.hooks
{
    public class CinemachineBrainHooked
    {
        public bool Prefix_GetEffectiveDeltaTime(ref float __result, CinemachineBrain __instance, bool fixedDelta)
        {
            if (!Globals.fixedTimeActive)
                return true;

            float targetSimulationSpeed = Globals.FixedStepSimulationSpeedDelta;
            float unscaledDeltaTime = Time.unscaledDeltaTime;
            unscaledDeltaTime = 1.0f / targetSimulationSpeed;
            __result = unscaledDeltaTime;

            return false;
        }


    }
}
 