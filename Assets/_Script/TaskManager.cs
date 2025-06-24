using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Gestore principale dei task, da collegare a un GameObject in Unity
public class TaskManager : MonoBehaviour
{
    // Riferimenti agli input field per titolo e descrizione
    public TMP_InputField titoloInput;
    public TMP_InputField task_input;

    // Dropdown per la selezione della priorità
    public TMP_Dropdown prioritaDropdown;

    // Prefab del task da istanziare
    public GameObject taskPrefab;
    public GameObject SettingPanel;
    public TMP_Dropdown dropdownWeekSelector;
    public GameObject weekSelectorPanel;

    // Riferimenti ai contenitori dei task per ogni giorno
    public Transform lunParent, martParent, mercParent, giovParent, venParent, sabParent, domParent;

    // Bottoni per aggiungere task ai vari giorni
    public Button btnLun, btnMart, btnMerc, btnGiov, btnVen, btnSab, btnDom;
    public Button btnClearForm, btnSvuotaTask, btnChangeWeek;
    public Button btnConfermaModifica;

    // Bottoni per esportare/importare i task
    public Button btnExportAll, btnImportAll;

    private int slotCorrente = 1;              // Settimana attualmente selezionata
    private TaskList taskList = new TaskList(); // Lista dei task caricati

    private const int maxSettimane = 4;         // Numero massimo di settimane gestite

    // Percorso del file di salvataggio per la settimana corrente
    private string FilePercorso => Path.Combine(Application.persistentDataPath, $"task_week_{slotCorrente}.dat");

    public EncryptionUtility encryptionUtility = new EncryptionUtility(); // Istanza della utility di cifratura
    NotificaPanel notificaPanel = new NotificaPanel(); // Istanza del pannello di notifica
    private GameObject taskInModifica = null;   // Riferimento al task in fase di modifica
    private int indiceTaskInModifica = -1;      // Indice del task in modifica nella lista

    // Inizializzazione di tutti i riferimenti e caricamento dei task
    private void Start()
    {
        // Collega i bottoni dei giorni alla funzione di aggiunta task
        btnLun.onClick.AddListener(() => AggiungiTask("Lun"));
        btnMart.onClick.AddListener(() => AggiungiTask("Mart"));
        btnMerc.onClick.AddListener(() => AggiungiTask("Merc"));
        btnGiov.onClick.AddListener(() => AggiungiTask("Giov"));
        btnVen.onClick.AddListener(() => AggiungiTask("Ven"));
        btnSab.onClick.AddListener(() => AggiungiTask("Sab"));
        btnDom.onClick.AddListener(() => AggiungiTask("Dom"));

        // Collega i bottoni delle altre funzioni
        btnClearForm.onClick.AddListener(PulisciForm);
        btnSvuotaTask.onClick.AddListener(SvuotaSettimanaCorrente);
        btnChangeWeek.onClick.AddListener(MostraWeekSelector);
        btnConfermaModifica.onClick.AddListener(ConfermaModifica);

        btnExportAll.onClick.AddListener(EsportaTuttiITask);
        btnImportAll.onClick.AddListener(ImportaTuttiITask);

        // Popola il dropdown delle settimane
        List<string> options = new List<string>();
        for (int i = 1; i <= maxSettimane; i++)
            options.Add($"Settimana {i}");
        dropdownWeekSelector.ClearOptions();
        dropdownWeekSelector.AddOptions(options);
        dropdownWeekSelector.onValueChanged.AddListener(SelezionaSettimana);

        // Popola il dropdown delle priorità
        if (prioritaDropdown != null)
        {
            prioritaDropdown.ClearOptions();
            prioritaDropdown.AddOptions(new List<string> { "Bassa", "Media", "Alta" });
        }

        weekSelectorPanel.SetActive(false);
        btnConfermaModifica.gameObject.SetActive(false);

        // Carica i task salvati per la settimana corrente
        CaricaTask();
    }

    // Restituisce il contenitore corretto in base al giorno
    private Transform GetParentPerGiorno(string giorno)
    {
        return giorno switch
        {
            "Lun" => lunParent,
            "Mart" => martParent,
            "Merc" => mercParent,
            "Giov" => giovParent,
            "Ven" => venParent,
            "Sab" => sabParent,
            "Dom" => domParent,
            _ => null,
        };
    }

