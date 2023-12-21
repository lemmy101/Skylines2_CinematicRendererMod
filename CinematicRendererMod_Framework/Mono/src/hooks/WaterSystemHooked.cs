using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Simulation;
using UnityEngine;

namespace LemmyModFramework.hooks
{
    public class WaterSystemHooked
    { 
        public static void SourceStep(WaterSystem __instance)
        {
            if (!Globals.fixedTimeActive)
                return;

            float targetSimulationSpeed = Globals.FixedStepSimulationSpeed;
            float unscaledDeltaTime = Time.unscaledDeltaTime;
            unscaledDeltaTime = 1.0f / targetSimulationSpeed;
            __instance.m_TimeStep = unscaledDeltaTime * 0.00000001f;
            __instance.TimeStepOverride = unscaledDeltaTime * 0.00000001f;//unscaledDeltaTime * 0.0001f;
        } 
    } 
}
 