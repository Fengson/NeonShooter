using UnityEngine;
using System.Collections;

public class FlyToPlayerWithGraceOfFuckingSwan : MonoBehaviour {

	public float speed;
	
	// Update is called once per frame
	void Update () {
		GameObject player = GameObject.FindWithTag ("Player");
		float step = speed * Time.deltaTime;

		transform.position = Vector3.MoveTowards(transform.position, player.transform.position, step);
		transform.eulerAngles = new Vector3 (1, 1, 1);
	
	}
}
