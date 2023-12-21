using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game;
using Game.UI;
using UnityEngine.Windows;
using UnityEngine;
using LemmyModFramework.hooks;

namespace LemmyModFramework.hooks
{
    public class CinematicCameraControllerHooked
    {
        public static bool Prefix_UpdateController(CinematicCameraController __instance, CinematicCameraController.Input input)
        {
            if (!Globals.fixedTimeActive)
                return true;

            float targetSimulationSpeed = Globals.FixedStepSimulationSpeed;
            __instance.m_RestrictToTerrain.Refresh();
            float unscaledDeltaTime = Time.unscaledDeltaTime;
            unscaledDeltaTime = 1.0f / targetSimulationSpeed;

            Vector3 position1 = __instance.m_Anchor.position;
            float terrainHeight1;
            __instance.m_RestrictToTerrain.ClampToTerrain(position1, true, out terrainHeight1);
            float t = Mathf.Min(position1.y - terrainHeight1, __instance.m_MaxMovementSpeedHeight) / __instance.m_MaxMovementSpeedHeight;
            Vector2 move = input.move;
            if ((double)move.sqrMagnitude > 1.0)
                move.Normalize();
            move *= unscaledDeltaTime * Mathf.Lerp(__instance.m_MinMoveSpeed, __instance.m_MaxMoveSpeed, t);
            Vector2 vector2 = input.rotate * unscaledDeltaTime * __instance.m_RotateSpeed + input.mouseRotate * __instance.m_MouseRotateSpeed;
            float num = (float)((double)input.zoom * (double)Mathf.Lerp(__instance.m_MinZoomSpeed, __instance.m_MaxZoomSpeed, t) * (double)unscaledDeltaTime + (double)input.mouseZoom * (double)Mathf.Lerp(__instance.m_MinMouseZoomSpeed, __instance.m_MaxMouseZoomSpeed, t));
            Vector3 eulerAngles = __instance.m_Anchor.rotation.eulerAngles;
            float terrainHeight2;
            Vector3 terrain = __instance.m_RestrictToTerrain.ClampToTerrain(position1 + Quaternion.AngleAxis(eulerAngles.y, Vector3.up) * new Vector3(move.x, -num, move.y), true, out terrainHeight2);
            terrain.y = Mathf.Min(terrain.y, terrainHeight2 + __instance.m_MaxHeight);
            Quaternion rotation = Quaternion.Euler(Mathf.Clamp((float)(((double)eulerAngles.x + 90.0) % 360.0) - vector2.y, 0.0f, 180f) - 90f, eulerAngles.y + vector2.x, 0.0f);
            Vector3 position2;
            __instance.m_Anchor.position = !__instance.m_RestrictToTerrain.enableObjectCollisions || !__instance.m_RestrictToTerrain.CheckForCollision(terrain, __instance.m_RestrictToTerrain.previousPosition, rotation, out position2) ? terrain : position2;
            __instance.m_Anchor.rotation = rotation;


            return false;
        }
    }
}
