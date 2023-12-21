using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Game;
using Game.Common;
using Game.Simulation;
using LemmyModFramework.hooks;
using LemmyModFramework.systems;
using Colossal;
using UnityEngine;
using System.Collections;
using Cinemachine;
using Game.Rendering;
using Game.Rendering.Utilities;
using Game.Tools;
using Game.UI.InGame;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace LemmyModFramework 
{
    public class Patcher
    {
        private static Harmony harmony;
        public static MonoBehaviour modBehaviour;
        private static GameObject go;

        public class MyBehaviour : MonoBehaviour
        {
            public void OnDestroy()
            {
                go = null;
            }
        }
        public static void InjectSystems(UpdateSystem updateSystem)
        {
            updateSystem.UpdateBefore<PreSimulationSystem>(SystemUpdatePhase.GameSimulation);
            updateSystem.UpdateAfter<PostSimulationSystem>(SystemUpdatePhase.GameSimulation);
        }

        public static void TakeScreenshot(string filename)
        {
            if (go == null || !go.activeInHierarchy)
            {
                UnityEngine.GameObject go = new GameObject("LemmyMod");
                Patcher.go = go;
                modBehaviour = go.AddComponent<MyBehaviour>();

            }
            modBehaviour.StartCoroutine(CaptureScreenshot(filename));
        }
         
        internal static IEnumerator CaptureScreenshot(string filename)
        {
            yield return (object)new WaitForEndOfFrame();
            string result;
            ScreenUtilityHooked.CaptureScreenshot(out result, filename);
        }

        public static void Init(Harmony harmony)
        { 

           Patcher.harmony = harmony;
             
            PatchClass<SimulationSystem, SimulationSystemHooked>();
            PatchClass<CinematicCameraController, CinematicCameraControllerHooked>();
            PatchClass<CinematicCameraUISystem, CinematicCameraUISystemHooked>();
            PatchClass<BatchDataSystem, BatchDataSystemHooked>();
            PatchClass<WaterSystem, WaterSystemHooked>();
            PatchClass<CinemachineBrain, CinemachineBrainHooked>();
            PatchClass<WindControl, WindControlHooked>();
            PatchClass<WaterSimulationResources, WaterSimulationResourcesHooked>();
         //   PatchClass<AdaptiveDynamicResolutionScale, AdaptiveDynamicResolutionScaleHooked>();
            //   PatchClass<UnityEngine.VFX.Utility.VFXVelocityBinder, VFXVelocityBinderHooked>();
          //  PatchClass<DynamicResolutionHandler, DynamicResolutionHandlerHooked>();
            //
            var methodToPatch = typeof(ScreenUtilityHooked).GetMethod("CaptureScreenshot", BindingFlags.Static | BindingFlags.Public);
           var orig = typeof(ScreenUtility).GetMethod("CaptureScreenshot", BindingFlags.Static | BindingFlags.Public);

           harmony.Patch(orig, prefix: new HarmonyMethod(methodToPatch));

           methodToPatch = typeof(RenderingUtilsHooked).GetMethod("CalculateLodParameters", BindingFlags.Static | BindingFlags.Public);
           orig = typeof(RenderingUtils).GetMethods().Where(a => a.Name.Contains("CalculateLodParameters")).ToList()[1];

           harmony.Patch(orig, prefix: new HarmonyMethod(methodToPatch));

           methodToPatch = typeof(AnimationSystemHooked).GetMethod("OnUpdate", BindingFlags.Static | BindingFlags.Public);
           orig = typeof(AnimationSystem).GetMethod("OnUpdate", BindingFlags.NonPublic | BindingFlags.Instance);

           harmony.Patch(orig, prefix: new HarmonyMethod(methodToPatch));
               
        }

        class PatchResults
        {
            public MethodInfo pre;
            public MethodInfo post; 
            public MethodInfo orig; 
        }
          
        public static void PatchClass<Type, SecondType>()
        {
            Logger.LogInfo("Attempting to patch class: " + typeof(Type).Name);
            try
            {
                var patchClass = typeof(SecondType);
        
                var methods = patchClass.GetMethods();

                Dictionary<string, PatchResults> results = new Dictionary<string, PatchResults>();

                foreach (var method in methods)
                {
                    if (!method.IsStatic)
                        continue;

                    string name = method.Name;
                    //     Logger.LogInfo("Attempting to patch: " + name);
                    bool post = true;

                    if (method.Name.StartsWith("Prefix_"))
                    {
                        post = false;
                        name = name.Substring(7);
                    }

                    PatchResults res = new PatchResults();
                    if (!results.ContainsKey(name))
                    {
                        var methodToPatch = typeof(Type).GetMethod(name);
                        if (methodToPatch == null)
                            methodToPatch = typeof(Type).GetMethod(name, BindingFlags.NonPublic);
                        if (methodToPatch == null)
                            methodToPatch = typeof(Type).GetMethod(name, BindingFlags.Instance);
                        if (methodToPatch == null)
                            methodToPatch = typeof(Type).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);

                        if (methodToPatch == null)
                        {
                            Logger.LogError("Method was blank: " + typeof(Type).Name + "." + name + " - could not patch.");

                            continue;
                        }

                        res = new PatchResults();
                        results[name] = res;
                        res.orig = methodToPatch;




                    }
                    else
                    {
                        res = results[name];
                    }

                    if (post)
                        res.post = method;
                    else res.pre = method;
                }

                foreach (var keyValuePair in results)
                {
                    var res = keyValuePair.Value;

                    string output = "\nPatching " + typeof(Type).Name + "." + res.orig.Name + "\n";
                    if (res.pre != null)
                        output += "\tPre:\t" + typeof(SecondType).Name + "." + res.pre.Name + "\n";
                    if (res.post != null)
                        output += "\tPost:\t" + typeof(SecondType).Name + "." + res.post.Name + "\n";
                     
                    Logger.LogInfo(output);
                    harmony.Patch(res.orig, prefix: res.pre == null ? null : new HarmonyMethod(res.pre), postfix: res.post == null ? null : new HarmonyMethod(res.post));

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Logger.LogError(e);
                throw;
            }
         
        }

    }
}
