using UnityEngine;
using System.Collections;

//added a UI library
using UnityEngine.UI;


//gameManager.cs keeps track of all game logic.
//It instructs the map and other display variables to update.
//It also receives events from the buttons on the game scene.

public class gameManager : MonoBehaviour {

	public bool startWithoutStartScreen;

	public static gameManager myGameManager;
	public displayManager myDisplay;
	public GameObject nextTurnButton;
	
	//Game variables
		
		public static bool isGameStarted=false;
		public  bool isGameEnded=false;
		
		public static int numberOfTurns=8;
		public static int maxPersonnel=9;
		public static int maxDistress=11;
		int minRationing=7;
		int maxRationing=1;
		bool isRationingButtonPressed=false;
	
		//Water Level variables.
		float midWaterLevel=5f;//33f;
		public static float minWaterLevel=0.0f;//29.2f;
		public static float maxWaterLevel=10.0f;//39.2f;
	
		//Water Consumption rate.
		float waterUseRate= 0.5f;
		float waterUseVariance= 0.3f;

		float leakRate=0.2f;
		float waterDriveRate=0.25f;

		//Infrastructure Event data.
		

		//Game Start Logic.
		public static int currentStartScreen=0;


	//Player attributes
	public static int currentTurn=0;
	public static int currentPersonnel=9;
	public static int currentDistress=1;
	public static int currentRationing=3;
	public static float currentWaterLevel=10.0f;//39.2f;

	//Incident costs.
	public static int leakPersonnelCost=4;
	public static int scarcityPersonnelCost=3;
	public static int theftPersonnelCost=1;
	

	// Use this for initialization
	void Awake () 
	{

		myGameManager = this;
		//initializing myDisplay:
		myDisplay = gameObject.GetComponent<displayManager> ();

	}
	
	void Start()	
	{		
		//update displays with default values.
		myDisplay.updateTurnDisplay ();
		myDisplay.updateRationingText ();

	}


//-------Button events.


	public void gameStart()
	{
		isGameStarted=true;
		print ("Starting game now.");
		
		//Generating an event.
		mapManager.mapInstance.updateIncidents();
		myDisplay.generateNewIncidentText();
		myDisplay.updateStatusDisplay();
	
		myDisplay.nextTurnButton_Text.text= "Go To Next Turn";
		
	}

	void restartGame()
	{

		//reset the game variables.
			isGameStarted=false;
			isGameEnded=false;
			
			currentTurn=0;
			currentStartScreen=0;
			currentPersonnel=8;
			currentDistress=1;
			currentRationing=3;
			currentWaterLevel=10.0f;
	
		//reset the display ( mapManager Incidents reset later).
			myDisplay.restartDisplay();

	}

	public void nextTurnButtonPressed()
	{

		print ("Next Turn Button pressed.");
		
		if(startWithoutStartScreen && !isGameStarted)
		{
		
			isGameStarted=true;
			print ("Starting game now.");
			
			//Generating an event.
			mapManager.mapInstance.updateIncidents();
			myDisplay.generateNewIncidentText();
			myDisplay.updateStatusDisplay();	
			//myDisplay.updateWaterDisplay();
			
			myDisplay.nextTurnButton_Text.text= "Go To Next Turn";
			
		
		}
		
		else if(isGameEnded)
			{
				//Used to restart the static variables.
				restartGame();
				//The quick way to restart.
				Application.LoadLevel(Application.loadedLevel);
//				StartCoroutine(toNextTurnSequence());
			
			}
		else
			StartCoroutine(toNextTurnSequence());


	}


	public void rationingButtonsPressed(bool isRationPeriod_Decreasing)
	{


		if (isRationPeriod_Decreasing) {

			if (currentRationing >= minRationing) {

				//You would show a warning in this case.
				//You're providing water every day of the week, can't go beyond that.

			} else
				currentRationing++;
				
		} 

		else 
		{

			if (currentRationing <= maxRationing) {
				
				//You would show a warning in this case.
				//You're providing water for the minimum days per week (just 1), can't go lower.
				
			} 

			else
				currentRationing--;	
		
		}

		//Toggling the buttonPressed for Distress update.
		isRationingButtonPressed=true;
		

		//Finally, update the display with the information.
		myDisplay.updateRationingText();


	}


//---Game events- these are actually based on button presses also.

	public bool tryToUsePersonnel(int personnelCost)
	{
	
	
		print ("in GameManager: trying to use Personnel..");
	
		//If there's enough personnel, go ahead and deploy them.
		if(currentPersonnel>=personnelCost)
			{
				currentPersonnel -= personnelCost;			
				
				//updating displayed personnel.
				myDisplay.updatePersonnelDisplay();
			
			
				return true;
			}
	
		else 
			return false;
	
	}


//-----Logic for the next turn.

