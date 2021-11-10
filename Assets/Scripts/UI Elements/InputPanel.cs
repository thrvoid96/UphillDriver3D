using UnityEngine;
using UnityEngine.EventSystems;

public class InputPanel : MonoBehaviour , IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public static InputPanel instance;
    [System.NonSerialized] public float horizontal;
    Vector2 _lastPosition = Vector2.zero;


    private void Awake()
    {
        instance = this;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        horizontal = 0;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.dragging)
        {
            Vector2 direction = eventData.position - _lastPosition;

            horizontal = direction.x / Screen.width;
            _lastPosition = eventData.position;
            horizontal = 0;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _lastPosition = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        horizontal = 0;
        _lastPosition = Vector2.zero;
    }
}
