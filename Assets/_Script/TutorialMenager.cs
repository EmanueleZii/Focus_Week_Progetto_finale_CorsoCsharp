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

        tutorialPanel.SetActive(false);
        characterGuide.SetActive(false);
    }

    public void StartTutorial()
    {
        currentStep = 0;
        tutorialPanel.SetActive(true);
        characterGuide.SetActive(true);
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
            EndTutorial();
        }
    }

    private void ShowStep()
    {
        tutorialText.text = steps[currentStep];
    }

    private void EndTutorial()
    {
        tutorialPanel.SetActive(false);
        characterGuide.SetActive(false);
    }
}
