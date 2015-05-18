using UnityEngine;
using System.Collections;

public class JumpPadController : MonoBehaviour {

    public GameObject UpParticle, DirectionParticle;
    public float particleRotationSpeed;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (DirectionParticle != null) {
            DirectionParticle.transform.RotateAround(new Vector3(0, 1, 0), Time.deltaTime * particleRotationSpeed);
        }
	}
}
