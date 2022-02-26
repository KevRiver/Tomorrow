using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneButton : MonoBehaviour
{
    public void OnClickStart(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    public void OnClickExit()
    {
        Application.Quit();
    }
}
