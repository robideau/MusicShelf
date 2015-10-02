using UnityEngine;
using System.Collections;

public class ShelfInteraction : MonoBehaviour {

	public GameObject shelves;
	public GameObject[] alphabeticalContainers;
	public LibraryController libraryController;
	public Raycaster raycaster;
	public ShelfGenerator shelfgenerator;

	// Use this for initialization
	void Start () {

		initializeContainerReferences ();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void initializeContainerReferences() {
		alphabeticalContainers = libraryController.getAlphabeticalTitles ();
	}

	public void interactableSelect(GameObject interactable) { //reactions to different menu selections
		string interactableName = interactable.name.Substring (4, 1); //reduce to name minus "Box "
		if (interactableName.Length == 1) { //if alphabetical box is selected
			for (int i = 0; i < alphabeticalContainers.Length; i++) {
				if(alphabeticalContainers.GetValue(i).ToString().Substring(5,1).ToLower() == interactableName) { //connect box to appropriate alphabetical holder
					//do something - open menu? "slide" container out? show albums/songs?

				}
			}
		}
	}
}
