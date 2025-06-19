using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Button avviaApplicazioneBtn;
    public Button avviaTutorialBtn;
    public Button opzioniBtn;
    public Button esciBtn;

    private bool show_setting = true;
    public GameObject SettingPanel;

    void Start()
    {
        avviaApplicazioneBtn.onClick.AddListener(AvviaApplicazione);
        avviaTutorialBtn.onClick.AddListener(AvviaTutorial);
        opzioniBtn.onClick.AddListener(ApriOpzioni);
        esciBtn.onClick.AddListener(EsciApplicazione);
    }

    void AvviaApplicazione()
    {
        SceneManager.LoadScene("FocusWeek");
    }

    void AvviaTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    void ApriOpzioni()
    {
        Debug.Log("Pannello opzioni aperto.");
    }

    void EsciApplicazione()
    {
        Application.Quit();
        Debug.Log("Applicazione chiusa.");
    }
    public void SettingsPanel()
    {

        if (show_setting)
        {
            SettingPanel.SetActive(false);
            show_setting = false;
        }
        else
        {
            SettingPanel.SetActive(true);
            show_setting = true;
        }
    }
    public void ImpostaFullScreen()
    {
        Screen.fullScreen = true;
    }
    public void ImpostaWindowed()
    {
        Screen.fullScreen = false;
    }
}