using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NeonShooter.Utils
{
    public static class BinaryConvert
    {
        public interface IBinaryWritable
        {
            void WriteTo(BinaryWriter bw);
        }

        //public interface IBinaryReadable<T>
        //{
        //    T ReadFrom(BinaryReader br);
        //}

        public static void Write<T>(this BinaryWriter bw, IEnumerable<T> enumerable)
            where T : IBinaryWritable
        {
            foreach (var bc in enumerable)
                bw.Write(bc);
        }

        public static void Write(this BinaryWriter bw, IBinaryWritable binaryConvertible)
        {
            binaryConvertible.WriteTo(bw);
        }

        public static void Write(this BinaryWriter bw, Vector2 vector)
        {
            bw.Write(vector.x);
            bw.Write(vector.y);
        }

        public static void Write(this BinaryWriter bw, Vector3 vector)
        {
            bw.Write(vector.x);
            bw.Write(vector.y);
            bw.Write(vector.z);
        }

        public static void Write(this BinaryWriter bw, Quaternion quaternion)
        {
            bw.Write(quaternion.x);
            bw.Write(quaternion.y);
            bw.Write(quaternion.z);
            bw.Write(quaternion.w);
        }

        public static void Write(this BinaryWriter bw, IVector3 vector)
        {
            bw.Write(vector.X);
            bw.Write(vector.Y);
            bw.Write(vector.Z);
        }

        //public static void Read<T>(this BinaryReader br, IEnumerable<T> enumerable)
        //    where T : IBinaryConvertible
        //{
        //    foreach (var bc in enumerable)
        //        br.Read(bc);
        //}

        //public static T ReadBinary<T>(this BinaryReader br, )
        //    where T : IBinaryReadable<T>
        //{
        //    return binaryConvertible.ReadFrom(br);
        //}

        public static Vector2 ReadVector2(this BinaryReader br)
        {
            return new Vector2(br.ReadSingle(), br.ReadSingle());
        }

        public static Vector3 ReadVector3(this BinaryReader br)
        {
            return new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
        }

        public static Quaternion ReadQuaternion(this BinaryReader br)
        {
            return new Quaternion(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
        }

        public static IVector3 ReadIVector3(this BinaryReader br)
        {
            return new IVector3(br.ReadInt32(), br.ReadInt32(), br.ReadInt32());
        }
    }
}
