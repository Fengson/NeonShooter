using UnityEngine;
using System.Collections;

public class JumpPadScript : MonoBehaviour {


	public Vector3 moveDirection;
	public float gravity = 9.00f;

	bool isJumping  = false;
	bool Jump = false;
	Vector3 tmpDirection;
	CharacterController controller = null;

	// Use this for initialization
	void Start() {
		CalculateMovement();
	}

	void CalculateMovement()
	{
		tmpDirection = moveDirection;
	}

	
	// Update is called once per frame
	void Update () {
	
		if (controller != null) 
		{
			if (controller.isGrounded)
			{
				isJumping = false;	
			}
			if (Jump)
			{
				isJumping = true;
			}
			if (isJumping)
			{
				Jump = false;
				controller.Move(tmpDirection * Time.deltaTime);
			}

			if(tmpDirection.y>0)
			{ 
				tmpDirection.y -= gravity * Time.deltaTime;
			}
		}
	}

	void OnTriggerEnter(Collider other) {	
		if(other.CompareTag("Player")){
			 controller = other.gameObject.GetComponent<CharacterController>();
			CalculateMovement();	
			Jump = true;
		}
	}
}
