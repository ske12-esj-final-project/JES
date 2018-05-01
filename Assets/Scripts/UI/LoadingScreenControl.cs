using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreenControl : MonoBehaviour {
	public GameObject loadingScreen;
	public Slider slider;
	public string nextScene;

	AsyncOperation async;

	// Use this for initialization
	public void LoadScene () {
		StartCoroutine(AsynchronousLoad());
	}
	
	IEnumerator AsynchronousLoad()
	{
		loadingScreen.SetActive(true);
		AsyncOperation ao = SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Single);
		ao.allowSceneActivation = false;

		while (!ao.isDone)
		{
			slider.value = ao.progress;
			if (ao.progress == 0.9f)
			{
				slider.value = 1f;
				ao.allowSceneActivation = true;
			}
			yield return null;
		}
	}
}
