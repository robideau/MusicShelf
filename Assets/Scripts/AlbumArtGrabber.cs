using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using Id3Lib;
using Mp3Lib;
using System.Linq;


public class AlbumArtGrabber : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Texture getAlbumArtAsTexture(GameObject audioHolder) {

		Texture2D albumArt = new Texture2D (1, 1);

		foreach (Transform file in audioHolder.transform.parent) {
			if (file.gameObject.name.EndsWith(".jpg") || file.gameObject.name.EndsWith(".png")) {
				byte[] bytes = System.IO.File.ReadAllBytes(file.gameObject.name);
				albumArt.LoadImage(bytes);
				return albumArt;
			}
		}

		return null;

	}
}
