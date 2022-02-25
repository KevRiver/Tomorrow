using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogController : MonoBehaviour
{
    public List<DialogContent> contentList;
    private int CurrentContentIdx = 0;

    bool IsDialogEnable = false;
    bool IsTypingRunning = false;

    public GameObject ScriptUI;
    public Image characterImage;
    public Text characterText, scriptText;
    public string NextSceneName;

    [System.Serializable]
    public struct DialogContent
    {
        public Sprite sprite;
        public string name;
        public string script;
    }

    public void ShowDialog()
    {
        IsDialogEnable = true;
        ScriptUI.SetActive(IsDialogEnable);
        StartCoroutine("Typing");
    }

    void Start()
    {
        if (IsDialogEnable)
            StartCoroutine("Typing");
        ScriptUI.SetActive(IsDialogEnable);
    }

    void Update()
    {
        if (IsDialogEnable && Input.GetButtonDown("Fire1"))
        {
            if (IsTypingRunning)
            {
                StopCoroutine("Typing");
                characterImage.color = new Color(255, 255, 255, contentList[CurrentContentIdx].sprite == null ? 0 : 255);
                characterImage.sprite = contentList[CurrentContentIdx].sprite;
                characterText.text = contentList[CurrentContentIdx].name;
                scriptText.text = contentList[CurrentContentIdx].script;
                CurrentContentIdx++;
                IsTypingRunning = false;
            }
            else
            {
                if (CurrentContentIdx < contentList.Count)
                    StartCoroutine("Typing");
                else
                    SceneManager.LoadScene(NextSceneName);
            }
        }
    }

    IEnumerator Typing()
    {
        IsTypingRunning = true;

        yield return new WaitForSeconds(0.5f);

        characterImage.color = new Color(255, 255, 255, contentList[CurrentContentIdx].sprite == null ? 0 : 255);
        characterImage.sprite = contentList[CurrentContentIdx].sprite;
        characterText.text = contentList[CurrentContentIdx].name;
        scriptText.text = "";

        for (int i = 0; i <= contentList[CurrentContentIdx].script.Length; i++)
        {
            scriptText.text = contentList[CurrentContentIdx].script.Substring(0, i);

            yield return new WaitForSeconds(0.15f);
        }

        IsTypingRunning = false;

        CurrentContentIdx++;
    }
}
