using UnityEngine;

public class NotificaPanel : MonoBehaviour
{
    public GameObject notificaPanel;

    // Nasconde il pannello di notifica
    private void NascondiNotifica()
    {
        if (notificaPanel != null)
            notificaPanel.SetActive(false);
    }
    
     // Mostra il pannello di notifica per 2 secondi
    public void MostraNotifica()
    {
        if (notificaPanel != null)
        {
            notificaPanel.SetActive(true);
            Invoke(nameof(NascondiNotifica), 2f);
        }
    }


}
