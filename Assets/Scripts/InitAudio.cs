// Initializes audio playback in Particle Room, plays song passed in through 'selectedSongInfo' object

using UnityEngine;
using System.IO;
using System.Collections;
using NAudio;
using NAudio.Wave;

public class InitAudio : MonoBehaviour {

	private IWavePlayer waveOutDevice;
	private IWaveIn waveInDevice;
	private AudioFileReader audioFileReader;
	public AudioSource audioSource;
	public AudioProcessor audioProcessor;
	private bool copyCreated = false;
	private string copyPath = null;

	// Use this for initialization
	void Awake () {
		GameObject selectedSongInfo = GameObject.Find("selectedSongInfo");

		// If we find the selectedSongInfo object
		// open the mp3 file and play the song.
		if(selectedSongInfo)
		{
			PreserveData[] pData = selectedSongInfo.GetComponents<PreserveData>();
			string path = pData[0].path;

			CloseWaveOut();
			PlayMp3(path);

			//If mp3 file (not allowed), then temporarily convert to wav file
			if (path.EndsWith(".mp3")) {
				string wavPath = path.Substring(0, path.Length-4);
				wavPath += ".wav";
				mp3ToWav(path, wavPath);
				path = wavPath;
				copyCreated = true;
				copyPath = path;
			}

			StartCoroutine(startAudioSource(path));
		}
//		CloseWaveOut();
//		WWW loadAudio = new WWW(selectedSongInfo.GetComponents<PreserveData>()[0].path);
//		Mp3FileReader mp3Reader = new Mp3FileReader (selectedSongInfo.GetComponents<PreserveData> () [0].path);
//		audioSource.Play();
	}

	void OnApplicationQuit()
	{
		CloseWaveOut();
		if (copyCreated) {
			if (copyPath != null) {
				File.Delete(copyPath);
			}
		}
		copyCreated = false;
	}

	private bool PlayMp3(string path)
	{
		try
		{

			waveOutDevice = new WaveOut();
			audioFileReader = new AudioFileReader(path);
			waveOutDevice.Init(audioFileReader);
			//waveOutDevice.Play();
		}
		catch (System.Exception ex)
		{
			UnityEngine.Debug.LogWarning("Error! " + ex.Message);
		}
		return false;
	}

	private void CloseWaveOut()
	{
		if (waveOutDevice != null)
		{
			waveOutDevice.Stop();
		}
		if (audioFileReader != null)
		{
			audioFileReader.Dispose();
			audioFileReader = null;
		}
		if (waveOutDevice != null)
		{
			waveOutDevice.Dispose();
			waveOutDevice = null;
		}
	}

	//If necessary, convert mp3 to wav - avoid Unity file stream restrictions
	private static void mp3ToWav(string mp3path, string outputPath) {
		using (Mp3FileReader reader = new Mp3FileReader(mp3path)) {
			using (WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(reader)) {
				WaveFileWriter.CreateWaveFile(outputPath, pcmStream);
			}
		}
	}

	//Load and play from audio source - use coroutine to avoid playing before WWW load is complete
	private IEnumerator startAudioSource(string path) {
		WWW loadAudio = new WWW("file://" + path);
		while (!loadAudio.isDone) {
			yield return null;
		}
		audioSource.clip = loadAudio.GetAudioClip(false);
		audioSource.Play();
	}
}
