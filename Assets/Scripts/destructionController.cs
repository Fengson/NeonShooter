using UnityEngine;
using System.Collections;

namespace NeonShooter
{
    public class DestructionController : MonoBehaviour
    {
        public GameObject crumbled;

        void Update()
        {
            if (Input.GetKey(KeyCode.Z))
            {
                Object.Instantiate(crumbled, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
    }
}