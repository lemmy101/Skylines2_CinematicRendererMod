using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Rendering;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace LemmyModFramework.hooks
{
    public class WindControlHooked
    {
        private static float time = 0;
        public static bool Prefix_SetGlobalProperties(WindControl __instance, CommandBuffer cmd, WindVolumeComponent wind)
        {
            if (!Globals.fixedTimeActive)
                return true;

            using (new ProfilingScope(cmd, ProfilingSampler.Get<ProfileId>(ProfileId.WindGlobalProperties)))
            {
                float targetSimulationSpeed = Globals.FixedStepSimulationSpeedDelta;
                float deltaTime = 1.0f / targetSimulationSpeed;
                float num1 = time;
                time += deltaTime;

                float3 xyz1 = math.mul(quaternion.Euler(0.0f, math.radians(wind.windDirection.value) + math.cos(6.28318548f * num1 / wind.windDirectionVariancePeriod.value) * math.radians(wind.windDirectionVariance.value), 0.0f), WindControl.kForward);
                float3 xyz2 = math.mul(quaternion.Euler(0.0f, wind.windDirection.value, 0.0f), WindControl.kForward);
                float num2 = wind.windGustStrengthControl.value.Evaluate(num1);
                float num3 = wind.windTreeGustStrengthControl.value.Evaluate(num1);
                __instance.m_ShaderVariablesWindCB._WindData_0 = (Matrix4x4)math.transpose(new float4x4(new float4(xyz1, 1f), new float4(xyz2, num1), float4.zero with
                {
                    w = math.min(1f, (Time.fixedTime - __instance._LastParametersSamplingTime) / wind.windParameterInterpolationDuration.value)
                }, new float4(__instance._WindBaseStrengthPhase.previous, __instance._WindBaseStrengthPhase2.previous, __instance._WindBaseStrengthPhase.current, __instance._WindBaseStrengthPhase2.current)));
                __instance.m_ShaderVariablesWindCB._WindData_1 = (Matrix4x4)math.transpose(new float4x4(new float4(wind.windBaseStrength.value * wind.windGlobalStrengthScale.value * wind.windGlobalStrengthScale2.value, wind.windBaseStrengthOffset.value, wind.windTreeBaseStrength.value * wind.windGlobalStrengthScale.value * wind.windGlobalStrengthScale2.value, wind.windTreeBaseStrengthOffset.value), new float4(0.0f, wind.windGustStrength.value * num2 * wind.windGlobalStrengthScale.value * wind.windGlobalStrengthScale2.value, wind.windGustStrengthOffset.value, __instance._WindFlutterGustVariancePeriod.current), new float4(__instance._WindGustStrengthVariancePeriod.current, __instance._WindGustStrengthVariancePeriod.previous, wind.windGustInnerCosScale.value, wind.windFlutterStrength.value * wind.windGlobalStrengthScale.value * wind.windGlobalStrengthScale2.value), new float4(wind.windFlutterGustStrength.value * wind.windGlobalStrengthScale.value * wind.windGlobalStrengthScale2.value, wind.windFlutterGustStrengthOffset.value, wind.windFlutterGustStrengthScale.value, __instance._WindFlutterGustVariancePeriod.previous)));
                __instance.m_ShaderVariablesWindCB._WindData_2 = (Matrix4x4)math.transpose(new float4x4(new float4(__instance._WindTreeBaseStrengthPhase.previous, __instance._WindTreeBaseStrengthPhase2.previous, __instance._WindTreeBaseStrengthPhase.current, __instance._WindTreeBaseStrengthPhase2.current), new float4(0.0f, wind.windTreeGustStrength.value * num3 * wind.windGlobalStrengthScale.value * wind.windGlobalStrengthScale2.value, wind.windTreeGustStrengthOffset.value, __instance._WindTreeFlutterGustVariancePeriod.current), new float4(__instance._WindTreeGustStrengthVariancePeriod.current, __instance._WindTreeGustStrengthVariancePeriod.previous, wind.windTreeGustInnerCosScale.value, wind.windTreeFlutterStrength.value * wind.windGlobalStrengthScale.value * wind.windGlobalStrengthScale2.value), new float4(wind.windTreeFlutterGustStrength.value * wind.windGlobalStrengthScale.value * wind.windGlobalStrengthScale2.value, wind.windTreeFlutterGustStrengthOffset.value, wind.windTreeFlutterGustStrengthScale.value, __instance._WindTreeFlutterGustVariancePeriod.previous)));
                __instance.m_ShaderVariablesWindCB._WindData_3 = (Matrix4x4)math.transpose(new float4x4(new float4(__instance._WindBaseStrengthVariancePeriod.previous, __instance._WindTreeBaseStrengthVariancePeriod.previous, __instance._WindBaseStrengthVariancePeriod.previous, __instance._WindTreeBaseStrengthVariancePeriod.current), new float4(__instance._WindGustStrengthPhase.previous, __instance._WindGustStrengthPhase2.previous, __instance._WindGustStrengthPhase.current, __instance._WindGustStrengthPhase2.current), new float4(__instance._WindTreeGustStrengthPhase.previous, __instance._WindTreeGustStrengthPhase2.previous, __instance._WindTreeGustStrengthPhase.current, __instance._WindTreeGustStrengthPhase2.current), new float4(0.0f, 0.0f, 0.0f, 0.0f)));
                ConstantBuffer.PushGlobal<ShaderVariablesWind>(cmd, in __instance.m_ShaderVariablesWindCB, WindControl.m_ShaderVariablesWind);
            }

            return false;
        }
        public static bool Prefix_UpdateCPUData(WindControl __instance, WindVolumeComponent wind)
        {
            if (!Globals.fixedTimeActive)
                return true;

            float targetSimulationSpeed = Globals.FixedStepSimulationSpeedDelta;
            float deltaTime = 1.0f / targetSimulationSpeed;
       
            __instance._LastParametersSamplingTime = time;
            __instance._WindBaseStrengthPhase.Update(wind.windBaseStrengthPhase.value);
            __instance._WindBaseStrengthPhase2.Update(wind.windBaseStrengthPhase2.value);
            __instance._WindTreeBaseStrengthPhase.Update(wind.windTreeBaseStrengthPhase.value);
            __instance._WindTreeBaseStrengthPhase2.Update(wind.windTreeBaseStrengthPhase2.value);
            __instance._WindBaseStrengthVariancePeriod.Update(wind.windBaseStrengthVariancePeriod.value);
            __instance._WindTreeBaseStrengthVariancePeriod.Update(wind.windTreeBaseStrengthVariancePeriod.value);
            __instance._WindGustStrengthPhase.Update(wind.windGustStrengthPhase.value);
            __instance._WindGustStrengthPhase2.Update(wind.windGustStrengthPhase2.value);
            __instance._WindTreeGustStrengthPhase.Update(wind.windTreeGustStrengthPhase.value);
            __instance._WindTreeGustStrengthPhase2.Update(wind.windTreeGustStrengthPhase2.value);
            __instance._WindGustStrengthVariancePeriod.Update(wind.windGustStrengthVariancePeriod.value);
            __instance._WindTreeGustStrengthVariancePeriod.Update(wind.windTreeGustStrengthVariancePeriod.value);
            __instance._WindFlutterGustVariancePeriod.Update(wind.windFlutterGustVariancePeriod.value);
            __instance._WindTreeFlutterGustVariancePeriod.Update(wind.windTreeFlutterGustVariancePeriod.value);

            return false;
        }
    }
} 
 