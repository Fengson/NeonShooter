using UnityEngine;

namespace NeonShooter
{
    public class Globals : MonoBehaviour
    {
        static Globals instance;
        public static Globals Instance
        {
            get
            {
                if (instance == null)
                {
                    var gameObject = GameObject.FindGameObjectWithTag("Globals");
                    if (!gameObject == null)
                        instance = gameObject.GetComponent<Globals>();
                }
                return instance;
            }
        }

        public float enemyLerpFactor = 5; // seems the best value. Not too jittery, but not too slow also
        public Shader invisibleShadowCasterShader;
    }
}
