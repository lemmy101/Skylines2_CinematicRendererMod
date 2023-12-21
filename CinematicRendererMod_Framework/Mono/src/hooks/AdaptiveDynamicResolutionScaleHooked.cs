using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Rendering.Utilities;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine.Rendering;
namespace LemmyModFramework.hooks
{
    public class AdaptiveDynamicResolutionScaleHooked
    {
        public static void SetParams(AdaptiveDynamicResolutionScale __instance,
            bool enabled,
            bool adaptive, 
            float minScale,
            AdaptiveDynamicResolutionScale.DynResUpscaleFilter filter,
            Camera camera)
        {

            if (Screen.currentResolution.height < 1080 * 8)
                Screen.SetResolution(1920 * 8, 1080 * 8, FullScreenMode.MaximizedWindow, 120);
        }  
    }
}
