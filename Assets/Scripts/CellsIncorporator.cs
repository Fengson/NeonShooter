using UnityEngine;
using System.Collections;

namespace NeonShooter
{
    public class CellsIncorporator : MonoBehaviour
    {
        public static int amount = 0;
        public AudioClip impact;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Cell"))
            {
                Destroy(other.gameObject);
                amount++;
                GetComponent<AudioSource>().PlayOneShot(impact, 0.7F);
            }
        }
    }
}