using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scripts.Inventory
{
    /// <summary>
    /// Quản lý tooltip theo cơ chế raycast tag (giống InventorySlotItemGrab).
    /// - Hover slot có Item => hiển thị Tooltip.
    /// - Không can thiệp drag/drop.
    /// </summary>
    public class InventoryHoverTooltip : MonoBehaviour
    {
        private const string InventorySlotTag = "InventorySlot";

        [Header("Refs")]
        [SerializeField] private Inventory inventory;   // tham chiếu tới Inventory chứa các InventorySlot
        [SerializeField] private Canvas tooltipCanvas;  // Canvas chứa Tooltip
        [SerializeField] private TooltipPresenter presenter; // mới: gán panel hiển thị nội dung

        [Header("Behaviour")]
        public float postReleaseCooldown = 0.15f;

        // state
        private InventorySlot _currentSlot;
        private int _currentIndex = -1, _row = -1, _col = -1;
        private float _reenableTime;

        void Reset()
        {
            if (!inventory) inventory = GetComponentInParent<Inventory>();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
                HideTooltip();

            if (Input.GetMouseButtonUp(0))
                _reenableTime = Time.unscaledTime + postReleaseCooldown;

            var slot = RaycastSlotUnderMouse(Input.mousePosition, InventorySlotTag) as InventorySlot;

            if (slot == null || slot.IsEmpty())
            {
                if (_currentSlot != null)
                    LogExitAndClear();
                return;
            }

            ComputeIndexRowCol(slot, out int idx, out int row, out int col);

            if (_currentSlot != slot)
            {
                if (_currentSlot != null) LogExitAndClear();

                _currentSlot = slot;
                _currentIndex = idx;
                _row = row;
                _col = col;

                // InventoryHoverTooltip.cs — ngay chỗ ENTER slot mới:
                if (!Input.GetMouseButton(0) && Time.unscaledTime >= _reenableTime)
                {
                    if (presenter != null && _currentSlot.Item != null)
                    {
                        var it = _currentSlot.Item;
                        var def = it.CachedDef ?? ItemDatabase.Instance?.GetItemById(it.DefId);
                        Debug.Log($"[Tooltip] ShowFor → {def?.DisplayName ?? it.DefId} | Lv{it.ItemLevel} | {it.Rarity} | Price {it.SellPrice}");
                        presenter.ShowFor(it);
                    }
                    if (TooltipSimple.I != null)
                    {
                        TooltipSimple.I.Show();
                        TooltipSimple.I.MoveTo(Input.mousePosition);
                    }
                }


                Debug.Log($"[HOVER ENTER] {_currentSlot.name} -> index {_currentIndex} (row {_row}, col {_col})");
            }
            else
            {
                if (TooltipSimple.I != null && TooltipSimple.I.gameObject.activeSelf)
                    TooltipSimple.I.MoveTo(Input.mousePosition);
            }
        }

        private void LogExitAndClear()
        {
            Debug.Log($"[HOVER EXIT ] {_currentSlot.name} -> index {_currentIndex} (row {_row}, col {_col})");
            HideTooltip();
            _currentSlot = null;
            _currentIndex = _row = _col = -1;
        }

        private void HideTooltip()
        {
            if (presenter != null) presenter.Hide();
            if (TooltipSimple.I != null && TooltipSimple.I.gameObject.activeSelf)
                TooltipSimple.I.Hide();
        }

        // ===== helpers =====
        private static Slot RaycastSlotUnderMouse(Vector3 mousePosition, string tag)
        {
            if (EventSystem.current == null) return null;

            var pd = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
                position = mousePosition
            };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pd, results);

            var slotGo = results.Select(r => r.gameObject)
                                .FirstOrDefault(go => go.CompareTag(tag));
            return slotGo ? slotGo.GetComponentInParent<Slot>() : null;
        }

        private static void ComputeIndexRowCol(InventorySlot slot, out int index, out int row, out int col)
        {
            var parent = slot.transform.parent as RectTransform;
            index = slot.transform.GetSiblingIndex();
            row = col = -1;

            var grid = parent ? parent.GetComponent<GridLayoutGroup>() : null;
            if (grid && grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount && grid.constraintCount > 0)
            {
                col = index % grid.constraintCount;
                row = index / grid.constraintCount;
            }
        }
    }
}
