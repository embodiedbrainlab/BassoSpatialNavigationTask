using UnityEngine;
using System.Collections;

public class OMTSendMessageExample_CS : MonoBehaviour {
/*
Some functions in OMT can be called remotely, either by referencing them directly or by using SendMessageOptions
The following functions have that ability:

ObjectMotionTrackTrigger(boolean);
RemoveWaypointGroup(int);

Below are examples on how to access them all using SendMessage.

You can of course reference them directly like this:
objectMotionTrackingScript.ObjectMotionTrackTrigger(true);
*/
	public OMT_CS omtComponent; 							//Object Motion Tracker
	public bool enableThisScript = false; 					//Stop this script from over-riding everything else
	public bool useLeftMouseTostartTracking = false;		//If true message will be sent - Triggered when left mouse button is clicked
	public bool startObjectMotionTracking = false; 		//Value to send in broadcast message
	public bool sendRemoveWaypointGroupMessage = false;	//Click in inspector to send remove waypoint group message
	public int waypointGroupIndexNumber; 					//-1 will remove ALL non-active groups


	//--------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------
	
	void Start()
	{
		StartCoroutine("MouseControlledTracking");
		StartCoroutine("RemotelyRemoveWaypointGroups");
	}
	
	//--------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------
	
	IEnumerator MouseControlledTracking ()
	{
		while(true)
		{
			if(enableThisScript == true)
			{
				if(useLeftMouseTostartTracking)
				{
					//The boolean is updated to true when the left mouse button is held down, and false when no left mouse button is held down
					if (Input.GetKey (KeyCode.Mouse0))
					{
						startObjectMotionTracking = true;
					} else {
						startObjectMotionTracking = false;
					}
				}
				
				omtComponent.SendMessage("ObjectMotionTrackTrigger", startObjectMotionTracking); //Send message broadcast to all scripts
			}
			yield return null;
		}
	}
	
	//--------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------

	IEnumerator RemotelyRemoveWaypointGroups ()
	{
		while(true)
		{
			if(enableThisScript == true)
			{
				if(sendRemoveWaypointGroupMessage == true)
				{
					sendRemoveWaypointGroupMessage = false;
					omtComponent.SendMessage("RemoveWaypointGroup", waypointGroupIndexNumber); //Send message broadcast to all scripts
				}
			}
			
			yield return null;
		}
	}
}
