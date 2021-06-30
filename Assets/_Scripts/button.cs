using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Start is called before the first frame update
    Transform Size;
    void Start()
    {
        Size = this.gameObject.transform;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Size.localScale = new Vector3(1.2f, 1.2f, 1.2f);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Size.localScale = new Vector3(1, 1, 1);
    }
}
