using UnityEngine;
using System.Collections;

using UnityEngine.UI;	//for using Text objects.

//for using Dictionaries.
using System.Collections.Generic;	


//displayManager contains variables and methods to display
//text information and user interface elements (except the map's incident elements,
//handled in mapManager.cs.)

public class displayManager : MonoBehaviour {


//--Coroutine variables.
	float end_flashDelay=1f;
	Color default_statusColor;
	Color flash_statusColor;

//---Start Menu variables.------

	public GameObject startMenuHandle;	
	//Start screen text.
	public GameObject[] startMenuTexts;
	//Start screen images.
	public GameObject[] startMenuImages;

	public messageContainer myMessages;


//---Game Menu variables.-------

	//This class points to the display text strings in the Game.
	public Text currentTurnText;
	public Text currentRationingText;
	public Text popDistressText;
	public Text nextTurnButton_Text;

	//Important! also displays the status Message Texts.
	public Text currentStatusText;
	public Text newsTitleText;
	public Text newsText;
	public Text waterStatusText;
	
		string incidentMessages="";

		string dispatchReportString="";
		string newsReportString="";


	//Water Use Graphic variables.
		public Text waterLevelText;
		public GameObject waterCube;
		
		//reference value for water display.
		//the totalWaterPercentage goes from 0 (min water value) to 100 (max water value)
		float totalWaterPercentage=1;
		
		//Angle levels for the water graphic elevation.
		float highWater_Ypos=20.8f;
		float lowWater_Ypos=1.25f;

		//Water Levels in Meters for reference.
		float maxWaterLevel;
		float minWaterLevel;

		//Color values for the text.
		Color startWaterLevelColor;
		Color dangerWaterLevelColor;
			int numberOfLevelMarkers=5;//4;
			float xOffset_LevelMarkers=0.4f;

	//Population Distress Color variables.
		Color defaultDistressColorValue;
		Color minDistressColorValue;
		Color maxDistressColorValue;
		Color[] distressColorValues;

	//Personnel Color Values
		Color unavailableWorkerColorValue;
		Color availableWorkerColorValue;
		

//------ Start Menu Buttons and Display Methods.

	public void startMenuNext()
	{

	
		//increment the counter.
		gameManager.currentStartScreen++;
		
		if(gameManager.currentStartScreen>= startMenuTexts.Length)
			startGame();	
		else
			{
				if(gameManager.currentStartScreen== (startMenuTexts.Length-1))
				{
					//show the begin Game prompt in the last text.
					GameObject beginGamePrompt= GameObject.FindGameObjectWithTag("beginGamePrompt");
					beginGamePrompt.GetComponent<Text>().enabled=true;
				
				}
				
			
				//hide the previous text and image.
				startMenuTexts[gameManager.currentStartScreen-1].SetActive(false);
				startMenuImages[gameManager.currentStartScreen-1].SetActive(false);	
					
				//show the current text and image.
				startMenuTexts[gameManager.currentStartScreen].SetActive(true);	
				startMenuImages[gameManager.currentStartScreen].SetActive(true);	
			
			
			}
		
		
	}
	
	public void startMenuPrevious()
	{
		if(gameManager.currentStartScreen>0)
		{
			//hide the previous text.
			startMenuTexts[gameManager.currentStartScreen].SetActive(false);
			
			gameManager.currentStartScreen--;
			
			//show the current text.
			startMenuTexts[gameManager.currentStartScreen].SetActive(true);			
					
			
		}	
		
	}
	
