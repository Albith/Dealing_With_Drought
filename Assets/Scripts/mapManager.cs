using UnityEngine;
using System.Collections;
using System.Collections.Generic;


using UnityEngine.UI;	//for using Text objects.

//mapManager Manages the location and processing of water emergencies over the map.
public class mapManager : MonoBehaviour {


	public static mapManager mapInstance;

	public Transform incidentGroupParent;
	public Vector3 [] incidentSpawnPositions;
	//The following array will contain the following number types:
		//0: position is unoccupied.
		//1: there is an unresolved incident here.
		//-1: this incident has been resolved and the mapObject can be cleared.
	public bool [] isPositionOccupied_Array;
	public static int [] numberOfIncidents= {0,0};	//Water Leaks , Scarcity Incidents.

	//array that stores location of water leaks and water scarcity.
	Vector3 [] incidentPoints;
	
	public Text mapStatusText;
	string tempMapStatus="";
	
	
	public enum mapMessage 
	{
		GENERAL_MESSAGE,
		CLICK_PROMPT_LEAK,
		CLICK_PROMPT_SCARCITY,
		INCIDENT_RESOLVED,
		LEAVE_EMPTY,
		NOT_ENOUGH_PERSONNEL,
		LEAK_BEING_REPAIRED,
		WATER_BEING_SENT			

	};
	
	
	// Use this for initialization
	void Awake () {
	
		mapInstance=this;
			
		//initializing the mapPositions;
		incidentSpawnPositions= new Vector3[7];
			incidentSpawnPositions[0]= new Vector3(7.7f, 3.6f, -43.057f );
			incidentSpawnPositions[1]= new Vector3(-28.4f, -1.3f, -43.057f );
			incidentSpawnPositions[2]= new Vector3(-7.7f, 15.6f, -43.057f );
			incidentSpawnPositions[3]= new Vector3(-16.8f, 9.6f, -43.057f );
			incidentSpawnPositions[4]= new Vector3(18.3f, 10.2f, -43.057f );
			incidentSpawnPositions[5]= new Vector3(-5.9f, -2.9f, -43.057f );
			incidentSpawnPositions[6]= new Vector3(-19.24f, -3.78f, -43.057f );
	
		
		//Setting occupied positions as false.		
		isPositionOccupied_Array= new bool[incidentSpawnPositions.Length];
		
		for(int i=0; i< incidentSpawnPositions.Length; i++)
			isPositionOccupied_Array[i]=false;
		
	}
	
	public bool tryResolvingIncident(int personnelCost)
	{
	
			bool isSolved=false;
		
			//If you have enough personnel, the incident is solved.
			if(gameManager.myGameManager.tryToUsePersonnel(personnelCost))
				isSolved=true;
									
				
			return isSolved;
		
	}
	
	public void updateMapMessage(mapMessage msgType, mapObject.incident incidentType)
	{
	
		if(msgType==mapMessage.CLICK_PROMPT_LEAK)
			{
				tempMapStatus="<color=blue><b><i>Requires "+ gameManager.leakPersonnelCost.ToString() +
							" personnel.</i></b></color> Click to attempt to resolve it.";
			}
	
		if(msgType==mapMessage.CLICK_PROMPT_SCARCITY)
			{
				tempMapStatus="<color=blue><b><i>Requires "+ gameManager.scarcityPersonnelCost.ToString() +
				" personnel.</i></b></color> Click to attempt to resolve it.";
			}
			
		else if(msgType==mapMessage.GENERAL_MESSAGE)
			tempMapStatus="<color=blue><b><i>Hover</i></b></color> and <color=blue><b><i>click over</i></b></color> incidents on the map.";
		
		else if(msgType==mapMessage.INCIDENT_RESOLVED)
			{
				//updating mapStatus text and incident counter.
				if(incidentType== mapObject.incident.WATER_LEAK)
					{
						tempMapStatus="You send out a brigade to repair the water leak.";
						numberOfIncidents[0]= numberOfIncidents[0]-1;
						
						if(numberOfIncidents[0]<=0)
							numberOfIncidents[0]=0;
						

						
					}
				
				else if(incidentType== mapObject.incident.WATER_SCARCITY)
					{
						tempMapStatus="You send out a brigade to prepare a water fill station.";
						numberOfIncidents[1]= numberOfIncidents[1]-1;					
						
						if(numberOfIncidents[1]<=0)
							numberOfIncidents[1]=0;
				
						//This decreases distress.
						gameManager.myGameManager.effectOfWaterDrive();
					}
					
				
			}
			
		else if(msgType==mapMessage.NOT_ENOUGH_PERSONNEL)
		{
			
			tempMapStatus="You don't have enough personnel to do this!";	
			
		}	
			
		else if(msgType==mapMessage.LEAVE_EMPTY)
			tempMapStatus="";
		
		else if(msgType==mapMessage.WATER_BEING_SENT)
			tempMapStatus="A brigade is manning a water fill station here.";
		
		else if(msgType==mapMessage.LEAK_BEING_REPAIRED)
			tempMapStatus="A brigade is repairing this water leak.";
		
		
		
		mapStatusText.text=tempMapStatus;		

			


	}


	public void disableAllIncidents()
	{
		
		GameObject [] currentIncidents= GameObject.FindGameObjectsWithTag("incident");
		
		if(currentIncidents.Length>0)
		{
			for(int i=0; i< currentIncidents.Length; i++)
			{
				//Updating the position occuppied boolean array.
				currentIncidents[i].GetComponent<mapObject>().isIncidentResolved= true;				
				currentIncidents[i].GetComponent<SpriteRenderer>().color= 
					currentIncidents[i].GetComponent<mapObject>().colorHighlighted;
					
				//Disable the collider as well.
				currentIncidents[i].GetComponent<CircleCollider2D>().enabled=false;	
					
			}
			
			
			
		}
		
		
	}		//end of disableAllIncidents().





