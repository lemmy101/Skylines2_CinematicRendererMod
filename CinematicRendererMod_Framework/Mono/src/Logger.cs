using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using BepInEx.Logging;
using BepInEx;
using HarmonyLib;

using UnityEngine;

namespace LemmyModFramework
{

    public class Logger
    {
        public static ManualLogSource LogSource;
        public static void LogDebug(object data) => LogSource.Log(LogLevel.Debug, data);
        public static void LogMessage(object data) => LogSource.Log(LogLevel.Message, data);
        public static void LogInfo(object data) => LogSource.Log(LogLevel.Info, data);
        public static void LogWarning(object data) => LogSource.Log(LogLevel.Warning, data);
        public static void LogError(object data) => LogSource.Log(LogLevel.Error, data);
    }
}
