using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.CinematicCamera;
using Game.Rendering.CinematicCamera;
using Game.UI.InGame;
using UnityEngine;

namespace LemmyModFramework.hooks
{
    internal class CinematicCameraUISystemHooked
    {
        private static int frame = 0;
        private static bool recording = false;
        private static bool was_playing = false;
        private static Resolution res;
        public static bool Prefix_set_playing(CinematicCameraUISystem __instance)
        {
            was_playing = __instance.m_Playing;
            return true;
        }

        public static void set_playing(CinematicCameraUISystem __instance)
        {
            if (__instance.m_Playing && !was_playing)
            {
                frame = 0;
                recording = true;
                float targetSimulationSpeed = Globals.FixedStepSimulationSpeedDelta;
                float unscaledDeltaTime = Time.unscaledDeltaTime;
                unscaledDeltaTime = 1.0f / targetSimulationSpeed;
                Time.captureDeltaTime = unscaledDeltaTime;
                res = Screen.currentResolution;

                Globals.fixedTimeActive = true;

                try
                {
                    string dir = Path.Combine(Application.persistentDataPath, "VideoFrames");


                    try
                    {
                        var files = Directory.GetFiles(dir);

                        foreach (var file in files)
                        {
                            try
                            {
                                File.Delete(file);
                            }
                            catch (Exception e)
                            {

                            }


                        }

                    }
                    catch (Exception e)
                    {
                    }

                    Directory.Delete(dir);

                }
                catch (Exception e)
                {
                }

            }
            else if (!__instance.m_Playing && was_playing)
            {
                frame = 0;

                if(recording)
                    VideoRenderer.Instance.Render(60);

                recording = false;
      
                Time.captureDeltaTime = 0;

                Globals.fixedTimeActive = false;

      
            }
        }
        public static bool Prefix_UpdatePlayback(CinematicCameraUISystem __instance)
        {
            if (!Globals.fixedTimeActive)
            {
                Time.captureDeltaTime = 0;
                return true;
            }

            float targetSimulationSpeed = Globals.FixedStepSimulationSpeedDelta;
            float unscaledDeltaTime = Time.unscaledDeltaTime;
            unscaledDeltaTime = 1.0f / targetSimulationSpeed;
            if (__instance.playing)
            {
                Time.captureDeltaTime = unscaledDeltaTime;

            }  
             
            __instance.t += unscaledDeltaTime * 4;
            // ISSUE: reference to a compiler-generated field
            CinematicCameraSequence cinematicCameraSequence = __instance.m_ActiveAutoplaySequence ?? __instance.activeSequence;
            if ((double)__instance.t >= (double)cinematicCameraSequence.playbackDuration)
            {
                if (cinematicCameraSequence.loop) 
                    __instance.t -= cinematicCameraSequence.playbackDuration;
                else
                    __instance.playing = false;

                recording = false;
            }
            __instance.t = Mathf.Min(__instance.t, cinematicCameraSequence.playbackDuration);
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            cinematicCameraSequence.Refresh(__instance.t, (IDictionary<string, PhotoModeProperty>)__instance.m_PhotoModeRenderSystem.photoModeProperties, __instance.m_CameraUpdateSystem.activeCameraController);

            string filename = "Video_";
            string result;

            if (__instance.m_Playing && recording)
            {
                string frameString = frame.ToString();
                while (frameString.Length < 5)
                    frameString = "0" + frameString;

                filename += frameString;
                Patcher.TakeScreenshot(filename);
                frame++;
            }
            else
            {
                frame = 0;
                Globals.fixedTimeActive = false;
                Time.captureDeltaTime = 0;
                if (recording)
                {
                    recording = false;
                    VideoRenderer.Instance.Render(60);
                }
            }

            return false;

        }
    }
}
