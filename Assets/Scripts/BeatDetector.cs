/* David Robideau - 2015
 * Uses AudioProcessor class to detect spikes in the waveform corresponding to beats.
 * This class determines how to handle a beat or a spectrum event coming from an AudioProcessor.
 */

using UnityEngine;
using System.Collections;

public class BeatDetector : MonoBehaviour, AudioProcessor.AudioCallbacks {

	public AudioProcessor audioProc;
	public GameObject testCube;

	void Start () {

		audioProc = FindObjectOfType<AudioProcessor>();
		audioProc.addAudioCallback (this);
	}

	void Update () {
		
	}

	//If a beat is detected
	public void onOnbeatDetected() {
		//testCube.GetComponent<Renderer> ().material.color = new Color (Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
	}

	//If a spectrum event is triggered - probably unnecessary, but needed for inheritance
	public void onSpectrum(float[] spectrum) {

	}
}