	public void startGame()
	{
		
		//Hide the start Menu Gameobject and all its components.
		startMenuHandle.SetActive(false);
		
		//Generate incidents, etc.		
		gameManager.myGameManager.gameStart();
		
	}



//----- Start() is automatically run when the game starts.
	void Start () {
	
	
		maxWaterLevel= gameManager.currentWaterLevel;
		minWaterLevel= gameManager.minWaterLevel;
	
		//initializing Color values.
			//status Display colors.
		default_statusColor= 
			GameObject.FindGameObjectWithTag("dispatchBckgd").GetComponent<SpriteRenderer>().color;
			flash_statusColor=  new Color32(246, 236, 193, 255);
		
		
		//Update: changes water Cube color. 
			startWaterLevelColor = new Color32 (92,151,233,147);
			dangerWaterLevelColor = new Color32 (127,150,138,147) ;//(142,171,160,147);
			placeWaterIndicators();

		//Population Distress color values and method calls.
			defaultDistressColorValue= new Color32 (101,73,14,255);
			
			minDistressColorValue= Color.green;
			maxDistressColorValue= Color.red;
			
			distressColorValues= new Color[11];
			
			setupDistressColorValues();
			updateDistressDisplay();

		//Personnel color values and method calls.
			unavailableWorkerColorValue= new Color32 (101,73,14,255);;
			availableWorkerColorValue= Color.white;
			
			setupPersonnelColorValues();
				
			if(gameManager.myGameManager.startWithoutStartScreen)
			{	
				if(startMenuHandle.activeSelf)	
					startMenuHandle.SetActive(false);
			
				 showGameMenu();
				 
				 nextTurnButton_Text.text= "Begin";
				 
			}
			
			else		
			{	
				if(!startMenuHandle.activeSelf)	
					startMenuHandle.SetActive(true);
				
			
			}
			
			
	}
	
		
	
//----Display the Game Start. This function is inactive right now.
	public void showGameMenu()
	{
		//currentStatusText.text=messageContainer.statusMessages[messageContainer.statusType.GAME_START];
		//waterLevelText.text=gameManager.currentWaterLevel+"m";
		
	
	}	
	
	
//------Setup methods for our two value meters: personnel available and population distress.
	void setupPersonnelColorValues()
	{
		
			GameObject personnelGroupParent= GameObject.FindGameObjectWithTag("personnelGroup");
				
			//assigning the color in the child objects.
			for(int i=0; i<personnelGroupParent.transform.childCount; i++)
			{
			
				personnelGroupParent.transform.GetChild(i).GetComponent<Image>().color=
					availableWorkerColorValue;
						
			}			
		
	}
	
	
	void setupDistressColorValues()
	{
		for(int i=0; i<distressColorValues.Length; i++)
		{
			distressColorValues[i]=  Color.Lerp(minDistressColorValue, 
			                                  maxDistressColorValue, 
			                                  (float)(i+1)/
			                                  		distressColorValues.Length );	
	
		}
	
	}
	
	//Sets up the markers in the diagram of the water reservoir.
	
	void placeWaterIndicators()
	{

	//--------Part 1: placing the level Indicators---------//
			
			//0.spawning the level Indicators.
			GameObject[] waterLevelIndicators= new GameObject[numberOfLevelMarkers];
	
			//1.Setup position variables.
			
			float yPositionOffset= 30.5276f;
			Vector3 startPosition= new Vector3( 4f,
		                                   		highWater_Ypos+yPositionOffset,
		                                   		34.81219f); 
	
			float yPositionGap= (startPosition.y - (lowWater_Ypos+yPositionOffset))/(numberOfLevelMarkers-1);
	
			Transform waterGraphicsParent= GameObject.FindGameObjectWithTag("waterGraphicsGroup").transform;
	
			//2.Spawn the indicators.
			//also, Place them at the appropriate locations 
						
			for(int i=0; i<numberOfLevelMarkers; i++)
			{
			
				waterLevelIndicators[i] = Instantiate(Resources.Load("waterLevelMarker")) as GameObject;
				
				//setting the gameobject as child of the water graphics group.
				waterLevelIndicators[i].transform.SetParent(waterGraphicsParent);
				
				
				//Setting the position and scale of our marker.
				
				if(i==0 || i==(numberOfLevelMarkers/2) || i==(numberOfLevelMarkers-1))
				{
					//setting the final position.
					waterLevelIndicators[i].transform.localPosition=
					new Vector3(
									startPosition.x-i*xOffset_LevelMarkers,
									startPosition.y-i*yPositionGap,
									startPosition.z
								);
				}
				
				else
				{
					//make the marker smaller in this case
					waterLevelIndicators[i].transform.localScale=
							new Vector3(
										waterLevelIndicators[i].transform.localScale.x*0.5f,
										waterLevelIndicators[i].transform.localScale.y,
										waterLevelIndicators[i].transform.localScale.z						
										);
					
					//setting the final position.
					waterLevelIndicators[i].transform.localPosition=
						new Vector3(
							startPosition.x-i*(xOffset_LevelMarkers+ 0.15f)-0.5f,
							startPosition.y-i*yPositionGap,
							startPosition.z
							);
					
					
				}
				
			}
	
					
	//------Part 2: placing percentage markers--------//
	
			//0.spawning the level Indicators.
			GameObject[] waterLevelTextLabels= new GameObject[numberOfLevelMarkers];
			
			float levelIndicator_lowPos= 5.36f;
	
			//1.Place the percentage markers.  Creating placement variables.
			yPositionOffset= -3.1f;
			
			startPosition= new Vector3( 42f,
			                            highWater_Ypos+yPositionOffset,
			                            34.81219f
			                           ); 
			
			yPositionGap= 
				(startPosition.y - levelIndicator_lowPos)/(numberOfLevelMarkers-1);
			
			
			RectTransform waterTextParent= 
					GameObject.FindGameObjectWithTag("waterTextGroup").GetComponent<RectTransform>();
		
		
		//Test.
		
		
		//2.Spawn the text. Give it the appropriate color.
		//also, Place them at the appropriate locations 
		
		for(int i=0; i<numberOfLevelMarkers; i++)
		{
			
			
			float waterLevelPercentage= 1-(i*(float)1/(numberOfLevelMarkers-1));
			print ("----Level Percentage-- "+waterLevelPercentage);
			
			//spawn the gameObject.
			waterLevelTextLabels[i] = Instantiate(Resources.Load("waterLevel_markerText")) as GameObject;

			//Set the displayed color.
			waterLevelTextLabels[i].GetComponent<Text>().color=  
									Color.Lerp(new Color32(255,165,0,255), 
			                                   Color.white, 
			                                   waterLevelPercentage);

			//Change the displayed percentage.
			waterLevelTextLabels[i].GetComponent<Text>().text=
				"<i>"+((waterLevelPercentage*10)).ToString("0.0")+"m</i>";

			//setting the gameobject as child of the water graphics group.
			//waterLevelTextLabels[i].transform.SetParent(waterTextParent);
			waterLevelTextLabels[i].GetComponent<RectTransform>().SetParent(waterTextParent);


			
			//setting the final position. the constants are the same as
				//the ones used for the markers.
			waterLevelTextLabels[i].GetComponent<RectTransform>().anchoredPosition=
				new Vector3(
					startPosition.x-i*(xOffset_LevelMarkers-0.15f)+0.55f,
					startPosition.y-i*yPositionGap,
					startPosition.z
					);
								
				
									
			//setting the final scale.
			waterLevelTextLabels[i].GetComponent<RectTransform>().localScale=
				new Vector3(0.1f, 0.1f, 1f);		
					
			
		}
			
			
	
	}
	
//----------Restarting the Display, after the game is ended.

