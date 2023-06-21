using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using System.Collections;
using System.Text.RegularExpressions;

public class controller : MonoBehaviour {

    // :vomit: I put a large amount of the task in one file.

	public int numberOfTargets;
	public GameObject[] Waypoints;
    public GameObject controlHint;
	public GameObject introPanel;
    public Text introText;
	public GameObject locationPanel;
	public Text locationText;
    public GameObject revisitPanel;
    public Text revisitText;
    public GameObject imagePanel;
    public Image deliveryImage;
    public GameObject recallPanel;
    public Text recallLocationText;
    public InputField recallLocationField;
    public InputField recallItemField;
    public GameObject wrapUpPanel;
    public Text wrapUpText;

    public GameObject ArrowPrefab;
    public AudioSource beepSource;

	private MoveTo Movement;
	private System.Random rng;
	private GameObject currentWaypoint;
	private bool readyToStartTour;
	private bool readyForNextWaypoint;
    private bool readyToBeginPhase = false;
    private float timer;
    private float t_revisitPhaseStart;
    private SpatialNavNode[] dataArray;
    private uint dataIndex = 0;
    private Recall[] responseArray;

    private GameObject[] prefabList;

	// Use this for initialization
	void Start () {
		rng = new System.Random();
		Movement = GetComponent<MoveTo>();
		// Shuffle waypoints. We will only be iterating over `numberOfTargets` members of the WP array
		Snippets.FisherYates<GameObject>(rng, Waypoints);
        dataArray = new SpatialNavNode[numberOfTargets];
        responseArray = new Recall[numberOfTargets];
		readyForNextWaypoint = true;
        StartCoroutine(BeginTask());
	}

	IEnumerator BeginTask() {
		readyToBeginPhase = false;
		locationPanel.SetActive(false);
        controlHint.SetActive(true);
        while (!readyToBeginPhase) {
			yield return new WaitForEndOfFrame();
		}
		introPanel.SetActive(false);
		StartCoroutine(SelfTour());
	}

	IEnumerator Tour() {
		int count = 0;
		introPanel.SetActive(false);
		// For each waypoint up until `numberOfTargets`
		while (count <= numberOfTargets) {
			if (readyForNextWaypoint && (count != numberOfTargets)) {
				locationPanel.SetActive(false);
				currentWaypoint = Waypoints[count];
				Movement.SetDestination(currentWaypoint);
				readyForNextWaypoint = false;
				count++;
			} else if (readyForNextWaypoint)
            {
                count++;
            }
			if (!Movement.IsMoving() && !Movement.IsReadyToLook()) {
				// Has reached and looked at location
				// Ready for UI display
				SetAndDisplayLocationUI();
			}
			yield return new WaitForEndOfFrame();
		}
        locationPanel.SetActive(false);
        StartCoroutine(RevisitIntermission());
    }

    IEnumerator SelfTour()
    {
        int count = 0;
        bool refresh = false;
        introPanel.SetActive(false);
        LineRenderer line = this.GetComponent<LineRenderer>();
        FirstPersonController fpc = GetComponent<FirstPersonController>();
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        NavMeshPath p = new NavMeshPath();

        fpc.enabled = true;
        agent.enabled = true;
        agent.updatePosition = false;
        agent.updateRotation = false;
        readyForNextWaypoint = true;

        enableTargets();
        while (count <= numberOfTargets)
        { 
            if (readyForNextWaypoint)
            {
                currentWaypoint = Waypoints[count];
                agent.Warp(this.gameObject.transform.position);
                agent.CalculatePath(currentWaypoint.transform.position, p);
                readyForNextWaypoint = false;
                count++;
                refresh = true;
            }
            if (refresh)
            {
                if (!agent.pathPending)
                {
                    Debug.Log("Drawing path...");
                    // Clear old prefabs from prefabList, if any
                    if(prefabList != null)
                    {
                        foreach (GameObject go in prefabList)
                        {
                            Destroy(go);
                        }
                    }

                    line.SetVertexCount(p.corners.Length);
                    prefabList = new GameObject[p.corners.Length];

                    Vector3[] varr = new Vector3[p.corners.Length];
                    // Create Vector List
                    for (int i = 0; i < p.corners.Length; i++)
                    {
                        Vector3 tempPos = new Vector3(p.corners[i].x, 1, p.corners[i].z);
                        varr[i] = tempPos;
                    }
                    line.SetPositions(varr);

                    // Instiate ArrowPrefabs
                    for (int i = 0; i < p.corners.Length - 1; i++)
                    {
                        GameObject arrow = (GameObject)Instantiate(ArrowPrefab, varr[i], Arrow.GetRotation(varr[i+1],varr[i]));
                        prefabList[i] = arrow;
                        Arrow a = arrow.GetComponent<Arrow>();
                        a.begin = varr[i];
                        a.end = varr[i + 1];
                    }
                    //Transform childTrans = currentWaypoint.transform.GetChild(1);
                    //Vector3 tempPos2 = new Vector3(childTrans.position.x, 1, childTrans.position.z);
                    //line.SetPosition(p.corners.Length, tempPos2);
                    refresh = false;
                }
                else {
                    Debug.Log("Path pending...");
                }
                SetAndDisplayLocationUI();
            }
            yield return new WaitForEndOfFrame();
        }
        locationPanel.SetActive(false);
        line.enabled = false;
        // Clean-up prefabs
        if (prefabList != null)
        {
            foreach (GameObject go in prefabList)
            {
                Destroy(go);
            }
        }
        StartCoroutine(RevisitIntermission());
    }

