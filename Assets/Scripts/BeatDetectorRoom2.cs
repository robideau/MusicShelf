/* David Robideau - 2015
 * Uses AudioProcessor class to detect spikes in the waveform corresponding to beats.
 * This class determines how to handle a beat or a spectrum event coming from an AudioProcessor.
 */

using UnityEngine;
using System.Collections;

public class BeatDetectorRoom2 : MonoBehaviour, AudioProcessor.AudioCallbacks {
	
	public AudioProcessor audioProc;
	public GameObject fullCube;
	public Material cubeAMat;
	public Material cubeBMat;
	private Color colorA = Color.red;
	private Color colorB = Color.white;
	public float growSpeed;
	public float spinSpeed;

	void Awake() {
		GameObject selectedSongInfo = GameObject.Find("selectedSongInfo");

		if (selectedSongInfo) {
			PreserveData[] pData = selectedSongInfo.GetComponents<PreserveData>();
			colorA = pData[0].colorScheme[0];
			colorB = pData[0].colorScheme[1];
		}
	}

	void Start () {
		
		audioProc = FindObjectOfType<AudioProcessor>();
		audioProc.addAudioCallback (this);

		cubeAMat.color = colorA;
		cubeBMat.color = colorB;
		print ("colored");
	}
	
	void Update () {
		foreach (Transform face in fullCube.transform) {
			foreach (Transform row in face) {
				foreach (Transform cube in row) {
					cube.localScale = Vector3.Lerp(cube.localScale, new Vector3(2, 2, 2), Time.deltaTime * growSpeed);
				}
			}
		}
		fullCube.transform.eulerAngles = Vector3.Lerp (fullCube.transform.eulerAngles, new Vector3 (0, fullCube.transform.eulerAngles.y + 360, 0), Time.deltaTime * spinSpeed);
	}
	
	//If a beat is detected
	public void onOnbeatDetected() {
		foreach (Transform face in fullCube.transform) {
			foreach (Transform row in face) {
				foreach (Transform cube in row) {
					cube.localScale = new Vector3(1, 1, 1);
				}
			}
		}
		if (cubeAMat.color == colorA) {
			cubeAMat.color = colorB;
			cubeBMat.color = colorA;
		} else {
			cubeAMat.color = colorA;
			cubeBMat.color = colorB;
		}
	}
	
	//If a spectrum event is triggered - probably unnecessary, but needed for inheritance
	public void onSpectrum(float[] spectrum) {
		
	}
}
