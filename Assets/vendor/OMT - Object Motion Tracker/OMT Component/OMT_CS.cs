using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OMT_CS : MonoBehaviour {

	public bool trackingActive = false;														//Enable / Disable Tracking
	public bool plottingActive = false;														//True if plotting is currently active

	public GameObject trackThis;															//Transform to track
	public enum waypointIntervalTypes {Distance, Seconds, DistanceAndSeconds, Frames};
	public waypointIntervalTypes waypointIntervalModes;										//Waypoint Interval setting
	public float waypointIntervalDistance = 1f;												//How often a waypoint is created over distance
	public float waypointIntervalSeconds = 1f;												//How often a waypoint is created over time
	public int framesInterval = 1;															//How often a waypoint is create over frame count
	private float waypointIntervaltimer;													//Calculated time for next waypoint to be plotted 
	private int nextFrameCountPlotPoint;													//Calculated frame number for next waypoint to be plotted
	public enum waypointPlottingModeTypes {Groups, Continuous};								//Group is triggered by input from keyboard or a timer to log waypoints
	public waypointPlottingModeTypes waypointPlottingModes = waypointPlottingModeTypes.Groups;
	public enum maxNumberOfwaypointsGroupPlottingModeRemovalTypes {None, Active, Extreme}; 	//Active: Only limits the active waypoint group. It is also the default method when using continuous waypoint plotting. Extreme: When using group waypoint plotting, the oldest waypoints from the oldest non-active waypoint group will be removed
	public maxNumberOfwaypointsGroupPlottingModeRemovalTypes maxNumberOfwaypointsGroupPlottingModeRemoval = maxNumberOfwaypointsGroupPlottingModeRemovalTypes.None;
	public int maxNumberOfwaypoints; 														//Set a maxiumum nuber of waypoints at any time
	public enum nonActiveWaypointMergeTypes {Simultaneous, Sequential};						//The order in which the non active waypoint groups are merged
	public nonActiveWaypointMergeTypes nonActiveWaypointMergeModes = nonActiveWaypointMergeTypes.Sequential;

	public bool storeRotations = false;														//Store rotation information when plotting a waypoint
	private bool runtimeStoreRotation;

	public bool storeScale = false;															//Store scale information when plotting a waypoint
	private bool runtimeStoreScale;

	public bool storeTimeStamp = false;														//Store time information when plotting a waypoint
	private bool runtimeStoreTimeStamp;

	public bool mergeActiveWaypointGroup = false;											//Do we want to merge the oldest waypoints in the active waypoint group?
	public bool mergeNonActiveWaypointGroups = false;										//Do we want to merge the oldest waypoints in the non-active waypoint group(s)?
	public bool adjustMergingSpeedWithMergeRatePercentage = false;							//Scale speed of merging based on the percentage speed given by the speed detection script
	public float activeMergeRatePercentage;
	public bool absoluteWaypoint = false;													//Creates an additional Waypoint at the start of the waypoint group (index [0])
	public Vector3 absoluteWayPointOffset;													//Amount of offset to apply to the absolute waypoint
	public Vector3 absoluteWaypointResult;													//Calculated result
	public bool absoluteWayPointCreated = false;											//Has an absolute waypoint been created?

	public enum offsetWaypointPlottingMethodSelection {Manual, Percentage, Actual};
	public offsetWaypointPlottingMethodSelection offsetWaypointPlottingMethod;

	public Vector3 offsetManualAmount = new Vector3(0,0,0);									//Offset ALL Results - Set if not using speed offset - This value on the Z axis will be changed if speed offset is set
	public Vector3 offsetMinPosition;
	public Vector3 offsetMaxPosition;
	public Vector3 offsetResult;															//Calculated offset to use
	public float offsetPercentage;
	public Vector3 offsetAdjustmentToApply;

	public GameObject offsetWithThisObjectPosition;

	public bool showGizmos = true;															//Show Gizmos debug in editor

	public float activeWaypointGroupMergeSpeed = 0.5f;										//For Active WayPoint Group - How quickly the last waypoint merges with the next waypoint - 0 will result in no merging
	public float nonActiveWaypointGroupMergeSpeed = 0.5f;									//For previous way point groups - How quickly the last waypoint merges with the next waypoint - 0 will result in no merging
	public float actualActiveMergeSpeed;													//How quickly the active waypoint group merge is actually merg
	public float actualNonActiveMergeSpeed;		 											//How quickly non-active waypoint groups are actually merging
	public int waypointCountActiveTotal;													//Count of all active group waypoints
	public int waypointCountCurrentTotal;													//Count of all current waypoints
	public int waypointTotalSinceStart;														//Total number of waypoints generated since start
	private Vector3 currentPosition;														//Calculation for movement detection
	private Vector3 previousPosition;														//Calculation for movement detection
	private float distanceCalculation;														//Current Distance
	private int penultimateWaypointInList;													//Calculations for waypoint merging							
	private int lastWaypoint;																//Calculations for waypoint merging
	private bool creatingWaypoints = false;													//True if actively creating waypoints
	
	[Serializable]
	public class WayPointsClass																//Waypoints class is used to populate the waypointGroups list
	{
		public string id;																	//Unique Name description for each group
		public bool activeWaypointGroup;													//Is this group currently having waypoints added to it
		public int waypointTotal;															//Artifical Count - This total MUST match the actual number of waypoints in the group - It's purpose is to allow me to check if any waypoints have been added ass trying to get a waypoint.count errors with null object.
		public List<Vector3> waypointPosition = new List<Vector3>();						//Transform Position of the Waypoint
		public List<Quaternion> waypointRotation = new List<Quaternion>();					//Transform Rotation at waypoint position
		public List<Vector3> waypointScale = new List<Vector3>();            				//Transform Scale at waypoint position
		public List<float> waypointTimeStamp = new List<float>();							//Time since start of scene when waypoint was created
	}
	private List<Vector3> newWaypointPosition = new List<Vector3>();						//Create New Constructor for new Waypoints (new is important!)
	private List<Quaternion> newWaypointRotation = new List<Quaternion>();					//Create New Constructor for new Waypoints (new is important!)
	private List<Vector3> newWaypointScale = new List<Vector3>();							//Create New Constructor for new Waypoints (new is important!)
	private List<float> newWaypointTime = new List<float>();								//Create New Constructor for new Waypoints (new is important!)

	public List<WayPointsClass> waypointGroups = new List<WayPointsClass>();				//The LIST of waypoints created
	public bool showDiagram = false;														//Show Diagram Toggle switch
	public bool showDebugInfo = false;														//Debug info toggle switch
	public bool showNotes = false;															//Notes info toggle switch
	public string warnings = "";															//This string is used to display warnings
	public string notes = "Use this text box to make notes about you settings.";

	//----------------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------

	void Start ()
	{
		runtimeStoreTimeStamp = storeTimeStamp;
		runtimeStoreScale = storeScale;
		runtimeStoreRotation = storeRotations;
		
		
		warnings = ""; // Reset Warnings
		
		//Auto assign the script to use the transform it is attached to
		if(trackThis == null)
		{
			trackThis = gameObject;
		}
		
		StartCoroutine("TrackingStart");
		StartCoroutine("nonActiveWaypointGroupMerge");
		StartCoroutine("ActiveWaypointGroupMerge");
		StartCoroutine("FrameCountCheck");
		StartCoroutine("MovementCheck");
		StartCoroutine("TimeCheck");
		StartCoroutine("MiscCalculations");
		StartCoroutine("AbsoluteWaypointPositionUpdate");
		StartCoroutine("MaxWaypointRemoveDelegate");
		
		
	}
	
	//----------------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------
	
	//Listener for other script to trigger waypoint plotting
	void ObjectMotionTrackTrigger(bool startMotionTracking)
	{
		trackingActive = startMotionTracking;
	}

    void RemoveAllWaypoints()
    {
        StartCoroutine( RemoveWaypointGroup( -1 ) );
    }
	
	//----------------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------

	//Function to remove groups of waypoints - Will NOT remove an Active Waypoint - Only works on Non-Active Waypoint Groups
	IEnumerator RemoveWaypointGroup(int waypointGroupToRemove)
	{
		//If -1 then remove all non-active waypoints
		if(waypointGroupToRemove == -1)
		{
			//If no tracking remove all waypoint groups
			if(trackingActive == false)
			{
				waypointGroups.Clear();
				yield return StartCoroutine("GetWaypointCountCurrentTotal"); //Update waypointCountCurrentTotal
				
			} else { //If tracking is active then remove all but last waypoint groups
				
				waypointGroups.RemoveRange(0,waypointGroups.Count-1);
				yield return StartCoroutine("GetWaypointCountCurrentTotal"); //Update waypointCountCurrentTotal
			}
		}
		
		//If a specific waypoint group is being targeted
		if(waypointGroupToRemove >= 0)
		{
			// Check that it is not an active waypoint group
			if(waypointGroups[waypointGroupToRemove].activeWaypointGroup == false)
			{
				waypointGroups.RemoveAt(waypointGroupToRemove);
				yield return StartCoroutine("GetWaypointCountCurrentTotal"); //Update waypointCountCurrentTotal
			}
		}
	}
	
	//----------------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------

	//Keeps checking tracking status - Creates new tracking setup and clears up old tracking setup
	IEnumerator TrackingStart()
	{
		while(true)
		{
			//Check if any of the forward commands are being given
			if(trackingActive == true)
			{
				
				//GROUP WAYPOINT SETUP
				if(waypointPlottingModes == waypointPlottingModeTypes.Groups)
				{
					//First check if logging of waypoints had stopped - If so setup next list
					if(creatingWaypoints == false)
					{
						creatingWaypoints = true; //Change switch so that new lists are not created
						
						//Create New Constructor for adding new Waypoint details
						newWaypointPosition = new List<Vector3>();
						newWaypointRotation = new List<Quaternion>();
						newWaypointScale = new List<Vector3>();
						newWaypointTime = new List<float>(); 
						
						if(waypointGroups.Count > 0)
						{
							var createNextWaypointList = new WayPointsClass();						//Create New Construct
							createNextWaypointList.activeWaypointGroup = true;						//Mark waypoint group as active
							createNextWaypointList.id = "Waypoint Group " + (waypointGroups.Count); //Set ID for new construct
							createNextWaypointList.waypointTotal = 0;								//Create artifical count
							waypointGroups.Add(createNextWaypointList);								//Create the new waypoint list
							
						} else {
							
							var createNewWaypointList = new WayPointsClass();						//Create New Construct
							createNewWaypointList.activeWaypointGroup = true;						//Mark waypoint group as active
							createNewWaypointList.id = "Waypoint Group 0";							//Set ID for new construct
							createNewWaypointList.waypointTotal = 0;								//Create artifical count
							waypointGroups.Add(createNewWaypointList);								//Create the new waypoint list
						}
					}
				}
				
				//CONINUOUS WAYPOINT SETUP
				//Check if not already started logging waypoint
				//If logging has not started preare Waypoint Group List
				if(waypointPlottingModes == waypointPlottingModeTypes.Continuous && creatingWaypoints == false)
				{
					creatingWaypoints = true;										//Change switch so that new lists are not created
					
					//Create New Constructor for adding new Waypoints
					newWaypointPosition = new List<Vector3>();
					newWaypointRotation = new List<Quaternion>();
					newWaypointScale = new List<Vector3>();
					newWaypointTime = new List<float>(); 
					
					if(waypointGroups.Count == 0)
					{
						var createSingleWaypointGroup = new WayPointsClass();		//Create New Construct
						createSingleWaypointGroup.activeWaypointGroup = true;		//Mark waypoint group as active
						createSingleWaypointGroup.id = "Continuous Waypoint Group"; //Set ID for new construct
						createSingleWaypointGroup.waypointTotal = 0;				//Create artifical count
						waypointGroups.Add(createSingleWaypointGroup);				//Create the new waypoint list
					}
				}
				
				plottingActive = true; //Movement check will start with this switch
				
			} else {
				
				//If no forward command then start to tidy up if previously plotting waypoints
				if(waypointPlottingModes == waypointPlottingModeTypes.Groups && creatingWaypoints == true)
				{	
					creatingWaypoints = false;											//Stop this if statement from running again
					yield return StartCoroutine("RemoveAbsoluteWaypoint");							//If using absoluteWaypoint then this will remove the additonal waypoint ready for it to be an old waypoint group
					waypointGroups[waypointGroups.Count-1].activeWaypointGroup = false; //Set current list as not active
				}
				plottingActive = false;													//Movement check will stop with this switch
			}	
		yield return null; 
		}
	}
	
	//----------------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------
	
	//Monitor Movement of the transform that we are plotting waypoints for
	IEnumerator FrameCountCheck()
	{
		while(true)
		{
			if(plottingActive == true && waypointIntervalModes == waypointIntervalTypes.Frames)
			{
				if(Time.frameCount >= nextFrameCountPlotPoint)
				{
					nextFrameCountPlotPoint = Time.frameCount + framesInterval; //Re-calculate next time to use for waypoint plotting
					yield return StartCoroutine("PlotWayPoint"); //Add Waypoint Function
				} else {
					yield return null;
				}
			} else {
				yield return null;
			}	
			
		}
	}

	//----------------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------

	//Monitor Movement of the transform that we are plotting waypoints for
	IEnumerator MovementCheck()
	{
		waypointIntervaltimer = Time.time + waypointIntervalSeconds; //Calculate time for next waypoint plotting
		while(true)
		{
			if(plottingActive == true && (waypointIntervalModes == waypointIntervalTypes.Distance || waypointIntervalModes == waypointIntervalTypes.DistanceAndSeconds))
			{
				currentPosition = trackThis.transform.position; //Get current position
				distanceCalculation = Vector3.Distance(previousPosition, currentPosition);//Calculate distance since last frame
				
				//If movement greater than the waypoint interval distance the plot a new waypoint
				if(distanceCalculation >= waypointIntervalDistance)
				{
					previousPosition = currentPosition; //Get last position ready to compare next time round				
					yield return StartCoroutine("PlotWayPoint"); //Add Waypoint Function
				} else {
					yield return null;
				}
			} else {
				yield return null;
			}	
			
		}
	}
	
	//----------------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------

	//Monitor Movement of the transform that we are plotting waypoints for
	IEnumerator TimeCheck()
	{
		while(true)
		{
			if(plottingActive == true && (waypointIntervalModes == waypointIntervalTypes.Seconds || waypointIntervalModes == waypointIntervalTypes.DistanceAndSeconds))
			{
				if(Time.time >= waypointIntervaltimer)
				{
					waypointIntervaltimer = Time.time + waypointIntervalSeconds; //Re-calculate next time to use for waypoint plotting
					yield return StartCoroutine("PlotWayPoint"); //Add Waypoint Function
				} else {
					yield return null;
				}
			} else {
				yield return null;
			}	
			
		}
	}
	
	//----------------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------
	
	//Function to add a waypoint - Always uses the last List as this will always be the active waypoint list
	//Remember to update the artifical count!
	void PlotWayPoint()
	{
		
		//Waypoint Position Calculations------------------------------------------------------------------------------
		waypointGroups[waypointGroups.Count-1].activeWaypointGroup = true;
		
		if(offsetWaypointPlottingMethod == offsetWaypointPlottingMethodSelection.Manual)
		{
			
			offsetResult = trackThis.transform.TransformPoint(offsetManualAmount); //Apply offset to waypoint being created
			newWaypointPosition.Insert(0, offsetResult);
			
			if(runtimeStoreRotation == true)
			{		
				newWaypointRotation.Insert(0, trackThis.transform.rotation);  //Prepare waypoint entry for list
			}

			if(runtimeStoreScale == true)
			{		
				newWaypointScale.Insert(0, trackThis.transform.localScale);  //Prepare waypoint entry for list
			}
			
			if(runtimeStoreTimeStamp == true)
			{
				newWaypointTime.Insert(0, Time.realtimeSinceStartup);  //Prepare waypoint entry for list
			}
		}
		
		//Calculate Offset Using Percentage
		if(offsetWaypointPlottingMethod == offsetWaypointPlottingMethodSelection.Percentage)
		{
			
			//Use lerp to calculate the position. The percentage value is divided by 100 so that it works between 1 and 0 for lerp
			offsetAdjustmentToApply = Vector3.Lerp(offsetMinPosition,offsetMaxPosition,(offsetPercentage/100));
			offsetResult = trackThis.transform.TransformPoint(offsetAdjustmentToApply);
			
			newWaypointPosition.Insert(0, offsetResult);  //Prepare waypoint entry for list
			
			if(runtimeStoreRotation == true)
			{
				newWaypointRotation.Insert(0, trackThis.transform.rotation);  //Prepare waypoint entry for list
			}

			if(runtimeStoreScale == true)
			{		
				newWaypointScale.Insert(0, trackThis.transform.localScale);  //Prepare waypoint entry for list
			}
			
			if(runtimeStoreTimeStamp == true)
			{
				newWaypointTime.Insert(0, Time.realtimeSinceStartup);  //Prepare waypoint entry for list
			}
			
			//Offset using an objects position	
		}	
		
		if(offsetWaypointPlottingMethod == offsetWaypointPlottingMethodSelection.Actual)
		{
			offsetResult = offsetWithThisObjectPosition.transform.position;
			newWaypointPosition.Insert(0, offsetResult);  //Prepare waypoint entry for list
			
			if(runtimeStoreRotation == true)
			{
				newWaypointRotation.Insert(0, trackThis.transform.rotation);  //Prepare waypoint entry for list
			}

			if(runtimeStoreScale == true)
			{		
				newWaypointScale.Insert(0, trackThis.transform.localScale);  //Prepare waypoint entry for list
			}
			
			if(runtimeStoreTimeStamp == true)
			{
				newWaypointTime.Insert(0, Time.realtimeSinceStartup);  //Prepare waypoint entry for list
			}
		}	
		
		
		//Plot normal waypoint
		if(absoluteWaypoint == false)
		{
			waypointGroups[waypointGroups.Count-1].waypointPosition = newWaypointPosition; //Add waypoint entry
			waypointGroups[waypointGroups.Count-1].waypointTotal = waypointGroups[waypointGroups.Count-1].waypointPosition.Count;
			
			if(runtimeStoreRotation == true)
			{
				waypointGroups[waypointGroups.Count-1].waypointRotation = newWaypointRotation; //Add waypoint entry
			}

			if(runtimeStoreScale == true)
			{		
				newWaypointScale.Insert(0, trackThis.transform.localScale);  //Prepare waypoint entry for list
			}
			
			if(runtimeStoreTimeStamp == true)
			{
				waypointGroups[waypointGroups.Count-1].waypointTimeStamp = newWaypointTime; //Add waypoint entry
			}
			
			waypointTotalSinceStart += 1; //Add to toal count	
		}
		
		
		//Add To List ------------------------------------------------------------------------------------------------				
		//Create absolute WayPoint + offset - then swap or add accordingly
		if(absoluteWaypoint == true)
		{
			//Use null to check if there are any waypoints - null because Count cannot work as there is nothing there to target then if there are some waypoints check if more than 2
			if(waypointGroups[waypointGroups.Count-1].waypointPosition != null && waypointGroups[waypointGroups.Count-1].waypointPosition.Count >2)
			{
				//update what will be the PREVIOUS waypoint with current info
				//Last waygroup is active waygroup . First entry in this case is the absolute waypoint which now becomes the offset waypoint
				
				//Remove Old absolute			
				waypointGroups[waypointGroups.Count-1].waypointPosition.RemoveAt(0);
				waypointGroups[waypointGroups.Count-1].waypointTotal = waypointGroups[waypointGroups.Count-1].waypointPosition.Count;
				
				if(runtimeStoreRotation == true)
				{
					waypointGroups[waypointGroups.Count-1].waypointRotation.RemoveAt(0);
				}

				if(runtimeStoreScale == true)
				{
					waypointGroups[waypointGroups.Count-1].waypointScale.RemoveAt(0);
				}
				
				if(runtimeStoreTimeStamp == true)
				{
					waypointGroups[waypointGroups.Count-1].waypointTimeStamp.RemoveAt(0);
				}			
				
				
				//Insert new offset waypoint		
				waypointGroups[waypointGroups.Count-1].waypointPosition[0] = offsetResult; //Replace the previous Extra Way point with the new offset
				waypointGroups[waypointGroups.Count-1].waypointTotal = waypointGroups[waypointGroups.Count-1].waypointPosition.Count;
				
				if(runtimeStoreRotation == true)
				{
					waypointGroups[waypointGroups.Count-1].waypointRotation[0] = trackThis.transform.rotation;
				}

				if(runtimeStoreScale == true)
				{
					waypointGroups[waypointGroups.Count-1].waypointScale[0] = trackThis.transform.localScale;
				}
				
				if(runtimeStoreTimeStamp == true)
				{
					waypointGroups[waypointGroups.Count-1].waypointTimeStamp[0] = Time.realtimeSinceStartup;
				}
				
				waypointGroups[waypointGroups.Count-1].waypointPosition.Insert(0, absoluteWaypointResult); //Create the NEW absolute waypoint at START
				waypointGroups[waypointGroups.Count-1].waypointTotal = waypointGroups[waypointGroups.Count-1].waypointPosition.Count;
				
				if(runtimeStoreRotation == true)
				{			
					//waypointGroups[waypointGroups.Count-1].waypointRotation.Add(trackThis.transform.rotation); //Create the NEW absolute waypoint on the end
					waypointGroups[waypointGroups.Count-1].waypointRotation.Insert(0, trackThis.transform.rotation); //Create the NEW absolute waypoint at START
				}

				if(runtimeStoreScale == true)
				{			
					//waypointGroups[waypointGroups.Count-1].waypointScale.Add(trackThis.transform.localScale); //Create the NEW absolute waypoint on the end
					waypointGroups[waypointGroups.Count-1].waypointScale.Insert(0, trackThis.transform.localScale); //Create the NEW absolute waypoint at START
				}
				
				if(runtimeStoreTimeStamp == true)
				{
					//waypointGroups[waypointGroups.Count-1].waypointTimeStamp.Add(Time.realtimeSinceStartup); //Create the NEW absolute waypoint on the end
					waypointGroups[waypointGroups.Count-1].waypointTimeStamp.Insert(0, Time.realtimeSinceStartup); //Create the NEW absolute waypoint at START				
				}
				
				waypointTotalSinceStart += 2; //Add to toal count
				absoluteWayPointCreated = true;
				
				//If array does not have enough waypoints 
			} else { 
				
				waypointGroups[waypointGroups.Count-1].waypointPosition = newWaypointPosition; //Prepare waypoint entry for list
				waypointGroups[waypointGroups.Count-1].waypointTotal = waypointGroups[waypointGroups.Count-1].waypointPosition.Count;
				
				if(runtimeStoreRotation == true)
				{
					waypointGroups[waypointGroups.Count-1].waypointRotation = newWaypointRotation; //Prepare waypoint entry for list
				}

				if(runtimeStoreScale == true)
				{
					waypointGroups[waypointGroups.Count-1].waypointScale = newWaypointScale; //Prepare waypoint entry for list
				}
				
				if(runtimeStoreTimeStamp == true)
				{
					waypointGroups[waypointGroups.Count-1].waypointTimeStamp = newWaypointTime; //Prepare waypoint entry for list
				}
				
				waypointGroups[waypointGroups.Count-1].waypointPosition.Insert(0, absoluteWaypointResult); //Add waypoint entry into list
				waypointGroups[waypointGroups.Count-1].waypointTotal = waypointGroups[waypointGroups.Count-1].waypointPosition.Count;
				
				if(runtimeStoreRotation == true)
				{			
					waypointGroups[waypointGroups.Count-1].waypointRotation.Insert(0, trackThis.transform.rotation); //Create the NEW absolute waypoint on the end
				}

				if(runtimeStoreScale == true)
				{			
					waypointGroups[waypointGroups.Count-1].waypointScale.Insert(0, trackThis.transform.localScale); //Create the NEW absolute waypoint at end
				}
				
				if(runtimeStoreTimeStamp == true)
				{
					waypointGroups[waypointGroups.Count-1].waypointTimeStamp.Insert(0, Time.realtimeSinceStartup); //Create the NEW absolute waypoint on the end
				}
				
				waypointTotalSinceStart += 2; //Add to toal count
				absoluteWayPointCreated = true;
			}
		}
	}
	
	//----------------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------
	
	//This function will remove the absolute waypoint when the active waypoint group is converted to an old waypoint group
	void RemoveAbsoluteWaypoint()
	{
		if(absoluteWayPointCreated == true && waypointGroups[waypointGroups.Count-1].waypointPosition.Count > 1)
		{
			waypointGroups[waypointGroups.Count-1].waypointPosition.RemoveAt(0);
			waypointGroups[waypointGroups.Count-1].waypointTotal = waypointGroups[waypointGroups.Count-1].waypointPosition.Count;
			
			if(runtimeStoreRotation == true)
			{
				waypointGroups[waypointGroups.Count-1].waypointRotation.RemoveAt(0);
			}

			if(runtimeStoreScale == true)
			{
				waypointGroups[waypointGroups.Count-1].waypointScale.RemoveAt(0);
			}	
			
			if(runtimeStoreTimeStamp == true)
			{
				waypointGroups[waypointGroups.Count-1].waypointTimeStamp.RemoveAt(0);
			}		
			
			absoluteWayPointCreated = false;
		}
	}
	
	//----------------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------
	
	//Decides which functions to use for waypoints exceeding the max waypoint count 
	IEnumerator MaxWaypointRemoveDelegate()
	{
		while(true)
		{
			//First check to make sure that waypoint removal is enabled
			if(maxNumberOfwaypointsGroupPlottingModeRemoval != maxNumberOfwaypointsGroupPlottingModeRemovalTypes.None)
			{
				//Active waypoint group
				if(maxNumberOfwaypointsGroupPlottingModeRemoval == maxNumberOfwaypointsGroupPlottingModeRemovalTypes.Active)
				{
					ActiveMaxWaypointRemove();
				}
				
				//Non active waypoint group(s)
				//if(maxNumberOfwaypointsGroupPlottingModeRemoval == maxNumberOfwaypointsGroupPlottingModeRemovalTypes.Extreme && waypointPlottingModes != waypointPlottingModeTypes.Continuous)
				if(maxNumberOfwaypointsGroupPlottingModeRemoval == maxNumberOfwaypointsGroupPlottingModeRemovalTypes.Extreme)//If there are waypoints left in the groups)
				{
					
					//Check to see if there is exactly 1 waypoint group remaining
					if(waypointGroups.Count == 1)
					{
						//Is it an active waypoint group?
						if(waypointGroups[0].activeWaypointGroup == true)
						{
							ActiveMaxWaypointRemove();
							
							//If it is a non-active waypoint group
						} else {
							NonActiveMaxWaypointRemove();
						}
					}
					
					//If there are waypoints left in the non active waypoint group
					if(waypointGroups.Count > 1)
					{
						NonActiveMaxWaypointRemove();
					}
				}
			}
			yield return null; 
		}
	}
	
	//\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
	//Active Waypoint Group - Removal of waypoints exceeding the max waypoint count - Triggered by MaxWaypointRemoveDelegate()
	void ActiveMaxWaypointRemove()
	{
		//Active waygroup last waypoint removal
		for (int i = 0; i < waypointGroups.Count; i++)
		{
			//If active waypoint group
			if(waypointGroups[i].activeWaypointGroup == true && waypointGroups[i].waypointTotal >= maxNumberOfwaypoints)
			{
				while(waypointGroups[i].waypointTotal >= maxNumberOfwaypoints)
				{
					waypointGroups[i].waypointPosition.RemoveAt(waypointGroups[i].waypointPosition.Count -1);
					
					if(runtimeStoreRotation == true)
					{
						waypointGroups[i].waypointRotation.RemoveAt(waypointGroups[i].waypointPosition.Count -1);
					}

					if(runtimeStoreScale == true)
					{
						waypointGroups[i].waypointScale.RemoveAt(waypointGroups[i].waypointScale.Count -1);
					}	
					
					if(runtimeStoreTimeStamp == true)
					{
						waypointGroups[i].waypointTimeStamp.RemoveAt(waypointGroups[i].waypointPosition.Count -1);
					}
					
					waypointGroups[i].waypointTotal = waypointGroups[i].waypointPosition.Count;
				}
			}
		}
	}
	
	//\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
	//Non-Active Waypoint Group - Removal of waypoints exceeding the max waypoint count - Triggered by MaxWaypointRemoveDelegate()
	//The oldest waypoint group is always at index [0] - The oldest waypoint is always at the last index.
	IEnumerator NonActiveMaxWaypointRemove()
	{
		//First Check if waypoint group removal will still mean that there are too many waypoints - if so remove it
		while(waypointGroups[0].activeWaypointGroup == false && waypointCountCurrentTotal - waypointGroups[0].waypointTotal >= maxNumberOfwaypoints)
		{
			waypointGroups.RemoveAt(0); //Remove Group
			yield return StartCoroutine("GetWaypointCountCurrentTotal"); //Update waypointCountCurrentTotal
			
		}
		
		//Remove excess waypoints from the waypoint group that contains the threshold for max waypoints
		while(waypointGroups[0].activeWaypointGroup == false && waypointCountCurrentTotal - waypointGroups[0].waypointTotal <= maxNumberOfwaypoints)
		{
			//Calculate the amount of waypoints that need removing
			var AmountToRemoveFromThisArray = waypointCountCurrentTotal - maxNumberOfwaypoints;
			
			//Remove Range of Waypoints from the end
			//print(waypointCountCurrentTotal + " | " + waypointGroups[0].waypointTotal + " (" + (waypointCountCurrentTotal - waypointGroups[0].waypointTotal) + ")");
			//print (AmountToRemoveFromThisArray + " | " + (waypointGroups[0].waypointPosition.Count -1));
			//Make sure the amount to remove is great than 0!
			if(AmountToRemoveFromThisArray > 0 )
			{
				waypointGroups[0].waypointPosition.RemoveRange(waypointGroups[0].waypointTotal - AmountToRemoveFromThisArray, AmountToRemoveFromThisArray);
				
				if(runtimeStoreRotation == true)
				{
					waypointGroups[0].waypointRotation.RemoveRange((waypointGroups[0].waypointTotal) - AmountToRemoveFromThisArray, AmountToRemoveFromThisArray);
				}

				if(runtimeStoreScale == true)
				{
					waypointGroups[0].waypointScale.RemoveRange((waypointGroups[0].waypointTotal) - AmountToRemoveFromThisArray, AmountToRemoveFromThisArray);
				}	
				
				if(runtimeStoreTimeStamp == true)
				{
					waypointGroups[0].waypointTimeStamp.RemoveRange((waypointGroups[0].waypointTotal) - AmountToRemoveFromThisArray, AmountToRemoveFromThisArray);
				}					
			}
			waypointGroups[0].waypointTotal = waypointGroups[0].waypointPosition.Count; //Get current count of waypoints in the group
			yield return StartCoroutine("GetWaypointCountCurrentTotal"); //Update waypointCountCurrentTotal
		}
	}
	
	//----------------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------
	
	//Makes the oldest waypoint move towards the second last waypoint - When they meet the oldest waypoint is removed - Keeps repeating until there are no waypoints left
	IEnumerator ActiveWaypointGroupMerge()
	{
		while(true)
		{
			//Check if there is a waypoint list
			if(mergeActiveWaypointGroup == true && waypointGroups.Count > 0)
			{
				for (int i = 0; i < waypointGroups.Count; i++)
				{
					//When Active Trail is found and it has more than 2 waypoints in it
					if(waypointGroups[i].activeWaypointGroup == true && waypointGroups[i].waypointTotal >=2)
					{
						//Converging Calculations-------------------------------------------------------------------------------------------------------------------------------
						waypointCountActiveTotal = waypointGroups[i].waypointPosition.Count; //Get number of waypoints
						
						if(adjustMergingSpeedWithMergeRatePercentage == true)
						{
							actualActiveMergeSpeed = ((activeWaypointGroupMergeSpeed * waypointCountActiveTotal) / 100) * activeMergeRatePercentage * Time.deltaTime; //
							
						} else {
							
							//actualActiveMergeSpeed = activeWaypointGroupMergeSpeed;
							actualActiveMergeSpeed = activeWaypointGroupMergeSpeed * waypointCountActiveTotal * Time.deltaTime;
						}
						
						//Apply Converging-------------------------------------------------------------------------------------------------------------------------------
						if(waypointGroups[i].waypointPosition.Count >= 2) //If more than 2 sets of waypoints to avoid out of index errors
						{
							var newPositionForLastWaypoint = Vector3.MoveTowards(waypointGroups[i].waypointPosition[waypointCountActiveTotal - 1], waypointGroups[i].waypointPosition[waypointCountActiveTotal - 2], actualActiveMergeSpeed); //Calculate new position for last waypoint - Using MoveTowards so the value does not overshoot.
							
							//If new position will be the same as the next waypoint check if it would be equal to the waypoint after that and keep repeating until a waypoint is found to be far enough away
							if(newPositionForLastWaypoint == waypointGroups[i].waypointPosition[waypointCountActiveTotal-2]) 
							{
								//Remove waypoint as merge speed has already passed this point
								waypointGroups[i].waypointPosition.RemoveAt(waypointCountActiveTotal-1); 
								waypointGroups[i].waypointTotal = waypointGroups[i].waypointPosition.Count;
								
								if(runtimeStoreRotation == true)
								{
									waypointGroups[i].waypointRotation.RemoveAt(waypointCountActiveTotal-1);
								}

								if(runtimeStoreScale == true)
								{
									waypointGroups[i].waypointScale.RemoveAt(waypointCountActiveTotal-1);
								}
								
								if(runtimeStoreTimeStamp == true)
								{
									waypointGroups[i].waypointTimeStamp.RemoveAt(waypointCountActiveTotal-1);
								}
								
							} else {
								
								waypointGroups[i].waypointPosition[waypointCountActiveTotal -1] = newPositionForLastWaypoint; //Update last waypoint position in LIST
							}
						}
					} else {
						waypointCountActiveTotal = 0; //Set to zero as none found.
					}
				}
			}
			yield return null; 
		}
	}
	
	//----------------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------
	
	//Sequential Merge function for non active waypoint groups
	IEnumerator nonActiveWaypointGroupMerge()
	{
		while(true)
		{
			if(waypointPlottingModes == waypointPlottingModeTypes.Groups && mergeNonActiveWaypointGroups == true)
			{
				//If there are waypoint lists.....
				for (int i = 0; i < waypointGroups.Count; i++)
				{
					//Check that current list is not an active waypoint trail
					if(waypointGroups[i].activeWaypointGroup == false)
					{
						//Use artificial waypoint total to see if there are any waypoints
						if(waypointGroups[i].waypointTotal >= 1)
						{
							//actualNonActiveMergeSpeed = activeWaypointGroupMergeSpeed; //Merge at constant speed
							actualNonActiveMergeSpeed = nonActiveWaypointGroupMergeSpeed * waypointGroups[i].waypointPosition.Count * Time.deltaTime; //Merging slows down as the number of waypoints decrease			
							
							//Apply Converging-------------------------------------------------------------------------------------------------------------------------------
							//If more than 2 sets of waypoints to avoid out of index errors
							if(waypointGroups[i].waypointPosition.Count >= 2) 
							{
								int thisWaypointGroupCount = waypointGroups[i].waypointPosition.Count;
								
								Vector3 newPositionForLastWaypoint = Vector3.MoveTowards(waypointGroups[i].waypointPosition[thisWaypointGroupCount - 1], waypointGroups[i].waypointPosition[thisWaypointGroupCount - 2], actualNonActiveMergeSpeed); //Calculate new position for last waypoint - Using MoveTowards so the value does not overshoot.
								
								//If new position will be the same as the next waypoint check if it would be equal to the waypoint after that and keep repeating until a waypoint is found to be far enough away
								if(newPositionForLastWaypoint == waypointGroups[i].waypointPosition[thisWaypointGroupCount - 2]) 
								{
									waypointGroups[i].waypointPosition.RemoveAt(thisWaypointGroupCount -1); //Remove waypoint as merge speed has already passed this point
									waypointGroups[i].waypointTotal = waypointGroups[i].waypointPosition.Count;	
									
									if(runtimeStoreRotation == true)
									{
										waypointGroups[i].waypointRotation.RemoveAt(thisWaypointGroupCount -1);		
									}

									if(runtimeStoreScale == true)
									{
										waypointGroups[i].waypointScale.RemoveAt(thisWaypointGroupCount -1);		
									}
									
									if(runtimeStoreTimeStamp == true)
									{
										waypointGroups[i].waypointTimeStamp.RemoveAt(thisWaypointGroupCount -1);
									}
									
								} else {
									
									waypointGroups[i].waypointPosition[thisWaypointGroupCount -1] = newPositionForLastWaypoint; //Update last waypoint position in LIST
								}
								
								if(nonActiveWaypointMergeModes == nonActiveWaypointMergeTypes.Sequential)
								{
									break; //break here has already done everything needed
								}
							}			
							
							//Remove Last Remaining Waypoint and the List as not needed
							if(waypointGroups[i].waypointPosition.Count <= 1)
							{
								waypointGroups.RemoveAt(i);
							}
						}	
					}
				}
			}
			
			yield return null; 
		}
	}
	
	//----------------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------
	
	//Misc calculations
	IEnumerator MiscCalculations()
	{
		while(true)
		{
			GetWaypointCountCurrentTotal();
			
			//Clamping Operations    
			maxNumberOfwaypoints = (int) Mathf.Clamp(maxNumberOfwaypoints, 3, Mathf.Infinity); //Used int.MaxValue to replace Mathf.Infinity in conversion to c#
			waypointIntervalDistance = Mathf.Clamp(waypointIntervalDistance, 0f, Mathf.Infinity);
			
			yield return null; 
		}	
	}
	
	//----------------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------
	
	//Get Current Waypoint total - Used for waypoint limiting etc But could be useful
	void GetWaypointCountCurrentTotal()
	{
		//Waypoint counting
		waypointCountCurrentTotal = 0;
		foreach(WayPointsClass group in waypointGroups)
		{
			if(group.waypointPosition != null)
			{
				waypointCountCurrentTotal += group.waypointPosition.Count;
			}
		}
	}
	
	//----------------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------
	
	//Update the position of the absolutewaypoint so that it matches tracked object position + any offset
	IEnumerator AbsoluteWaypointPositionUpdate()
	{
		while(true)
		{
			//Updating of Absolutewaypoint
			if(absoluteWayPointCreated == true && waypointGroups.Count > 0)
			{
				for (int i = 0; i < waypointGroups.Count; i++)
				{
					//When Active Trail is found 
					if(waypointGroups[i].activeWaypointGroup == true && waypointGroups[i].waypointTotal >= 1)
					{
						//Calculate absolute waypoint position
						absoluteWaypointResult = trackThis.transform.TransformPoint(absoluteWayPointOffset);	    
						waypointGroups[i].waypointPosition[0] = absoluteWaypointResult;
					}
				}			
			}
			
			yield return null;
		}	
	}
	
	//----------------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------
	
	//Draw lines in the scene window at run time
	//function OnDrawGizmosSelected () { //Draw when gameobject is selected
	void OnDrawGizmos() //Draw all the time
	{
		if(showGizmos == true)
		{
			Gizmos.color = Color.red;
			
			if(waypointGroups.Count > 0)
			{
				for (int i = 0; i < waypointGroups.Count; i++)
				{
					if(waypointGroups[i].waypointTotal > 0) //Check artificial count as checking real waypointPosition.Count will return a null object error
					{
						for (int ii = 0; ii < waypointGroups[i].waypointPosition.Count; ii++)
						{
							Gizmos.DrawWireSphere(waypointGroups[i].waypointPosition[ii], 0.1f);
							
							if(ii < waypointGroups[i].waypointPosition.Count-1)
							{
								Gizmos.DrawLine(waypointGroups[i].waypointPosition[ii], waypointGroups[i].waypointPosition[ii+1]);
							}
						}
					}					
				}
			}
		}
	}
}