    IEnumerator RevisitIntermission()
    {
        //revisitPhaseStart = Time.time;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GetComponent<FirstPersonController>().enabled = false;
        readyToBeginPhase = false;
		introText.text = "You will now revisit the landmarks in the order they were previously shown. Please review the control hint at the corner of the screen.\r\n\r\nYou will now recall the landmarks you visited and the items you delivered in the order previously shown.\r\n";
        introPanel.SetActive(true);
        controlHint.SetActive(true);
        revisitPanel.SetActive(true);
        while (!readyToBeginPhase) {
            yield return new WaitForEndOfFrame();
        }
        introPanel.SetActive(false);
        StartCoroutine(RevisitPhase());
    }

    IEnumerator RevisitPhase()
    {
        t_revisitPhaseStart = Time.time;
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        NavMeshPath currentPath = new NavMeshPath();
        FirstPersonController fpc = GetComponent<FirstPersonController>();
        OMT_CS motionTracker = fpc.GetComponent<OMT_CS>();

        fpc.enabled = true;
        motionTracker.trackingActive = true;
        agent.updatePosition = false;
        agent.updateRotation = false;
        this.transform.rotation = Quaternion.identity;

        enableTargets();

        currentWaypoint = Waypoints[dataIndex];
        agent.CalculatePath(currentWaypoint.transform.position, currentPath);
        //enableTarget(currentWaypoint);
        
        SetDestinationUI();
        imagePanel.SetActive(true);
        deliveryImage.sprite = currentWaypoint.GetComponent<DeliveryItem>().itemImage;
        readyForNextWaypoint = false;
        while (dataIndex < numberOfTargets)
        {
            if (readyForNextWaypoint)
            {
                addSpatialNavNode(currentPath);
                //disableTarget(currentWaypoint);
                currentWaypoint = Waypoints[dataIndex];
                agent.CalculatePath(currentWaypoint.transform.position, currentPath);
                //enableTarget(currentWaypoint);
                SetDestinationUI();
                deliveryImage.sprite = currentWaypoint.GetComponent<DeliveryItem>().itemImage;
                readyForNextWaypoint = false;
            }
            yield return new WaitForEndOfFrame();
        }
        disableTargets();
        motionTracker.trackingActive = false;
        imagePanel.SetActive(false);
        StartCoroutine(RecallPhaseIntermission());
    }

    IEnumerator RecallPhaseIntermission()
    {
        FirstPersonController fpc = GetComponent<FirstPersonController>();
        fpc.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        readyToBeginPhase = false;
        introText.text = "Now, you will be asked to recall the locations you visited and the items you delivered in the order they were given to you.";
        introPanel.SetActive(true);
        controlHint.SetActive(false);
        revisitPanel.SetActive(false);
        while (!readyToBeginPhase)
        {
            yield return new WaitForEndOfFrame();
        }
        introPanel.SetActive(false);
        StartCoroutine(RecallPhase());
    }

    IEnumerator RecallPhase()
    {
        int count = 0;
        recallLocationField.onValidateInput += delegate (string input, int charIndex, char addedChar) { return inputValidate(addedChar); };
        recallItemField.onValidateInput += delegate (string input, int charIndex, char addedChar) { return inputValidate(addedChar); };
        float timeAtLastSubmit = Time.time;
        readyToBeginPhase = false; // Reusing this for each submission step
        recallPanel.SetActive(true);
        while (count < numberOfTargets)
        {
            recallLocationText.text = "What was location " + (count+1) + "?";
            if (readyToBeginPhase && count != numberOfTargets)
            {
                readyToBeginPhase = false;
                responseArray[count] = new Recall(recallLocationField.text.Trim(), recallItemField.text.Trim(), Time.time - timeAtLastSubmit);
                timeAtLastSubmit = Time.time;
                count++;
                recallLocationField.text = "";
                recallItemField.text = "";
            } else if (readyToBeginPhase)
            {
                count++;
            }
            yield return new WaitForEndOfFrame();
        }
        //Finish task;
        recallPanel.SetActive(false);
        StartCoroutine(WrapUpPhase());
    }

