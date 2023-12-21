using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace LemmyModFramework.hooks
{ 
    internal class DynamicResolutionHandlerHooked
    {
        public static bool Prefix_FlushScalableBufferManagerState(DynamicResolutionHandler __instance)
        {
            DynamicResolutionHandler.s_GlobalHwUpresActive = __instance.HardwareDynamicResIsEnabled();
            DynamicResolutionHandler.s_GlobalHwFraction = __instance.m_CurrentFraction;
            
            ScalableBufferManager.ResizeBuffers((float)3, (float)3);

            return false;
        }
        public static bool Prefix_ProcessSettings(DynamicResolutionHandler __instance, GlobalDynamicResolutionSettings settings)
        {
            __instance.m_ForceSoftwareFallback = false;
            if (settings.forceResolution == false)
            {
                DynamicResolutionHandler.s_ActiveInstanceDirty = true;
            }
            settings.forceResolution = true;
            settings.forcedPercentage = 300.0f;

            __instance.m_Enabled = settings.enabled && (Application.isPlaying || settings.forceResolution);
            if (!__instance.m_Enabled)
            {
                __instance.m_CurrentFraction = 1f;
            } 
            else
            {
                __instance.type = settings.dynResType;
                __instance.m_UseMipBias = settings.useMipBias;
                __instance.m_MinScreenFraction = 1;//Mathf.Clamp(settings.minPercentage / 100f, 0.1f, 1f);
                __instance.m_MaxScreenFraction = 3;//Mathf.Clamp(settings.maxPercentage / 100f, __instance.m_MinScreenFraction, 4f);
                DynamicResUpscaleFilter resUpscaleFilter;
                __instance.filter = DynamicResolutionHandler.s_CameraUpscaleFilters.TryGetValue(DynamicResolutionHandler.s_ActiveCameraId, out resUpscaleFilter) ? resUpscaleFilter : settings.upsampleFilter;
                __instance.m_ForcingRes = settings.forceResolution;
                if (__instance.m_ForcingRes)
                    __instance.m_CurrentFraction = Mathf.Clamp(settings.forcedPercentage / 100f, 0.1f, 3f);
            }
            __instance.m_CachedSettings = settings;

            return false;
        } 
    }
}
  