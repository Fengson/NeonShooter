using System;
using UnityEngine;

namespace NeonShooter.Cube
{
    public class CubeCellPlaneSide
    {
        public static readonly CubeCellPlaneSide Right = new CubeCellPlaneSide(Vector3.right, Vector3.up);
        public static readonly CubeCellPlaneSide Left = new CubeCellPlaneSide(Vector3.left, Vector3.down);
        public static readonly CubeCellPlaneSide Up = new CubeCellPlaneSide(Vector3.up, Vector3.left);
        public static readonly CubeCellPlaneSide Down = new CubeCellPlaneSide(Vector3.down, Vector3.right);
        public static readonly CubeCellPlaneSide Front = new CubeCellPlaneSide(Vector3.forward, Vector3.zero);
        public static readonly CubeCellPlaneSide Back = new CubeCellPlaneSide(Vector3.back, Vector3.up * 2);

        public Vector3 Translation { get; private set; }
        public Vector3 Rotation { get; private set; }

        private CubeCellPlaneSide(Vector3 translation, Vector3 rotation)
        {
            Translation = translation;
            Rotation = rotation;
        }

        public override string ToString()
        {
            if (this == Right) return "Right";
            if (this == Left) return "Left";
            if (this == Up) return "Up";
            if (this == Down) return "Down";
            if (this == Front) return "Front";
            if (this == Back) return "Back";

            throw new Exception("Invalid CubeCellPlaneSide. This should never happen.");
        }
    }
}