    IEnumerator WrapUpPhase()
    {
        wrapUpPanel.SetActive(true);
        FirstPersonController fpc = GetComponent<FirstPersonController>();
        OMT_CS motionTracker = fpc.GetComponent<OMT_CS>();
        //JSONObject json = JSONWriter.buildJSON(dataArray, t_revisitPhaseStart, responseArray, motionTracker);
        //string jsonString = json.Print() + ";";
		CSVWriter.writeCSV (dataArray, t_revisitPhaseStart, responseArray, motionTracker);
        //Debug.Log(jsonString);
        //byte[] enc = easy.Crypto.encrypt(jsonString, "7SpBUI73CFK03iUBY8G2dX3eTXd1AU92");
        //Application.ExternalCall("setAndDisplayEnc", System.Convert.ToBase64String(enc));
        UnityEngine.SceneManagement.SceneManager.LoadScene("LeFin");
        //wrapUpText.text = "Your data is ready! If it didn't automatically appear, manually engage the slide-up from the webpage controls. Do not close this window until the data is properly submitted!";

        yield return new WaitForEndOfFrame();
    }


    // New validation for InputField
    char inputValidate(char chr)
    {
        if ((chr < 'a' || chr > 'z') && (chr < 'A' || chr > 'Z') && (chr < '0' || chr > '9') && (chr != ' '))
        {
            chr = '\0';
        }
        return chr;
    }


	public void SetReadyForNext(bool v) {
		readyForNextWaypoint = v;
	}

	public void SetReadyToBeginPhase(bool v) {
		readyToBeginPhase = v;
	}

	public void SetAndDisplayLocationUI() {
		locationText.text = currentWaypoint.transform.name;
		locationPanel.SetActive(true);
	}

    public void SetDestinationUI()
    {
        revisitText.text = "Please deliver the " + currentWaypoint.GetComponent<DeliveryItem>().itemName + " to the " + currentWaypoint.transform.name;
    }

    // Adds a node to the data array and increments dataIndex counter
    public void addSpatialNavNode(NavMeshPath path)
    {
        dataArray[dataIndex] = new SpatialNavNode(currentWaypoint, path, Time.time - t_revisitPhaseStart, currentWaypoint.GetComponent<DeliveryItem>().itemName);
        //Debug.Log(dataArray[dataIndex].ToString());
        dataIndex++;
    }

    public void enableTarget(GameObject wp)
    {
        GameObject target = wp.transform.GetChild(1).gameObject;
        target.SetActive(true);
    }

    public void disableTarget(GameObject wp)
    {
        GameObject target = wp.transform.GetChild(1).gameObject;
        target.SetActive(false);
    }

    public void enableTargets()
    {
        foreach (GameObject wp in Waypoints)
        {
            wp.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    public void disableTargets()
    {
        foreach (GameObject wp in Waypoints)
        {
            wp.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void triggerWaypointEntered(GameObject obj)
    {
        if (obj.GetInstanceID() == currentWaypoint.GetInstanceID())
        {
            beepSource.Play();
            SetReadyForNext(true);
        }
    }
}

public class SpatialNavNode
{
    public GameObject waypoint;
    public NavMeshPath path;
    public string item;
    public float arrivalTime;

    public SpatialNavNode(GameObject wp, string i)
    {
        this.waypoint = wp;
        this.arrivalTime = Time.time;
        this.item = i;
    }

    public SpatialNavNode(GameObject wp, float t, string i)
    {
        this.waypoint = wp;
        this.arrivalTime = t;
        this.item = i;
    }

    public SpatialNavNode(GameObject wp, NavMeshPath p, string i)
    {
        this.waypoint = wp;
        this.arrivalTime = Time.time;
        this.path = p;
        this.item = i;
    }

    public SpatialNavNode(GameObject wp, NavMeshPath p, float t, string i)
    {
        this.waypoint = wp;
        this.arrivalTime = t;
        this.path = p;
        this.item = i;
    }

    public override string ToString()
    {
        return waypoint.transform.name + " " + arrivalTime + " " + item;
    }
}

public class Recall
{
    public string location;
    public string item;
    public float time;

    public Recall(string l, string i, float t)
    {
        this.location = l;
        this.item = i;
        this.time = t;
    }
}
