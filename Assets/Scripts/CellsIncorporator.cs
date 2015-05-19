using UnityEngine;
using System.Collections;
using NeonShooter.Players.Cube;

namespace NeonShooter
{
    public class CellsIncorporator : MonoBehaviour
    {
        public static int amount = 1000;
        public AudioClip impact;

        void OnTriggerEnter(Collider other)
        {
			if (other.gameObject.CompareTag("Cubeling") && other.gameObject.GetComponent<IsCubelingPickabe>().pickable)
            {
                Destroy(other.gameObject);
                amount++;
                //TODO different sounds for different weapons
                GetComponent<AudioSource>().PlayOneShot(impact, 0.7F);
				gameObject.GetComponent<CubeOfCubes>().AddCubeling();
            }
        }
    }
}