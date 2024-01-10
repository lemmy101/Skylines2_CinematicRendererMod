using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Game;
using Game.SceneFlow;
using Game.Settings;
using Game.Simulation;
using Unity.Jobs;
using UnityEngine;

namespace LemmyModFramework.hooks
{
    public class SimulationSystemHooked
    {
 
    
        public static bool Prefix_OnUpdate()
        {
            if(Globals.fixedTimeActive)
                return false;
            
            return true;
        }
        public static void OnUpdate(SimulationSystem __instance)
        {
            if (!Globals.fixedTimeActive)
                return;

            int steps = (int)(120.0f / Globals.FixedStepSimulationSpeed);
            if (steps < 1)
                steps = 1;
            float targetSimulationSpeed =Globals.FixedStepSimulationSpeedDelta;
            float deltaTime = 1.0f / targetSimulationSpeed;
       
            // ISSUE: reference to a compiler-generated field
            __instance.m_WatchDeps.Complete();
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            __instance.frameDuration = deltaTime;
            // ISSUE: reference to a compiler-generated field
            __instance.m_Stopwatch.Reset();
            // ISSUE: reference to a compiler-generated field
            __instance.m_StepCount = steps;
           
            // ISSUE: reference to a compiler-generated field
            if (__instance.m_IsLoading)
            {
                if ((double)__instance.loadingProgress != 1.0)
                {
                    // ISSUE: reference to a compiler-generated method
                    __instance.UpdateLoadingProgress();
                    return;
                }
                if (!GameManager.instance.isGameLoading)
                {
                    // ISSUE: reference to a compiler-generated field
                    __instance.m_IsLoading = false;
                    GameplaySettings gameplay = SharedSettings.instance?.gameplay;
                    __instance.selectedSpeed = gameplay == null || !gameplay.pausedAfterLoading ? 1f : 0.0f;
                }
            }
            else if (GameManager.instance.isGameLoading)
            {
                __instance.selectedSpeed = 0.0f;
                __instance.m_StepCount = steps = 0;
            }
            float num1;
            if ((double)__instance.selectedSpeed == 0.0)
            {
                num1 = 0;
                __instance.smoothSpeed = 0.0f;
                __instance.m_StepCount = steps = 0;
            }
            else
            {
                __instance.smoothSpeed = 1.0f;
                float num2 = deltaTime * __instance.selectedSpeed;
               
                // ISSUE: reference to a compiler-generated field
                __instance.m_Timer += num2;
                int x1 = (int)Mathf.Floor(__instance.m_Timer * 60f);

                __instance.m_Timer = Mathf.Clamp(__instance.m_Timer - (float)(x1 / 60f), 0.0f, deltaTime);

            }
            // ISSUE: reference to a compiler-generated field
            __instance.frameTime = __instance.m_Timer * 60f;
            // ISSUE: reference to a compiler-generated field
            __instance.m_LastSpeed = __instance.selectedSpeed;
            // ISSUE: reference to a compiler-generated field
            __instance.m_UpdateSystem.Update(SystemUpdatePhase.PreSimulation);

            for (int iterationIndex = 0; iterationIndex < steps; ++iterationIndex)
            {
                // ISSUE: reference to a compiler-generated field
                __instance.m_StepCount = steps;
                // ISSUE: reference to a compiler-generated field
                __instance.m_Stopwatch.Start();
            
                {
                    ++__instance.frameIndex;
                    // ISSUE: reference to a compiler-generated field
                    if (__instance.m_ToolSystem.actionMode.IsEditor())
                    {
                        // ISSUE: reference to a compiler-generated field
                        __instance.m_UpdateSystem.Update(SystemUpdatePhase.EditorSimulation, __instance.frameIndex, 0);
                    }
                    // ISSUE: reference to a compiler-generated field
                    if (__instance.m_ToolSystem.actionMode.IsGame())
                    {
                        // ISSUE: reference to a compiler-generated field
                        __instance.m_UpdateSystem.Update(SystemUpdatePhase.GameSimulation, __instance.frameIndex, 0);
                    }
                }
                // ISSUE: reference to a compiler-generated field
                // ISSUE: object of a compiler-generated type is created
                // ISSUE: variable of a compiler-generated type
                SimulationSystem.SimulationEndTimeJob jobData = new SimulationSystem.SimulationEndTimeJob()
                {
                    m_Stopwatch = GCHandle.Alloc((object)__instance.m_Stopwatch)
                };
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                __instance.m_WatchDeps = jobData.Schedule<SimulationSystem.SimulationEndTimeJob>(__instance.m_EndFrameBarrier.producerHandle);
            }
            // ISSUE: reference to a compiler-generated field
            __instance.m_UpdateSystem.Update(SystemUpdatePhase.PostSimulation);
        }
    }
}
