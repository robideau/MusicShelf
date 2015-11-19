using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Id3Lib;
using Mp3Lib;

public class ShelfInteraction : MonoBehaviour {

	public OVRCameraRig playerCamRig;
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
	public float folderSpeed;
	private GameObject interactable;

	private bool boxIsMoving = false;
	private Vector3 boxEndPosition;
	private Vector3 boxOriginalPosition;
	private bool boxIsOpen = false;

	private bool waitingForArtistSelection = false;
	private bool waitingForAlbumSelection = false;

	private Transform selectedArtist;
	private Transform selectedArtistContainer;
	private Transform selectedAlbum;

	private bool folderIsMoving = false;
	private bool foldersOut = false;
	private bool foldersLifted = false;
	private bool artistSelected = false;

	public float albumSpeed;
	private bool albumsOut = false;
	private bool albumLiftingUp = false;
	private bool albumLiftingDown = false;
	private bool albumSelected = false;

	private bool songSelected = false;
	private bool songsOut = false;
	GameObject[] songHolders = new GameObject[100];
	private int selectedSong = 0;
	private int songIndex;

	public Vector3[] folderOriginalPositions;
	public Vector3[] folderMidPositions;
	public Vector3[] folderEndPositions;
	public Vector3[] albumEndPositions;

	private Vector3 scrollTarget = new Vector3(0, 0, 0);

	private int previouslySelectedAlbumIndex;
	private Transform previouslySelectedAlbum;
	private GameObject alphabeticalContainer;
	private int selectedFolder = 0;
	private int selectedAlbumIndex = 0;
	private Color32 folderDefaultColor = Color.white;

