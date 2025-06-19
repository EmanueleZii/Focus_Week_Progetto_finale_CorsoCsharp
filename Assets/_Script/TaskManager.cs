using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Text;
using System.Security.Cryptography;

[System.Serializable]
public class TaskData
{
    public string giorno;
    public string titolo;
    public string descrizione;
}

[System.Serializable]
public class TaskDataList
{
    public List<TaskData> tasks = new List<TaskData>();
}

public class TaskManager : MonoBehaviour
{
    protected string napoli = "eviva napoli!";

    public TMP_InputField titoloInput;
    public TMP_InputField task_input;
    public GameObject taskPrefab;
    public GameObject SettingPanel;

    public Transform lunParent;
    public Transform martParent;
    public Transform mercParent;
    public Transform giovParent;
    public Transform venParent;
    public Transform sabParent;
    public Transform domParent;

    public Button btnLun;
    public Button btnMart;
    public Button btnMerc;
    public Button btnGiov;
    public Button btnVen;
    public Button btnSab;
    public Button btnDom;

    public Button btnClearForm;
    public Button btnSvuotaTask;

    public GameObject notificaPanel;

    private bool show_setting = true;
    private TaskDataList taskList = new TaskDataList();

    private string filePercorso => Path.Combine(Application.persistentDataPath, "taskdata.dat");

    private readonly byte[] aesKey = Encoding.UTF8.GetBytes("0123456789abcdef0123456789abcdef"); // 32 byte
    private readonly byte[] aesIV = Encoding.UTF8.GetBytes("abcdef9876543210"); // 16 byte

    private void Start()
    {
        btnLun.onClick.AddListener(() => AggiungiTask("Lun"));
        btnMart.onClick.AddListener(() => AggiungiTask("Mart"));
        btnMerc.onClick.AddListener(() => AggiungiTask("Merc"));
        btnGiov.onClick.AddListener(() => AggiungiTask("Giov"));
        btnVen.onClick.AddListener(() => AggiungiTask("Ven"));
        btnSab.onClick.AddListener(() => AggiungiTask("Sab"));
        btnDom.onClick.AddListener(() => AggiungiTask("Dom"));

        btnClearForm.onClick.AddListener(PulisciForm);
        btnSvuotaTask.onClick.AddListener(SvuotaTuttiITask);

        CaricaTask();
    }

    public void AggiungiTask(string giorno)
    {
        if (string.IsNullOrWhiteSpace(titoloInput.text)) return;
        if (string.IsNullOrWhiteSpace(task_input.text)) return;

        GameObject nuovoTask = Instantiate(taskPrefab);
        TMP_Text txt = nuovoTask.GetComponentInChildren<TMP_Text>();
        if (txt != null)
        {
            txt.text = titoloInput.text + ": \n" + task_input.text;
        }

        Transform targetParent = giorno switch
        {
            "Lun" => lunParent,
            "Mart" => martParent,
            "Merc" => mercParent,
            "Giov" => giovParent,
            "Ven" => venParent,
            "Sab" => sabParent,
            "Dom" => domParent,
            _ => null
        };

        if (targetParent != null)
        {
            nuovoTask.transform.SetParent(targetParent, false);
            taskList.tasks.Add(new TaskData
            {
                giorno = giorno,
                titolo = titoloInput.text,
                descrizione = task_input.text
            });

            titoloInput.text = "";
            task_input.text = "";
            MostraNotifica();
            SalvaTask();
        }
        else
        {
            Destroy(nuovoTask);
        }
    }

    public void PulisciForm()
    {
        titoloInput.text = "";
        task_input.text = "";
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

        taskList.tasks.Clear();
        SalvaTask();
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

    public void SettingsPanel()
    {
        SettingPanel.SetActive(!show_setting);
        show_setting = !show_setting;
    }

    public void ImpostaFullScreen()
    {
        Screen.fullScreen = true;
    }

    public void ImpostaWindowed()
    {
        Screen.fullScreen = false;
    }

    private void SalvaTask()
    {
        string json = JsonUtility.ToJson(taskList, true);
        byte[] dati = Encoding.UTF8.GetBytes(json);
        byte[] criptati = Cripta(dati);
        File.WriteAllBytes(filePercorso, criptati);
    }

    private void CaricaTask()
    {
        if (!File.Exists(filePercorso)) return;

        byte[] datiCriptati = File.ReadAllBytes(filePercorso);
        byte[] dati = Decripta(datiCriptati);
        string json = Encoding.UTF8.GetString(dati);
        taskList = JsonUtility.FromJson<TaskDataList>(json);

        foreach (TaskData task in taskList.tasks)
        {
            GameObject nuovoTask = Instantiate(taskPrefab);
            TMP_Text txt = nuovoTask.GetComponentInChildren<TMP_Text>();
            if (txt != null)
            {
                txt.text = task.titolo + ": \n" + task.descrizione;
            }

            Transform targetParent = task.giorno switch
            {
                "Lun" => lunParent,
                "Mart" => martParent,
                "Merc" => mercParent,
                "Giov" => giovParent,
                "Ven" => venParent,
                "Sab" => sabParent,
                "Dom" => domParent,
                _ => null
            };

            if (targetParent != null)
            {
                nuovoTask.transform.SetParent(targetParent, false);
            }
            else
            {
                Destroy(nuovoTask);
            }
        }
    }

    private byte[] Cripta(byte[] dati)
    {
        using Aes aes = Aes.Create();
        aes.Key = aesKey;
        aes.IV = aesIV;

        using MemoryStream ms = new MemoryStream();
        using CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
        cs.Write(dati, 0, dati.Length);
        cs.FlushFinalBlock();
        return ms.ToArray();
    }

    private byte[] Decripta(byte[] datiCriptati)
    {
        using Aes aes = Aes.Create();
        aes.Key = aesKey;
        aes.IV = aesIV;

        using MemoryStream ms = new MemoryStream(datiCriptati);
        using CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using MemoryStream output = new MemoryStream();
        cs.CopyTo(output);
        return output.ToArray();
    }
}
