using UnityEngine;

namespace NeonShooter
{
    public class Globals : MonoBehaviour
    {
        public const float DefaultLerpFactor = 5; // seems the best value. Not too jittery, but not too slow also
        public const float DefaultCubelingSuckSpeed = 25;
        public const float DefaultCubelingSpawnerPickDelay = 2;
        public const float DefaultCubelingScatterVelocityFactor = 10;
        public const double DefaultCubelingSyncInterval = 5;


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

        public static float CubelingSpawnerPickDelay
        {
            get
            {
                if (Instance == null) return DefaultCubelingSpawnerPickDelay;
                return Instance.cubelingsSpawnerPickDelay;
            }
        }

        public static float CubelingScatterVelocityFactor
        {
            get
            {
                if (Instance == null) return DefaultCubelingScatterVelocityFactor;
                return Instance.cubelingScatterVelocityFactor;
            }
        }

        public static double CubelingSyncInterval
        {
            get
            {
                if (Instance == null) return DefaultCubelingSyncInterval;
                return Instance.cubelingSyncInterval;
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

        private static int ToFirstBit(int value)
        {
            int bit = 1;
            int i;
            for (i = 0; i < 32; i++)
            {
                if ((value & bit)!= 0) break;
                if (i < 31) bit = bit << 1;
            }
            return i;
        }

        public static int PlayersLayer
        {
            get
            {
                if (Instance == null) return ToFirstBit(LayerMask.NameToLayer("Default"));
                return ToFirstBit(Instance.playersLayer.value);
            }
        }

        public static int AtomsLayer
        {
            get
            {
                if (Instance == null) return ToFirstBit(LayerMask.NameToLayer("Default"));
                return ToFirstBit(Instance.atomsLayer.value);
            }
        }

		public static int CubelingsLayer
		{
			get
			{
				if (Instance == null) return ToFirstBit(LayerMask.NameToLayer("Default"));
				return ToFirstBit(Instance.cubelingsLayer.value);
			}
		}

        public float enemyLerpFactor = DefaultLerpFactor;
        public float cubelingsSuckSpeed = DefaultCubelingSuckSpeed;
        public float cubelingsSpawnerPickDelay = DefaultCubelingSpawnerPickDelay;
        public float cubelingScatterVelocityFactor = DefaultCubelingScatterVelocityFactor;
        public double cubelingSyncInterval = DefaultCubelingSyncInterval;

        public Material invisibleShadowCasterMaterial;
        public GameObject projectilePrefab;
        public GameObject playerCubelingPrefab;
		public GameObject playerCubelingPrefabSize5;
		public GameObject playerCubelingPrefabSize15;
		public GameObject playerCubelingPrefabSize25;
        public GameObject enemyCubelingPrefab;
        public GameObject vacuumConePrefab;

        public LayerMask playersLayer;
        public LayerMask atomsLayer;
		public LayerMask cubelingsLayer;

        public GameObject TEMP_selectedMap;

        void Start()
        {
            if (TEMP_selectedMap == null)
                throw new System.Exception("Globals: MAP IS NOT SELECTED.");
            if (TEMP_selectedMap.tag != "Map")
                throw new System.Exception("Globals: SELECTED MAP'S TAG IS NOT \"Map\"");

            GameObject.Instantiate(TEMP_selectedMap, Vector3.zero, Quaternion.identity);
        }
    }
}
