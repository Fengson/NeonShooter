using UnityEngine;

namespace NeonShooter
{
    public class Globals : MonoBehaviour
    {
        public const float DefaultLerpFactor = 5; // seems the best value. Not too jittery, but not too slow also
        public const float DefaultCubelingSuckSpeed = 25;

        static Globals instance;
        public static Globals Instance
        {
            get
            {
                if (instance == null)
                {
                    var gameObject = GameObject.FindGameObjectWithTag("Globals");
                    if (gameObject != null)
                        instance = gameObject.GetComponent<Globals>();
                }
                return instance;
            }
        }

        public static float LerpFactor
        {
            get
            {
                if (Instance == null) return DefaultLerpFactor;
                return Instance.enemyLerpFactor;
            }
        }

        public static float CubelingSuckSpeed
        {
            get
            {
                if (Instance == null) return DefaultCubelingSuckSpeed;
                return Instance.cubelingsSuckSpeed;
            }
        }

        static Material defaultMaterial;
        public static Material DefaultMaterial
        {
            get
            {
                if (defaultMaterial == null)
                    defaultMaterial = new Material(Shader.Find("Standard"));
                return defaultMaterial;
            }
        }

        public float enemyLerpFactor = DefaultLerpFactor;
        public float cubelingsSuckSpeed = DefaultCubelingSuckSpeed;
        public Material invisibleShadowCasterMaterial;
        public GameObject projectilePrefab;
    }
}
