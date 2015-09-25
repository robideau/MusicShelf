//David Robideau
//
//Generates meshes and necessary properties for the library "shelves"

using UnityEngine;
using System.Collections;

public class ShelfGenerator : MonoBehaviour {

	public int containerCount; //Number of "openable" containers that compose the shelf
	public GameObject musicLibrary; //Holds a reference to the library
	public float xPosInitial; //The starting position of the shelves - top left corner of top left shelf
	public float yPosInitial; //Top left corner of top left shelf
	public float zPos;
	public float shelfWidth; //Width of each shelf
	public float shelfHeight; //Heigh of each shelf
	public int shelvesPerRow;
	public bool assignLetters; //Determines whether or not to trigger the assignShelfLetters() function

	// Use this for initialization
	void Start () {

		for (int i = 0; i < containerCount/shelvesPerRow; i++) {
			for (int j = 0; j < shelvesPerRow; j++) {
				GameObject newShelf = GameObject.CreatePrimitive(PrimitiveType.Cube);
				newShelf.transform.position = new Vector3(xPosInitial + (shelfWidth * j), yPosInitial - (shelfHeight * i), zPos);
				newShelf.transform.localScale = new Vector3(shelfWidth, shelfHeight, 1);
			}
		}


		if (assignLetters) {
			assignShelfLetters ();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void assignShelfLetters() {

	}
}