	IEnumerator toNextTurnSequence() {

		//Disable the turn button, while game logic is calculated.
		nextTurnButton.GetComponent<Button> ().interactable = false;
		

		//Update variables here.
			//Updating the water level.
			//calculate and display Distress.
			//update and calculate Personnel.
				updateWaterLevels();
				updateDistress();
				refreshPersonnel();
		
		//increment turn and check for end conditions.

			currentTurn++;
		
			//to show Distress in case of a Distress Ending:
			myDisplay.updateDistressDisplay();
		
		
		//If the game hasn't ended:
			checkForEndingCondition();
			if(!isGameEnded)
			{   
				//generate new incidents and update the map display.
					
						if(mapManager.numberOfIncidents[0]>0 ||
						   mapManager.numberOfIncidents[1]>0 )
						{
							myDisplay.repeatIncidentText();	
						}
					
					
					mapManager.mapInstance.updateIncidents();
			
					
					myDisplay.generateNewIncidentText();
					
					
					
					//generate new Status Text Messages.		
					myDisplay.updateStatusDisplay();
					
				//update the remaining text and graphics displays.	
					myDisplay.updateTurnDisplay();
					myDisplay.updatePersonnelDisplay();
					
					yield return new WaitForSeconds(0.25f);	
			
					nextTurnButton.GetComponent<Button> ().interactable = true;
				
			}
	}

//--------Parameter update methods.

	public void refreshPersonnel()
	{
	
		currentPersonnel=maxPersonnel;
	
	}

	public void effectOfWaterDrive()
	{
		
		currentDistress--;
		
		//Keeping our var within bounds.
		if(currentDistress<1)
			currentDistress=1;
		
		//update the Display.
		myDisplay.updateDistressDisplay();
		
		
		
	}
	
	void updateDistress()
	{
	
		//Before updating the display, we check probabilities etc.
		//This is based on the:
			//rationing, and whether leaks have been fixed or not.
			//and whether water is reaching the people.
	
	
			//if leaks are unresolved, increase distress.
			if(mapManager.numberOfIncidents[0]>1)
				currentDistress++;
				//Keeping our var within bounds.
					if(currentDistress>maxDistress)
						currentDistress=maxDistress;
		
		
		
		
		//If rationing is above average, reduce distress.
			//Only do this for the turn where the button is pressed.
				if(isRationingButtonPressed && currentTurn>0)
					{	
				
						if(currentRationing>5)
							currentDistress -= 2;
			
						else if(currentRationing>3)
							currentDistress -= 1;	
			
						//Keeping our var within bounds.
						if(currentDistress<1)
							currentDistress=1;
						
						
						//Toggle the rationing Button Pressed boolean back off.
						isRationingButtonPressed=false;
						
						
					}
					
			//if rationing is too low, distress increases.			
				if(currentRationing==2)
					currentDistress+= 1;
				
				else if(currentRationing==1)
					currentDistress+= 2;
		
					//Keeping our var within bounds.
					if(currentDistress>maxDistress)
							currentDistress=maxDistress;
	
	

			
			print ("Current Distress is "+currentDistress);
	
	
	}


	void updateWaterLevels()
	{
		//Calculating Water use.

		//Water consumption= 
			//average_use + variance*random(-1,1) +waterLeak_use* numberOfLeaks * (some variance?)
			//+waterDrive_use* numberOfWaterDrives * (some variance?)

			currentWaterLevel -= (waterUseRate*(currentRationing) + waterUseVariance * Random.Range ( 0, 1f));
			currentWaterLevel -= (mapManager.numberOfIncidents[0]*leakRate + mapManager.numberOfIncidents[1]*waterDriveRate);  //adding incident data to water consumption.
		
		
			print ("number of leaks: "+mapManager.numberOfIncidents[0]+
				   ", number of scarcity: "+mapManager.numberOfIncidents[1]);
		
			if(currentWaterLevel<0f)
				currentWaterLevel=0.0f;
		
		//Update water graphics.
			myDisplay.updateWaterDisplay();


	}


//---Checking for the ending condition.

	void checkForEndingCondition()
	{
		
		//Dry ending.
		if(currentWaterLevel<=minWaterLevel)
		{
			isGameEnded=true;
			myDisplay.displayEnding(messageContainer.statusType.DRY_ENDING);		
			myDisplay.updateDistressDisplay();
			
			StartCoroutine(delayForGameRestart());
	
		}

		//Good ending, you've passed the number of turns necessary.
		else if (currentTurn >= numberOfTurns) 
		{
			isGameEnded=true;
			myDisplay.displayEnding(messageContainer.statusType.SURVIVAL_ENDING);	
			myDisplay.updateDistressDisplay();
			
			StartCoroutine(delayForGameRestart());
					
		}
								
												
		//Distress ending.
		else if(currentDistress>=maxDistress)
		{
			isGameEnded=true;
			myDisplay.displayEnding(messageContainer.statusType.DISTRESS_ENDING);	
			myDisplay.updateDistressDisplay();
			
			StartCoroutine(delayForGameRestart());
			
		}	


	}

	IEnumerator delayForGameRestart()
	{

		yield return new WaitForSeconds(1.5f);

		//Re-activate the end turn button.
		nextTurnButton.GetComponent<Button> ().interactable = true;
		

		//Change the button display.
		myDisplay.nextTurnButton_Text.text= "Play Again";	
		

	}
	
	
	
}
