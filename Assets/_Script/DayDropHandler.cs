using UnityEngine;
using UnityEngine.EventSystems;

public class DayDropHandler : MonoBehaviour, IDropHandler
{
    public string nomeGiorno;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.transform.SetParent(transform, false);
            TaskManager taskManager = FindObjectOfType<TaskManager>();
            if (taskManager != null)
            {
                taskManager.UpdateTaskGiorno(eventData.pointerDrag.gameObject, nomeGiorno);
            }
        }
    }
}
