using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;

public class ButtonUIMenager : MonoBehaviour
{
    public GameObject settingbtn;
    private bool isSettingsOpen = false;
    public void TutorialButton()
    {
        //Debug.Log("Tutorial Button Clicked");
        SceneManager.LoadScene("Tutorial");
    }
    public void AppButton()
    {
        //Debug.Log("Tutorial Button Clicked");
        SceneManager.LoadScene("FocusWeek");
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void SettingsButton()
    {
        // Debug.Log("Settings Button Clicked");
        isSettingsOpen = !isSettingsOpen;
        settingbtn.SetActive(isSettingsOpen);
    }
}
