using UnityEngine;
using UnityEngine.UI;

namespace DarkPixelRPGUI.Scripts.UI.Equipment
{

    /// <summary>
    /// Cơ sở cho mọi ô chứa item (kho đồ, trang bị...).
    /// Quản lý: item hiện tại, icon, placeholder, và đồng bộ UI.
    /// </summary>
    public abstract class Slot : MonoBehaviour
    {
        [SerializeField] protected NullableSerializableObjectField<Item> initialItem;
        [SerializeField] protected Image itemIcon;
        [SerializeField] protected Image placedItemImage;
        protected Item _item;
        public Item Item => _item;
        public Image ItemIcon => itemIcon;

        public bool IsEmpty()
        {
            return _item == null;
        }

        public virtual void PlaceItem(Item itemToPlace)
        {
            if (itemToPlace == null) return; // Ngăn set item null
            _item = itemToPlace;
            ShowItem();
            Debug.Log($"PlaceItem called for slot: {gameObject.name}, Item: {_item}");
        }

        /// <summary>Hiển thị hình mờ của item khi rê vào ô trống (preview).</summary>
        public void PlaceholdItem(Item itemToPlacehold)
        {
            if (itemToPlacehold != null && itemToPlacehold.Sprite != null)
            {
                itemIcon.sprite = itemToPlacehold.Sprite;
                itemIcon.color = new Color(1f, 1f, 1f, 0.5f);
                itemIcon.enabled = true;
                Debug.Log($"PlaceholdItem called for slot: {gameObject.name}");
            }
        }

        /// <summary>Xóa item khỏi slot và reset UI.</summary>
        public virtual void ClearSlot()
        {
            Debug.Log($"ClearSlot called for slot: {gameObject.name}, Item: {_item}");
            itemIcon.sprite = null;
            itemIcon.color = Color.white;
            itemIcon.enabled = false;
            _item = null;
            if (placedItemImage)
            {
                placedItemImage.enabled = false;
            }
        }

        /// <summary>Ẩn placeholder (khi rời ô).</summary>
        public void RemovePlaceholder()
        {
            if (itemIcon.enabled && itemIcon.color.a < 1f)
            {
                itemIcon.sprite = null;
                itemIcon.color = Color.white;
                itemIcon.enabled = false;
                Debug.Log($"RemovePlaceholder called for slot: {gameObject.name}");
            }
        }

        /// <summary>Đồng bộ lại UI dựa trên trạng thái hiện tại.</summary>
        public void ResetVisual()
        {
            itemIcon.color = Color.white;
            if (_item != null && _item.Sprite != null)
            {
                itemIcon.sprite = _item.Sprite;
                itemIcon.enabled = true;
                Debug.Log($"ResetVisual called for slot: {gameObject.name}, Item: {_item}");
            }
            else if (itemIcon.sprite != null)
            {
                itemIcon.enabled = true;
                Debug.Log($"ResetVisual kept existing sprite for slot: {gameObject.name}");
            }
            else
            {
                itemIcon.enabled = false;
                Debug.Log($"ResetVisual disabled itemIcon for slot: {gameObject.name}, no Item or sprite");
            }
        }

        /// <summary>Cập nhật icon .</summary>
        public void ShowItem()
        {
            if (_item != null && _item.Sprite != null)
            {
                itemIcon.sprite = _item.Sprite;
                itemIcon.color = Color.white;
                itemIcon.enabled = true;
                if (placedItemImage)
                {
                    placedItemImage.enabled = true;
                }
                Debug.Log($"ShowItem called for slot: {gameObject.name}, Sprite: {_item.Sprite}");
            }
            else if (itemIcon.sprite != null)
            {
                itemIcon.color = Color.white;
                itemIcon.enabled = true;
                if (placedItemImage)
                {
                    placedItemImage.enabled = true;
                }
                Debug.Log($"ShowItem kept existing sprite for slot: {gameObject.name}");
            }
            else
            {
                itemIcon.sprite = null;
                itemIcon.enabled = false;
                if (placedItemImage)
                {
                    placedItemImage.enabled = false;
                }
                Debug.LogWarning($"ShowItem failed for slot: {gameObject.name}, no Item or sprite");
            }
        }
        protected void OnValidate()
        {
            if (itemIcon == null) return;

            // Chỉ cập nhật nếu _item chưa được gán trong runtime
            if (_item == null && initialItem.Value != null)
            {
                _item = initialItem.Value;
                ShowItem();
            }
            else if (initialItem.Value == null)
            {
                // Không gọi Clear() để tránh xóa icon runtime khi chỉnh trong Editor
                itemIcon.sprite = null;
                itemIcon.enabled = false;
                if (placedItemImage)
                {
                    placedItemImage.enabled = false;
                }
            }
        }
    }
}