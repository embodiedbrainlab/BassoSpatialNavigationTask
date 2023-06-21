using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OMTParticles_CS : MonoBehaviour {
/*

Generate Shuriken Particles along the length of a waypoint group in different ways

FullLength: Generate particles at every waypoint int the waypoint group
PingPong : Generate particles sequentialy along the waypoints int the waypoint group - NOTE: Waypoint merging at a high speed will mess with this!
StartEnd: Emit particles at the first and last index of the waypoint group
Emit(position: Vector3, velocity: Vector3, size: float, lifetime: float, color: Color32): void;

*/

	public OMT_CS omtComponent;									//Object Motion Tracker
	public ParticleSystem particleComponent;					//Target Particle System
	
	public enum particleDisplayMethodTypes {FullLength, PingPong, StartEnd};
	public particleDisplayMethodTypes particleDisplayMethod;		//Selectable particle display methods
	
	public Vector3 particleVelocity;							//Sets the direction and speed of the particle when emitted
	public float particleSize;									//Sets the particle size
	public float particleLifetime;								//Sets the lifetime of the particle
	public Color32 particleColor;								//Sets particle color
	public Material particleMaterial;							//Sets particle Material
	public float pingpongDelay;									//Delay each pingpong step by a set time
	public List<ParticlePingPongClass> pingPongCountList = new List<ParticlePingPongClass>();	//List to control/monitor the pingpong setup for each waypoint group
	
	
	public class ParticlePingPongClass											
	{
		public string waypointGroupID;							//Used to keep track of the correct waypoint group											
		public int pingPongCurrent;								//Current waypointPosition index to use							
		public int pingPongCount;								//Mathf pingpong counting									
	}

	//--------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------

	void Start ()
	{
		StartCoroutine("StartEndParticles");
		StartCoroutine("FullLengthParticles");
		StartCoroutine("PingPongParticles");
		particleComponent.GetComponent<Renderer>().material = particleMaterial;	//Set material to be used for particles
	}

	//--------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------

	IEnumerator StartEndParticles ()
	{
		while(true)
		{
			if(particleDisplayMethod == particleDisplayMethodTypes.StartEnd)
			{
				for(int i = 0; i < omtComponent.waypointGroups.Count; i++)
				{
					//Create particles at first index
					GetComponent<ParticleSystem>().Emit(omtComponent.waypointGroups[i].waypointPosition[0], particleVelocity, particleSize, particleLifetime, particleColor);
					
					//Create particles at last index
					GetComponent<ParticleSystem>().Emit(omtComponent.waypointGroups[i].waypointPosition[omtComponent.waypointGroups[i].waypointPosition.Count - 1], particleVelocity, particleSize, particleLifetime, particleColor);
				}
			}
			yield return null;
		}  
	}
	
	//--------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------
	
	IEnumerator FullLengthParticles ()
	{
		while(true)
		{
			if(particleDisplayMethod == particleDisplayMethodTypes.FullLength)
			{
				for(int i = 0; i < omtComponent.waypointGroups.Count; i++)
				{
					for(int ii = 0; ii < omtComponent.waypointGroups[i].waypointPosition.Count; ii++)
					{
						GetComponent<ParticleSystem>().Emit(omtComponent.waypointGroups[i].waypointPosition[ii], particleVelocity, particleSize, particleLifetime, particleColor);
					}
				}
			}
			yield return null;
		}  
	}
	
	//--------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------

	IEnumerator PingPongParticles ()
	{
		while(true)
		{
			if(particleDisplayMethod == particleDisplayMethodTypes.PingPong)
			{
				yield return StartCoroutine("PingPongWaypointGroupsMonitor");							//Get current status of waypoint groups
				
				for(int i = 0; i < omtComponent.waypointGroups.Count; i++)
				{
					if(omtComponent.waypointGroups[i].id == pingPongCountList[i].waypointGroupID)
					{ 
						if(omtComponent.waypointGroups[i].waypointPosition.Count-1 > 0)
						{
							//Set pingpong count then emit particles at that position
							pingPongCountList[i].pingPongCount = (int) Mathf.PingPong (pingPongCountList[i].pingPongCurrent++, omtComponent.waypointGroups[i].waypointPosition.Count -1); 
							GetComponent<ParticleSystem>().Emit(omtComponent.waypointGroups[i].waypointPosition[pingPongCountList[i].pingPongCount], particleVelocity, particleSize, particleLifetime, particleColor);
						}
					}
				}
			}
		yield return new WaitForSeconds(pingpongDelay);
		}  
	}
	
	//--------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------
	
	void PingPongWaypointGroupsMonitor ()
	{
		//Check for pingpong class that refers to destroyed waypoint groups
		for(int iii = 0; iii < pingPongCountList.Count; iii++)
		{		    	
			//Reset for next cycle
			var idMatchFound = false;													
			
			for(int iiii = 0; iiii < omtComponent.waypointGroups.Count; iiii++)
			{
				//If names match then set to true so that it is ignored
				if(omtComponent.waypointGroups[iiii].id == pingPongCountList[iii].waypointGroupID)
				{
					//ID MATCH Found!
					idMatchFound = true;
				}
			}
			
			//If no ID match was found because the waypoint group has been destroyed, remove from the pingPongCountList	
			if(idMatchFound == false)
			{
				//ID MATCH NOT Found - Removing!
				pingPongCountList.RemoveAt(iii);
			}
		}
		
		//Check for new waypoint groups and add if needed
		for(int i = 0; i < omtComponent.waypointGroups.Count; i++)
		{		    	
			//Reset for next cycle
			var idFound = false;
			
			for(int ii = 0; ii < pingPongCountList.Count; ii++)
			{
				//If match found then no need to add a new pingpong
				if(pingPongCountList[ii].waypointGroupID == omtComponent.waypointGroups[i].id)
				{
					//ID Found!
					idFound = true;
				}
			}
			
			//If waypoint ID has not been found in the pingpong list then add
			if(idFound == false)
			{
				//ID NOT Found - Creating!
				ParticlePingPongClass createParticlePingPongList = new ParticlePingPongClass();						
				createParticlePingPongList.waypointGroupID = omtComponent.waypointGroups[i].id;					
				createParticlePingPongList.pingPongCurrent = 0;
				createParticlePingPongList.pingPongCount = 0;								
				pingPongCountList.Add(createParticlePingPongList);	
			}
		}
	}
}