	public void deleteAllIncidents()
	{

		GameObject [] currentIncidents= GameObject.FindGameObjectsWithTag("incident");
		
		if(currentIncidents.Length>0)
		{
			for(int i=0; i< currentIncidents.Length; i++)
				{
						//Updating the position occuppied boolean array.
						int positionId= currentIncidents[i].GetComponent<mapObject>().positionID;
						isPositionOccupied_Array[positionId]=false;
						
						//Removing the Map Objects from the scene.
						Destroy (currentIncidents[i]);
						
					
				}
				
				
				
			}
			
			
		}		//end of deleteAllIncidents().


	public void updateIncidents()
	{
	
		//0. First, deleting any incidents that may be in the incidentsList.
		
		GameObject [] currentIncidents= GameObject.FindGameObjectsWithTag("incident");
		
		if(currentIncidents.Length>0)
			{
			for(int i=0; i< currentIncidents.Length; i++)
				{
				
				if(currentIncidents[i].GetComponent<mapObject>().isIncidentResolved)
						{
						
							//Updating the position occuppied boolean array.
							int positionId= currentIncidents[i].GetComponent<mapObject>().positionID;
							isPositionOccupied_Array[positionId]=false;
									
							//Removing the Map Objects from the scene.
							Destroy (currentIncidents[i]);
												
																						
						}
				
				
							
				}
									
			
			}
	
	
		//1. By this point, only unresolved incidents should remain.
		//Let's figure out how many incidents should be on screen.
	
		int tempNumberOfIncidents=0;
	
		//determine how many incidents should occur.
		if(gameManager.currentTurn>=6)
			tempNumberOfIncidents= Random.Range (4,6);
	
		else if(gameManager.currentTurn>=4)
			tempNumberOfIncidents= Random.Range (3,5);
		
		else if(gameManager.currentTurn>=2)
			tempNumberOfIncidents= Random.Range(2,4);
			
		else if(gameManager.currentTurn>=0)
			tempNumberOfIncidents= Random.Range(1,3);
		
		
		print ("MapManager.cs, generateNewIncidents(): number of Incidents is : "
				+tempNumberOfIncidents
				+", for currentTurn "+gameManager.currentTurn);
		
					
		//2. If there are less incidents on screen than the tempNumber, make new incidents.
		
		int totalNumberOfIncidents= numberOfIncidents[0]+numberOfIncidents[1];
		
		if( tempNumberOfIncidents>totalNumberOfIncidents )
		{
				
				for(int i=0; i<(tempNumberOfIncidents - totalNumberOfIncidents); i++)
				{
					
					//A. Find a location that is unoccupied.
					//you can do this by randomizing.
					

					int spawnIndex=0;

					
						//Here we find a map location that is free.
						//loop through the array, to find an available spot.
						
						bool freeSpaceFound=false;
						
						while(!freeSpaceFound)
							{
								spawnIndex= Random.Range (0, isPositionOccupied_Array.Length);
								
								if(!isPositionOccupied_Array[spawnIndex])
									freeSpaceFound=true;
							
							}
					
										
					//B. Spawn one of the two kinds of events.
					
					GameObject newIncident;
					
					int randIncidentType= Random.Range (0,4);
					
					//Instantiating the Incident and increasing the counter.
					if(randIncidentType<=1 )
						{
							newIncident = Instantiate(Resources.Load("leakIncident")) as GameObject;
							//the 0 index corresponds to leak incidents.
							numberOfIncidents[0]= numberOfIncidents[0]+1;
						}
					
					else	
						{
							newIncident = Instantiate(Resources.Load("scarcityIncident")) as GameObject;
							//the 0 index corresponds to scarcity incidents.
							numberOfIncidents[1]= numberOfIncidents[1]+1;
						}
						//setting the position ID for this object.				
						newIncident.GetComponent<mapObject>().positionID=spawnIndex;
						

						
										
					//C. After increasing the incident counter,
						 //we set the boolean for the occupied positions to true.
						isPositionOccupied_Array[spawnIndex]=true;
				
					//D. Placing the spawned Incident as a child of the Incident Group Panel.
						newIncident.transform.SetParent(incidentGroupParent);
						//setting the position of the newIncident.
						newIncident.transform.localPosition= incidentSpawnPositions[spawnIndex];
					
					
					
				}
		}	
		
		
		//Let's try this. Hopefully the program won't crash on the while loop.
		
		print ("MapManager.cs, generateNewIncidents(): generated Incidents are: "+
		  	   " Leaks "+numberOfIncidents[0].ToString()+
		       ", Scarcity "+numberOfIncidents[1].ToString()+".");
								
		
	}

	//Implementing the Fisher-Yates algorithm.
//	void shuffleIncidents()
//	{
//		int size = incidentsToCall.Length;
//		for (int i = 0; i < size; i++)
//		{
//			// NextDouble returns a random number between 0 and 1.
//			// ... It is equivalent to Math.random() in Java.
//			int newIndex = i + (int)(Random.Range(0f,1f) * (size - i));
//			int temp = incidentsToCall[newIndex];
//			incidentsToCall[newIndex] = incidentsToCall[i];
//			incidentsToCall[i] = temp;
//		}
//	
//	}
	
	
	
}
