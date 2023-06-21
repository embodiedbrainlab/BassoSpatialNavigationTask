using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

//INFO: If OMT is not storing Rotation and Scale data this script will use the objects exisitng rotation

public class OMTTimeTravel_CS : MonoBehaviour {

	public OMT_CS omtComponent;														//Object Motion Tracker
	public Transform target;														//Object to be moved - Normally the object being tracked by OMT
	
	//Time Travel Setup
	public enum timeTravelTypesAvailable {PointInTime, TimeFromNow};
	public timeTravelTypesAvailable timeTravelType;									//Time travel options
	
	public bool timeTravel;															//Start calculations for time travel
	public bool playSequence;

	//Time Travel details to use
	public float time;																//Time value to be used
	
	//Results
	public int currentWaypointGroup;
	public int CurrentIndex;														//Current index position based on search result
	public float CurrentIndexTime;													//Time value of the current index
	public Vector3 CurrentIndexPosition;											//Position value of the current index
	public Vector3 CurrentIndexScale;												//Scale value of the current index
	public Quaternion CurrentIndexRotation;											//Rotation value of the current index
	
	public float timeTravelStartTime;												//Earliest point in time since level started that waypoint data exists
	public float timeTravelEndTime;													//Most recent point in time since level started that waypoint data exists
	public int timeTravelPositionsTotal;											//Number of positions that exist

	public bool enableScrubBar = true;
	public int waypointGroupToScrub;
	private float scrubBarPosition;
	private int lastWaypoint;

	//Misc Stuff
	public List<TimeTravelClass> timeTravelList = new List<TimeTravelClass>();		//Calculated list result of OMT waypoints
	
	public class TimeTravelClass													//TimeTravelClass is used to create a time structure
	{
		public Vector3 waypointPosition;											//Position of the Waypoint
		public Vector3 waypointScale;												//Position of the Waypoint
		public Quaternion waypointRotation;											//Transform Rotation at waypoint position
		public float waypointTimeStamp;												//Time since start of scene when waypoint was created
	}


	//******************************************************************************************************************

	
	// Update is called once per frame
	void Update () {
		if(timeTravel == true && omtComponent.waypointGroups.Count > 0)
		{
			timeTravel = false;					 //Reset to make sure it runs once		
			StartCoroutine("TimeTravelMachine"); //Start time travel!
		}

		if(enableScrubBar && omtComponent.waypointGroups.Count > 0)
		{

			lastWaypoint = omtComponent.waypointGroups[waypointGroupToScrub].waypointPosition.Count-1;
			//target.position = omtComponent.waypointGroups[waypointGroupToScrub].waypointPosition[scrubBarPosition];
			target.position = omtComponent.waypointGroups[waypointGroupToScrub].waypointPosition[(int)scrubBarPosition];

			if(omtComponent.storeRotations)
			{
				target.rotation = omtComponent.waypointGroups[waypointGroupToScrub].waypointRotation[(int)scrubBarPosition];	
			} else {
				target.rotation = target.rotation;
			}
			
			
			if(omtComponent.storeScale)
			{
				target.localScale = omtComponent.waypointGroups[waypointGroupToScrub].waypointScale[(int)scrubBarPosition];			
			} else {
				target.localScale = target.localScale;										
			}
		}
	}

	//******************************************************************************************************************

	//This is where the type of time travel is decided and executed
	void TimeTravelMachine()
	{
		if(!omtComponent.storeTimeStamp)
		{
			Debug.LogError("WARNING: This script needs Time Tracking to be enabled in the OMT component! Make sure the box is ticked before starting scene");
			return;
		}

		TimeTravelListBuild(); //Build a list based on all available waypoint data
		
		//If travelling back to a defined point in time
		if(timeTravelType == timeTravelTypesAvailable.PointInTime)
		{
			StartCoroutine("FindPointInTime");
			StartCoroutine("DisplayCalculatedIndexPositionInformation");
			target.position = timeTravelList[CurrentIndex].waypointPosition;
			target.rotation = timeTravelList[CurrentIndex].waypointRotation;
			target.localScale = timeTravelList[CurrentIndex].waypointScale;

			if(playSequence)
			{
				StartCoroutine("SequencePlay");
			}
		}
		
		//If travelling back x amount of seconds from now
		if(timeTravelType == timeTravelTypesAvailable.TimeFromNow)
		{
			StartCoroutine("FindTimeFromNow");
			StartCoroutine("DisplayCalculatedIndexPositionInformation");
			target.position = timeTravelList[CurrentIndex].waypointPosition;
			target.rotation = timeTravelList[CurrentIndex].waypointRotation;
			target.localScale = timeTravelList[CurrentIndex].waypointScale;

			if(playSequence)
			{
				StartCoroutine("SequencePlay");
			}
		}


	}

	//******************************************************************************************************************
	
