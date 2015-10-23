//David Robideau
//
//Generates meshes and necessary properties for the library "shelves"

using UnityEngine;
using System.Collections;

public class ShelfGenerator : MonoBehaviour {


	public GameObject boxShelfPrefab; //Holds the box shelf prefab
	public int containerCount; //Number of "openable" containers that compose the shelf
	public GameObject musicLibrary; //Holds a reference to the library
	public GameObject shelfContainer;
	public float xPosInitial; //The starting position of the shelves - top left corner of top left shelf
	public float yPosInitial; //Top left corner of top left shelf
	public float zPos;
	public float shelfWidth; //Width of each shelf
	public float shelfHeight; //Heigh of each shelf
	public int shelvesPerRow;
	public bool assignLetters; //Determines whether or not to trigger the assignShelfLetters() function
	public BoxNameSet boxNameSet;

	public string[] BoxNames = {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "#", "options", "misc", "exit"};

	public enum BoxNameSet {
		DefaultSet
		//Any other sets we may want
	}

	// Use this for initialization
	void Start () {
	
		int currentShelfIndex = 0;

		for (int i = 0; i < containerCount/shelvesPerRow; i++) {
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
		}
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
