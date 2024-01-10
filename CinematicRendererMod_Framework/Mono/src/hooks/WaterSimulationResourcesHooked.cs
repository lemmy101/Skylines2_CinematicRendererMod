using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace LemmyModFramework.hooks
{
    public class WaterSimulationResourcesHooked
    {
        private static float time_since_startup = 0; 
        public static bool Prefix_Update(WaterSimulationResources __instance, float timeMultiplier)
        {
            if (!Globals.fixedTimeActive)
                return true;

            float targetSimulationSpeed = Globals.FixedStepSimulationSpeedDelta;
            float deltaTime = 1.0f / targetSimulationSpeed;
            __instance.m_Time = time_since_startup;
            __instance.deltaTime = deltaTime;
            __instance.simulationTime += deltaTime;
            time_since_startup += deltaTime;
            
            return false;
        }
    }
}
 