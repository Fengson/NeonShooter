using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour {

	public Text lifeText;
	int life = 100;

	// Use this for initialization
	void Start () {
		lifeText = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		lifeText.text = "Life: " + (life + DestroyCublingsAndCountTheShit.amount);
	}
}
