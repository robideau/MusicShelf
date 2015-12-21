/* David Robideau - 2015
 * Finds and returns album art images in a two-step process
 * 1) Scan the parent directory for any .jpg or .png files, assume cover art
 * 2) If no files are found, scrape audio file for metadata album art
 */

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

	/*	The only necessary method - follows two step process outlined above
	 *	Returns album art as a new texture
	 */
	public Texture getAlbumArtAsTexture(GameObject audioHolder, FileInfo f) {

		Texture2D albumArt = new Texture2D (1, 1); //empty texture holder

		foreach (Transform file in audioHolder.transform.parent) { //loads all files from current directory - will not result in massive searches because we only pass files until an album container is created
			if (file.gameObject.name.EndsWith(".jpg") || file.gameObject.name.EndsWith(".png")) { //pull album art from images found in directory
				byte[] bytes = System.IO.File.ReadAllBytes(file.gameObject.name); //pull byte stream from image file
				albumArt.LoadImage(bytes); //load byte stream into new texture
				return albumArt;
			}
			else {
				if (file.name != "AlbumTitle") {//scrape album art from music file data
					TagLib.File tagFile = TagLib.File.Create(file.name);
					TagLib.IPicture albumPic = tagFile.Tag.Pictures [0]; //pull first available image
					MemoryStream stream = new MemoryStream (albumPic.Data.Data);
					byte[] tagBytes;
					byte[] buffer = new byte[16 * 1024];
					using (MemoryStream ms = new MemoryStream()) { //read image data into new stream
						int read;
						while ((read = stream.Read(buffer, 0, buffer.Length)) > 0) {
							ms.Write(buffer, 0, read);
						}
						tagBytes = ms.ToArray();
					}
					albumArt.LoadImage (tagBytes); //convert stream data into new texture
					return albumArt;
				}
			}
		}

		return null;

	}
}
