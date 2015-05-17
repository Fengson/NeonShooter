using System;
using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.Cube
{
    public static class CubeGameObjectMaker
    {
        public const float CellSize = 1f;

        private static Mesh planeMesh = null;

        public static Mesh PlaneMesh
        {
            get
            {
                if (planeMesh == null)
                    planeMesh = MeshBuilder.CreatePlane(CellSize);
                return planeMesh;
            }
        }

        public static GameObject CreateCubeCellPlane(CubeCellPlaneSide side, bool visible)
        {
            Vector3 translation = side.Translation * 0.5f * CellSize;
            Vector3 rotation = side.Rotation * 90;
            Material material = (visible || Globals.Instance == null) ?
                null : Globals.Instance.invisibleShadowCasterMaterial;
            var cubeCellPlane = GameObjectMaker.CreateGameObject(
                String.Format("CubeCellPlane [ {0} ]", side.ToString()),
                PlaneMesh, translation, rotation, Vector3.one, material);
            return cubeCellPlane;
        }
    }
}
