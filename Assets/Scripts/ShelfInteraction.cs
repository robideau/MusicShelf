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
	public GameObject artistFolderPrefab;
	public GameObject artistFolderContainer;
	public Shader GUI3DTextShader;
	public float boxSpeed;
	private GameObject interactable;

	private bool boxIsMoving = false;
	private Vector3 boxEndPosition;
	private Vector3 boxOriginalPosition;
	private bool boxIsOpen = false;

	// Use this for initialization
	void Start () {

		initializeContainerReferences ();
		interactable = new GameObject ();
		interactable.transform.parent = new GameObject ().transform;
		boxEndPosition = new Vector3 (0, 0, 0);

	}
	
	// Update is called once per frame
	void Update () {
	
		if (boxIsMoving) {
			moveBox(interactable.transform.parent.gameObject, boxEndPosition);
		}

		if (Input.GetKeyDown ("x") && !boxIsMoving) {
			boxIsMoving = true;
			boxEndPosition = boxOriginalPosition;
		}

		if (interactable.transform.parent.transform.localPosition == boxEndPosition) {
			boxIsOpen = false;
		}

	}

	void initializeContainerReferences() {
		alphabeticalContainers = libraryController.getAlphabeticalTitles ();
	}

	public void interactableSelect(GameObject interactable) { //reactions to different menu selections
		//string interactableName = interactable.name.Substring (4, 1); //reduce to name minus "Box "
		this.interactable = interactable;
		Transform interactableTransform = interactable.transform.parent;
		string interactableName = interactableTransform.gameObject.name.Substring(4,1);
		if (interactableName.Length == 1) { //if alphabetical box is selected
			for (int i = 0; i < alphabeticalContainers.Length; i++) {
				if(alphabeticalContainers.GetValue(i).ToString().Substring(5,1).ToLower() == interactableName) { //connect box to appropriate alphabetical holder
					//do something - open menu? "slide" container out? show albums/songs?
					GameObject alphabeticalContainer = (GameObject)alphabeticalContainers.GetValue(i);

					pullArtists(alphabeticalContainer, interactable);


					//For testing purposes, load first song from box and prepare to pass to visualizer
					//GameObject alphabeticalContainer = (GameObject)alphabeticalContainers.GetValue(i);
					/*Transform selectedSongTransform = alphabeticalContainer.transform.GetChild(0).GetChild(0).GetChild(0);
					GameObject selectedSong = selectedSongTransform.gameObject;
					PreserveData p = selectedSongInfo.GetComponent<PreserveData>();
					p.path = selectedSong.name;
					Application.LoadLevel("ParticleRoom");*/
				}
			}
		}
	}

	public void pullArtists(GameObject alphabeticalContainer, GameObject interactable) {

		GameObject artistFolderContainerOriginal = artistFolderContainer;

		artistFolderContainer.transform.position = interactable.transform.position;

		foreach (Transform artist in alphabeticalContainer.transform) {

			//Create a new folder with the appropriate label
			GameObject newArtistFolder = Instantiate (artistFolderPrefab);
			newArtistFolder.GetComponentInChildren<TextMesh> ().text = artist.gameObject.name.Trim ();
			newArtistFolder.GetComponentInChildren<TextMesh> ().fontSize = 90 - artist.gameObject.name.Length;
			newArtistFolder.GetComponentInChildren<MeshRenderer> ().material.shader = GUI3DTextShader;

			//Place folder in appropriate shelf
			newArtistFolder.transform.SetParent (artistFolderContainer.transform);
			newArtistFolder.transform.position = artistFolderContainer.transform.position;
			newArtistFolder.transform.rotation = interactable.transform.rotation;
			newArtistFolder.name = artist.gameObject.name;

		}

		//Slide box out
		if (!boxIsOpen && !boxIsMoving) {
			Transform boxOriginalTransform = interactable.transform.parent.transform;
			boxOriginalPosition = boxOriginalTransform.localPosition;
			float currentRotation = (270 - (interactable.transform.parent.transform.eulerAngles.y * Mathf.PI)) / 180; //Mathf.Sin() and Mathf.Cos() work with radians
			Vector3 boxEndPosition = new Vector3 (boxOriginalTransform.localPosition.x - (Mathf.Sin (currentRotation)), boxOriginalTransform.localPosition.y, boxOriginalTransform.localPosition.z + (Mathf.Cos (currentRotation)));
			this.boxEndPosition = boxEndPosition;
			boxIsMoving = true;
			boxIsOpen = true;
		}
		//Pull folder out and place in front of camera

		//Wait for artist selection or cancel
		//bool artistSelected = false;
		//raycaster.enabled = false;
		/*while (!artistSelected) {
			if (Input.GetKeyDown ("x")) {
				artistSelected = true;
				raycaster.enabled = true;
				//Set box back to original position
				//Reset artist folder container
			}

		}*/





	}

	public void moveBox(GameObject box, Vector3 boxEndPosition) {
		if (box.transform.localPosition == boxEndPosition) {
			boxIsMoving = false;
		} else {
			box.transform.localPosition = Vector3.Lerp (box.transform.localPosition, boxEndPosition, Time.deltaTime * boxSpeed * 2);
		}
	}
}
