using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LobbyItemController : MonoBehaviour {

    public string playerName;
    public bool isReady;

    public GameObject name, bulb;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (isReady) {
            bulb.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0);
        }else{
            bulb.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
        }
	}

    public void setName ( string name ) {
        this.playerName = name;
        this.name.GetComponent<Text>().text = name;
    }
}
