using UnityEngine;

namespace NeonShooter.Utils
{
    public struct IVector3
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Z { get; private set; }

        public IVector3(int x, int y, int z)
            : this()
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static implicit operator Vector3(IVector3 iVector)
        {
            return new Vector3(iVector.X, iVector.Y, iVector.Z);
        }
    }
}
