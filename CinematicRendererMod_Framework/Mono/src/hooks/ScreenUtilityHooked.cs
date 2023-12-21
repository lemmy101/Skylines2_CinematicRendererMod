using System;
using Colossal;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LemmyModFramework.hooks
{
    public class ScreenUtilityHooked
    {
        public static bool CaptureScreenshot(out string __result, string fileName = null, int supersize = 1)
        {
            if (fileName == null)
            {
                __result = "";
                return true;
            }

            Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture(supersize);
            string str = Path.Combine(Application.persistentDataPath, "VideoFrames");
            if (!Directory.Exists(str))
                Directory.CreateDirectory(str);
            string path = Path.Combine(str, Path.ChangeExtension(fileName, ".png"));
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Object.DestroyImmediate((Object)tex);
            __result =  path;

            return false;
        }
    } 
}