	public void restartDisplay()
	{
	
		//Stop flashing the status display.
			StopCoroutine(flashStatusDisplay());
				
			//set to the original color.
			GameObject dispatchBackground= GameObject.FindGameObjectWithTag("dispatchBckgd");
			dispatchBackground.GetComponent<SpriteRenderer>().color= default_statusColor;				
					
			//also restarting the mapManager.
			mapManager.mapInstance.deleteAllIncidents();
			//deleting static vars.
			mapManager.numberOfIncidents[0]=0;
			mapManager.numberOfIncidents[1]=0;
	
	}	
	
	
//----------Coroutines for Display.

	public void PrepareDisplayForEnd()
		{
		
			//Flash the status display.
			StartCoroutine(flashStatusDisplay());
			

			//Gray out other parts of the display.
				//Darken the map.
				
			
			//Clear incidents, if any.
			gameObject.GetComponent<mapManager>().disableAllIncidents();		
				//The second function argument is not used.
				gameObject.GetComponent<mapManager>().updateMapMessage(mapManager.mapMessage.LEAVE_EMPTY, 
																   mapObject.incident.WATER_THEFT);		
		
	
	
		}
	
	IEnumerator flashStatusDisplay()
		{
			GameObject dispatchBackground= GameObject.FindGameObjectWithTag("dispatchBckgd");
			
			while (true)
			{
				
				dispatchBackground.GetComponent<SpriteRenderer>().color= flash_statusColor;
				
				
				yield return new WaitForSeconds(end_flashDelay);
			
			
				dispatchBackground.GetComponent<SpriteRenderer>().color= default_statusColor;
				
			
				yield return new WaitForSeconds(end_flashDelay);
			
			}
		
		}
	
	
//----------Update methods for the different Display values.
	
	public void repeatIncidentText()
	{
	
		incidentMessages= "<i>There are still <b>unattended incidents</b> to resolve.\n"+
						  " Not resolving these problems will difficult people's lives further.</i>\n";
	
	}
	
