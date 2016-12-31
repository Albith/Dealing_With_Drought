using UnityEngine;
using System.Collections;

//arrowObject is used to change the rationing period in the game.
public class arrowObject : MonoBehaviour {

	Color colorNormal;
	Color colorHighlighted;
	Color colorPressed;

	bool isButtonPressed= false;

	public bool isButtonIncreasing;

	// Use this for initialization
	void Start () {
	
		colorNormal = Color.white;
		colorHighlighted= new Color32(147,159,216,255);
		colorPressed = new Color32 (238, 160, 61, 255);

	}
	
	void OnMouseDown()
	{
		//Updating the rationing status.
		gameManager.myGameManager.rationingButtonsPressed (isButtonIncreasing);
		gameObject.GetComponent<SpriteRenderer> ().color = colorPressed;

	}

	void OnMouseUp()
	{
		
		gameObject.GetComponent<SpriteRenderer> ().color = colorHighlighted;
		
	}


	void OnMouseEnter()
	{
	
		print ("Entered arrow object.");
		gameObject.GetComponent<SpriteRenderer> ().color = colorHighlighted;

	}

	void OnMouseExit()
	{
		gameObject.GetComponent<SpriteRenderer> ().color = colorNormal;

	}

}
