using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LibraryCommands : MonoBehaviour {

	public UnityEngine.UI.InputField pointToDirectory;
	public GameObject pointToDirectoryObj;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown ("tab")) {
			if (pointToDirectoryObj.activeSelf) { //if input field already active
				pointToDirectory.DeactivateInputField();
				pointToDirectoryObj.SetActive(false);
			}
			else {
				pointToDirectory.ActivateInputField();
				pointToDirectoryObj.SetActive(true);
			}
		}	

	}
}
