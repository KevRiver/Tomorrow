using UnityEngine.SceneManagement;
using UnityEngine;

public class CutSceeneChanger : MonoBehaviour
{
    public string sceneNameToChange;

    public void sceneChange()
    {
        SceneManager.LoadScene(sceneNameToChange);
    }
}
