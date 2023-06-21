#pragma warning disable
using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(OMT_CS))] 

public class OMTEditor_CS : Editor {

	SerializedObject obj; //Create serialized object to update later on - SerializedProperty below are part of the serialized object
	
	SerializedProperty trackThis;
	SerializedProperty trackingActive;
	SerializedProperty waypointIntervalModes;
	SerializedProperty waypointPlottingModes; 
	SerializedProperty maxNumberOfwaypointsGroupPlottingModeRemoval;
	SerializedProperty maxNumberOfwaypoints;
	SerializedProperty storeRotations;
	SerializedProperty storeScale;
	SerializedProperty storeTimeStamp;
	SerializedProperty mergeActiveWaypointGroup;
	SerializedProperty mergeNonActiveWaypointGroups;
	SerializedProperty adjustMergingSpeedWithMergeRatePercentage;
	SerializedProperty activeMergeRatePercentage;
	SerializedProperty absoluteWaypoint;
	SerializedProperty absoluteWayPointOffset; 
	SerializedProperty offsetWaypointPlottingMethod; 
	SerializedProperty adjustOffsetWaypointWithPercentage;
	SerializedProperty showGizmos;
	SerializedProperty offsetManualAmount;
	SerializedProperty offsetPercentage;
	SerializedProperty offsetMinPosition;
	SerializedProperty offsetMaxPosition;
	SerializedProperty offsetWithThisObjectPosition;
	SerializedProperty framesInterval; 
	SerializedProperty waypointIntervalDistance;
	SerializedProperty waypointIntervalSeconds;
	SerializedProperty nonActiveWaypointMergeModes;
	SerializedProperty activeWaypointGroupMergeSpeed;
	SerializedProperty nonActiveWaypointGroupMergeSpeed;
	SerializedProperty warnings;
	SerializedProperty notes;
	SerializedProperty showNotes;
	SerializedProperty showDebugInfo;
	
	//var OMTSkin : GUISkin;
	GUISkin OMTSkin;
	SerializedProperty showDiagram;
	Texture2D diagram;

	void OnEnable ()
	{
		//Get Diagram
		diagram = (Texture2D)Resources.Load("OMTDiagram",typeof(Texture2D));

		//Setup Serialized Object
		obj = new SerializedObject(target); 
		
		// Setup the required editable SerializedProperties
		trackThis = obj.FindProperty ("trackThis");
		trackingActive = obj.FindProperty ("trackingActive");
		waypointIntervalModes = obj.FindProperty ("waypointIntervalModes");
		waypointPlottingModes = obj.FindProperty ("waypointPlottingModes");	
		maxNumberOfwaypointsGroupPlottingModeRemoval = obj.FindProperty ("maxNumberOfwaypointsGroupPlottingModeRemoval");
		maxNumberOfwaypoints = obj.FindProperty ("maxNumberOfwaypoints");
		storeRotations = obj.FindProperty ("storeRotations");
		storeScale = obj.FindProperty ("storeScale");
		storeTimeStamp = obj.FindProperty ("storeTimeStamp");
		mergeActiveWaypointGroup = obj.FindProperty ("mergeActiveWaypointGroup");
		mergeNonActiveWaypointGroups = obj.FindProperty ("mergeNonActiveWaypointGroups");
		adjustMergingSpeedWithMergeRatePercentage = obj.FindProperty ("adjustMergingSpeedWithMergeRatePercentage");
		activeMergeRatePercentage = obj.FindProperty ("activeMergeRatePercentage");
		absoluteWaypoint = obj.FindProperty ("absoluteWaypoint");
		absoluteWayPointOffset = obj.FindProperty ("absoluteWayPointOffset"); 
		adjustOffsetWaypointWithPercentage = obj.FindProperty ("adjustOffsetWaypointWithPercentage");
		showGizmos = obj.FindProperty ("showGizmos");
		offsetWaypointPlottingMethod = obj.FindProperty ("offsetWaypointPlottingMethod");
		offsetManualAmount = obj.FindProperty ("offsetManualAmount");
		offsetPercentage = obj.FindProperty ("offsetPercentage");
		offsetMinPosition = obj.FindProperty ("offsetMinPosition");
		offsetMaxPosition = obj.FindProperty ("offsetMaxPosition");
		offsetWithThisObjectPosition = obj.FindProperty ("offsetWithThisObjectPosition");
		framesInterval = obj.FindProperty ("framesInterval");
		waypointIntervalDistance = obj.FindProperty ("waypointIntervalDistance");
		waypointIntervalSeconds = obj.FindProperty ("waypointIntervalSeconds");
		nonActiveWaypointMergeModes = obj.FindProperty("nonActiveWaypointMergeModes");
		activeWaypointGroupMergeSpeed = obj.FindProperty ("activeWaypointGroupMergeSpeed");
		nonActiveWaypointGroupMergeSpeed = obj.FindProperty ("nonActiveWaypointGroupMergeSpeed");
		warnings = obj.FindProperty ("warnings");
		notes = obj.FindProperty ("notes");
		showNotes = obj.FindProperty ("showNotes");
		showDebugInfo = obj.FindProperty ("showDebugInfo");
		showDiagram = obj.FindProperty ("showDiagram");	
	
	}

