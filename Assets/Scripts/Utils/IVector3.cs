using System.IO;
using UnityEngine;

namespace NeonShooter.Utils
{
    public struct IVector3 : BinaryConvert.IBinaryWritable
    {
        private const int Prime1 = 179424691;
        private const int Prime2 = 512927377;

        public static readonly IVector3 Zero = new IVector3(0, 0, 0);

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

        public static bool operator ==(IVector3 v1, IVector3 v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z;
        }

        public static bool operator !=(IVector3 v1, IVector3 v2)
        {
            return !(v1 == v2);
        }

        public static Vector3 operator *(IVector3 v, float a)
        {
            return new Vector3(v.X * a, v.Y * a, v.Z * a);
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is IVector3 && ((IVector3)obj) == this;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = Prime1;
                hash = hash * Prime2 + X.GetHashCode();
                hash = hash * Prime2 + Y.GetHashCode();
                hash = hash * Prime2 + Z.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return string.Format("IVector3 [ {0}, {1}, {2} ]", X, Y, Z);
        }

        public void WriteTo(BinaryWriter bw)
        {
            bw.Write(this);
        }
    }
}
