using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ShelfInteraction : MonoBehaviour {

	public GameObject shelves;
	public GameObject[] alphabeticalContainers;
	public LibraryController libraryController;
	public Raycaster raycaster;
	public ShelfGenerator shelfgenerator;
	public GameObject selectedSongInfo;

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
		//string interactableName = interactable.name.Substring (4, 1); //reduce to name minus "Box "
		Transform interactableTransform = interactable.transform.parent;
		string interactableName = interactableTransform.gameObject.name.Substring(4,1);
		if (interactableName.Length == 1) { //if alphabetical box is selected
			for (int i = 0; i < alphabeticalContainers.Length; i++) {
				if(alphabeticalContainers.GetValue(i).ToString().Substring(5,1).ToLower() == interactableName) { //connect box to appropriate alphabetical holder
					//do something - open menu? "slide" container out? show albums/songs?




					//For testing purposes, load first song from box and prepare to pass to visualizer
					GameObject alphabeticalContainer = (GameObject)alphabeticalContainers.GetValue(i);
					Transform selectedSongTransform = alphabeticalContainer.transform.GetChild(0);
					GameObject selectedSong = selectedSongTransform.gameObject;
					PreserveData p = selectedSongInfo.GetComponent<PreserveData>();
					p.path = selectedSong.name;
					Application.LoadLevel("ParticleRoom");
				}
			}
		}
	}
}
