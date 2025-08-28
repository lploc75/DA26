using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scripts.Inventory
{
    /// <summary>
    /// Quản lý tooltip theo cơ chế raycast tag (giống InventorySlotItemGrab).
    /// - Chỉ hover những slot có vật phẩm (IsEmpty() == false).
    /// - Log index / row / col khi ENTER/EXIT.
    /// - Không can thiệp drag&drop (không implement BeginDrag/Drag/EndDrag).
    /// - Tooltip tự tránh out screen (do TooltipSimple đảm nhiệm).
    /// Gắn script này lên GameObject 'Inventory' (cha của các InventorySlot).
    /// </summary>
    public class InventoryHoverTooltip : MonoBehaviour
    {
        private const string InventorySlotTag = "InventorySlot";

        [Header("Refs")]
        [SerializeField] private Inventory inventory;   // tham chiếu tới Inventory chứa các InventorySlot
        [SerializeField] private Canvas tooltipCanvas;  // Canvas chứa TooltipSimple (nếu cần check thêm)

        [Header("Behaviour")]
        public float postReleaseCooldown = 0.15f;       // nghỉ ngắn sau khi thả chuột để tooltip không bật lại ngay

        // state
        private InventorySlot _currentSlot;             // slot đang hover
        private int _currentIndex = -1, _row = -1, _col = -1;
        private float _reenableTime;                    // thời điểm cho phép bật lại sau khi thả chuột

        void Reset()
        {
            if (!inventory) inventory = GetComponentInParent<Inventory>();
        }

        void Update()
        {
            // Nếu nhấn/giữ chuột (chuẩn bị kéo) -> ẩn tooltip
            if (Input.GetMouseButtonDown(0))
                HideTooltip();

            // Khi nhả chuột -> đặt cooldown
            if (Input.GetMouseButtonUp(0))
                _reenableTime = Time.unscaledTime + postReleaseCooldown;

            // Raycast theo tag như code drag của bạn
            var slot = RaycastSlotUnderMouse(Input.mousePosition, InventorySlotTag) as InventorySlot;

            // Nếu không trỏ vào slot hợp lệ -> EXIT nếu đang có slot trước đó
            if (slot == null)
            {
                if (_currentSlot != null)
                    LogExitAndClear();
                return;
            }

            // Chỉ xử lý slot có item
            if (slot.IsEmpty())
            {
                if (_currentSlot != null)
                    LogExitAndClear();
                return;
            }

            // Tính index/row/col theo GridLayoutGroup của parent
            ComputeIndexRowCol(slot, out int idx, out int row, out int col);

            // Nếu chuyển sang slot mới -> EXIT slot cũ + ENTER slot mới
            if (_currentSlot != slot)
            {
                if (_currentSlot != null)
                    LogExitAndClear();

                _currentSlot = slot;
                _currentIndex = idx;
                _row = row;
                _col = col;

                // Đang giữ chuột hoặc còn cooldown -> không bật tooltip
                if (!Input.GetMouseButton(0) && Time.unscaledTime >= _reenableTime)
                {
                    if (TooltipSimple.I != null)
                    {
                        TooltipSimple.I.Show();
                        TooltipSimple.I.MoveTo(Input.mousePosition);
                    }
                }

                // Log ENTER có cột/hàng
                if (row >= 0 && col >= 0)
                    Debug.Log($"[HOVER ENTER] {_currentSlot.name} -> index {_currentIndex} (row {_row}, col {_col})");
                else
                    Debug.Log($"[HOVER ENTER] {_currentSlot.name} -> index {_currentIndex}");
            }
            else
            {
                // Đang đứng trên cùng một slot -> cập nhật vị trí tooltip
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
            if (TooltipSimple.I != null && TooltipSimple.I.gameObject.activeSelf)
                TooltipSimple.I.Hide();
        }

        // ===== helpers =====

        /// <summary>
        /// Raycast theo tag như InventorySlotItemGrab.FindSlot
        /// </summary>
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

        /// <summary>
        /// Tính index/row/col theo GridLayoutGroup cha của slot.
        /// Row/Col chỉ có khi GridLayoutGroup dùng FixedColumnCount.
        /// </summary>
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
