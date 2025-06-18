using UnityEngine;
using TMPro;

public class TaskManager : MonoBehaviour
{
    [Header("Input Task")]
    public TMP_InputField titoloInput;

    [Header("Task Prefab")]
    public GameObject taskPrefab;

    [Header("Contenitori Giornalieri")]
    public Transform lunParent;
    public Transform martParent;
    public Transform mercParent;
    public Transform giovParent;
    public Transform venParent;
    public Transform sabParent;
    public Transform domParent;

    [Header("Popup Notifica (Opzionale)")]
    public GameObject notificaPanel;

    public void AggiungiTask(string giorno)
    {
        if (string.IsNullOrWhiteSpace(titoloInput.text)) return;

        GameObject nuovoTask = Instantiate(taskPrefab);
        nuovoTask.GetComponentInChildren<TMP_Text>().text = titoloInput.text;

        switch (giorno)
        {
            case "Lun": nuovoTask.transform.SetParent(lunParent, false); break;
            case "Mart": nuovoTask.transform.SetParent(martParent, false); break;
            case "Merc": nuovoTask.transform.SetParent(mercParent, false); break;
            case "Giov": nuovoTask.transform.SetParent(giovParent, false); break;
            case "Ven": nuovoTask.transform.SetParent(venParent, false); break;
            case "Sab": nuovoTask.transform.SetParent(sabParent, false); break;
            case "Dom": nuovoTask.transform.SetParent(domParent, false); break;
        }

        titoloInput.text = ""; // Pulisce input dopo aggiunta
    }

    public void PulisciForm()
    {
        titoloInput.text = "";
    }

    public void SvuotaTuttiITask()
    {
        Svuota(lunParent);
        Svuota(martParent);
        Svuota(mercParent);
        Svuota(giovParent);
        Svuota(venParent);
        Svuota(sabParent);
        Svuota(domParent);
    }

    private void Svuota(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    public void MostraNotifica()
    {
        if (notificaPanel != null)
        {
            notificaPanel.SetActive(true);
            Invoke(nameof(NascondiNotifica), 2f); 
        }
    }

    private void NascondiNotifica()
    {
        if (notificaPanel != null)
            notificaPanel.SetActive(false);
    }

    public void ApriImpostazioni()
    {
        Debug.Log("Apertura impostazioni");

    }
}