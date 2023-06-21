using UnityEngine;
using System.Collections;

public class OMTRaycast_CS : MonoBehaviour {

/*
This script shows how to create a linecast for each OMT waypoint group. lastDetectedHit will show the transform last hit
*/
	
	public OMT_CS omtComponent;						//Object Motion Tracker
	public bool showDebugInSceneWindow;		//Show linecast in scene window
	public Transform lastDetectedHit;			//Last Transform Detected by linecast
	private RaycastHit hit; 				//Linecast Hit Information

	//----------------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------

	void Update ()
	{
		for(int i = 0; i < omtComponent.waypointGroups.Count; i++)
		{
			for(int ii = 1; ii < omtComponent.waypointGroups[i].waypointPosition.Count; ii++) //int = 1 because I need to know the previous value as well as the current array position value
			{	 
				if(showDebugInSceneWindow)
				{
					Debug.DrawLine(omtComponent.waypointGroups[i].waypointPosition[ii-1], omtComponent.waypointGroups[i].waypointPosition[ii], Color.green); //Use draw line instead of drawray as you can use two vector3 cordinates to draw a line (instead of range and direction)  
				}
				
				if (Physics.Linecast (omtComponent.waypointGroups[i].waypointPosition[ii-1], omtComponent.waypointGroups[i].waypointPosition[ii], out hit))
				{
					lastDetectedHit = hit.transform;
				}
			}
		}
	}
}
