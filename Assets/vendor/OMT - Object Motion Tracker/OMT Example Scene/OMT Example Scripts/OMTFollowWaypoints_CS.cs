using UnityEngine;
using System.Collections;

/*
Make a transform follow waypoint groups in different ways

LengthLast: Start at the last index and move along to the first index in the waypoint group
LengthFirst: Start at the first index and move along to the last index in the waypoint group
FirstWaypoint: Follow the last waypoint in the waypoint group
LastWaypoint: Follow the last waypoint in the waypoint group

*/

public class OMTFollowWaypoints_CS : MonoBehaviour {

	public OMT_CS omtComponent;																//Object Motion Tracker
	public enum WaypointFollowModeOptions {LengthFirst, LengthLast, FirstWaypoint, LastWaypoint};	//Waypoint Follow Modes
	public WaypointFollowModeOptions waypointFollowMode;									//Expose Follow Modes in the inspector
	WaypointFollowModeOptions previousWaypointFollowMode;									//Previous Follow Mode selected
	public Transform ObjectToFollowWaypoint;												//Target Object to move along the waypoint group
	public float moveSpeed;																	//Object Move speed overt time
	public int waypointGroupToFollow;														//The index number of the waypoint group
	int previouswaypointGroupToFollow = -1;													//Used to check if target waypoint group has changed
	string waypointGroupToFollowID;															//ID Is used to keep track of the waypoint group. Just in case a waypoint group is removed and the index position of the target waypoint group changes as a result
	int indexCount;																			//Index count for the Length follow option

	//--------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------
	
	void Start()
	{
		StartCoroutine ("MoveObject");
	}
	
	//--------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------
	
	IEnumerator MoveObject()
	{
		while(true)
		{
			yield return StartCoroutine("FollowSetup");
			
			for(int i = 0; i < omtComponent.waypointGroups.Count; i++)
			{
				//Move along waypoints starting at the last index
				if(waypointFollowMode == WaypointFollowModeOptions.LengthLast)
				{
					//If waypoint group ID matches and the index count has not reached 0
					if(waypointGroupToFollowID == omtComponent.waypointGroups[i].id && indexCount > 0)
					{
						//If the object has not reached the waypoint position move again
						if(ObjectToFollowWaypoint.position != omtComponent.waypointGroups[i].waypointPosition[indexCount-1])
						{
							ObjectToFollowWaypoint.position = Vector3.MoveTowards(ObjectToFollowWaypoint.position, omtComponent.waypointGroups[i].waypointPosition[indexCount-1],  moveSpeed * Time.deltaTime);
						}
						
						//If object has reached the position, chnage the index count and start moving to next waypoint position IF index count has not reached 0
						if(ObjectToFollowWaypoint.position == omtComponent.waypointGroups[i].waypointPosition[indexCount-1])
						{
							indexCount -= 1; 
							
							if(indexCount > 0)
							{
								ObjectToFollowWaypoint.position = Vector3.MoveTowards(ObjectToFollowWaypoint.position, omtComponent.waypointGroups[i].waypointPosition[indexCount],  moveSpeed * Time.deltaTime);		    		
							}
						}
					}
				}
				
				//Move along waypoints starting at the first index
				if(waypointFollowMode == WaypointFollowModeOptions.LengthFirst)
				{
					//If waypoint group ID matches and the index count has not reached the last index
					if(waypointGroupToFollowID == omtComponent.waypointGroups[i].id && indexCount < omtComponent.waypointGroups[i].waypointPosition.Count)
					{
						//If the object has not reached the waypoint position move again
						if(ObjectToFollowWaypoint.position != omtComponent.waypointGroups[i].waypointPosition[indexCount])
						{
							ObjectToFollowWaypoint.position = Vector3.MoveTowards(ObjectToFollowWaypoint.position, omtComponent.waypointGroups[i].waypointPosition[indexCount],  moveSpeed * Time.deltaTime);
						}
						
						//If object has reached the position, chnage the index count and start moving to next waypoint position IF index count has not reached the last index
						if(ObjectToFollowWaypoint.position == omtComponent.waypointGroups[i].waypointPosition[indexCount])
						{
							indexCount += 1; 
							
							if(indexCount < omtComponent.waypointGroups[i].waypointPosition.Count)
							{
								ObjectToFollowWaypoint.position = Vector3.MoveTowards(ObjectToFollowWaypoint.position, omtComponent.waypointGroups[i].waypointPosition[indexCount],  moveSpeed * Time.deltaTime);		    		
							}
						}
					}
				}
				
				//Follow position of last waypoint in the target waypoint group
				if(waypointFollowMode == WaypointFollowModeOptions.FirstWaypoint)
				{
					//If waypoint group ID matches
					if(waypointGroupToFollowID == omtComponent.waypointGroups[i].id)
					{
						//Get the current index number of the last index
						var lastWaypointIndexNumber = omtComponent.waypointGroups[i].waypointPosition.Count - 1;
						
						//Move Towards the waypoint
						ObjectToFollowWaypoint.position = Vector3.MoveTowards(ObjectToFollowWaypoint.position, omtComponent.waypointGroups[i].waypointPosition[lastWaypointIndexNumber],  moveSpeed * Time.deltaTime);
					}
				}
				
				//Follow position of last waypoint in the target waypoint group
				if(waypointFollowMode == WaypointFollowModeOptions.LastWaypoint)
				{
					//If waypoint group ID matches
					if(waypointGroupToFollowID == omtComponent.waypointGroups[i].id)
					{
						//Move Towards the waypoint
						ObjectToFollowWaypoint.position = Vector3.MoveTowards(ObjectToFollowWaypoint.position, omtComponent.waypointGroups[i].waypointPosition[0],  moveSpeed * Time.deltaTime);
					}
				}
			}
			yield return null;
		}
	}
	
	//--------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------
	
	void FollowSetup()
	{
		//Detect if any settings have changed - If so setup required parameters
		if(previouswaypointGroupToFollow != waypointGroupToFollow || previousWaypointFollowMode != waypointFollowMode )
		{
			//Reset Previous values for next cycle
			previouswaypointGroupToFollow = waypointGroupToFollow;
			previousWaypointFollowMode = waypointFollowMode;
			
			//Get ID of the target waypoint group
			for(int i = 0; i < omtComponent.waypointGroups.Count; i++)
			{
				//Find the waypoint group
				if(i == waypointGroupToFollow)
				{
					waypointGroupToFollowID = omtComponent.waypointGroups[i].id;					//Get the waypoint group ID
					
					if(waypointFollowMode == WaypointFollowModeOptions.LengthLast)
					{
						indexCount = omtComponent.waypointGroups[i].waypointPosition.Count-1;	//Set indexCount to last index position
					}
					
					if(waypointFollowMode == WaypointFollowModeOptions.LengthFirst)
					{
						indexCount = 0;															//Set indexCount to first index
					}
					
				}
			}
		}
	}
}
