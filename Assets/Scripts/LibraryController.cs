﻿//David Robideau
//
//Handles basic music library operations - loading, indexing, and selecting songs

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using Id3Lib;
using Mp3Lib;
using System.Linq;


public class LibraryController : MonoBehaviour {

	string absolutePath = "./MusicLibraryDefault/";	//Default music library location
	//string[] fileTypes = {".ogg", ".wav", ".mp3"};	//Supported file types
	List<FileInfo> musicFiles = new List<FileInfo>(); //holds all found music files
	public UnityEngine.UI.InputField pointToDirectory; //Input field UI object
	public GameObject pointToDirectoryObj; //Input field represented as active/inactive game object
	bool filesLoaded = true; //If files have been searched/loaded since directory selected
	public GameObject musicLibrary; //Contains a reference to the MusicLibrary container object
	string[] startingChars = {"0-9", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"}; //jesus fucking christ just kill me right now
	private GameObject[] alphabeticalTitles = new GameObject[27]; //Holds alphabetical-order-title file containers
	public GameObject alphabeticalContainers;
	public AlbumArtGrabber artGrabber;
	public GameObject albumSleevePrefab;

	int songIndex = 0;

	// Use this for initialization
	void Start () {

		generateAlphabeticalTitleHolders(); //Generates "TitleX" containers, where X is each letter/number of the alphabet - move to update?

		string[] configLines = System.IO.File.ReadAllLines (@"./DirConfig.txt");
		absolutePath = configLines [0].Substring (25, configLines [0].Length - 25);
		filesLoaded = false;

	}
	
	// Update is called once per frame
	void Update () {

		/*LOAD MUSIC LIBRARY override for debug*/
		/*if (Input.GetKeyDown ("tab")) { //Activate/deactivate input field
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
		}*/

		if (!filesLoaded) {
			try {
				loadSoundFiles (); //try to load music files
			} catch (DirectoryNotFoundException) { //if directory is invalid, stop
				print("Directory could not be found.");
			}
			
			filesLoaded = true; //don't allow another load from this directory
			alphabetizeByArtist ();
		}
		



	}

	void loadSoundFiles() {

		DirectoryInfo directoryInfo = new DirectoryInfo (absolutePath); //open indicated directory

		FileInfo[] musicFileArray = directoryInfo.GetFiles ("*", SearchOption.AllDirectories);
		for (int i = 0; i < musicFileArray.Length; i++) {
			musicFiles.Add (musicFileArray[i]);
		}

		//musicFiles = directoryInfo.GetFiles(); //add all files regardless of extension

		musicFiles.RemoveAll(tempFile => !tempFile.FullName.EndsWith(".mp3") && !tempFile.FullName.EndsWith(".jpg") && !tempFile.FullName.EndsWith(".png"));

//		for(int i = 0; i < musicFiles.Count; i++) { //remove all files of incorrect extensions - does not work with subdirectories?
//			if (!musicFiles.ElementAt(i).FullName.EndsWith(".mp3") &&
//			    !musicFiles.ElementAt(i).FullName.EndsWith(".jpg") && 
//			    !musicFiles.ElementAt(i).FullName.EndsWith(".png")) 
//			    {
//				musicFiles.RemoveAt (i);
//
//			}
//		}



	}

	void alphabetizeByTitle() { //Sorts files by name - does not alphabetize beyond first letter

		GameObject[] audioHolders = new GameObject[10000]; //temporary placeholder max size

		print (musicFiles.Count);

		foreach(FileInfo f in musicFiles) {
			//print("Loading "+f.FullName);
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

		
		GameObject[] audioHolders = new GameObject[10000]; //temporary placeholder max size
		
		//print (musicFiles.Count);

		string artist = "No Artist";
		string album = "No Album";

		foreach(FileInfo f in musicFiles) {
			//print("Loading "+f.FullName);
			GameObject audioHolder = new GameObject(f.Name);
			audioHolder.name = f.FullName;
			audioHolders[songIndex] = audioHolder;
			songIndex++;
			if (!(f.FullName.EndsWith(".jpg") || f.FullName.EndsWith(".png"))) {
				ID3v1 tagger = new ID3v1();

			    FileStream mp3Stream = new FileStream(f.FullName, FileMode.Open, FileAccess.Read, FileShare.None); 

				Mp3File currentMP3 = new Mp3File(f);

				currentMP3.Update();

				try {
					tagger.Deserialize(mp3Stream);				
					mp3Stream.Close();
					artist = tagger.Artist;
					album = tagger.Album;
				} catch (Id3Lib.Exceptions.TagNotFoundException ex) {
					mp3Stream.Close();
					album = "No Album";
					artist = "No artist";
					print(ex);
				}

			}
			
			for (int i = 0; i < alphabeticalTitles.Length; i++) {
				
				string firstChar = alphabeticalTitles.GetValue(i).ToString().Substring(5,1); 

				//Organize into specific artists and albums
				if (artist.StartsWith(firstChar)) {

					//if artist holder doesn't already exist, create - otherwise add song to artist holder
					bool artistExists = false;
					Transform currentArtistHolder = null;
					foreach(Transform artistHolder in alphabeticalTitles[i].transform) {
						if (artistHolder.name.Trim() == artist.Trim()) {
							artistExists = true;
							currentArtistHolder = artistHolder;
						}
					}
					if (!artistExists) {
						currentArtistHolder = new GameObject(artist).transform;
						currentArtistHolder.transform.parent = alphabeticalTitles[i].transform;
					}

					//audioHolder.transform.parent = currentArtistHolder;

					bool albumExists = false;
					Transform currentAlbumHolder = null;
					foreach(Transform albumHolder in currentArtistHolder.transform) {
						if (albumHolder.name.Trim () == album.Trim()) {
							albumExists = true;
							currentAlbumHolder = albumHolder;
						}
					}
					if (!albumExists) {
						currentAlbumHolder = Instantiate(albumSleevePrefab).transform;
						currentAlbumHolder.transform.parent = currentArtistHolder;
						currentAlbumHolder.name = album;
					}
					/*if (!albumExists) {
						currentAlbumHolder = new GameObject(album).transform;
						currentAlbumHolder.transform.parent = currentArtistHolder;
						currentAlbumHolder.gameObject.AddComponent<Renderer>();
						currentAlbumHolder.gameObject.GetComponent<Renderer>().material.mainTexture = artGrabber.getAlbumArtAsTexture(audioHolder);
					}*/

					audioHolder.transform.parent = currentAlbumHolder;
					currentAlbumHolder.gameObject.GetComponent<Renderer>().material.mainTexture = artGrabber.getAlbumArtAsTexture(audioHolder, f);
					if (currentAlbumHolder.gameObject.GetComponent<Renderer>().material.mainTexture == null) {
						currentAlbumHolder.gameObject.GetComponent<Renderer>().material.mainTexture = Resources.Load("defaultAlbumArt") as Texture2D;
					}


				}


			}

		}
		
		
		return;

	}

	void alphabetizeByAlbum() {
		
		
		GameObject[] audioHolders = new GameObject[10000]; //temporary placeholder max size
		
		print (musicFiles.Count);
		
		foreach(FileInfo f in musicFiles) {
			//print("Loading "+f.FullName);
			GameObject audioHolder = new GameObject(f.Name);
			audioHolders[songIndex] = audioHolder;
			songIndex++;
			
			ID3v1 tagger = new ID3v1();
			
			FileStream mp3Stream = new FileStream(f.FullName, FileMode.Open); 
			
			Mp3File currentMP3 = new Mp3File(f);
			
			currentMP3.Update();
			
			tagger.Deserialize(mp3Stream);

			mp3Stream.Close();

			string album = tagger.Album;
			
			//print ("album: " + album);
			
			for (int i = 0; i < alphabeticalTitles.Length; i++) {
				
				string firstChar = alphabeticalTitles.GetValue(i).ToString().Substring(5,1); 
				
				if (album.StartsWith(firstChar)) {
					
					audioHolder.transform.parent = alphabeticalTitles[i].transform;
					
				}
				
			}
		}

		
		
		return;
		
	}

	void generateAlphabeticalTitleHolders() {
		int i = 0;
		
		foreach (string firstChar in startingChars) {
			
			GameObject currentFirstChar = new GameObject("Title" + firstChar.ToUpper());
			alphabeticalTitles.SetValue(currentFirstChar, i);
			i++;
			currentFirstChar.transform.parent = alphabeticalContainers.transform;
			
		}
	}

	public GameObject[] getAlphabeticalTitles() {
		return alphabeticalTitles;
	}

}