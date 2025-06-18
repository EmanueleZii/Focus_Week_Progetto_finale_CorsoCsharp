using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskManager : MonoBehaviour
{
    public TMP_InputField titoloInput;
    public GameObject taskPrefab;

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

        GameObject nuovoTask = Instantiate(taskPrefab);
        TMP_Text txt = nuovoTask.GetComponentInChildren<TMP_Text>();
        if (txt != null) txt.text = titoloInput.text;

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
}