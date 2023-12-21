using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;

namespace LemmyModFramework
{

    public class VideoRenderer
    {
        public static VideoRenderer Instance = new VideoRenderer();



        [DllImport("shell32.dll")]
        private static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, ref IntPtr ppszPath);

        public void GetVideoLibraryFolders()
        {
            var pathPtr = default(IntPtr);
            var videoLibGuid = new Guid("491E922F-5643-4AF4-A7EB-4E7A138D8174");
            SHGetKnownFolderPath(videoLibGuid, 0, IntPtr.Zero, ref pathPtr);

            string path = Marshal.PtrToStringUni(pathPtr);
            Marshal.FreeCoTaskMem(pathPtr);
            List<string> foldersInLibrary = new List<string>();

            using (XmlReader reader = XmlReader.Create(path))
            {
                while (reader.ReadToFollowing("simpleLocation"))
                {
                    reader.ReadToFollowing("url");
                    foldersInLibrary.Add(reader.ReadElementContentAsString());
                }
            }

            for (int i = 0; i < foldersInLibrary.Count; i++)
            {
                if (foldersInLibrary[i].Contains("knownfolder"))
                {
                    foldersInLibrary[i] = foldersInLibrary[i].Replace("knownfolder:{", "");
                    foldersInLibrary[i] = foldersInLibrary[i].Replace("}", "");

                    SHGetKnownFolderPath(new Guid(foldersInLibrary[i]), 0, IntPtr.Zero, ref pathPtr);
                    foldersInLibrary[i] = Marshal.PtrToStringUni(pathPtr);
                    Marshal.FreeCoTaskMem(pathPtr);
                }
            }

            // foldersInLibrary now contains the path to all folders in the Videos Library

        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public void Render(int framerate = 60)
        {

            string dir = Path.Combine(Application.persistentDataPath, "VideoFrames");
            string ffmpeg_dir = AssemblyDirectory;
             
        
            try
            {
                if (File.Exists(dir + "\\out.mp4"))
                {
                    File.Delete(dir + "\\out.mp4");
                }
            }
            catch (Exception e)
            {
                
            }

            try
            {
                Process process = new Process();
                process.StartInfo.FileName = ffmpeg_dir + "\\ffmpeg.exe";
                string args = "-pattern_type sequence -i \"" + dir +
                              "\\Video_%05d.png\" -c:v libx264 -pix_fmt yuv444p -r 60 -s:v " + Screen.width + "x" +
                              Screen.height + " \"" + dir + "\\out.mp4\"";
                process.StartInfo.Arguments = args;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                process.Start();
                process.WaitForExit();// Waits here for the process to exit.

            }
            catch (Exception e)
            {
                
            }

            try
            {
                var pathPtr = default(IntPtr);
                var videoLibGuid = new Guid("491E922F-5643-4AF4-A7EB-4E7A138D8174");
                SHGetKnownFolderPath(videoLibGuid, 0, IntPtr.Zero, ref pathPtr);

                string path = Marshal.PtrToStringUni(pathPtr);
                Marshal.FreeCoTaskMem(pathPtr);
                List<string> foldersInLibrary = new List<string>();

                using (XmlReader reader = XmlReader.Create(path))
                {
                    while (reader.ReadToFollowing("simpleLocation"))
                    {
                        reader.ReadToFollowing("url");
                        foldersInLibrary.Add(reader.ReadElementContentAsString());
                    }
                }

                for (int i = 0; i < foldersInLibrary.Count; i++)
                {
                    if (foldersInLibrary[i].Contains("knownfolder"))
                    {
                        foldersInLibrary[i] = foldersInLibrary[i].Replace("knownfolder:{", "");
                        foldersInLibrary[i] = foldersInLibrary[i].Replace("}", "");

                        SHGetKnownFolderPath(new Guid(foldersInLibrary[i]), 0, IntPtr.Zero, ref pathPtr);
                        foldersInLibrary[i] = Marshal.PtrToStringUni(pathPtr);
                        Marshal.FreeCoTaskMem(pathPtr);
                    }
                }

                string pathLib = foldersInLibrary[0];

                File.Copy(dir + "\\out.mp4", pathLib + "\\" + (DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".mp4") );
            }
            catch (Exception e)
            {
                string dir2 = Path.Combine(Application.persistentDataPath, "Videos");
                if (!Directory.Exists(dir2))
                    Directory.CreateDirectory(dir2);
                File.Copy(dir + "\\out.mp4", dir2 + "\\" + (DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".mp4"));

            }


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

            try
            {
                Directory.Delete(dir);

            }
            catch (Exception e)
            {
            }
        }
    }
}
  