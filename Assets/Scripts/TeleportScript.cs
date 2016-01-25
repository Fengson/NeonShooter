using UnityEngine;
using System.Collections;

public class TeleportScript : MonoBehaviour {

    public Vector3 moveToDirection;
    CharacterController controller = null;
    
    void OnTriggerEnter(Collider other)
    {
        controller = other.gameObject.GetComponent<CharacterController>();
        controller.transform.position = moveToDirection;
    }

}
