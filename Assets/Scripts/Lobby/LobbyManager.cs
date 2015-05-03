using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviour {

    public GameObject prefabItemuListy;
    public GameObject panelDoWyswietlenia;
    public List<LobbyItemController> plakietki;
    
    public int yCordToShow = 50;
    
	// Use this for initialization
	void Start () {
        yCordToShow = 50;

        // to fuck out \/ (debug)
        DodajGracza("Pawel");
        DodajGracza("Janusz");
        DodajGracza("Andrzej");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void DodajGracza ( string n ) {
        var p = Instantiate(prefabItemuListy);
        p.GetComponent<LobbyItemController>().setName(n);
        p.transform.SetParent(panelDoWyswietlenia.transform);
        p.transform.position = new Vector2(-30, yCordToShow);
        p.transform.localScale = new Vector2(0.7f, 0.7f);
        yCordToShow -= 10;
    }

    public void ReadyButtonClicked () {
        print("ReadyButtonClicked");
    }
}
