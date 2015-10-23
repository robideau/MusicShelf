//David Robideau
//
//A simple raycaster script - sends out rays in 3D space and detects any colliders
//Returns the name of the collider and changes material color

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Raycaster : MonoBehaviour {

	public ShelfInteraction shelfInteraction;
	RaycastHit hit;	//potential raycaster hits
	Ray ray; //initial ray
	Renderer pastRenderer; //used to return objects to original color - must be initialized
	bool highlighterStarted = false; //determines if there is an original color to return to yet - only used once
	Color original; //stores objects' original colors
	public UnityEngine.UI.Text selectedText;
	public OVRCameraRig playerCameraRig;

	// Use this for initialization
	void Start () {
		pastRenderer = null; //placeholder past renderer
		original = Color.white; //placeholder color
	}
	
	// Update is called once per frame
	void Update () {

		/* DEBUG */
		/*Vector3 forwardRay = transform.TransformDirection (Vector3.forward);

		if (Physics.Raycast (transform.position, forwardRay, 10)) {
			print ("Something in front of object");
		}*/

		//Vector3 cameraRay = new Vector3 (.5f, .5f, 0f);

		//ray = Camera.main.ViewportPointToRay (cameraRay); //focuses on center of camera (mouse-based)
		ray = new Ray(playerCameraRig.centerEyeAnchor.transform.position, playerCameraRig.centerEyeAnchor.transform.forward);

		if (Physics.Raycast (ray, out hit) && hit.collider.gameObject.tag == "Interactable") { //if interactable object detected

			if (highlighterStarted) { //if other objects have been detected
				pastRenderer.material.color = original; //reset previous object's original color
			}

			//Object hitObj = hit.collider.gameObject; 
			//string hitObjName = hitObj.name;

			Renderer hitRenderer = hit.collider.GetComponent<Renderer>();
			pastRenderer = hit.collider.GetComponent<Renderer>();
			original = hit.collider.GetComponent<Renderer>().material.color; //store object's original color
			hitRenderer.material.color = ToColor (16761477); //highlight focused object

			if(Input.GetKeyDown("f")) {
				shelfInteraction.interactableSelect(hit.collider.gameObject);
			}

			//DEBUG
			//print (hitObjName);

			//selectedText.text = "Selected box: " + hitObjName;

			highlighterStarted = true; //allow other objects' original colors to be restored

		}
	}

	public Color32 ToColor(int HexVal)
	{
		byte R = (byte)((HexVal >> 16) & 0xFF);
		byte G = (byte)((HexVal >> 8) & 0xFF);
		byte B = (byte)((HexVal) & 0xFF);
		return new Color32(R, G, B, 255);
	}
}
