using UnityEngine;
using System.Collections;

public class MoveTo : MonoBehaviour {

	public float rotationSpeed = 10f;
	private NavMeshAgent agent;
	private GameObject target;
	private bool moving = false;
	private bool readyToLook = false;

	// Use this for initialization
	void Awake () {
		agent = GetComponent<NavMeshAgent>();
	}

	void Update() {
		if (moving) {
			checkMovement();
		}
		if (readyToLook) {
			StartCoroutine(rotateTowardsTarget());
		}
	}

	void checkMovement() {
		if (!agent.pathPending) {
			if (agent.remainingDistance <= agent.stoppingDistance) {
				if (agent.hasPath || agent.velocity.sqrMagnitude == 0f) {
					moving = false;
					readyToLook = true;
				}
			}
		}
	}

	public void SetDestination(GameObject end) {
		this.target = end;
		agent.updateRotation = true;
        //agent.updatePosition = false;
		this.agent.destination = this.target.transform.position;
		moving = true;
	}

	private IEnumerator rotateTowardsTarget() {
		agent.updateRotation = false;
		Transform lookPos = target.transform.GetChild(0);
		Vector3 direction = (lookPos.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(direction);
		//transform.rotation = lookRotation;
		while (Quaternion.Angle(transform.rotation, lookRotation) > 1) {
			transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
			yield return new WaitForEndOfFrame();
		}
		readyToLook = false;
	}

	public bool IsMoving() {
		return moving;
	}

	public bool IsReadyToLook() {
		return readyToLook;
	}
}