	public void generateNewIncidentText()
	{
		incidentMessages="";
		
		
		print ("displayManager.cs, generateNewIncidentText(): generated Incidents are: "+
		       " Leaks "+mapManager.numberOfIncidents[0].ToString()+
		       ", Scarcity "+mapManager.numberOfIncidents[1].ToString()+".");
		
		
		//Water Leak incidents.
		if(mapManager.numberOfIncidents[0]==1)
			incidentMessages += "A <i><b>water leak</b></i> has been reported.";
		else if(mapManager.numberOfIncidents[0]>1)
			incidentMessages += "Several <i><b>water leaks</b></i> have been reported.";
		
		//Water Scarcity incidents.
		if(mapManager.numberOfIncidents[1]==1)
			incidentMessages += "Residents <i>without potable water</i> request assistance.";
		else if(mapManager.numberOfIncidents[0]>1)
			incidentMessages += "Regions <i>without potable water</i> request assistance.";		
		
	}
	
	string getWeekPeriodbyTurn(int turnNumber)
	{
	
		string result="";
		
		//each turn is two weeks long.
		//May-June-July-August.
		
		switch(turnNumber)
		{
			case 0:
				result= "May 3-16";
				break;		
	
			case 1:
				result= "May 17-31";
				break;

			case 2:
				result= "June 1-14";
				break;			
					
			case 3:
				result= "June 15-30";
				break;			
				
			case 4:
				result= "July 1-15";
				break;			
				
			case 5:
				result= "July 16-31";
				break;			
				
			case 6:
				result= "August 1-15";
				break;			
				
			case 7:
				result= "August 16-31";
				break;			
					
			case 8:
				result= "September 1-15";
				break;			
					
			default:
				break;
		
		}
		
		return result;
	
	
	}
	

	public void updateStatusDisplay()
	{
	
		//Updates BOTH the news display and the water status display.
	
		//Status Messages are structured like this:
		
			//1.Turn statement.
			//2.report of repairs from last week.
			//3.parameter check.
			//4.incident report.
		
		
			//1.Turn statement in the News Briefs display.
			
				string currentTurnString="NEWS BRIEFS- <b>" + 
								 getWeekPeriodbyTurn(gameManager.currentTurn)+
								"</b>, ";
				
				if(gameManager.currentTurn== (gameManager.numberOfTurns-1))
				{
				
					currentTurnString +=(gameManager.numberOfTurns-gameManager.currentTurn) + 
						" week left.";
						
				}	
			
				else if(gameManager.currentTurn== gameManager.numberOfTurns)
				{
					
					currentTurnString += "drought ends";
					
				}	
					
							
				else 
				{
					currentTurnString +=(gameManager.numberOfTurns-gameManager.currentTurn) + 
									" weeks left."; 
				}
					
					//Week " + (gameManager.currentTurn * 2 +1).ToString () + 
		 			//" of "+(gameManager.numberOfTurns*2+1)+".</b>";
				
				
				//1b. updating news briefs.
				newsTitleText.text= currentTurnString;
				
				newsReportString = displayAccordingToDistressLevel();
				
				//1c.updating report text on screen.
				newsText.text= newsReportString;
			
			//2.Parameter Check.
				//first, water reservoir level.
				dispatchReportString = displayReservoirLevel();
				//2b.Incident report.
				dispatchReportString += incidentMessages;
								
				//2c.output string to the screen.
				waterStatusText.text= dispatchReportString;
		

	}
	
	string displayReservoirLevel()
	{
	
		string resultString="";
	
		//Water levels remain over observation levels.
		if(gameManager.currentWaterLevel> (gameManager.minWaterLevel+gameManager.maxWaterLevel)/2 )
			resultString= "Reservoirs are at an <b><color=olive>observation</color> level</b>.\n";
						  
		//Water levels are entering adjustment levels.
		else if(gameManager.currentWaterLevel> (gameManager.minWaterLevel+gameManager.maxWaterLevel)/3 )
			resultString= "Reservoir water supply is at a <b><color=yellow >control</color> level</b>.\n";
		
		//Critical level approaching.
		else resultString= "Water supplies are approaching <b><color=red>critical</color> levels</b>. Drastic control measures are advised.\n";
		
		
		return resultString;
			
	}
	
