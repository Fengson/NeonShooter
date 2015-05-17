using UnityEngine;

namespace NeonShooter.Utils
{
    public static class GameObjectMaker
    {
        public static GameObject CreateGameObject(string name, Mesh mesh, Material material)
        {
            var gameObject = new GameObject(name);
            var meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            var renderer = gameObject.AddComponent<MeshRenderer>();
            if (material == null) material = Globals.DefaultMaterial;
            else renderer.material = material;
            return gameObject;
        }

        public static GameObject CreateGameObject(string name, Mesh mesh,
            Vector3 translation, Vector3 rotation, Vector3 scale, Material material)
        {
            var gameObject = CreateGameObject(name, mesh, material);
            gameObject.transform.localPosition = translation;
            gameObject.transform.localEulerAngles = rotation;
            gameObject.transform.localScale = scale;
            return gameObject;
        }

        public static GameObject CreateGameObject(string name,
            Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            var gameObject = new GameObject(name);
            gameObject.transform.localPosition = translation;
            gameObject.transform.localEulerAngles = rotation;
            gameObject.transform.localScale = scale;
            return gameObject;
        }
    }
}
