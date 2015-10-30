//David Robideau
//
//Generates meshes and necessary properties for the library "shelves"

using UnityEngine;
using System.Collections;

public class ShelfGenerator : MonoBehaviour {


	public GameObject boxShelfPrefab; //Holds the box shelf prefab
	public int containerCount; //Number of "openable" containers that compose the shelf
	public GameObject musicLibrary; //Holds a reference to the library
	public GameObject shelfContainer; //Empty parent to hold all shelves within musicLibrary
	//public float xPosInitial; //Determines where to start generating shelves
	public float yPosInitial; 
	//public float zPosInitial; 
	public float shelfWidth; //Width of each shelf
	public float shelfHeight; //Heigh of each shelf
	public float radius; //for use in radial configuration
	//public int shelvesPerRow; //for use in "bookshelf" configuration
	public int shelvesPerStack; //for use in radial configuration
	public bool assignLetters; //Determines whether or not to trigger the assignShelfLetters() function
	public BoxNameSet boxNameSet; //set of box names - may delete, this is mostly irrelevant at this point
	public Shader GUI3DTextShader;

	public string[] BoxNames = {"a", "b", "c",
								"d", "e", "f",
								"g", "h", "i",
								"j", "k", "l",
								"m", "n", "o",
								"p", "q", "r",
								"s", "t", "u",
								"v", "w", "x",
								"y", "z", "#",
								"options", "misc", "credits"};

	public enum BoxNameSet { //delete? currently unused
		DefaultSet
		//Any other sets we may want
	}

	// Use this for initialization
	void Start () {
	
		int currentShelfIndex = 0;
		int stackCount = containerCount / shelvesPerStack;
		int stackArcOccupation = 180 / (stackCount-1); // the chunk of a 180 degree arc around the player that each stack occupies - can be modified to work with different FOV

		//Radial configuration
		for (int i = 0; i < stackCount; i++) { //all shelves
			for (int j = 0; j < shelvesPerStack; j++) { //one "stack" of shelves
				GameObject newShelf = Instantiate (boxShelfPrefab);
				newShelf.transform.eulerAngles = new Vector3 (0, -90 + (stackArcOccupation * (i)), 0);
				float currentRotation = Mathf.PI - (((180-(stackArcOccupation * i)) * Mathf.PI))/180; //Mathf.Sin() and Mathf.Cos() work with radians
				newShelf.transform.position = new Vector3 (radius * Mathf.Sin(currentRotation), yPosInitial - (shelfHeight * j), radius * Mathf.Cos (currentRotation)); //basic circular geometry
				newShelf.tag = "Interactable";
				if (assignLetters) {
					assignShelfLetters (newShelf, currentShelfIndex);
				}
				newShelf.transform.parent = shelfContainer.transform;
				TextMesh titleText = newShelf.GetComponentInChildren<TextMesh> ();
				titleText.GetComponent<MeshRenderer>().material.shader = GUI3DTextShader;
				titleText.GetComponent<MeshRenderer>().material.color = Color.black;
				titleText.color = Color.black;
				titleText.text = BoxNames [currentShelfIndex].ToUpper ();
				currentShelfIndex++;
			}
		}


		//Bookshelf configuration
		/*for (int i = 0; i < containerCount/shelvesPerRow; i++) {
			for (int j = 0; j < shelvesPerRow; j++) {
				//GameObject newShelf = GameObject.CreatePrimitive(PrimitiveType.Cube);
				GameObject newShelf = Instantiate(boxShelfPrefab);
				newShelf.transform.eulerAngles = new Vector3(0, -90, 0);
				newShelf.transform.position = new Vector3(xPosInitial + (shelfWidth * j), yPosInitial - (shelfHeight * i), zPos);
				//newShelf.transform.localScale = new Vector3(shelfWidth, shelfHeight, 1);
				newShelf.tag = "Interactable";
				if (assignLetters) {
					assignShelfLetters(newShelf, currentShelfIndex);
				}
				newShelf.transform.parent = shelfContainer.transform;
				TextMesh titleText = newShelf.GetComponentInChildren<TextMesh>();
				titleText.color = Color.black;
				titleText.text = BoxNames[currentShelfIndex].ToUpper();
				currentShelfIndex++;
			}
		}*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void assignShelfLetters(GameObject currentShelf, int currentShelfIndex) {

		if (boxNameSet == BoxNameSet.DefaultSet) {
			currentShelf.name = "Box " + BoxNames[currentShelfIndex];

		}

	}
}