	//Build sorted list of waypoint data
	void TimeTravelListBuild()
	{
		timeTravelList.Clear();																								//Clear list ready for use
		
		if(omtComponent.waypointGroups.Count > 0)
		{
			for (int i = 0; i < omtComponent.waypointGroups.Count; i++)
			{
				currentWaypointGroup = i;

				for(int ii = omtComponent.waypointGroups[i].waypointPosition.Count -1; ii >= 0; ii--) 						//Reverse sort of waypoints in the waypoint group
				{
					
					var createNextTimeTravelEntry = new TimeTravelClass();													//Create New Construct
					createNextTimeTravelEntry.waypointPosition = omtComponent.waypointGroups[i].waypointPosition[ii];		//Add Position data

					if(omtComponent.storeRotations)
					{
						createNextTimeTravelEntry.waypointRotation = omtComponent.waypointGroups[i].waypointRotation[ii];	//Add rotation data
					} else {
						createNextTimeTravelEntry.waypointRotation = target.rotation;
					}


					if(omtComponent.storeScale)
					{
						createNextTimeTravelEntry.waypointScale = omtComponent.waypointGroups[i].waypointScale[ii];			//Add Scale data
					} else {
						createNextTimeTravelEntry.waypointScale = target.localScale;										//Add Scale data	
					}

					createNextTimeTravelEntry.waypointTimeStamp = omtComponent.waypointGroups[i].waypointTimeStamp[ii];		//Add time stamp						
					timeTravelList.Add(createNextTimeTravelEntry);															//Add Entry into timeTravelList						
				}
			}
			
			timeTravelStartTime = timeTravelList.First().waypointTimeStamp;													//Display start time data for the list created
			timeTravelEndTime = timeTravelList.Last().waypointTimeStamp;													//Display end time data for the list created
			timeTravelPositionsTotal = timeTravelList.Count;																//Display total number of positions available
		}
	}

	//******************************************************************************************************************

	//Find the index value of the nearest point in time
	void FindPointInTime()
	{
		int closestIndex = 0;																								//Index number of the point in time found to be closest
		float closestDifference = Mathf.Infinity;																		//The closest time difference between the time value being searched for and the actual value found
		float currentDifference = 0.0f;																						//The current time difference between the time value being searched for and the actual value found
		
		for (int i = 0; i < timeTravelList.Count; i++)
		{
			currentDifference = Mathf.Abs (time - timeTravelList[i].waypointTimeStamp);
			
			//Will use the LAST closest match. So if the difference between 2 times happens to be the same. The last one found is used.
			if (currentDifference <= closestDifference)
			{
				closestIndex = i;
				closestDifference = currentDifference;
			}
		}
		CurrentIndex = closestIndex; //Set index position
	}
	
	//******************************************************************************************************************
	
	//Find the index value of the nearest point in time x amount of seconds from now
	void FindTimeFromNow()
	{
		int closestIndex = 0;																								//Index number of the point in time found to be closest
		float closestDifference = Mathf.Infinity;																		//The closest time difference between the time value being searched for and the actual value found
		float currentDifference = 0.0f;																						//The current time difference between the time value being searched for and the actual value found
		float currentTime = Time.time;
		
		for (int i = 0; i < timeTravelList.Count; i++)
		{
			currentDifference = Mathf.Abs ((currentTime - time) - timeTravelList[i].waypointTimeStamp);
			
			//Will use the LAST closest match. So if the difference between 2 times happens to be the same. The last one found is used.
			if (currentDifference <= closestDifference)
			{
				closestIndex = i;
				closestDifference = currentDifference;
			}
		}
		
		CurrentIndex = closestIndex; //Set index position
	}
	
	//******************************************************************************************************************

	IEnumerator SequencePlay()
	{

		for (int i = currentWaypointGroup; i < omtComponent.waypointGroups.Count; i++)
		{

			//for(int ii = CurrentIndex; ii < omtComponent.waypointGroups[i].waypointPosition.Count; ii++) 					
			//for(int ii = omtComponent.waypointGroups[i].waypointPosition.Count -1; ii >= 0; ii--)
			for(int ii = CurrentIndex; ii >= 0; ii--) 	
			{
				target.position = omtComponent.waypointGroups[i].waypointPosition[ii];
				target.rotation = omtComponent.waypointGroups[i].waypointRotation[ii];
				target.localScale = omtComponent.waypointGroups[i].waypointScale[ii];
				yield return null;
			}
		}
	}

	//******************************************************************************************************************

	//Shows the data for the index found by other functions - useful to check results
	void DisplayCalculatedIndexPositionInformation()
	{
		CurrentIndexTime = timeTravelList[CurrentIndex].waypointTimeStamp;
		CurrentIndexPosition = timeTravelList[CurrentIndex].waypointPosition;
		CurrentIndexScale = timeTravelList[CurrentIndex].waypointScale;
		CurrentIndexRotation = timeTravelList[CurrentIndex].waypointRotation;	
	}

	//******************************************************************************************************************


	void OnGUI() {
		if(enableScrubBar && omtComponent.waypointGroups.Count > 0)
		{
			scrubBarPosition = GUI.HorizontalSlider(new Rect(10, Screen.height-70, Screen.width-20, 50), scrubBarPosition, lastWaypoint, 0);
			scrubBarPosition = (int)scrubBarPosition;
		}
	}
}