	//--------------------------------------------------------------------------------------------------------------------
	public override void OnInspectorGUI()
	{  
		
		//Get GUI Skin
		if(OMTSkin == null)
		{
			//Get GUI Skin
			OMTSkin = Resources.Load<GUISkin>("OMTSkin");
		}
		
		//Assign GUI Skin
		GUI.skin = OMTSkin;

		OMT_CS OMT_CS = (OMT_CS)target;
		obj.Update(); //Update serialized object's representation.
		
		//EditorGUIUtility.fieldWidth = 50; //Set width of information fields
		EditorGUIUtility.labelWidth = 260; //Set width of label field so all text can be seen at all times
		
		//Show diagram option
		if(GUILayout.Button("<b>Waypoint Diagram</b>"))
		{
			showDiagram.boolValue = !showDiagram.boolValue;
		}
		if(showDiagram.boolValue == true)
		{	EditorGUILayout.BeginVertical("Box"); //Surround the debug info in a box
			GUILayout.Box(diagram);
			EditorGUILayout.EndVertical(); //Stop Surrounding the debug info in a box
		}
		EditorGUILayout.Separator (); //Add a space between Inspector Item
		//~~~~~~~~~~~		
		
		//Waypoint Plotting Setup
		EditorGUILayout.BeginVertical("Box"); //Surround the debug info in a box
		GUILayout.Label("<color=#ffffff>Waypoint Plotting Setup</color>"); //Show Notes label
		EditorGUILayout.Separator (); //Add a space between Inspector Items
		EditorGUILayout.PropertyField (trackingActive, new GUIContent ("Tracking Active", "Enable / Disable Tracking"));
		EditorGUILayout.Separator (); //Add a space between Inspector Items		
		EditorGUILayout.PropertyField (trackThis, new GUIContent ("Track This", "If no gameObject is specified, the gameObject that OMT is attached to will be used by default. You can place this script on a different gameObject to the one being tracked."));		
		EditorGUILayout.Separator (); //Add a space between Inspector Items
		EditorGUILayout.PropertyField (waypointPlottingModes, new GUIContent ("Waypoint Plotting Modes", "Groups: Multiple waypoint groups will be created. When the command is given to stop tracking, the waypoint group will no longer be classed as active and will become it's own separate non-active waypoint group.\n\nContinuous: Only one waypoint group will exist at any time. So when the command to stop tracking is given, no further waypoints will be created. However when tracking resumes, the next waypoint will still be linked to the previous way point. A continuous waypoint group is never destroyed."));
		EditorGUILayout.Separator (); //Add a space between Inspector Items
		
		EditorGUILayout.PropertyField (waypointIntervalModes, new GUIContent ("Waypoint Interval Modes", "Distance: Waypoint will be plotting after moving a certain distance.\n\nSeconds: Waypoint will be plotted at set amount of seconds.\n\nDistanceAndSeconds: Both distance and seconds will plot waypoints.\n\nFrames: Set a frame interval for plotting waypoints. For example 1=Every Frame or 10 = Every 10 frames."));
		if(waypointIntervalModes.enumValueIndex == 0 || waypointIntervalModes.enumValueIndex == 2 )
		{
			EditorGUILayout.PropertyField (waypointIntervalDistance, new GUIContent ("Distance interval for waypoint plotting", "The amount of Unity units to move before plotting a waypoint. Please keep in mind that sudden velocity increases or very high velocity rates will mean that the actual waypoint distance will 'drift'."));
		}
		if(waypointIntervalModes.enumValueIndex == 1 || waypointIntervalModes.enumValueIndex == 2 )
		{
			EditorGUILayout.PropertyField (waypointIntervalSeconds, new GUIContent ("Time interval for waypoint plotting",  "The amount of seconds to pass before plotting a waypoint."));
		}
		if(waypointIntervalModes.enumValueIndex == 3)
		{
			EditorGUILayout.PropertyField (framesInterval, new GUIContent ("Frame interval for waypoint plotting",  "The amount of frames to pass before plotting a waypoint."));
		}
		EditorGUILayout.Separator (); //Add a space between Inspector Items	
		EditorGUILayout.PropertyField (maxNumberOfwaypointsGroupPlottingModeRemoval, new GUIContent ("Waypoint Removal Method", "None: Waypoint removal is Off.\n\nActive: Only limits the active waypoint group. It is also the default method when using continuous waypoint plotting.\n\nExtreme: When using group waypoint plotting, the oldest waypoints from the oldest non-active waypoint group will be removed."));	
		if(maxNumberOfwaypointsGroupPlottingModeRemoval.enumValueIndex != 0)
		{
			EditorGUILayout.PropertyField (maxNumberOfwaypoints, new GUIContent ("Max Number Of Waypoints", "The Maximum number of waypoints at any time. Note: Includes active AND non-active waypoint groups. The minimum allowed value in this field is 3"));
		}
		
		
		if(!Application.isPlaying)
		{
			EditorGUILayout.Separator (); //Add a space between Inspector Items
			EditorGUILayout.PropertyField (storeRotations, new GUIContent ("Track: Rotation", "Store the transfom rotation at point of waypoint creation.") );
			EditorGUILayout.PropertyField (storeScale, new GUIContent ("Track: Scale", "Store the transfom scale at point of waypoint creation.") );
			EditorGUILayout.PropertyField (storeTimeStamp, new GUIContent ("Track: Time", "Store the time elapsed since the start of the scene at point of waypoint creation.") );
		}
		
		
		
		EditorGUILayout.Separator (); //Add a space between Inspector Items
		EditorGUILayout.PropertyField (showGizmos, new GUIContent ("Show Gizmos in Scene Window", "Draw Gizmos in the Scene window to visualize the current waypoints being plotted."));
		EditorGUILayout.EndVertical(); //Stop Surrounding the debug info in a box
		//~~~~~~~~~~~
		
		//Absolute Waypoint		
		EditorGUILayout.Separator (); //Add a space between Inspector Items
		EditorGUILayout.BeginVertical("Box"); //Surround the debug info in a box
		GUILayout.Label("<color=#00bff3>Absolute Waypoint</color>"); //Show Notes label
		EditorGUILayout.Separator (); //Add a space between Inspector Items
		EditorGUILayout.PropertyField (absoluteWaypoint, new GUIContent ("Add Absolute Waypoint", "Creates an additional waypoint at the start (index position [0] in the active waypoint group), and uses the position of the gameObject plus any offset being applied. This is useful to tweak the waypoints local to the gameObject. To tweak even further the next waypoint after that is the offset waypoint (index position [1]), which is where the actual plotting of waypoints takes place, this can also have offsets applied to it.\n\nOn the next plotting cycle the previous absolute waypoint is removed, then the new offset waypoint is added, then a new absolute waypoint is added. This is to maintain waypoint history continuity."));
		EditorGUILayout.PropertyField (absoluteWayPointOffset, new GUIContent ("Absolute Waypoint Offset", "The amount to offset the absolute waypoint by."));
		EditorGUILayout.EndVertical();		
		//~~~~~~~~~~~
		
		//Offset Waypoint		
		EditorGUILayout.Separator (); //Add a space between Inspector Items
		EditorGUILayout.BeginVertical("Box"); //Surround the debug info in a box
		GUILayout.Label("<color=#ed145b>Offset Waypoint</color>"); //Show Notes label
		EditorGUILayout.Separator (); //Add a space between Inspector Items
		
		EditorGUILayout.PropertyField (offsetWaypointPlottingMethod, new GUIContent ("Method of Offset", "Manual: Specify the amount of offset relative to the tracker object.\n\nPercentage: Adjust the amount of distance to offset using a percentage of a min and max Vector3 values. This allows you to associate the offset with another value, for example object speed, power-ups, etc.\n\nActual: Use a GameObject worldspace position for the offset waypoint position.")); 
		if(offsetWaypointPlottingMethod.enumValueIndex == 0)
		{
			EditorGUILayout.PropertyField (offsetManualAmount, new GUIContent ("Manual Offset Amount", "Offset the current waypoint plot relative to the gameObject Z axis. This offset is not applied to the percentage offset option below."));
		}
		if(offsetWaypointPlottingMethod.enumValueIndex == 1)
		{
			EditorGUILayout.Slider (offsetPercentage, 0, 100, new GUIContent ("Offset %", "The percentage of offset to apply. For example if 50% then the offset will be between the min and max offset positions."));
			EditorGUILayout.PropertyField (offsetMinPosition, new GUIContent ("Min % Offset Distance", "Min: The minimum distance of offset to apply.")); 
			EditorGUILayout.PropertyField (offsetMaxPosition, new GUIContent ("Max % Offset Distance", "Max: The maximum distance of offset to apply."));
		}
		if(offsetWaypointPlottingMethod.enumValueIndex == 2)
		{
			EditorGUILayout.PropertyField (offsetWithThisObjectPosition, new GUIContent ("Offset using gameObject Position", "Offset using an object position. For example there may be a rotating element to the player avatar. This will allow the offset to simply follow it. But keeping the absolute waypoint relevant to the tracked object."));
		}
		
		EditorGUILayout.EndVertical(); //Stop Surrounding the debug info in a box
		//~~~~~~~~~~~		
		
		//Waypoint Merging
		EditorGUILayout.Separator (); //Add a space between Inspector Items		
		EditorGUILayout.BeginVertical("Box"); //Surround the debug info in a box
		GUILayout.Label("Waypoint Merging"); //Show Notes label
		EditorGUILayout.Separator (); //Add a space between Inspector Items	
		EditorGUILayout.PropertyField (mergeActiveWaypointGroup, new GUIContent ("Merge Active Waypoint Group", "Enabled: The current active waypoint group will gradually merge using the merge speed settings, starting with the oldest and moving on to the next waypoint in line.\n\nDisabled: No merging will take place and the active waypoint list will continue to grow (Unless you are using Max Waypoints setting).\n\nMerging can also be disabled by setting Merge Speed to 0."));
		EditorGUILayout.PropertyField (activeWaypointGroupMergeSpeed, new GUIContent ("Merge Speed: Active Waypoint Group", "This value applies to only the Active Waypoint group.\n\nThe speed at which the oldest waypoints are merged to the next waypoint in line.\n\nSmaller numbers result in slower merging.\n\nSetting this to 0 will stop waypoint merging, however there is the option above to enable/disable waypoint merging.\n\nIf merging cannot keep up due to settings, the last waypoint in the group will be removed each frame as a last resort.\n\nMerging will cause the position information for the oldest waypoint to be updated as it moves closer to the next waypoint."));
		EditorGUILayout.PropertyField (adjustMergingSpeedWithMergeRatePercentage, new GUIContent ("Merge Active Speed % Adjust", "Enabled: The Merge speed for the active waypoint group can be increased and decreased using a percentage. This allows you to associate the offset with another value, for example object speed, power-ups, etc.\n\nFor example if the active waypoint group merge speed is 1 and the percentage is 10% then merging speed will be 0.1.\n\nDisabled: Merging will use the active waypoint group merge speed value."));
		EditorGUILayout.Slider (activeMergeRatePercentage, 0, 100, new GUIContent ("Merge Rate %", "The percentage of the active waypoint group merge speed to use."));
		
		//Groups is position 0 in the enum
		if(waypointPlottingModes.enumValueIndex == 0)
		{
			EditorGUILayout.Separator (); //Add a space between Inspector Item
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();
			EditorGUILayout.PropertyField (mergeNonActiveWaypointGroups, new GUIContent ("Merge non-active Waypoint Groups", "Enabled: The non-active waypoint groups will all gradually merge at the same speed, starting with the oldest waypoint and moving on to the next waypoint in it's group.\n\nDisabled: No merging will take place and the non-active waypoint groups will persist.\n\nMerging can also be disabled by setting Merge Speed to 0."));
			EditorGUILayout.PropertyField (nonActiveWaypointMergeModes, new GUIContent ("Non-Active Waypoint Group Merging Modes", "Simultaneous: All non-active waypoint groups will merge at the same time.\n\nSequential: All non-active waypoint groups will merge in order, starting with the oldest waypoint group."));
			EditorGUILayout.PropertyField (nonActiveWaypointGroupMergeSpeed, new GUIContent ("Merge Speed: Non-active Waypoint Groups", "This value applies to all non-active waypoint groups.\n\nHow quickly the oldest waypoints are merged to the next waypoint in line.\n\nSmaller numbers result in slower merging.\n\nSetting this to 0 will stop waypoint merging, however there is the option above to enable/disable non-active waypoint group merging.\n\nIf merging cannot keep up due to settings, the last waypoint in the group will be removed each frame as a last resort.\n\nRemember that merging will cause the position information for the oldest waypoint to be updated."));
		}
		EditorGUILayout.EndVertical(); //Stop Surrounding the debug info in a box
		//~~~~~~~~~~~
		
		//WARNINGS			
		//Waypoint tracker warnings
		if(OMT_CS.warnings != "")
		{
			EditorGUILayout.Separator (); //Add a space between Inspector Items
			EditorGUILayout.BeginVertical("TextArea");
			EditorGUILayout.HelpBox(OMT_CS.warnings.ToString(), MessageType.Error, true);
			EditorGUILayout.EndVertical();
		}
		
		//~~~~~~~~~~~	
		//Toggle for Notes
		EditorGUILayout.Separator (); //Add a space between Inspector Items
		if(GUILayout.Button("<b>Notes</b>"))
		{
			showNotes.boolValue = !showNotes.boolValue;
			
		}		
		if(showNotes.boolValue)
		{
			EditorGUILayout.BeginVertical("Box"); //Surround the debug info in a box
			GUILayout.Label("<color=white>Notes</color>"); //Show Notes label
			notes.stringValue = EditorGUILayout.TextArea(notes.stringValue); //Notes text input field
			EditorGUILayout.EndVertical();
		}
		//~~~~~~~~~~~
		
		//DEBUG OPTIONS
		//Toggle for Debug display
		EditorGUILayout.Separator (); //Add a space between Inspector Items
		if(GUILayout.Button("<b>Debug</b>"))
		{
			showDebugInfo.boolValue = !showDebugInfo.boolValue;
		}
		
		if(showDebugInfo.boolValue)
		{
			
			EditorGUILayout.BeginVertical("Box"); //Surround the debug info in a box
			GUILayout.Label("<color=white>Debug</color>"); //Show Notes label
			GUILayout.TextField("Various waypoint related calculations - Useful for Debug/Testing.\nNone of the fields in this section are editable."); //Show Notes label
			EditorGUILayout.Separator (); //Add a space between Inspector Items
			EditorGUILayout.Toggle("Tracking Is Active", OMT_CS.trackingActive);
			EditorGUILayout.Toggle("Plotting Is Active", OMT_CS.plottingActive);
			EditorGUILayout.Vector3Field("Calculated Offset", OMT_CS.offsetResult); 
			EditorGUILayout.LabelField("Waypoint Active Merge Speed", OMT_CS.actualActiveMergeSpeed.ToString());
			EditorGUILayout.LabelField("Waypoint Non-Active Merge Speed", OMT_CS.actualNonActiveMergeSpeed.ToString());
			EditorGUILayout.LabelField("Waypoint Count: Active", OMT_CS.waypointCountActiveTotal.ToString());
			EditorGUILayout.LabelField("Waypoint Count: All Current", OMT_CS.waypointCountCurrentTotal.ToString());
			EditorGUILayout.LabelField("Waypoint Count: Since Start", OMT_CS.waypointTotalSinceStart.ToString());
			
			//Get Waypoint Groups
			var controller = target as OMT_CS;
			EditorGUILayout.Separator (); //Add a space between Inspector Items
			GUILayout.TextField("Waypoints groups should be empty (0) before start!\nYou may get errors if already poplulated on start."); //Show Notes label
			SerializedProperty waypointsList = obj.FindProperty ("waypointGroups");
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(waypointsList, true);

			if(EditorGUI.EndChangeCheck())
				obj.ApplyModifiedProperties();
			EditorGUIUtility.LookLikeControls();
			EditorGUILayout.EndVertical();
		}
		
		EditorGUILayout.Separator (); //Add a space between Inspector Items	
		obj.ApplyModifiedProperties (); //Update the serialized object data 
	}
}
