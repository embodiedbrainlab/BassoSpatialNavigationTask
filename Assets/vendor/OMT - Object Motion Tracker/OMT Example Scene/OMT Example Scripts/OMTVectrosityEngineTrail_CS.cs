/*	Script has been commented out to avoid errors for users who do not have vectrosity installed -Remove this comment to enable this script

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;

public class OMTVectrosityEngineTrail_CS : MonoBehaviour {

	public OMT_CS omtComponent;							//Object Motion Tracker - Array of points to generate spline(s) are from here
	
	public Material engineTrailMaterial; 				//Material used for rendering the trail 
	public Color engineTrailColor;						//Base color - aplha will be adjusted
	
	public float engineTrailStartSize = 2.0f;			//Start Size of the trail
	public float engineTrailEndSize = 0.001f;			//End Size of the trail
	
	//Alpha trail must be between one and zero
	public float engineTrailAlphaStart = 1f;
	public float engineTrailAlphaEnd = 0.01f;
	
	private int segments; 								//Calculated amount (waypoints.Count)
	private VectorLine spline;							//Calculated amount
	private int numberOfSplinePointsToCreate;			//Calculated amount
	private float engineTrailSizeIncrement;				//Calculated amount
	private float engineTrailSizeToUse;					//Calculated amount
	private float engineColorAlphaIncrement;			//Calculated amount
	private float engineColorAlphaToUse;				//Calculated amount
	public float distanceFromCamera; 					//Calculated distance used in the percentage calculation
	
	public float maxDistanceViewable = 250f; 			//The limit that 100% is achieved at - this can be offset by percentageMax below
	public float percentageToReduceBy; 					//Calculated amount to reduce vector size
	public float percentageMax = 90f; 					//Max allowed cercentage decrease allowed - For example 90 so that it remains visable even at max distance viewable range
	
	public string splineName = "OMT Vectrosity Trail "; //The name applied to the spline object that is created
	private string finalSplineName;						//Calculated name
	
	public bool addRootObjectNameToSplineName = true; 	//Modify the name of the trail
	
	private float[] myWidths = new float[0];			//Array of widths - each part of the spline has it's own width - As a result this array must be the same length as the waypoint array & splinepoints array
	private Color[] myColors = new Color[0]; 			//See above
	
	public List<Vector3> splinePoints; 					//Calculated amount
	public List<VectorLine> splineList;					//Calculated amount
	
	
	public bool enableSplineDebug;						//Switch to collect debug
	public List<splineDebugClass> splineDebug = new List<splineDebugClass>();	//Master Debug List
	
	//Class is used to store the List which is a class
	public class splineDebugClass								
	{ 	
		public List<splineDebugGroups> splineInfo = new List<splineDebugGroups>();
	}
	
	public class splineDebugGroups
	{
		public string groupID;							//ID of the waypint group
		public string waypoint;							//Waypoint index number
		public int spline;								//Spline Number
	}

	private int currentii;
	
	//----------------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------
	
	void Start ()
	{
		StartCoroutine("DrawVectrosityTrail");
	}
	
	//----------------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------
	
	IEnumerator DrawVectrosityTrail ()
	{
		while(true)
		{
			//First destroy the old splines
			for(int iiii = 0; iiii < splineList.Count; iiii++)
			{
				VectorLine targetSpline = splineList[iiii];
				VectorLine.Destroy (ref targetSpline);
			}
			
			//Now the old splines have been removed, clear the List ready for new splines
			splineList.Clear();
			
			
			
			//If there are waypoint groups.....
			for (int i = 0; i < omtComponent.waypointGroups.Count; i++)
			{
				
				
				//If there are at least 2 waypoints in this waypoint group then start creating a spline
				if(omtComponent.waypointGroups[i].waypointPosition.Count >= 2)
				{
					
					numberOfSplinePointsToCreate = omtComponent.waypointGroups[i].waypointPosition.Count; //Get number of spline points
					segments = omtComponent.waypointGroups[i].waypointPosition.Count; //Set segment count to match number of spline points (waypoints)
					
					distanceFromCamera = Vector3.Distance(Camera.main.transform.position, transform.position); //Used for calculating enginetrail size from distance
					percentageToReduceBy = (distanceFromCamera / maxDistanceViewable) * 100f; //Calculate percentage to reduce enginetrail widths by
					percentageToReduceBy = Mathf.Clamp(percentageToReduceBy, 0f, percentageMax); //Clamp percentageToReduceBy so that it does not go below 0 or over stated amount
					
					splinePoints.Clear(); //Reset ready for new calculations
					
					engineTrailSizeToUse = engineTrailStartSize;	//Reset ready for new calculations 
					engineColorAlphaToUse = engineTrailAlphaStart;	//Reset ready for new calculations 
					
					engineTrailSizeIncrement = (engineTrailStartSize - engineTrailEndSize) / numberOfSplinePointsToCreate;		//Calculate width increments
					engineTrailSizeIncrement -= (engineTrailSizeIncrement) / percentageToReduceBy;	//Reduce enginetrail size based percentage calculation on distance from main camera
					
					engineColorAlphaIncrement = (engineTrailAlphaStart - engineTrailAlphaEnd) / numberOfSplinePointsToCreate;	//Calculate alpha increments 
					
					Array.Resize<float>(ref myWidths, numberOfSplinePointsToCreate); //Set widths array to match number Of Spline Points To Create
					Array.Resize<Color>(ref myColors, numberOfSplinePointsToCreate); //Set colors array to match number Of Spline Points To Create
					
					//Add spline points from the wapoint tracker script
					for (int ii = 0; ii < omtComponent.waypointGroups[i].waypointPosition.Count; ii++)
					{
						currentii = ii;
						engineTrailSizeToUse -= engineTrailSizeIncrement; //Decrease engineTrailSizeToUse ready for next spline point
						engineColorAlphaToUse -= engineColorAlphaIncrement;	//Decrease engineColorAlphaToUse ready for next spline point
						
						myWidths[ii] = engineTrailSizeToUse; //Add line width for this spline point
						myColors[ii] = new Color (engineTrailColor.r, engineTrailColor.g, engineTrailColor.b, engineColorAlphaToUse);
						
						splinePoints.Add(omtComponent.waypointGroups[i].waypointPosition[ii]); //Add spline point

					}
					
					//Add root Transform name to spline name if needed - Also adds the waypoint group number
					if(addRootObjectNameToSplineName == true)
					{
						finalSplineName = splineName + this.transform.root.name + " " + i;
					} else {
						finalSplineName = splineName + " " + i;
					}



					//Setup spline
					splineList.Add(new VectorLine(finalSplineName, new Vector3[segments+1], engineTrailMaterial, engineTrailStartSize, LineType.Continuous, Joins.Fill));

					if(enableSplineDebug == true)
					{
						DebugFun(i,currentii,omtComponent.waypointGroups[i].id);
					}
				
					//Check if this spline exists if not try again - Possible bug or Vectrosity issue but seems to work fine.
					if(splineList.Count <= i)
					{
						//Try to Setup spline again
						splineList.Add(new VectorLine(finalSplineName + i, new Vector3[segments+1], engineTrailMaterial, engineTrailStartSize, LineType.Continuous, Joins.Fill)); 
					}
					
					if(splineList.Count > i)
					{
						splineList[i].MakeSpline (splinePoints.ToArray(), segments); //Make the spline
						
						//Many vectrosity elements such as SetWidths, SetColorsSmooth need to match the same length as the spline array 		
						splineList[i].SetColorsSmooth (myColors);
						
						splineList[i].SetWidths (myWidths); //Assign widths for each point in the spline
						splineList[i].smoothWidth = true; //Smooth width change between points
						
						//Future Testing / Implementation
						//spline.AddTangents();
						//spline.AddNormals();
						//spline.SetTextureScale(1.0, 1);
						//spline.continuousTexture = true;
					}
				}
			}
			
			//Draw the trail splines!
			foreach(VectorLine splineToDraw in splineList)
			{
				splineToDraw.continuousTexture = true;
				splineToDraw.AddTangents();
				splineToDraw.AddNormals();
				splineToDraw.Draw3D();	
			}
			
			yield return null;
		}
	}
	
	//----------------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------

	void DebugFun(int group, int waypoint, string id)
	{
		//Debug test
		splineDebugGroups debugAdd = new splineDebugGroups();					//Get ready to add class to list				
		debugAdd.groupID = omtComponent.waypointGroups[group].id;	//Get waypoint group ID
		debugAdd.waypoint = waypoint.ToString();							//Get waypoint index number
		debugAdd.spline = splineList.Count;						//Get soline list count
		
		var debugAddGroup = new splineDebugClass();				//Create a group for when creating initial List entry
		debugAddGroup.splineInfo.Add(debugAdd);
		
		//Check if already created
		if(splineDebug.Count > group)
		{
			//If created look for correct group using ID
			for (int z = 0; z < splineDebug.Count; z++)
			{
				//When found Add in corect group
				if(splineDebug[z].splineInfo[0].groupID == debugAdd.groupID)
				{
					splineDebug[z].splineInfo.Add(debugAdd);
				}
			}
			
		} else {
			//As not found, create new group
			splineDebug.Add(debugAddGroup);
		}
	}
}

*/
