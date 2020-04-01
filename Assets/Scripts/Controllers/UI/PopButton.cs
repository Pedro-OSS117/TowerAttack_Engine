using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PopButton : Button, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int index = 0;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("coucou OnBeginDrag : " + index);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("coucou OnDrag : " + index);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("coucou OnEndDrag : " + index);
    }
}
