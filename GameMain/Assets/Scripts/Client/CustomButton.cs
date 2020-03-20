using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CustomButton : Selectable, IDragHandler
{
    [SerializeField]
    protected bool isDraggable = true;

    [SerializeField]
    protected RectTransform BelongCanvas = null;

    public UnityEvent onPointerDown = new UnityEvent();
    public UnityEvent onPointerPress = new UnityEvent();
    public UnityEvent onPointerUp = new UnityEvent();
    public UnityEvent onPointerEnter = new UnityEvent();
    public UnityEvent onPointerExit = new UnityEvent();
    public UnityEvent onPointerDrag = new UnityEvent();

    protected RectTransform rectTrans = null;
    protected Vector2 initPos = Vector2.zero;

    protected override void Awake()
    {
        rectTrans = GetComponent<RectTransform>();

        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        SaveButtonPosition(rectTrans.localPosition);
    }

    protected virtual void FixedUpdate()
    {
        if (IsPressed())
            onPointerPress.Invoke();
    }

    /// <summary>
    /// 指定初始座標(動態產生,需要循環利用的按鈕會呼叫)
    /// </summary>
    public virtual void SaveButtonPosition(Vector3 _pos)
    {
        initPos = _pos;

        ResetButtonPosition();
    }

    public virtual void ResetButtonPosition()
    {
        rectTrans.localPosition = initPos;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (onPointerDown != null)
            onPointerDown.Invoke();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        if (isDraggable == true) rectTrans.localPosition = initPos;

        if (onPointerUp != null)
            onPointerUp.Invoke();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (onPointerEnter != null)
            onPointerEnter.Invoke();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (isDraggable == true) return;

        if (onPointerExit != null)
            onPointerExit.Invoke();
    }

    public virtual void OnPointerPress(PointerEventData eventData)
    {
        if (onPointerPress != null)
            onPointerPress.Invoke();
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (isDraggable == false) return;

        //rectTrans.localPosition = Tools.TouchToCanvasPos(BelongCanvas, eventData.position, GameUIManager.Instance.UICamera);

        if (onPointerDrag != null)
            onPointerDrag.Invoke();
    }
}
