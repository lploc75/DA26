using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DragItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private ItemSlotUI ownerSlot;
    private Canvas rootCanvas;          // Canvas gốc để tính toạ độ UI
    private CanvasGroup canvasGroup;

    void Start()
    {
        // icon này phải là con của ItemSlotUI
        ownerSlot = GetComponentInParent<ItemSlotUI>();
        rootCanvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (ownerSlot == null || ownerSlot.IsEmpty) return;

        canvasGroup.blocksRaycasts = false; // Cho phép raycast xuyên qua khi kéo
        DragDropManager.Instance.StartDrag(ownerSlot, GetComponent<Image>());
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!DragDropManager.IsDragging) return;
        DragDropManager.Instance.UpdateDragIconPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // Tìm slot mục tiêu dưới con trỏ
        ItemSlotUI target = null;
        var results = DragDropManager.Instance.RaycastUI(eventData);
        foreach (var r in results)
        {
            target = r.gameObject.GetComponent<ItemSlotUI>() ?? r.gameObject.GetComponentInParent<ItemSlotUI>();
            if (target != null) break;
        }

        DragDropManager.Instance.EndDrag(target);
    }
}
