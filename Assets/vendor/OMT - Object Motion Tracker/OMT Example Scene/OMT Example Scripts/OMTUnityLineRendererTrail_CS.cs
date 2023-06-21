using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OMTUnityLineRendererTrail_CS : MonoBehaviour {

/*
Create a trail behind an object using Unity's in buil lin Renderer.
A new gameObject is created and a line renderer component is added to it. The gameObject is then moved into a container to keep them nice and tidy.
*/

	//----------------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------

	public OMT_CS omtComponent;														//Object Motion Tracker - Array of points to generate spline(s) are from here
	public Color startColor = Color.white;											//Line start color
	public Color endColor = new Color(1,1,1,0);											//Line end color
	public float startWidth = 0.1f;													//Line start width
	public float endWidth = 0.1f;													//Line end width
	public Material LineRendererMaterial;											//Line Material
	public List<GameObject> lineRendererGameObjectsList = new List<GameObject>();	//List of GameObjects created
	public List<LineRenderer> lineRenderersList = new List<LineRenderer>();			//List of lineRenderers created
	private GameObject lineRendererContainer;										//Container for created lineRenderers		

	//----------------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------
	
	void Start()
	{
		lineRendererContainer = new GameObject( "OMT Line Renderer Trails Parent");	//Create Container
		StartCoroutine("DrawTrail");
	}
	
	//----------------------------------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------------------------------
	
	IEnumerator DrawTrail()
	{
		while(true)
		{
			//Tidy up Previously created line renderer game objects and lists
			for (int iii = 0; iii < lineRendererGameObjectsList.Count; iii++)
			{    	
				Destroy(lineRendererGameObjectsList[iii]);													//Destory created gameobject/linerenderer
			}
			
			lineRendererGameObjectsList.Clear();															//Clear list of gameObjects ready for new data
			lineRenderersList.Clear();																		//Clear list of lineRenderers ready for new data
			
			//If there are waypoint lists.....
			for (int i = 0; i < omtComponent.waypointGroups.Count; i++)
			{
				GameObject lineRenderGameObject = new GameObject( "Line Renderer: " + i );					//Create Gameobject - Ready for us to attach a linerenderer
				lineRenderGameObject.transform.parent = lineRendererContainer.transform;					//Make gameObject a child of the lineRendererContainer gamoeObject
				lineRendererGameObjectsList.Add(lineRenderGameObject);										//Add gameObject to a list for easy access
				
				lineRendererGameObjectsList[i].AddComponent<LineRenderer>();									//Add a lineRenderer component to the gameObject
				lineRenderersList.Add(lineRendererGameObjectsList[i].GetComponent<LineRenderer>());			//Add the linerenderer to a List for easy access
				lineRenderersList[i].useWorldSpace = true;													//Set linerender to use world space for cordinates
				lineRenderersList[i].material = LineRendererMaterial;										//Assign the selected material to the linerender
				lineRenderersList[i].SetColors(startColor, endColor);										//Set start and end colors
				lineRenderersList[i].SetVertexCount(omtComponent.waypointGroups[i].waypointPosition.Count);	//Set vertex count (important!)
				lineRenderersList[i].SetWidth(startWidth, endWidth);										//Set line render widths
				
				//Now add the waypoints to the linerenderer
				for (int ii = 0; ii < omtComponent.waypointGroups[i].waypointPosition.Count; ii++)
				{
					lineRenderersList[i].SetPosition(ii,omtComponent.waypointGroups[i].waypointPosition[ii]); 
				}
			}
			yield return null;
		}
	}
}