    // Aggiunge un nuovo task al giorno selezionato
    private void AggiungiTask(string giorno)
    {
        // Se si sta modificando un task, non aggiungere
        if (taskInModifica != null)
            return;

        // Controlla che i campi obbligatori siano compilati
        if (string.IsNullOrWhiteSpace(titoloInput.text) || string.IsNullOrWhiteSpace(task_input.text))
            return;

        // Istanzia il prefab del task
        GameObject nuovoTask = Instantiate(taskPrefab);
        string dataOra = DateTime.Now.ToString("g");

        // Imposta il testo del task
        TMP_Text txt = nuovoTask.transform.Find("TaskText")?.GetComponent<TMP_Text>();
        if (txt != null)
            txt.text = $"{titoloInput.text} ({dataOra}):\n{task_input.text}";

        // Aggiunge il task al contenitore del giorno corretto
        Transform parent = GetParentPerGiorno(giorno);
        if (parent != null)
        {
            nuovoTask.transform.SetParent(parent, false);

            // Crea e aggiunge il nuovo task alla lista
            TaskData newTaskData = new TaskData()
            {
                giorno = giorno,
                titolo = titoloInput.text,
                descrizione = task_input.text,
                dataOraCreazione = dataOra,
                priorita = prioritaDropdown != null ? prioritaDropdown.value : 0
            };
            taskList.tasks.Add(newTaskData);

            // Collega i listener ai bottoni del task
            AggiungiListenerTask(nuovoTask, newTaskData);
            // Applica lo stile in base alla priorità
            ApplicaStilePriorita(nuovoTask, newTaskData.priorita);

            // Pulisce il form, salva e mostra notifica
            PulisciForm();
            SalvaTask();
           notificaPanel.MostraNotifica();
        }
        else
        {
            Destroy(nuovoTask);
        }
    }

    // Collega i listener ai bottoni di modifica e cancellazione del task
    private void AggiungiListenerTask(GameObject taskGO, TaskData taskData)
    {
        Button[] buttons = taskGO.GetComponentsInChildren<Button>();

        foreach (Button btn in buttons)
            btn.onClick.RemoveAllListeners();

        Button btnEdit = null;
        Button btnDelete = null;

        // Identifica i bottoni tramite nome
        foreach (Button btn in buttons)
        {
            if (btn.gameObject.name == "BtnDelete")
                btnDelete = btn;
            else if (btn.gameObject.name == "BtnEdit")
                btnEdit = btn;
        }

        // Listener per la modifica
        if (btnEdit != null)
        {
            btnEdit.onClick.AddListener(() =>
            {
                int index = taskList.tasks.IndexOf(taskData);
                if (index >= 0)
                    CaricaTaskInForm(taskGO, index);
            });
        }

        // Listener per la cancellazione
        if (btnDelete != null)
        {
            btnDelete.onClick.AddListener(() =>
            {
                int index = taskList.tasks.IndexOf(taskData);
                if (index >= 0)
                {
                    taskList.tasks.RemoveAt(index);
                    Destroy(taskGO);

                    // Se si stava modificando questo task, resetta il form
                    if (taskInModifica == taskGO)
                    {
                        PulisciForm();
                        taskInModifica = null;
                        indiceTaskInModifica = -1;
                        btnConfermaModifica.gameObject.SetActive(false);
                    }
                    SalvaTask();
                    notificaPanel.MostraNotifica();
                }
            });
        }
    }

    // Carica i dati di un task nel form per la modifica
    private void CaricaTaskInForm(GameObject taskGO, int index)
    {
        TaskData task = taskList.tasks[index];
        titoloInput.text = task.titolo;
        task_input.text = task.descrizione;
        if (prioritaDropdown != null)
            prioritaDropdown.value = task.priorita;

        taskInModifica = taskGO;
        indiceTaskInModifica = index;

        btnConfermaModifica.gameObject.SetActive(true);
    }

    // Conferma la modifica di un task esistente
    public void ConfermaModifica()
    {
        if (taskInModifica != null && indiceTaskInModifica >= 0)
        {
            TaskData task = taskList.tasks[indiceTaskInModifica];
            task.titolo = titoloInput.text;
            task.descrizione = task_input.text;
            task.priorita = prioritaDropdown != null ? prioritaDropdown.value : 0;

            TMP_Text txt = taskInModifica.transform.Find("TaskText")?.GetComponent<TMP_Text>();
            if (txt != null)
                txt.text = $"{task.titolo} ({task.dataOraCreazione}):\n{task.descrizione}";

            ApplicaStilePriorita(taskInModifica, task.priorita);

            PulisciForm();
            taskInModifica = null;
            indiceTaskInModifica = -1;

            btnConfermaModifica.gameObject.SetActive(false);

            SalvaTask();
            notificaPanel.MostraNotifica();
        }
    }

    // Pulisce tutti i campi del form
    public void PulisciForm()
    {
        titoloInput.text = "";
        task_input.text = "";
        if (prioritaDropdown != null)
            prioritaDropdown.value = 0;
        taskInModifica = null;
        indiceTaskInModifica = -1;
        btnConfermaModifica.gameObject.SetActive(false);
    }

    // Svuota tutti i task della settimana corrente
    public void SvuotaSettimanaCorrente()
    {
        Svuota(lunParent);
        Svuota(martParent);
        Svuota(mercParent);
        Svuota(giovParent);
        Svuota(venParent);
        Svuota(sabParent);
        Svuota(domParent);

        taskList.tasks.Clear();

        PulisciForm();
        SalvaTask();
    }

