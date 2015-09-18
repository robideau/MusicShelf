//David Robideau
//
//A simple raycaster script - sends out rays in 3D space and detects any colliders
//Returns the name of the collider and changes material color

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Raycaster : MonoBehaviour {

	RaycastHit hit;	//potential raycaster hits
	Ray ray; //initial ray
	Renderer pastRenderer; //used to return objects to original color - must be initialized
	bool highlighterStarted = false; //determines if there is an original color to return to yet - only used once
	Color original; //stores objects' original colors
	public UnityEngine.UI.Text selectedText;

	// Use this for initialization
	void Start () {
		pastRenderer = null; //placeholder past renderer
		original = Color.white; //placeholder color
	}
	
	// Update is called once per frame
	void Update () {

		/* USE FOR DEBUG */
		/*Vector3 forwardRay = transform.TransformDirection (Vector3.forward);

		if (Physics.Raycast (transform.position, forwardRay, 10)) {
			print ("Something in front of object");
		}*/


		ray = Camera.main.ScreenPointToRay (Input.mousePosition); //focuses on center of camera

		if (Physics.Raycast (ray, out hit) && hit.collider.gameObject.tag == "Interactable") { //if interactable object detected

			if (highlighterStarted) { //if other objects have been detected
				pastRenderer.material.color = original; //reset previous object's original color
			}

			Object hitObj = hit.collider.gameObject; 
			string hitObjName = hitObj.name;

			Renderer hitRenderer = hit.collider.GetComponent<Renderer>();
			pastRenderer = hit.collider.GetComponent<Renderer>();
			original = hit.collider.GetComponent<Renderer>().material.color; //store object's original color
			hitRenderer.material.color = Color.green; //highlight focused object

			//FOR DEBUG
			//print (hitObjName);

			selectedText.text = "Selected box: " + hitObjName;

			highlighterStarted = true; //allow other objects' original colors to be restored

		}
	}
}
