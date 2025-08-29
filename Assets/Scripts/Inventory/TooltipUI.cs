using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class TooltipSimple : MonoBehaviour
{
    public static TooltipSimple I;

    [Header("Setup")]
    public Canvas rootCanvas;                 // Canvas chứa Tooltip (nên là Screen Space - Overlay)
    public RectTransform tooltipRect;         // RectTransform của chính Tooltip
    public Vector2 cursorOffset = new Vector2(18f, -18f);
    public float padding = 8f;                // đệm tránh dính mép

    RectTransform canvasRect;
    Camera uiCam;
    bool visible;

    void Awake()
    {
        I = this;
        if (!rootCanvas) rootCanvas = GetComponentInParent<Canvas>();
        if (!tooltipRect) tooltipRect = GetComponent<RectTransform>();
        canvasRect = rootCanvas.GetComponent<RectTransform>();
        uiCam = rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera;

        // tránh chặn chuột
        var cg = GetComponent<CanvasGroup>();
        if (!cg) cg = gameObject.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;

        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        visible = true;
        MoveTo(Input.mousePosition); // đặt lần đầu để không giật
    }

    public void Hide()
    {
        visible = false;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (visible) MoveTo(Input.mousePosition);
    }

    public void MoveTo(Vector2 screenPos)
    {
        // Tự chọn pivot theo vị trí chuột -> tooltip “mở” về phía còn không gian
        Vector2 pivot = new Vector2(
            Mathf.Clamp01(screenPos.x / Screen.width),
            Mathf.Clamp01(screenPos.y / Screen.height)
        );
        tooltipRect.pivot = pivot;

        // offset dựa theo pivot (gần mép phải/đáy thì lật hướng)
        Vector2 off = new Vector2(
            (pivot.x < 0.5f ? Mathf.Abs(cursorOffset.x) : -Mathf.Abs(cursorOffset.x)),
            (pivot.y < 0.5f ? Mathf.Abs(cursorOffset.y) : -Mathf.Abs(cursorOffset.y))
        );

        // screen -> local của Canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPos + off, uiCam, out var local);

        // Clamp trong biên Canvas để không out màn hình
        Vector2 size = tooltipRect.rect.size;
        float halfW = canvasRect.rect.width * 0.5f;
        float halfH = canvasRect.rect.height * 0.5f;

        float minX = -halfW + size.x * tooltipRect.pivot.x + padding;
        float maxX = halfW - size.x * (1f - tooltipRect.pivot.x) - padding;
        float minY = -halfH + size.y * tooltipRect.pivot.y + padding;
        float maxY = halfH - size.y * (1f - tooltipRect.pivot.y) - padding;

        local.x = Mathf.Clamp(local.x, minX, maxX);
        local.y = Mathf.Clamp(local.y, minY, maxY);

        tooltipRect.anchoredPosition = local;
    }
}
