using UnityEngine;
using System.Collections;

public class destructionController : MonoBehaviour {

	public GameObject crumbled;
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.Z)) {
			Object.Instantiate(crumbled,transform.position,transform.rotation);
			Destroy(gameObject);
		}
	}
}
