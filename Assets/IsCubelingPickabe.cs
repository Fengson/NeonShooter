using UnityEngine;
using System.Collections;

public class IsCubelingPickabe : MonoBehaviour {

	public bool pickable = false;

	//If cubes are part of out player there should be some time when he can't pick them up
	public bool timed = true;
	public Material inactive;

	float timer = 5; 
	
	void Update(){ 
		if (timed && timer > 0) {
			timer -= Time.deltaTime;
			this.gameObject.GetComponent<Renderer>().material = inactive;
		} else {
			pickable = true;
			this.gameObject.GetComponent<Renderer>().material = new Material(Shader.Find("Diffuse"));
			//Debug.Log("Pickable");
		}
	}


}
