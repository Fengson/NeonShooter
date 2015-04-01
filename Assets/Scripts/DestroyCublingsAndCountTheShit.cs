using UnityEngine;
using System.Collections;

public class DestroyCublingsAndCountTheShit : MonoBehaviour {

	public static int amount = 0;
	public AudioClip impact;
	
	void OnTriggerEnter (Collider other) {
		if (other.gameObject.CompareTag ("Cubeling")) {
			Destroy (other.gameObject);
			amount ++;
			GetComponent<AudioSource>().PlayOneShot(impact, 0.7F);
		}
	}
}
