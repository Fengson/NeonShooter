using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NeonShooter.AppWarp.Json
{
    public static class JsonConverter
    {
        public static JsonObject ToJson(this Vector3 vector)
        {
            return new JsonObject(
                new JsonPair("X", vector.x),
                new JsonPair("Y", vector.y),
                new JsonPair("Z", vector.z));
        }

        public static JsonObject ToJson(this Vector2 vector)
        {
            return new JsonObject(
                new JsonPair("X", vector.x),
                new JsonPair("Y", vector.y));
        }

        public static JsonObject ToJson(this Quaternion quaternion)
        {
            return new JsonObject(
                new JsonPair("X", quaternion.x),
                new JsonPair("Y", quaternion.y),
                new JsonPair("Z", quaternion.z),
                new JsonPair("W", quaternion.w));
        }

        public static JsonObject ToJson(this IVector3 vector)
        {
            return new JsonObject(
                new JsonPair("X", vector.X),
                new JsonPair("Y", vector.Y),
                new JsonPair("Z", vector.Z));
        }

        public static long AsLong(this JSONNode json)
        {
            return Convert.ToInt64(json.Value);
        }

        public static Vector3 AsVector3(this JSONNode json)
        {
            var x = json["X"].AsFloat;
            var y = json["Y"].AsFloat;
            var z = json["Z"].AsFloat;
            return new Vector3(x, y, z);
        }

        public static Vector2 AsVector2(this JSONNode json)
        {
            var x = json["X"].AsFloat;
            var y = json["Y"].AsFloat;
            return new Vector2(x, y);
        }

        public static Quaternion AsQuaternion(this JSONNode json)
        {
            var x = json["X"].AsFloat;
            var y = json["Y"].AsFloat;
            var z = json["Z"].AsFloat;
            var w = json["W"].AsFloat;
            return new Quaternion(x, y, z, w);
        }

        public static IVector3 AsIVector3(this JSONNode json)
        {
            var x = json["X"].AsInt;
            var y = json["Y"].AsInt;
            var z = json["Z"].AsInt;
            return new IVector3(x, y, z);
        }

        public static List<T> AsList<T>(this JSONNode jsonArray, Func<JSONNode, T> converter)
        {
            List<T> result = new List<T>();
            for (int i = 0; i < jsonArray.Count; i++)
            {
                result.Add(converter(jsonArray[i]));
            }
            return result;
        }
    }
}