	string displayAccordingToDistressLevel()
	{
		
		string resultString="";
		
		resultString= myMessages.getNewsBrief(gameManager.currentTurn);
		
		//This section is not used right now.
		
//		//Distress is low.
//		if(gameManager.currentDistress<= gameManager.maxDistress/4)
//			{
//				if(gameManager.currentTurn<gameManager.numberOfTurns/2)
//				resultString= messageContainer.statusMessages[messageContainer.statusType.POP_STATE_POSITIVE_1];
//		
//				else
//				resultString= messageContainer.statusMessages[messageContainer.statusType.POP_STATE_POSITIVE_2];
//			
//			}
//		
//		//Distress is medium-low
//		if(gameManager.currentDistress<= gameManager.maxDistress/3)
//		{
//			if(gameManager.currentTurn<gameManager.numberOfTurns/2)
//				resultString= messageContainer.statusMessages[messageContainer.statusType.POP_STATE_POSITIVE_3];
//			
//			else
//				resultString= messageContainer.statusMessages[messageContainer.statusType.POP_STATE_POSITIVE_4];
//			
//		}
//		
//		
//		
//		//Distress is medium.
//		else if(gameManager.currentDistress <=gameManager.maxDistress/2)
//			{
//				if(gameManager.currentTurn<gameManager.numberOfTurns/2)
//					resultString= messageContainer.statusMessages[messageContainer.statusType.POP_STATE_AVERAGE_1];
//				
//				else
//					resultString= messageContainer.statusMessages[messageContainer.statusType.POP_STATE_AVERAGE_2];
//				
//			}	
//				
//		//Distress is high.
//		else if(gameManager.currentDistress >= gameManager.maxDistress-3)
//			{
//				resultString= 
//				"People are highly distressed. Supply them with <b>more water</b> to improve their quality of life.\n";
//			
//				if(gameManager.currentTurn<=gameManager.numberOfTurns/2)
//					resultString += messageContainer.statusMessages[messageContainer.statusType.POP_STATE_TOUGH_1];
//				
//				else
//					resultString += messageContainer.statusMessages[messageContainer.statusType.POP_STATE_TOUGH_2];
//				
//			}	
		
		
		return resultString;
		
	}
	

//--end of display methods for status msg update, now going to more update methods.	
					
										
															
	//updates the Graphics in the Personnel Display, depending on the available personnel.
	public void updatePersonnelDisplay()
	{
		
		GameObject personnelGroupParent= GameObject.FindGameObjectWithTag("personnelGroup");
			
		//Available workers are painted white.
		for(int i=0; i<gameManager.currentPersonnel; i++)
		{
			personnelGroupParent.transform.GetChild(i).GetComponent<Image>().color=
				availableWorkerColorValue;
			
		}	
		
		//If there are any unavailable workers, gray them out.
		if(gameManager.currentPersonnel< gameManager.maxPersonnel)
		{
			
			for(int i=gameManager.currentPersonnel; i<gameManager.maxPersonnel; i++)
			{
				
				personnelGroupParent.transform.GetChild(i).GetComponent<Image>().color=
					unavailableWorkerColorValue;
				
			}		
		
		}					
		
	}
		
	
	public void updateDistressDisplay()
	{
	
		print ("Updating Distress Values, @updateDistressDisplay().");
	
		//1.Updating the text.
		
		string distressStatus="<i>low</i>";
		
		//Arbitrary selection of keyword, based loosely on distress value.
		if(gameManager.currentDistress<4)
			distressStatus="<size=25><b><i><color=darkblue>low</color></i></b></size>";
		else if(gameManager.currentDistress<9)
			distressStatus="<size=25><b><i><color=maroon>avg</color></i></b></size>";
		else
			distressStatus="<size=25><b><i><color=red>high</color></i></b></size>";
		
		
		popDistressText.text = 
			"<i>Population Distress: </i>"+distressStatus;
	
	
		//2.Updating Graphics in the meter.
		GameObject distressMeterParent= GameObject.FindGameObjectWithTag("distressGroup");
		
		//If distress is greater than 0, display those values.		
		if(gameManager.currentDistress>0)
		{
			
			//assigning the color in the child objects.
			for(int i=0; i<gameManager.currentDistress; i++)
			{
				distressMeterParent.transform.GetChild(i).GetComponent<Image>().color=
					distressColorValues[i];
					
								
			}	
		}	
		
		//Assign a default color value to the rest of the meter parts.
		//assigning the color in the child objects.
		for(int i=gameManager.currentDistress; i<distressColorValues.Length; i++)
		{
			distressMeterParent.transform.GetChild(i).GetComponent<Image>().color=
				defaultDistressColorValue;
					
		}	
		
		
	}	
	
	//Updating the turns displayed.
	public void updateTurnDisplay () {
	
		currentTurnText.text = 
			"Week <b>" + (gameManager.currentTurn * 2+1).ToString () + 
			"</b> of " + (gameManager.numberOfTurns * 2+1).ToString () + ".";

	}



