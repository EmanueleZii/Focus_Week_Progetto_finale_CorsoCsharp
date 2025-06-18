using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskManager : MonoBehaviour
{
    //il riferimento e puramente casuale :)
    protected string napoli = "eviva napoli!";
    //fine riferimento
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
            /*txt.text = titoloInput.text;
            txt.text = task_input.text;*/
        }

        /*Transform targetParent = giorno switch
            {
                "Lun" => lunParent,
                "Mart" => martParent,
                "Merc" => mercParent,
                "Giov" => giovParent,
                "Ven" => venParent,
                "Sab" => sabParent,
                "Dom" => domParent,
                default => null
            };
            */

        Transform targetParent = null;

        if (giorno == "Lun") targetParent = lunParent;
        else if (giorno == "Mart") targetParent = martParent;
        else if (giorno == "Merc") targetParent = mercParent;
        else if (giorno == "Giov") targetParent = giovParent;
        else if (giorno == "Ven") targetParent = venParent;
        else if (giorno == "Sab") targetParent = sabParent;
        else if (giorno == "Dom") targetParent = domParent;

        if (targetParent != null)
        {
            nuovoTask.transform.SetParent(targetParent, false);
            titoloInput.text = "";
            task_input.text = "";
            MostraNotifica();
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