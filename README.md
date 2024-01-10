# Cinematic Renderer Mod

Automatically render the cinematic camera sequences to an mp4 file in your Videos library folder!

Demo video here: 

For this initial version, the video will be at 60fps and in the same resolution the game is rendering in.

The renderer will capture each frame as it is displayed (this may be at an extremely slow FPS while rendering), and will adjust the simulation speed of the game to perfectly synchronize with the rendered frames, providing smooth and perfect 60fps playback in the exported video file. 

Slower PCs will just render the sequence slower, meaning even a low end machine if capable of running the game on high settings will be able to export perfect 4k 60fps video of their city on top settings if they are willing to leave it rendering for a long while.

The simulation and camera synchronization to the frames is perfect, getting rid of issues such as vehicles moving at intervals out of sync with the video frames and other issues that are present in screen capturing the cinematic camera.

Future versions (when I figure out how to edit the UI) will add options for codecs, resolution, framerate and other cool stuff!

# IMPORTANT NOTES

* The codec that the video encoder uses will not work with Windows Media Player without installing additional codecs, but will work on the much better VLC (https://www.videolan.org/), as well as in all video editing software and YouTube.

* At present this mod is incompatible with Preserve_Photo_Mode mod (https://thunderstore.io/c/cities-skylines-ii/p/Nyoko/Preserve_Photo_Mode/) which seems to kill camera movement in the sequences.

# Installation - Manual

Install BepInEx 5, and download the correct version of the mod. The BepInEx 5 version is on Thunderstore.

Run the game once, then close it to properly initialize BepInEx. You can close it when the game loads into the main menu.

Download the mod from Thunderstore.io or the release page. Unzip it into the Cities Skylines II/BepInEx/plugins folder.

Launch the game, and your mods should be loaded automatically.

# Updates

1.2.0 - Fixed bug in ffmpeg parameters that was interpreting frames from the game as 25 fps, resulting in frame replication and 25fps with duplicate frames in the output 60fps video. Results are MUCH smoother and true 60fps now. Also slowed simulation to account for extra 35 frames per second needed from Skylines. New video here: 

1.1.0 - Changes to TGA frame format and moved frame saving to another thread to speed up rendering speed significantly. Added console output window to show frames exporting when sequence ends playback.

# BUILDING

Unfortunately at this time to build you need to reference modified Skylines 2 mono assemblies. I will look some way to provide il patches to patch the appropriate assemblies. Code is for now just provided for reference.

In short though, there are many private and protected members of BepInEx injected classes that need to be exposed as public to get it to compile.