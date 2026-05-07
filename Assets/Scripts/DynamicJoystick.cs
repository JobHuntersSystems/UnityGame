using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicJoystick : MonoBehaviour, IPointerDownHandler,
IDragHandler, IPointerUpHandler
{
    [Header("Configuración")]
    public float handleRange = 1f;

    [Header("Referencias")]
    public RectTransform background;
    public RectTransform handle;

    private Canvas canvas;
    private Vector2 inputDirection;

    public Vector2 Direction => inputDirection;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        background.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        background.gameObject.SetActive(true);
        MoveBackground(eventData.position);
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 screenCenter = RectTransformUtility.WorldToScreenPoint(null, background.position);
        Vector2 radius = background.sizeDelta / 2 * canvas.scaleFactor;

        inputDirection = (eventData.position - screenCenter) / radius;

        if (inputDirection.magnitude > 1f)
            inputDirection = inputDirection.normalized;

        handle.anchoredPosition = inputDirection * (background.sizeDelta / 2) * handleRange;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        background.gameObject.SetActive(false);
        handle.anchoredPosition = Vector2.zero;
        inputDirection = Vector2.zero;
    }

    private void MoveBackground(Vector2 screenPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform as RectTransform,
            screenPosition,
            null,
            out Vector2 localPoint
        );
        background.anchoredPosition = localPoint;
    }
}