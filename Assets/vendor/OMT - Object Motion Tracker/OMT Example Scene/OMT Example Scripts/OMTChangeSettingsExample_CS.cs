using UnityEngine;
using System.Collections;

public class OMTChangeSettingsExample_CS : MonoBehaviour {

/*
Example script showing how to change various settings at the start. Kind of like a configurator.
*/
	
	public OMT_CS omtComponent;
	bool applyChangesOnStart = false;		//Enable / Disable setup on Start()
	
	//--------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------
	
	void Start ()
	{
		if(applyChangesOnStart == true)
		{
			/*WAYPOINT PLOTTING SETUP*/
			
			//Enable tracking - Primes the tracker ready to do stuff.
			omtComponent.trackingActive = true;															

			//Assign a transform to track - In this example we are using the transform this script is attached to.
			omtComponent.trackThis = GameObject.Find("Tracking This");														
			
			//Set waypoint plotting mode to Continuous (0=Groups 1=Continous)
			omtComponent.waypointPlottingModes = OMT_CS.waypointPlottingModeTypes.Continuous;
			
			//Set waypoint interval mode to use both time and distance intervals (0=Distance 1=Seconds 2=Both)	
			omtComponent.waypointIntervalModes = 0;
			
			//Distance before waypoint is plotted
			omtComponent.waypointIntervalDistance = 0.1f;
			
			//Time before waypoint is plotted					
			omtComponent.waypointIntervalSeconds = 0;
			
			//Set waypoint removal method to none (0=None 1=Active 2=Extreme)
			omtComponent.maxNumberOfwaypointsGroupPlottingModeRemoval = 0;
			
			//Set Max number of waypoints
			omtComponent.maxNumberOfwaypoints = 0;
			
			//Store waypoint rotation information - NOTE: Cannot be changed after starting the scene
			omtComponent.storeRotations = true;
			
			//Do not store waypoint creation time - NOTE: Cannot be changed after starting the scene							
			omtComponent.storeTimeStamp = true;
			
			//Enable Gizmos in the scene window - Will display waypoint information using 3d gizmos
			omtComponent.showGizmos = true;
			
			
			
			/*ABSOLUTE WAYPOINT*/
			
			//Enable the absolute waypoint
			omtComponent.absoluteWaypoint = true;
			
			//Set the absolute waypoint to be 1 unit above the tracked object
			omtComponent.absoluteWayPointOffset = new Vector3(0,1,0);
			
			
			
			/*OFFSET WAYPOINT*/
			
			//Set offset waypoint offset plotting method to manual (0=Manual 1=Percentage 2=Actual)
			omtComponent.offsetWaypointPlottingMethod = 0;
			
			//Set the offset to apply to the offset waypoint by 1 unit above and 1 unit behind the tracked object
			omtComponent.offsetManualAmount = new Vector3(0,1,-1);
			
			//Set the Z axis Offset with speed min and max values to 0 
			omtComponent.offsetMinPosition = new Vector3(0,0,0);
			omtComponent.offsetMaxPosition = new Vector3(0,0,0);
			
			//Clear or add and object for the offset waypoint to match. In this case we clear it just to provide an example of it's useage
			omtComponent.offsetWithThisObjectPosition = null;
			
			
			
			/*WAYPOINT MERGING*/
			
			//Enable waypoint merging on the active waypoint group
			omtComponent.mergeActiveWaypointGroup = false;
			
			//Active waypoint group merge speed
			omtComponent.activeWaypointGroupMergeSpeed = 0;
			
			//Disable the active waypoint group merging speed to be adjusted by percentage 	
			omtComponent.adjustMergingSpeedWithMergeRatePercentage = false;
			
			//Set Active merge rate percentage to 10. This means the active waypoint group merge speed will actually be 0.2
			omtComponent.activeMergeRatePercentage = 10;
			
			//Set non-active waypoint group merging mode to sequential (0=Simultaneous 1=Sequential)
			omtComponent.nonActiveWaypointMergeModes = OMT_CS.nonActiveWaypointMergeTypes.Sequential;

			//Disable merging on non active waypoint groups
			omtComponent.mergeNonActiveWaypointGroups = false;
			
			//Set non-active waypoint group merge speed
			omtComponent.nonActiveWaypointGroupMergeSpeed = 0;
		}
		
	}
}
