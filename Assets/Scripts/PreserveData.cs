using UnityEngine;
using System.Collections;

public class PreserveData : MonoBehaviour {

	public string path;
	public Color[] colorScheme;
	public void Awake() {
		DontDestroyOnLoad (transform.gameObject);
	}
}
