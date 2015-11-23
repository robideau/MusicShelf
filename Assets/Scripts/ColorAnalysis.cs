using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ColorAnalysis : MonoBehaviour {

	Dictionary<Color, int> colorCounts; //Holds all scanned colors and an int representing the frequency of that color appearing on the album art

	//Scan the album art, determining the most frequently occuring colors
	public Color[] findColorScheme(Texture albumArtTexture, int colorDetail, float colorTolerance) {

		Texture2D albumArt = (albumArtTexture as Texture2D);

		Color[] colors = new Color[albumArtTexture.height * albumArtTexture.width];
		Color[] colorScheme = new Color[colorDetail];

		colorCounts = new Dictionary<Color, int> (); //initialize color count collection

		for (int i = 0; i < albumArt.width; i++) {
			for (int j = 0; j < albumArt.height; j++) {
				Color scannedColor = albumArt.GetPixel(i, j); //scan for color
				if (!colorCounts.ContainsKey(scannedColor)) { //if color has not been scanned before, create new entry
					colorCounts.Add(scannedColor, 1);
				}
				else {
					colorCounts[scannedColor]++; //add one to counter value for scanned color
				}
			}
		}

		int colorCountsSize = colorCounts.Count;

		

		//if dictionary is not full enough to satisfy colorDetail, fill with current most frequent colors (dummies - no value)
		if (colorCounts.Count < colorDetail) {
			for (int c = 0; c < (colorDetail - colorCountsSize)/colorCountsSize; c++) {
				for (int cc = 0; cc < colorCountsSize; cc++) {
					colorCounts.Add(colorCounts.ElementAt(cc).Key, 0);
				}
			}
		}

		//Determine top N colors, N being the colorDetail provided
		Dictionary<Color, int> topColors = colorCounts.OrderByDescending (pair => pair.Value).Take (colorCounts.Count).ToDictionary (pair => pair.Key, pair => pair.Value);
		colors = topColors.Keys.ToArray ();

		//stabilization - consolidate overly similar colors, then resort
		for (int d = 0; d < colors.Length-1; d++) {
			if (colors[d].r - colors[d+1].r < colorTolerance &&
			    colors[d].g - colors[d+1].g < colorTolerance &&
			    colors[d].b - colors[d+1].b < colorTolerance) {

				colors[d+1] = colors[d]; //first, merge all similar colors into multiple instances of one color
			
			}
		}
		int colorSchemeIndex = 0;
		bool duplicateColor = false;
		for (int e = 0; e < colors.Length; e++) {
			for (int s = colorSchemeIndex; s < colorScheme.Length; s++) {
				for (int t = 0; t < colorSchemeIndex; t++) {
					if (colors[e] == colorScheme[t]) {
						duplicateColor = true;
					}
				}
				if (duplicateColor) break;
				if (!(colors[e] == colorScheme[s])) {
					colorScheme[colorSchemeIndex] = colors[e]; //fill a new consolidated array of colors, no duplicates
					colorSchemeIndex++;
					break;
				}
			}
			duplicateColor = false;
		}

		//if colorScheme does not satisfy colorDetail
		if (colorSchemeIndex < colorDetail) {
			for (int n = 0; colorSchemeIndex < colorDetail; n++) {
				colorScheme[colorSchemeIndex] = colorScheme[n]; //fill remaining spots with existing colors, retaining weighted nature of array
				colorSchemeIndex++;
			}
		}

		return colorScheme;
	}

	public Color32 ToColor(int HexVal)
	{
		byte R = (byte)((HexVal >> 16) & 0xFF);
		byte G = (byte)((HexVal >> 8) & 0xFF);
		byte B = (byte)((HexVal) & 0xFF);
		return new Color32(R, G, B, 255);
	}
}
