using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void play()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
      
    }
    public void Exit()
    {
        Application.Quit();
    }
  
}
