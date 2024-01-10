using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Tools;
using Unity.Entities;
using UnityEngine;

namespace LemmyModFramework.hooks
{
    public class AnimationSystemHooked
    {
        public static bool OnUpdate(AnimationSystem __instance)
        {
            float targetSimulationSpeed = Globals.FixedStepSimulationSpeedDelta;
            float unscaledDeltaTime = Time.unscaledDeltaTime;
            unscaledDeltaTime = 1.0f / targetSimulationSpeed;
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            __instance.__TypeHandle.__Game_Tools_Animation_RW_ComponentTypeHandle.Update(ref __instance.CheckedStateRef);
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            __instance.__TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle.Update(ref __instance.CheckedStateRef);
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: reference to a compiler-generated field
            __instance.Dependency = new AnimationSystem.AnimateJob()
            {
                m_DeltaTime = unscaledDeltaTime,
                m_TransformType = __instance.__TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle,
                m_AnimationType = __instance.__TypeHandle.__Game_Tools_Animation_RW_ComponentTypeHandle
            }.ScheduleParallel<AnimationSystem.AnimateJob>(__instance.m_AnimatedQuery, __instance.Dependency);

            return false;
        }
    }
}
