//David Robideau
//
//Handles basic music library operations - loading, indexing, and selecting songs

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using Id3Lib;
using Mp3Lib;


public class LibraryController : MonoBehaviour {

	string absolutePath = "./MusicLibraryDefault/";	//Default music library location
	//string[] fileTypes = {"ogg", "wav", "mp3"};	//Supported file types
	FileInfo[] musicFiles; //holds all found music files
	public UnityEngine.UI.InputField pointToDirectory; //Input field UI object
	public GameObject pointToDirectoryObj; //Input field represented as active/inactive game object
	bool filesLoaded = true; //If files have been searched/loaded since directory selected
	public GameObject musicLibrary; //Contains a reference to the MusicLibrary container object
	string[] startingChars = {"0-9", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"}; //jesus fucking christ just kill me right now
	private GameObject[] alphabeticalTitles = new GameObject[27]; //Holds alphabetical-order-title file containers

	int songIndex = 0;

	// Use this for initialization
	void Start () {

		generateAlphabeticalTitleHolders(); //Generates "TitleX" containers, where X is each letter/number of the alphabet - move to update?

	}
	
	// Update is called once per frame
	void Update () {

		/*LOAD MUSIC LIBRARY*/
		if (Input.GetKeyDown ("tab")) { //Activate/deactivate input field
			if (pointToDirectoryObj.activeSelf) {
				pointToDirectory.DeactivateInputField();
				pointToDirectoryObj.SetActive(false);
			}
			else {
				pointToDirectory.ActivateInputField();
				pointToDirectoryObj.SetActive(true);
			}
		}

		if (Input.GetKeyDown ("return") && pointToDirectoryObj.activeSelf) {
			absolutePath = pointToDirectory.text; //get file path from input field
			filesLoaded = false; //force file reload
		}

		if (!filesLoaded) {
			try {
				loadSoundFiles (); //try to load music files
			} catch (DirectoryNotFoundException) { //if directory is invalid, stop
				print("Directory could not be found.");
			}
			
			filesLoaded = true; //don't allow another load from this directory
		}

		if (Input.GetKeyDown ("1") && filesLoaded && !pointToDirectoryObj.activeSelf) { //alphabetize by filename - replace later with different action

			alphabetizeByTitle();

		}

		if (Input.GetKeyDown ("2") && filesLoaded && !pointToDirectoryObj.activeSelf) { //alphabetize by filename - replace later with different action
			
			alphabetizeByArtist();
			
		}



		/*SELECT SONGS*/
		//TODO

	}

	void loadSoundFiles() {

		DirectoryInfo directoryInfo = new DirectoryInfo (absolutePath); //open indicated directory
		musicFiles = directoryInfo.GetFiles(); //add all files 


	}

	void alphabetizeByTitle() { //Sorts files by name - does not alphabetize beyond first letter

		GameObject[] audioHolders = new GameObject[100]; //temporary placeholder max size

		print (musicFiles.Length);

		foreach(FileInfo f in musicFiles) {
			print("Loading "+f.FullName);
			GameObject audioHolder = new GameObject(f.Name);
			audioHolders[songIndex] = audioHolder;
			//alphabetizeByTitle(audioHolder, currentTitleIndex);
			//audioHolder.transform.parent = musicLibrary.transform;
			//currentTitleIndex++;
			songIndex++;

			for (int i = 0; i < alphabeticalTitles.Length; i++) {
				
				string firstChar = alphabeticalTitles.GetValue(i).ToString().Substring(5,1); 
				
				if (audioHolder.name.ToUpper().StartsWith(firstChar)) {
					
					audioHolder.transform.parent = alphabeticalTitles[i].transform;

					
				}
				
				
			}
			
			//currentTitleIndex++;
		}


		return;
	}


	void alphabetizeByArtist() {

		
		GameObject[] audioHolders = new GameObject[100]; //temporary placeholder max size
		
		print (musicFiles.Length);
		
		foreach(FileInfo f in musicFiles) {
			print("Loading "+f.FullName);
			GameObject audioHolder = new GameObject(f.Name);
			audioHolders[songIndex] = audioHolder;
			//alphabetizeByTitle(audioHolder, currentTitleIndex);
			//audioHolder.transform.parent = musicLibrary.transform;
			//currentTitleIndex++;
			songIndex++;

			TagModel tagModel = new TagModel();
			
			TagHandler tagHandler = new TagHandler(tagModel);

			Mp3File currentMP3 = new Mp3File(f);
			
			currentMP3.TagHandler = tagHandler;

			print(currentMP3.TagHandler.Artist.Length);


			string artist = currentMP3.TagHandler.Artist;

			print ("artist: " + artist);

			for (int i = 0; i < alphabeticalTitles.Length; i++) {
				
				string firstChar = alphabeticalTitles.GetValue(i).ToString().Substring(5,1); 



				if (artist.StartsWith(firstChar)) {
					
					audioHolder.transform.parent = alphabeticalTitles[i].transform;
					
					
				}
				
				
			}
			
			//currentTitleIndex++;
		}
		
		
		return;

	}

	void generateAlphabeticalTitleHolders() {
		int i = 0;
		
		foreach (string firstChar in startingChars) {
			
			GameObject currentFirstChar = new GameObject("Title" + firstChar.ToUpper());
			alphabeticalTitles.SetValue(currentFirstChar, i);
			i++;
			currentFirstChar.transform.parent = musicLibrary.transform;
			
		}
	}
}
