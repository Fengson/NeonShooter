using UnityEngine;
using System.Collections;
using NeonShooter.Players;

public class JumpPadScript : MonoBehaviour {


	public Vector3 moveDirection;
	public float gravity = 9.00f;

	bool isJumping  = false;
	bool Jump = false;
	Vector3 tmpDirection;
	CharacterController controller = null;
    	private bool touchingJumpPad = false;

    // Use this for initialization
    void Start() {
		CalculateMovement();
	}

	void CalculateMovement()
	{
		tmpDirection = moveDirection;
	}


    void FixedUpdate()
    {

        if (controller != null)
        {
            if (Jump)
            {             
                isJumping = true;
            }

            if (controller.isGrounded)
            {
                // jumper mode
                if (touchingJumpPad == false)
                {
                    controller = null;
                    isJumping = false;
                    return;
                }

                Jump = true;
                isJumping = true;
               
              
            }

            if (isJumping )
            {
               
                Jump = false;

                controller.Move(tmpDirection * Time.deltaTime);

                if (tmpDirection.y > 0)
                {
                    tmpDirection.y -= gravity * Time.deltaTime;
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (touchingJumpPad) return;
        touchingJumpPad = true;

        if (other.CompareTag("Player"))
        {
            Jump = true;
            CalculateMovement();
            
			var player = other.GetComponent<Player>() as Player;
			if (player.sounds[4] != null) player.sounds[4].Play();
    
            controller = other.gameObject.GetComponent<CharacterController>();
        }
    }

   

    void OnTriggerExit(Collider other)
    {
        if (touchingJumpPad == false) return;
        touchingJumpPad = false;     
    }
}
