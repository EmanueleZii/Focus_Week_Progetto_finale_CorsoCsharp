using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialPanel;
    public TMP_Text tutorialText;
    public GameObject characterGuide;

    public Button btnTutorial;
    public Button btnNextStep;
    public Button btnSkipTutorial;
    public Button btnRestartTutorial;

    private string[] steps = {
        "Benvenuto! Questo è il tuo planner settimanale.",
        "Puoi aggiungere un task scrivendo un titolo e una descrizione.",
        "Clicca sul giorno in cui vuoi salvare il task.",
        "Hai anche i pulsanti per svuotare tutti i task o pulire il modulo.",
        "Puoi cambiare la modalità tra fullscreen e windowed.",
        "E infine, puoi aprire le impostazioni con l'apposito pulsante!"
    };

    private int currentStep = 0;

    private void Start()
    {
        btnTutorial.onClick.AddListener(StartTutorial);
        btnNextStep.onClick.AddListener(NextStep);
        btnSkipTutorial.onClick.AddListener(EndTutorial);
        btnRestartTutorial.onClick.AddListener(RestartTutorial);

        tutorialPanel.SetActive(false);
        characterGuide.SetActive(false);
        tutorialText.gameObject.SetActive(true);
        btnNextStep.gameObject.SetActive(true);
        btnSkipTutorial.gameObject.SetActive(true);
        btnRestartTutorial.gameObject.SetActive(true);
    }

    public void StartTutorial()
    {
        currentStep = 0;
        tutorialPanel.SetActive(true);
        characterGuide.SetActive(true);
        tutorialText.gameObject.SetActive(true);
        btnNextStep.gameObject.SetActive(true);
        btnSkipTutorial.gameObject.SetActive(true);
        btnRestartTutorial.gameObject.SetActive(true);
        ShowStep();
    }

    public void NextStep()
    {
        currentStep++;
        if (currentStep < steps.Length)
        {
            ShowStep();
        }
        else
        {
            tutorialText.gameObject.SetActive(false);
            btnNextStep.gameObject.SetActive(false);
            btnSkipTutorial.gameObject.SetActive(false);
            btnRestartTutorial.gameObject.SetActive(true);
        }
    }

    private void ShowStep()
    {
        tutorialText.text = steps[currentStep];
    }

    public void EndTutorial()
    {
        tutorialPanel.SetActive(false);
        characterGuide.SetActive(false);
        tutorialText.gameObject.SetActive(true);
        btnNextStep.gameObject.SetActive(true);
        btnSkipTutorial.gameObject.SetActive(true);
    }

    public void RestartTutorial()
    {
        StartTutorial();
    }
}