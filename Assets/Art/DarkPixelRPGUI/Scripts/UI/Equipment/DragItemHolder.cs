using UnityEngine;
using UnityEngine.UI;

namespace DarkPixelRPGUI.Scripts.UI.Equipment
{
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

        public void RemoveTarget(Slot slot)
        {
            if (slot != _targetSlotToDrop) return;
            TargetSlotToDrop(_sourceSlot);
        }

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