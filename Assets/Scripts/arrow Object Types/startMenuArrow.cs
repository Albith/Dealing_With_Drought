using UnityEngine;
using System.Collections;

//This arrow object is used to move between tutorial screens.
public class startMenuArrow : MonoBehaviour {

	Color colorNormal;
	Color colorHighlighted;
	Color colorPressed;

	bool isButtonPressed= false;

	public bool isGoingToNext;
	//0->to Previous Scene.

	// Use this for initialization
	void Start () {
	
		colorNormal = Color.white;
		colorHighlighted= new Color32(147,159,216,255);
		colorPressed = new Color32 (238, 160, 61, 255);

	}
	
	void OnMouseDown()
	{
		//Updating the rationing status.	
		if(isGoingToNext)
			gameManager.myGameManager.myDisplay.startMenuNext();
		else 	
			gameManager.myGameManager.myDisplay.startMenuPrevious();
			
		gameObject.GetComponent<SpriteRenderer> ().color = colorPressed;

	}

	void OnMouseUp()
	{
		
		gameObject.GetComponent<SpriteRenderer> ().color = colorHighlighted;
		
	}


	void OnMouseEnter()
	{
		gameObject.GetComponent<SpriteRenderer> ().color = colorHighlighted;

	}

	void OnMouseExit()
	{
		gameObject.GetComponent<SpriteRenderer> ().color = colorNormal;

	}

}
