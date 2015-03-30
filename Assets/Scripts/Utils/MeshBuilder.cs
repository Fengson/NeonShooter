using UnityEngine;

namespace NeonShooter.Utils
{
    public static class MeshBuilder
    {
        public static Mesh CreatePlane(float size)
        {
            return CreatePlane(size, size);
        }

        public static Mesh CreatePlane(float width, float height)
        {
            float halfWidth = width * 0.5f;
            float halfHeight = height * 0.5f;

            Mesh mesh = new Mesh();
            mesh.name = "PlaneMesh";
            mesh.vertices = new Vector3[]
            {
                new Vector3(-halfWidth, -halfHeight, 0),
                new Vector3(halfWidth, -halfHeight, 0),
                new Vector3(halfWidth, halfHeight, 0),
                new Vector3(-halfWidth, halfHeight, 0)
            };
            mesh.uv = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(1, 0)
            };
            mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}