	// Use this for initialization
	void Start () {

		folderOriginalPositions = new Vector3[1000];
		folderMidPositions = new Vector3[1000];
		folderEndPositions = new Vector3[1000];
		albumEndPositions = new Vector3[1000];
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

		if (folderIsMoving && !foldersOut) {
			int folderIndex = 0;
			foreach (Transform artistFolder in artistFolderContainer.transform) {
				moveFolder(artistFolder.gameObject, folderEndPositions[folderIndex]);
				folderIndex++;
			}
		}

		if (folderIsMoving && foldersOut) {
			int folderIndex = 0;
			foreach (Transform artistFolder in artistFolderContainer.transform) {
				moveFolder(artistFolder.gameObject, folderOriginalPositions[folderIndex]);
				folderIndex++;
			}
		}
	

		if (Input.GetKeyDown ("x") && !boxIsMoving && waitingForArtistSelection && !artistSelected) {
			boxIsMoving = true;
			boxEndPosition = boxOriginalPosition;
			foreach (Transform artistFolder in artistFolderContainer.transform) {
				Destroy(artistFolder.gameObject);
			}
			waitingForAlbumSelection = false;
			selectedFolder = 0;
		}

		if (interactable.transform.parent.transform.localPosition == boxOriginalPosition && waitingForArtistSelection) {
			raycaster.enabled = true;
			waitingForArtistSelection = false;
			boxIsMoving = false;
			boxIsOpen = false;
		}

		/*Artist selection*/
		try {
			if (foldersOut) {
				artistFolderContainer.transform.GetChild(selectedFolder).GetChild(2).GetComponent<MeshRenderer>().material.color = ToColor(16761477);

				selectedArtist = artistFolderContainer.transform.GetChild(selectedFolder).transform;

				if (Input.GetKeyDown ("s") && selectedFolder+1 < artistFolderContainer.transform.childCount && !waitingForAlbumSelection) {
					artistFolderContainer.transform.GetChild(selectedFolder).GetChild(2).GetComponent<MeshRenderer>().material.color = folderDefaultColor;
					selectedFolder++;
				}
				if (Input.GetKeyDown ("w") && selectedFolder-1 >= 0) {
					artistFolderContainer.transform.GetChild(selectedFolder).GetChild(2).GetComponent<MeshRenderer>().material.color = folderDefaultColor;
					selectedFolder--;
				}
				if (Input.GetKeyDown ("f") && !waitingForAlbumSelection) {
					//Pull folders out of view, make appropriate albums visible, wait for album selection
					folderIsMoving = true;
					foldersLifted = false;
					foldersOut = false;
					artistSelected = true;
					foreach (Transform artist in alphabeticalContainer.transform) {
						if (selectedArtist.gameObject.name == artist.gameObject.name) {
							selectedArtistContainer = artist;
							foreach (Transform album in artist) {
								album.gameObject.SetActive(true);
								album.position = new Vector3(0, -7, 0);
							}
						}
					}
				}
			}
		} catch (UnityException) {
			//There are no artists under that letter
		}

		/*Album selection*/
		try {
			if (artistSelected && !albumSelected) {

				selectedAlbum = selectedArtistContainer.transform.GetChild(selectedAlbumIndex);
				selectedAlbum.GetChild(0).GetComponent<MeshRenderer>().material.shader = GUI3DTextShader;
				selectedAlbum.GetChild(0).gameObject.SetActive(true);
				selectedAlbum.GetChild(0).GetComponent<TextMesh>().text = selectedAlbum.name;
				if (!foldersLifted || !waitingForAlbumSelection) {
					foreach (Transform artistFolder in artistFolderContainer.transform) {
						liftFolder(artistFolder.gameObject, artistFolder.position + new Vector3(0, 20, 0));
					}
					//Move albums into view
					int albumIndex = 0;
					Vector3 currentOffset;
					float currentY = selectedArtistContainer.position.y;
					if (!waitingForAlbumSelection) {
						foreach (Transform album in selectedArtistContainer) {
							currentOffset = new Vector3(folderEndPositions[0].x * -0.2f, 0, folderEndPositions[0].z * -0.2f);
							albumEndPositions[albumIndex] = (new Vector3(folderEndPositions[0].x * 0.8f, currentY, folderEndPositions[0].z * 0.8f) - currentOffset*(currentY/-0.5f));
							positionAlbum(album.gameObject, albumEndPositions[albumIndex]);
							albumIndex++;
							currentY -= 0.1f;
						}
					}
				}
				if (Input.GetKeyDown("x")) {
					folderIsMoving = true;
					artistSelected = false;
					waitingForAlbumSelection = false;
					//Move albums back, deactivate
					foreach (Transform album in selectedArtistContainer) {
						album.gameObject.SetActive(false);
					}
					albumsOut = false;
					foldersLifted = false;
				}

				if (Input.GetKeyDown("w")) {
					waitingForAlbumSelection = true;
					if (selectedAlbumIndex > 0) {
						selectedAlbum.GetChild(0).gameObject.SetActive(false);
						selectedAlbumIndex--;

						//Move albums down
						albumLiftingDown = true;
						previouslySelectedAlbum = selectedAlbum;
						previouslySelectedAlbumIndex = previouslySelectedAlbum.GetSiblingIndex();
						scrollTarget = selectedAlbum.parent.GetChild(previouslySelectedAlbumIndex-1).localPosition + new Vector3(0, -3, 0);
					}
				}
				if (Input.GetKeyDown("s")) {
					waitingForAlbumSelection = true;
					if (selectedAlbumIndex < selectedArtistContainer.childCount-1) {
						selectedAlbum.GetChild(0).gameObject.SetActive(false);
						selectedAlbumIndex++;

						//Move albums up
						albumLiftingUp = true;
						previouslySelectedAlbum = selectedAlbum;
						previouslySelectedAlbumIndex = previouslySelectedAlbum.GetSiblingIndex();
						scrollTarget = selectedAlbum.parent.GetChild(previouslySelectedAlbumIndex+1).localPosition + new Vector3(0, 3, 0);
					}
				}
				if (Input.GetKeyDown("f") && albumsOut && !albumSelected) {
					albumSelected = true;
				}
				selectedAlbum = selectedArtistContainer.transform.GetChild(selectedAlbumIndex);
				if (albumLiftingUp) {
					liftAlbumUp(previouslySelectedAlbum.gameObject, scrollTarget);
				}
				if (albumLiftingDown) {
					liftAlbumDown(selectedAlbum.gameObject, albumEndPositions[selectedAlbumIndex]);
				}

			}
		} catch (UnityException) {
		
		}


		/*Song selection*/
		try {
			if (albumSelected && !songSelected) {
				//print ("Album selected");
				//Generate song names
				folderIsMoving = false;


				if (!songsOut) {
					songIndex = 0;
					foreach (Transform song in selectedAlbum) {
						selectedAlbum.gameObject.transform.GetChild(0).gameObject.SetActive(false);
						if (song.gameObject.name != "AlbumTitle" &&
						   (song.gameObject.name.EndsWith(".mp3") ||
						 	song.gameObject.name.EndsWith(".wav") ||
						 	song.gameObject.name.EndsWith(".ogg"))) {

							FileInfo f = new FileInfo(song.gameObject.name);
							ID3v1 tagger = new ID3v1();
							FileStream mp3Stream = new FileStream(f.FullName, FileMode.Open, FileAccess.Read, FileShare.None); 
							Mp3File currentMP3 = new Mp3File(f);
							
							currentMP3.Update();

							string songName = "";

							try {
								tagger.Deserialize(mp3Stream);				
								mp3Stream.Close();
								songName = tagger.Song;
							} catch (Id3Lib.Exceptions.TagNotFoundException ex) {
								songName = "No name detected";
								print(ex);
							}

							songHolders[songIndex] = new GameObject(song.gameObject.name);
							songHolders[songIndex].transform.position = selectedAlbum.transform.GetChild(0).transform.position;
							songHolders[songIndex].transform.eulerAngles = selectedAlbum.transform.GetChild(0).transform.eulerAngles;
							//songHolders[songIndex].transform.eulerAngles -= new Vector3(0, -90, 0);
							songHolders[songIndex].transform.position -= new Vector3(0, songIndex * 0.3f - 1.5f, 0);
							songHolders[songIndex].AddComponent<TextMesh>();
							songHolders[songIndex].GetComponent<TextMesh>().text = songName;
							songHolders[songIndex].GetComponent<TextMesh>().fontSize = 90 - songName.Length;
							songHolders[songIndex].GetComponent<TextMesh>().characterSize = .03f;

							songHolders[songIndex].GetComponent<MeshRenderer>().material.shader = GUI3DTextShader;


							songIndex++;
						}
					}
					songHolders[0].GetComponent<MeshRenderer>().material.color = Color.yellow;
					songsOut = true;
				}
				else {
					songHolders[selectedSong].GetComponent<MeshRenderer>().material.color = Color.yellow;
					if (Input.GetKeyDown("s") && (selectedSong < songIndex-1)) {
						songHolders[selectedSong].GetComponent<MeshRenderer>().material.color = Color.white;
						selectedSong++;
					}
					if (Input.GetKeyDown("w") && (selectedSong > 0)) {
						songHolders[selectedSong].GetComponent<MeshRenderer>().material.color = Color.white;
						selectedSong--;
					}
					if (Input.GetKeyDown("x")) {
						selectedAlbum.GetChild(0).gameObject.SetActive(true);
						for (int i = 0; i <= songIndex; i++) {
							Destroy(songHolders[i]);
						}
						songsOut = false;
						albumSelected = false;
						selectedSong = 0;
						songIndex = 0;
					}
					if (Input.GetKeyDown("f")) {
						PreserveData p = selectedSongInfo.GetComponent<PreserveData>();
						p.path = songHolders[selectedSong].name;
						Application.LoadLevel("ParticleRoom");
					}
				}


			}
		} catch (UnityException) {

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
					alphabeticalContainer = (GameObject)alphabeticalContainers.GetValue(i);

					if (!waitingForArtistSelection) {
						pullArtists(alphabeticalContainer, interactable);
					}

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
			newArtistFolder.transform.Rotate(new Vector3(90, 0, 90));
			newArtistFolder.name = artist.gameObject.name;
			
		}
		
		//Slide box out
		if (!boxIsOpen && !boxIsMoving && !waitingForArtistSelection) {
			Transform boxOriginalTransform = interactable.transform.parent.transform;
			boxOriginalPosition = boxOriginalTransform.localPosition;
			float currentRotation = (270.0f - (interactable.transform.parent.transform.eulerAngles.y * Mathf.PI)) / 180.0f; //Mathf.Sin() and Mathf.Cos() work with radians
			Vector3 boxEndPosition = new Vector3 (boxOriginalTransform.localPosition.x - (Mathf.Sin (currentRotation)), boxOriginalTransform.localPosition.y, boxOriginalTransform.localPosition.z + (Mathf.Cos (currentRotation)));
			this.boxEndPosition = boxEndPosition;
			boxIsMoving = true;
			folderIsMoving = true;
			boxIsOpen = true;
			waitingForArtistSelection = true;
			raycaster.enabled = false;
			
			//Pull folders out and place in front of camera
			float currentY = playerCamRig.transform.position.y;
			Vector3 currentOffset;
			int folderIndex = 0;
			foreach (Transform folder in artistFolderContainer.transform) {
				
				//print("Folder position initial: " + folder.position);
				currentOffset = new Vector3 (folder.position.x * 0.01f, 0, folder.position.z * 0.01f);
				folderOriginalPositions [folderIndex] = folder.position;
				folderEndPositions [folderIndex] = (new Vector3 (folder.position.x * 0.3f, currentY, folder.position.z * 0.3f) - currentOffset * (currentY / -0.5f));
				//print ("Folder end position: " + folderEndPositions[folderIndex]);
				//folder.position = (new Vector3(folder.position.x * 0.3f, currentY, folder.position.z * 0.3f) - currentOffset*(currentY/-0.5f));
				//print ("Desired end position: " + folder.position);
				//print ("Folder position final: " + folder.position);
				//folder.gameObject.transform.localPosition = new Vector3(playerCamRig.transform.localPosition.x, playerCamRig.transform.localPosition.y, playerCamRig.transform.localPosition.z);
				currentY -= 0.2f;
				folderIndex++;	
			}
			foldersOut = false;
			
			//Wait for artist selection or cancel
		}	

		
	}
	
