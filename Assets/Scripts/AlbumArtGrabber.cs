using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using Id3Lib;
using Mp3Lib;
using System.Linq;
using TagLib;
using System.Drawing.Imaging;

public class AlbumArtGrabber : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Texture getAlbumArtAsTexture(GameObject audioHolder, FileInfo f) {

		Texture2D albumArt = new Texture2D (1, 1);

		foreach (Transform file in audioHolder.transform.parent) {
			if (file.gameObject.name.EndsWith(".jpg") || file.gameObject.name.EndsWith(".png")) {
				byte[] bytes = System.IO.File.ReadAllBytes(file.gameObject.name);
				albumArt.LoadImage(bytes);
				return albumArt;
			}
			else {			
				TagLib.File tagFile = TagLib.File.Create(file.name);
				TagLib.IPicture albumPic = tagFile.Tag.Pictures [0];
				MemoryStream stream = new MemoryStream (albumPic.Data.Data);
				byte[] tagBytes;
				byte[] buffer = new byte[16 * 1024];
				using (MemoryStream ms = new MemoryStream()) {
					int read;
					while ((read = stream.Read(buffer, 0, buffer.Length)) > 0) {
						ms.Write(buffer, 0, read);
					}
					tagBytes = ms.ToArray();
				}
				albumArt.LoadImage (tagBytes);
				return albumArt;
			}
		}

		;




		return null;

	}
}
