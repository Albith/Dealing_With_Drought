using UnityEngine;
using System.Collections;

//mapObject is the class that manages the interaction with each water emergency button.

public class mapObject : MonoBehaviour {

	Color colorNormal;
	public Color colorHighlighted;
	Color colorPressed;

	bool isButtonPressed= false;
	public bool isIncidentResolved= false;

	public int positionID=0;

	public int personnelCost=0;
	
	
	public enum incident 
	{
		WATER_LEAK,
		WATER_SCARCITY,	
		WATER_THEFT
	};

	public incident incidentType;

	//incident icons for animation.
	float blinkDuration=0.6f;
	


	// Use this for initialization
	void Start ()
	{
	
		colorNormal = Color.white;
		colorHighlighted= new Color32(147,159,216,255);
		colorPressed = new Color32 (238, 160, 61, 255);

		//Assigning an incident value.
		if(incidentType==incident.WATER_LEAK)
			personnelCost=gameManager.leakPersonnelCost;

		else if(incidentType==incident.WATER_SCARCITY)
			personnelCost=gameManager.scarcityPersonnelCost;
	
		else if(incidentType==incident.WATER_THEFT)
			personnelCost=gameManager.theftPersonnelCost;
		
	}
	
	public void setID(int newID)
	{
		
		positionID=newID;
		
	}
	

//---

	public void refresh()
	{
		//resetting states, image sprites and colors.
		
		isButtonPressed= false;
		isIncidentResolved= false;
		gameObject.GetComponent<SpriteRenderer> ().color = colorNormal;
		
		
		if(incidentType==incident.WATER_LEAK)
		{	
		   gameObject.GetComponent<SpriteRenderer> ().sprite = 
		   Resources.Load<Sprite>("brokenPipeIcon");
		}
		
		else  //It's a  water scarcity event.
		{
			gameObject.GetComponent<SpriteRenderer> ().sprite = 
			Resources.Load<Sprite>("thirst2Icon");
		
		}
	
	}

//--Showing animated 

	IEnumerator blinkPersonnel()
	{
	
		while(true)
		{
		
			//show Icons.
			foreach(Transform child in gameObject.transform)
			{
				child.gameObject.SetActive(true);
			}
			
				
		
			yield return new WaitForSeconds(blinkDuration);
		
			//hide Icons.
			foreach(Transform child in gameObject.transform)
			{
				child.gameObject.SetActive(false);
			}
		
			yield return new WaitForSeconds(blinkDuration);
		
		}
	
	}

	
//---Event Methods.
	void OnMouseDown()
	{
	
		print ("MapObject.cs, OnMouseDown(): Trying to resolve incident...");
		
	
		if(!isIncidentResolved && gameManager.isGameStarted)
			{
			
					gameObject.GetComponent<SpriteRenderer> ().color = colorPressed;
			
			
			
					//Send a message to the mapManager, to update the status Message.
					if(mapManager.mapInstance.tryResolvingIncident(personnelCost))
					{
						
						//disable the button.
						isIncidentResolved=true;
						
						
						//show a success message on mapManager (to be done by the mapManager object).
						mapManager.mapInstance.updateMapMessage(mapManager.mapMessage.INCIDENT_RESOLVED, 
						                                        incidentType);
					
					
						//Start coroutine to show and blink the personnel icons.
						StartCoroutine(blinkPersonnel());
					
					}
					
					else
					{
						//show a failure message on mapManager (to be done by the mapManager object).
						mapManager.mapInstance.updateMapMessage(mapManager.mapMessage.NOT_ENOUGH_PERSONNEL, 
					                                        incidentType);
					}
		
	
	
			}
	
	}

	void OnMouseUp()
	{
	
		
		if(!isIncidentResolved)
			gameObject.GetComponent<SpriteRenderer> ().color = colorHighlighted;
		
	}


	void OnMouseEnter()
	{
		if(!isIncidentResolved)
		{
			//Send a message to the mapManager, to update the status Message.
			if(incidentType==incident.WATER_LEAK)
				mapManager.mapInstance.updateMapMessage(mapManager.mapMessage.CLICK_PROMPT_LEAK, 
			                                        incidentType);
		
			else
				mapManager.mapInstance.updateMapMessage(mapManager.mapMessage.CLICK_PROMPT_SCARCITY, 
				                                        incidentType);
		
			//Highlight the color.
			gameObject.GetComponent<SpriteRenderer> ().color = colorHighlighted;
		}
		
		else
			{
				
			if(incidentType==incident.WATER_LEAK)
				mapManager.mapInstance.updateMapMessage(mapManager.mapMessage.LEAK_BEING_REPAIRED, 
				                                        incidentType);
			else
				mapManager.mapInstance.updateMapMessage(mapManager.mapMessage.WATER_BEING_SENT, 
			                                        incidentType);	
			
			}
		
	}

	void OnMouseExit()
	{
	
		//Send a message to the mapManager, to update the status Message.
		mapManager.mapInstance.updateMapMessage(mapManager.mapMessage.GENERAL_MESSAGE, 
		                                        incidentType);
		
	
		if(!isIncidentResolved)
		{

			gameObject.GetComponent<SpriteRenderer> ().color = colorNormal;
		}
	}
	
	

}