	public Color32 ToColor(int HexVal)
	{
		byte R = (byte)((HexVal >> 16) & 0xFF);
		byte G = (byte)((HexVal >> 8) & 0xFF);
		byte B = (byte)((HexVal) & 0xFF);
		return new Color32(R, G, B, 255);
	}
	
	public void moveBox(GameObject box, Vector3 boxEndPosition) {
		if (box.transform.localPosition == boxEndPosition) {
			boxIsMoving = false;
		} else {
			box.transform.localPosition = Vector3.Lerp (box.transform.localPosition, boxEndPosition, Time.deltaTime * boxSpeed * 2);
		}
	}

	public void moveFolder(GameObject folder, Vector3 folderEndPosition) {
		if (folder.transform.position == folderEndPosition) {
			folderIsMoving = false;
			if (foldersOut) {
				foldersOut = false;
			}
			else {
				foldersOut = true;
			}
		} else {
			folder.transform.position = Vector3.Lerp (folder.transform.position, folderEndPosition, Time.deltaTime * folderSpeed * 2);
		}
	}

	public void liftFolder(GameObject folder, Vector3 folderEndPosition) {
		if (folder.transform.position == folderEndPosition) {
			folderIsMoving = false;
			if (foldersLifted) {
				folder.SetActive(true);
				foldersLifted = false;
				foldersOut = true;
			}
			else {
				foldersLifted = true;
				waitingForAlbumSelection = true;
				folder.SetActive(false);
			}
			if (artistSelected && !foldersLifted) {
				artistSelected = false;
			}
		} else {
			folder.transform.position = Vector3.Lerp (folder.transform.position, folderEndPosition, Time.deltaTime * folderSpeed * 0.5f);
		}
	}

	public void positionAlbum(GameObject album, Vector3 albumEndPosition) {
		if (album.transform.position == albumEndPosition) {
			waitingForAlbumSelection = true;
			if (!albumsOut) {
				albumsOut = true;
			}
			else {
				albumsOut = false;
			}
		} else {
			album.transform.position = Vector3.Lerp (album.transform.position, albumEndPosition, Time.deltaTime * albumSpeed * 2);
			album.transform.localEulerAngles = selectedArtist.localEulerAngles + new Vector3(-90, 90, 0);
		}
	}

	public void liftAlbumUp(GameObject album, Vector3 albumEndPosition) {
		if (album.transform.localPosition == albumEndPosition) {
			albumLiftingUp = false;
		} else {
			album.transform.localPosition = Vector3.Lerp (album.transform.localPosition, albumEndPosition, Time.deltaTime * albumSpeed * 2);
		}
	}

	public void liftAlbumDown(GameObject album, Vector3 albumEndPosition) {
		if (album.transform.localPosition == albumEndPosition) {
			albumLiftingDown = false;
		} else {
			album.transform.localPosition = Vector3.Lerp (album.transform.localPosition, albumEndPosition, Time.deltaTime * albumSpeed * 2);
		}
	}

}
