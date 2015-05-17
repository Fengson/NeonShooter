using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
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

        public static IJsonObject ToJson(this Quaternion quaternion)
        {
            return new JsonObject(
                new JsonPair("X", quaternion.x),
                new JsonPair("Y", quaternion.y),
                new JsonPair("Z", quaternion.z),
                new JsonPair("W", quaternion.w));
        }

        public static Vector3 ToVector3(JSONNode json)
        {
            var x = json["X"].AsFloat;
            var y = json["Y"].AsFloat;
            var z = json["Z"].AsFloat;
            return new Vector3(x, y, z);
        }

        public static Vector2 ToVector2(JSONNode json)
        {
            var x = json["X"].AsFloat;
            var y = json["Y"].AsFloat;
            return new Vector2(x, y);
        }

        public static Quaternion ToQuaternion(JSONNode json)
        {
            var x = json["X"].AsFloat;
            var y = json["Y"].AsFloat;
            var z = json["Z"].AsFloat;
            var w = json["W"].AsFloat;
            return new Quaternion(x, y, z, w);
        }
    }
}
