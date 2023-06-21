using UnityEngine;
using System.Collections;

public class OMTReadDataFromWaypointGroupExample_CS : MonoBehaviour {
/*
How to access a waypoint data. Lets you select which waypoint group and which index. If an invalid waypoint group or index is selected, nothing will happen.
For example:

targetWaypointPosition = omtComponent.waypointGroups[0].waypointPosition[2];
targetWaypointRotation = omtComponent.waypointGroups[0].waypointRotation[2];
targetWaypointTimeStamp = omtComponent.waypointGroups[0].waypointTimeStamp[2];

Will get data for the 3rd waypoint from the first waypoint group.
*/
	
	public OMT_CS omtComponent; 							//Object Motion Tracker
	public int targetWaypointGroup;							//Index number of the target waypoint group
	public int targetWayPointIndexNumber;					//Index number of the waypoint we want to get data from
	public Vector3 targetWaypointPosition;					//The position of the waypoint
	public Quaternion targetWaypointRotation;				//The rotation of the transform being tracked at the targetWaypointPosition
	public float targetWaypointTimeStamp;					//How long after the scene started the waypoint was created
	
	//--------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------
	
	void Start ()
	{
		StartCoroutine ("ShowValuesAtSelectedPosition");						//Start looping function
	}
	
	
	//--------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------
	
	
	IEnumerator ShowValuesAtSelectedPosition()
	{
		while(true)
		{
			targetWaypointGroup = Mathf.Clamp(targetWaypointGroup, 0, omtComponent.waypointGroups.Count); //Clamp to prevent selecting non-existing index
			
			
			if(omtComponent.waypointGroups.Count > 0 && omtComponent.waypointGroups.Count > targetWaypointGroup)
			{
				targetWayPointIndexNumber = Mathf.Clamp(targetWayPointIndexNumber, 0, omtComponent.waypointGroups[targetWaypointGroup].waypointPosition.Count); //Clamp to prevent selecting non-existing index
				
				if(omtComponent.waypointGroups[targetWaypointGroup].waypointPosition.Count > targetWayPointIndexNumber)
				{
					//When waypoint exists get position
					targetWaypointPosition = omtComponent.waypointGroups[targetWaypointGroup].waypointPosition[targetWayPointIndexNumber];			//Get position data
					
					//First check if the objectMotionTracker script is set to log waypoint rotations
					if(omtComponent.storeRotations == true)
					{
						targetWaypointRotation = omtComponent.waypointGroups[targetWaypointGroup].waypointRotation[targetWayPointIndexNumber];		//Get rotation data
					}
					
					//First check if the objectMotionTracker script is set to log waypoint time information
					if(omtComponent.storeTimeStamp == true)
					{
						targetWaypointTimeStamp = omtComponent.waypointGroups[targetWaypointGroup].waypointTimeStamp[targetWayPointIndexNumber];	//Get time data
					}
				}
			}
			yield return null;
		}
	}
}
