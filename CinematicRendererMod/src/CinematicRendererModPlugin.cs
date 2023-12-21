using System;
using System.Reflection;
using BepInEx;
using Game;
using Game.Common;
using HarmonyLib;
using LemmyModFramework;
using UnityEngine;
using Logger = LemmyModFramework.Logger; 

namespace LemmyMod_Mono
{
     

    [BepInPlugin("CinematicRendererMod", "CinematicRendererMod", "1.0.0.0")]
    public class CinematicRendererModPlugin : BaseUnityPlugin 
    {
         
        private void Awake()  
        {
            LemmyModFramework.Logger.LogSource = Logger;
            // CinematicRendererModPlugin startup logic
            var harmony = new Harmony($"LemmyMod");
            Patcher.Init(harmony);
            Logger.LogInfo($"CinematicRendererModPlugin LemmyMod is loaded!");

            var methodToPatch = typeof(CinematicRendererModPlugin).GetMethod("InjectSystems", BindingFlags.Static | BindingFlags.Public);
            var orig = typeof(SystemOrder).GetMethod("Initialize", BindingFlags.Static | BindingFlags.Public);

            harmony.Patch(orig, postfix: new HarmonyMethod(methodToPatch));

        } 
         
        public const string GUID = "CinematicRendererMod";

        private void UiUpdate()
        {

        } 

        void LogHandler(string message, LogType type)
        {
            if (type == LogType.Error)
                Logger.LogError(message);
            else
                Logger.LogInfo(message);
        }
     
        public static void InjectSystems(UpdateSystem updateSystem)
        {
            Patcher.InjectSystems(updateSystem);
        }

        private void OnInitialized()
        {


        }
    }
}