    // Distrugge tutti i figli di un contenitore (usato per svuotare i giorni)
    private void Svuota(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    
    // Mostra o nasconde il selettore delle settimane
    public void MostraWeekSelector()
    {
        weekSelectorPanel.SetActive(!weekSelectorPanel.activeSelf);
    }

    // Cambia la settimana selezionata e ricarica i task
    public void SelezionaSettimana(int index)
    {
        slotCorrente = index + 1;
        PulisciForm();
        CaricaTask();
    }

    // Salva i task su file cifrato
    private void SalvaTask()
    {
        try
        {
            string json = JsonUtility.ToJson(taskList, true);
            byte[] datiJson = Encoding.UTF8.GetBytes(json);
            byte[] criptati = encryptionUtility.Cripta(datiJson);
            File.WriteAllBytes(FilePercorso, criptati);
        }
        catch (Exception e)
        {
            Debug.LogError("Errore salvataggio: " + e.Message);
        }
    }

    // Carica i task dal file cifrato
    private void CaricaTask()
    {
        taskList = new TaskList();

        // Svuota tutti i giorni prima di caricare
        Svuota(lunParent);
        Svuota(martParent);
        Svuota(mercParent);
        Svuota(giovParent);
        Svuota(venParent);
        Svuota(sabParent);
        Svuota(domParent);

        if (!File.Exists(FilePercorso))
            return;

        try
        {
            byte[] criptati = File.ReadAllBytes(FilePercorso);
            byte[] datiJson = encryptionUtility.Decripta(criptati);
            string json = Encoding.UTF8.GetString(datiJson);
            taskList = JsonUtility.FromJson<TaskList>(json);

            // Ricrea i task nell'interfaccia
            foreach (TaskData task in taskList.tasks)
            {
                GameObject nuovoTask = Instantiate(taskPrefab);
                TMP_Text txt = nuovoTask.transform.Find("TaskText")?.GetComponent<TMP_Text>();
                if (txt != null)
                    txt.text = $"{task.titolo} ({task.dataOraCreazione}):\n{task.descrizione}";

                Transform parent = GetParentPerGiorno(task.giorno);
                if (parent != null)
                {
                    nuovoTask.transform.SetParent(parent, false);
                    AggiungiListenerTask(nuovoTask, task);
                    ApplicaStilePriorita(nuovoTask, task.priorita);
                }
                else
                {
                    Destroy(nuovoTask);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Errore caricamento: " + e.Message);
        }
    }
    // Aggiorna il giorno di un task (usato per drag & drop)
    public void UpdateTaskGiorno(GameObject taskGO, string nuovoGiorno)
    {
        TMP_Text txt = taskGO.transform.Find("TaskText")?.GetComponent<TMP_Text>();
        if (txt == null) return;

        TaskData td = null;
        int idx = -1;
        for (int i = 0; i < taskList.tasks.Count; i++)
        {
            TaskData task = taskList.tasks[i];
            string expectedText = $"{task.titolo} ({task.dataOraCreazione}):\n{task.descrizione}";
            if (expectedText == txt.text)
            {
                td = task;
                idx = i;
                break;
            }
        }

        if (td != null)
        {
            td.giorno = nuovoGiorno;
            SalvaTask();
        }
    }

    // Applica uno stile visivo al task in base alla priorità (bordo colorato)
    private void ApplicaStilePriorita(GameObject taskGO, int priorita)
    {
        Outline outline = taskGO.GetComponentInChildren<Outline>();

        if (outline == null)
        {
            Image bgImage = taskGO.GetComponentInChildren<Image>();
            if (bgImage != null)
            {
                outline = bgImage.gameObject.GetComponent<Outline>();
                if (outline == null)
                    outline = bgImage.gameObject.AddComponent<Outline>();

                outline.effectDistance = new Vector2(2f, 2f);
            }
            else
            {
                return;
            }
        }

        // Cambia il colore del bordo in base alla priorità
        switch (priorita)
        {
            case 0: // Bassa
                outline.effectColor = Color.green;
                break;
            case 1: // Media
                outline.effectColor = Color.yellow;
                break;
            case 2: // Alta
                outline.effectColor = Color.red;
                break;
            default:
                outline.effectColor = Color.white;
                break;
        }
    }

    // Esporta tutti i task della settimana corrente in un file JSON non cifrato
    public void EsportaTuttiITask()
    {
        string path = Path.Combine(Application.persistentDataPath, $"task_backup_week_{slotCorrente}.json");
        string json = JsonUtility.ToJson(taskList, true);
        File.WriteAllText(path, json);
        Debug.Log($"Backup esportato in: {path}");
    }

    // Importa i task da un file JSON di backup e li salva cifrati
    public void ImportaTuttiITask()
    {
        string path = Path.Combine(Application.persistentDataPath, $"task_backup_week_{slotCorrente}.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            taskList = JsonUtility.FromJson<TaskList>(json);
            SalvaTask();
            CaricaTask();
        }
    }
}