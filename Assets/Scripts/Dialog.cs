using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dialog : MonoBehaviour, IDragHandler
{
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void HideDialog()
    {
        gameObject.SetActive(false);
    }
    public void ShowDialog()
    {
        gameObject.SetActive(true);
    }
}