	public void updateRationingText()
	{

		string dayString = "days";

		if (gameManager.currentRationing == 1)
			dayString = "day";


		currentRationingText.text = 
			"<i>Restrict</i> water available\nto <i>" + 
			gameManager.currentRationing.ToString () + 
			"</i> "+ dayString + " a week.";
	
	}


	public void updateWaterDisplay()
	{
		//Calculate waterPercentage.
		totalWaterPercentage= (gameManager.currentWaterLevel-minWaterLevel)/
							  (maxWaterLevel-minWaterLevel);
		
		
		
		//Update the water graphic.
		float currentHeight= Mathf.Lerp(lowWater_Ypos, 
		                               highWater_Ypos, totalWaterPercentage);
		
		print ("currentHeight is "+currentHeight);
		
		waterCube.transform.localPosition = 
			new Vector3(waterCube.transform.localPosition.x, 
			            currentHeight, 
			            waterCube.transform.localPosition.z);

		
		//Update the water number graphic.
		waterLevelText.text=gameManager.currentWaterLevel.ToString("0.0")+"m <size=30>remaining</size>";
			//color.
			
			//the water cube changes color now, not the text.
			waterCube.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color=  Color.Lerp(dangerWaterLevelColor, 
										  startWaterLevelColor, 
										  totalWaterPercentage );

	}


//----->Showing ending Message
	public void displayEnding(messageContainer.statusType myEnding)
	{
	
		//brightening up the display. This will be flashing.
		//original b2ab8b.
				
		PrepareDisplayForEnd();
		//previously doing this in current Status Text.
		
		

		
	
		if(myEnding==messageContainer.statusType.SURVIVAL_ENDING)
			{
				//updating news Menu.
				string endOfNewsBrief_Title="NEWS BRIEFS- <b>" + 
					getWeekPeriodbyTurn(gameManager.currentTurn)+
						"</b>.";
				
				//1b. updating news briefs.
				newsTitleText.text= endOfNewsBrief_Title;
			
			
				waterStatusText.text=messageContainer.gameStateMessages[messageContainer.statusType.SURVIVAL_ENDING];
				newsText.text= "Water Rationing in Puerto Rico comes to an end.";	
			}
		
		else if(myEnding==messageContainer.statusType.DRY_ENDING)
			{
				string endOfNewsBrief_Title;
	
				if(gameManager.currentTurn == gameManager.numberOfTurns)
				{
					endOfNewsBrief_Title="NEWS BRIEFS- <b>" + 
					getWeekPeriodbyTurn(gameManager.currentTurn)+
						"</b>.";
			
				}
				
				else
				{
					//updating news Menu.
					 endOfNewsBrief_Title="NEWS BRIEFS- <b>" + 
						getWeekPeriodbyTurn(gameManager.currentTurn)+
							"</b>,";
					
					endOfNewsBrief_Title +=(gameManager.numberOfTurns-gameManager.currentTurn) + 
					" weeks left.";
				}
				//1b. updating news briefs.
				newsTitleText.text= endOfNewsBrief_Title;
			
				waterStatusText.text=messageContainer.gameStateMessages[messageContainer.statusType.DRY_ENDING];
				newsText.text= "Surface water supplies in Puerto Rico beyond repair, Groundwater supplies are vulnerable.";
			
			}
		else if(myEnding==messageContainer.statusType.DISTRESS_ENDING)
			{
				string endOfNewsBrief_Title;
				
				if(gameManager.currentTurn == gameManager.numberOfTurns)
				{
					endOfNewsBrief_Title="NEWS BRIEFS- <b>" + 
						getWeekPeriodbyTurn(gameManager.currentTurn)+
							"</b>.";
					
				}
				
				else
				{
					//updating news Menu.
					endOfNewsBrief_Title="NEWS BRIEFS- <b>" + 
						getWeekPeriodbyTurn(gameManager.currentTurn)+
							"</b>,";
					
					endOfNewsBrief_Title +=(gameManager.numberOfTurns-gameManager.currentTurn) + 
						" weeks left.";
				}
				//1b. updating news briefs.
				newsTitleText.text= endOfNewsBrief_Title;
			
				waterStatusText.text=messageContainer.gameStateMessages[messageContainer.statusType.DISTRESS_ENDING];
				newsText.text= "West Nile Virus (WNV) cases reach ten in the Metropolitan Area.";	
			}
	}

}
