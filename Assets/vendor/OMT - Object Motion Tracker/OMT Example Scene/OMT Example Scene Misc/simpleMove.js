#pragma strict

var lookSpeed : float = 5.0;
var moveSpeed : float = 10.0;
var rotationX : float = 0.0;
var rotationY : float = 0.0;
var mouseLockTabKey : boolean = false;
var mouseHiddenF1Key : boolean = false;

function Update(){

	//Enable / Disable Mouse look lock
	if(Input.GetKeyDown(KeyCode.Tab)) {

			mouseLockTabKey = !mouseLockTabKey;
		}
			
	
	if(mouseLockTabKey == false) {
	
	//If mouseLook lock is not enabled
	rotationX += Input.GetAxis("Mouse X")*lookSpeed;
    rotationY += Input.GetAxis("Mouse Y")*lookSpeed;
    
     
    }

    	//Detects F1 Key on downward press
		if(Input.GetKeyDown(KeyCode.F1)) {
			
			//Check the mouse setting variable
			if(mouseHiddenF1Key == true) {
			
				Cursor.visible = true;
				Screen.lockCursor = false;
				mouseHiddenF1Key = false;
					
			} else {
		
				Cursor.visible = false; 
				Screen.lockCursor = true;
				mouseHiddenF1Key = true;
		
			}
		}
		
    rotationY = Mathf.Clamp (rotationY, -90, 90);

    transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
    transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
    
    transform.position += transform.forward*moveSpeed*Input.GetAxis("Vertical")*Time.deltaTime;
    transform.position += transform.right*moveSpeed*Input.GetAxis("Horizontal")*Time.deltaTime;
    
    var vDir: float = 0;
    
    if (Input.GetKey("q")) vDir = 1;
    if (Input.GetKey("e")) vDir = -1;
    	transform.position += transform.up*moveSpeed*vDir*Time.deltaTime;
	}
