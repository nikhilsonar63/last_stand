using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public Slider loadingBar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(lodGame());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   IEnumerator lodGame()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(2);
        operation.allowSceneActivation = false;
        while (operation.progress < 0.9f)
        {
            loadingBar.value = operation.progress / 0.9f;
            yield return null;
        }
        loadingBar.value = 1f;
        operation.allowSceneActivation = true;
    }
}
