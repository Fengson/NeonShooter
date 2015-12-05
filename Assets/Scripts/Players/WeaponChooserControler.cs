using UnityEngine;
using System.Collections;
using NeonShooter.Players;

public class WeaponChooserControler : MonoBehaviour {

    public bool lockMouse;

    public Player player;

    private Vector3 lastMousePosition;
 

	// Use this for initialization
	void Start () {
        lockMouse = false;

        if (player == null)
        {
            Debug.LogWarning("Player in WeaponChooseController is NULL");
        }
	}
	
	// Update is called once per frame
	void Update () {
        /*
        if (Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            player.ChangeWeaponToNext();
        }*/
	}

    private void WeaponCircle()
    {
        if (Input.GetMouseButtonDown(2))
        {
            //Start recording directioon of mouse
            lastMousePosition = Input.mousePosition;
        }


        if (Input.GetMouseButton(2))
        {
            Debug.Log("Pressed middle click.");
            lockMouse = true;
            Vector3 newMousePosition = Input.mousePosition;
            Vector3 mouseDirection = lastMousePosition - newMousePosition;
            mouseDirection.Normalize();


            lastMousePosition = newMousePosition;

        }
        else
        {
            lockMouse = false;
        }

        if (Input.GetMouseButtonUp(2))
        {
            //ChooseWeapon
        }

    }
}
