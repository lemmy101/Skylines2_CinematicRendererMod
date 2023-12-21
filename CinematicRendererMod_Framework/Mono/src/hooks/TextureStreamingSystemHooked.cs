using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colossal.Entities;
using Colossal.IO.AssetDatabase;
using Colossal.IO.AssetDatabase.VirtualTexturing;
using UnityEngine.Experimental.Rendering;
using UnityEngine;

namespace LemmyModFramework.hooks
{
    public class TextureStreamingSystemHooked
    {
        public static bool Initialize(TextureStreamingSystem __instance, int desiredMipBias, UnityEngine.Rendering.VirtualTexturing.FilterMode filterMode)
        {
          
            __instance.CleanupVT();
            __instance.m_VirtualTexturingConfig.filterMode = filterMode;
            __instance.m_MipBias = Mathf.Max(__instance.m_DatasetMipBias, desiredMipBias);
         //   if (__instance.m_MipBias != desiredMipBias)
           //     COSystemBase.baseLog.InfoFormat("Desired mipbias {0} is unavailable in the current dataset and was forced to {1} dataset default", (object)desiredMipBias, (object)__instance.m_MipBias);
        //    COSystemBase.baseLog.InfoFormat("Initializing VT with mipbias {0} - {1}", (object)__instance.m_MipBias, (object)__instance.m_VirtualTexturingConfig.filterMode);
            __instance.m_VtProceduralCPU = new VTProceduralCPU(__instance.m_VirtualTexturingConfig);
            __instance.m_VtDatabase = new VTDatabase(__instance.m_VirtualTexturingConfig, __instance.m_AtlasMaterialsGrouper.midMipLevelsCount, (IVTStackCreator)__instance.m_VtProceduralCPU);
            if (__instance.m_AtlasMaterialsGrouper.IsInitialized)
                __instance.m_AtlasMaterialsGrouper.PreRegisterToVT(__instance);
            foreach (SurfaceAsset asset in Colossal.IO.AssetDatabase.AssetDatabase.global.GetAssets<SurfaceAsset>(new SearchFilter<SurfaceAsset>()))
            { 
                asset.ClearVTAtlassingInfos();
                if (!__instance.m_AtlasMaterialsGrouper.SetPreReservedIndex(asset))
                {
                    if (asset.isVTMaterial)
                    {
                        asset.RegisterToVT(__instance);
                        __instance.m_VTAssetsToInit.Enqueue(asset);
                    }
                }
                else if (__instance.m_AtlasMaterialsGrouper.IsDuplicate(asset.guid))
                    __instance.m_VTAssetsShallowInit.Enqueue(asset);
                else
                    __instance.m_VTAssetsToInit.Enqueue(asset);
            }
            __instance.m_VTMaterialsCountToLoad = __instance.m_VTAssetsToInit.Count;
            __instance.m_VTMaterialsDuplicatesCountToInit = __instance.m_VTAssetsShallowInit.Count;
            RenderTexture renderTexture = new RenderTexture(1, 1, GraphicsFormat.R32_SFloat, GraphicsFormat.None);
            renderTexture.name = "VTWorkingSetBiasReadbackRT";
            __instance.m_WorkingSetLodBiasRT = renderTexture;
            __instance.m_WorkingSetLodBiasRT.Create();
            Material material = new Material(Resources.Load<Shader>("VirtualTexturingBiasReadback"));
            material.name = "VTWorkingSetBiasReadback";
            __instance.m_WorkingSetLodBiasMat = material;

            return false;
        }
    }
}
