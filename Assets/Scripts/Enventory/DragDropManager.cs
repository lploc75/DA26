using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDropManager : MonoBehaviour
{
    public static DragDropManager Instance { get; private set; }
    public static bool IsDragging => Instance != null && Instance.draggedItem != null;

    [Header("Refs")]
    public Canvas topCanvas;        // Canvas trên cùng (Screen Space - Overlay hoặc camera), để vẽ icon kéo
    public Image dragIconPrefab;    // Prefab 1 Image đơn giản

    [Header("State")]
    public ItemSlotUI originSlot;
    public ItemTest draggedItem;
    public Image dragIcon;

    EventSystem es;
    GraphicRaycaster raycaster;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        es = EventSystem.current;
        if (topCanvas == null)
        {
            topCanvas = FindObjectOfType<Canvas>(); // fallback
        }
        raycaster = topCanvas.GetComponent<GraphicRaycaster>();
        if (raycaster == null) raycaster = topCanvas.gameObject.AddComponent<GraphicRaycaster>();
    }

    public void StartDrag(ItemSlotUI slot, Image sourceIcon)
    {
        originSlot = slot;
        draggedItem = slot.currentItem;

        // Tạo icon tạm
        if (dragIcon != null) Destroy(dragIcon.gameObject);
        dragIcon = Instantiate(dragIconPrefab, topCanvas.transform);
        dragIcon.raycastTarget = false;
        dragIcon.sprite = sourceIcon.sprite;
        dragIcon.SetNativeSize();
    }

    public void UpdateDragIconPosition(PointerEventData eventData)
    {
        if (dragIcon == null) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            topCanvas.transform as RectTransform,
            eventData.position,
            topCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : topCanvas.worldCamera,
            out Vector2 pos
        );
        dragIcon.rectTransform.anchoredPosition = pos;
    }

    public List<RaycastResult> RaycastUI(PointerEventData eventData)
    {
        var results = new List<RaycastResult>();
        raycaster.Raycast(eventData, results);
        return results;
    }

    public void EndDrag(ItemSlotUI targetSlot)
    {
        // Dọn icon tạm
        if (dragIcon != null) Destroy(dragIcon.gameObject);
        dragIcon = null;

        if (originSlot == null || draggedItem == null)
        {
            ResetDrag();
            return;
        }

        // Không có target -> trả về
        if (targetSlot == null)
        {
            ResetDrag();
            return;
        }

        // Kiểm tra hợp lệ: targetSlot.slotType phải trùng loại item, trừ khi target là inventory (None)
        bool isTargetInventory = targetSlot.slotType == SlotType.None;
        bool isOriginInventory = originSlot.slotType == SlotType.None;

        bool typeMatch = isTargetInventory || targetSlot.slotType == draggedItem.slotType;

        if (!typeMatch)
        {
            // Không hợp lệ -> trả về
            ResetDrag();
            return;
        }

        // Hợp lệ -> swap
        ItemTest targetItem = targetSlot.currentItem;

        // Nếu target là equipment, nhưng item target (nếu có) phải hợp lệ khi trả về chỗ origin
        if (!isOriginInventory && targetItem != null)
        {
            // Item target trả về origin phải hợp lệ với origin (hoặc origin là inventory)
            bool originIsInventory = isOriginInventory;
            bool backMatch = originIsInventory || originSlot.slotType == targetItem.slotType;
            if (!backMatch)
            {
                ResetDrag();
                return;
            }
        }

        // Swap
        targetSlot.SetItem(draggedItem);
        if (isTargetInventory == false)
        {
            Debug.Log($"Equipped {draggedItem.itemName} to {targetSlot.slotType} slot");
        }

        if (targetItem != null) originSlot.SetItem(targetItem);
        else originSlot.ClearSlot();

        ResetDrag();
    }

    void ResetDrag()
    {
        originSlot = null;
        draggedItem = null;
    }
}
