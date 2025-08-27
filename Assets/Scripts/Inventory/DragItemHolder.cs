using System;
using UnityEngine;
using UnityEngine.UI;

namespace DarkPixelRPGUI.Scripts.UI.Equipment
{
    /// <summary>
    /// Controller singleton giữ trạng thái đang kéo: icon theo chuột, slot nguồn, slot đích, v.v.
    /// </summary>
    public class DragItemHolder : MonoBehaviour
    {
        public static DragItemHolder Instance;

        [SerializeField] private Image displayItemImage;
        [SerializeField] private Transform transformToMove;
        [HideInInspector]
        public bool dragging;
        [HideInInspector]
        public Item dragItem;

        private Slot _targetSlotToDrop;
        private Slot _sourceSlot;

        /// <summary>Bắt đầu kéo từ một Slot nguồn.</summary>
        public void StartDrag(Slot sourceSlot)
        {
            if (dragging) return;
            dragging = true;

            if (sourceSlot == null)
            {
                Debug.LogError("Source Slot is null!");
                return;
            }

            _sourceSlot = sourceSlot;
            dragItem = _sourceSlot.Item;
            if (_sourceSlot.ItemIcon != null)
            {
                _sourceSlot.ItemIcon.color = new Color(1f, 1f, 1f, 0.5f);
            }
            TargetSlotToDrop(_sourceSlot);

            transformToMove.position = Input.mousePosition;
            displayItemImage.sprite = dragItem.Sprite;
            displayItemImage.enabled = dragging;

            Debug.Log("Started dragging item from slot: " + _sourceSlot.name);
        }

        /// <summary>Kết thúc kéo (reset trạng thái controller).</summary>
        public void DropItem()
        {
            if (!dragging) return;
            dragging = false;

            if (_sourceSlot == null)
            {
                Debug.LogError("_sourceSlot is null when dropping the item!");
                return;
            }

            dragItem = null;
            displayItemImage.enabled = dragging;

            _targetSlotToDrop = null;
        }

        /// <summary>Đặt slot hiện đang được chọn làm nơi thả.</summary>
        public void TargetSlotToDrop(Slot slot)
        {
            if (_targetSlotToDrop != null)
            {
                _targetSlotToDrop.RemovePlaceholder();
            }
            _targetSlotToDrop = slot;
            if (_targetSlotToDrop.IsEmpty())
            {
                _targetSlotToDrop.PlaceholdItem(dragItem);
            }
        }

        /// <summary>Hủy chọn slot đích nếu nó trùng với slot truyền vào.</summary>
        public void RemoveTarget(Slot slot)
        {
            if (slot != _targetSlotToDrop) return;
            TargetSlotToDrop(_sourceSlot);
        }

        /// <summary>Trả về slot nguồn đang kéo.</summary>
        public Slot GetDraggedSlot()
        {
            if (_sourceSlot == null)
            {
                Debug.LogError("_sourceSlot is null!");
                return null;
            }
            else
            {
                Debug.Log("Source Slot: " + _sourceSlot.name);
                return _sourceSlot;
            }
        }

        private void Update()
        {
            if (!dragging) return;
            transformToMove.position = Input.mousePosition;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
    }
}