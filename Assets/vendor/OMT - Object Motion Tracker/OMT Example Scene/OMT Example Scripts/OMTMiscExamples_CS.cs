using UnityEngine;
using System.Collections;

public class OMTMiscExamples_CS : MonoBehaviour {



	/*
	Misc Examples
	1 - GetNumberOfWaypointGroups() Shows how to get the total of all waypoint groups
	2 - DetectActiveWayPointGroup() Show how to find an active waypoint group
	*/
	
	public OMT_CS omtComponent;										//Object Motion Tracker

	//Variable used by the GetNumberOfWaypointGroups() function
	public int numberOfWaypointGroupsAvailable;						//Number of WayPoint Groups
	
	//Variable used by the DetectActiveWayPointGroup() function
	public bool activeWaypointGroupFound;							//True if an active waypoint group has been found


	//--------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------
	
	void Start ()
	{
		StartCoroutine ("DetectActiveWayPointGroup");									//Start Function
		StartCoroutine ("GetNumberOfWaypointGroups");									//Start Function
	}
	
	//--------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------
	
	//Get the number of waypoint groups
	IEnumerator GetNumberOfWaypointGroups()
	{
		while(true)
		{
			//First check to see if there is any waypoint information
			if(omtComponent.waypointGroups.Count > 0)
			{
				numberOfWaypointGroupsAvailable = omtComponent.waypointGroups.Count; //Get the number of waypoint groups
			} else {
				numberOfWaypointGroupsAvailable = 0; //Set to zero if no waypoint groups have been found
			}
			yield return null;
		}
	}
	
	//--------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------
	
	//Detects if there is an active Waypoint group
	//An active waypoint group is a waypoint group that is having new waypoint data assigned to it
	//There is only 1 active waypoint group at any time
	//Currently the active waypoint group occupies the last index in the waypoints group array
	IEnumerator DetectActiveWayPointGroup()
	{
		while(true)
		{
			if(omtComponent.waypointGroups.Count > 0)
			{
				for(int i = 0; i < omtComponent.waypointGroups.Count; i++)
				{
					if(omtComponent.waypointGroups[i].activeWaypointGroup == true)
					{
						activeWaypointGroupFound = true;
					} else {
						activeWaypointGroupFound = false;
					}
				}
			}
			yield return null;
		}
	}
}
