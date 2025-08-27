using System;
using UnityEngine;

namespace Scripts.Inventory
{
    /// <summary>
    /// Class Item: đại diện cho vật phẩm cơ bản, chứa sprite để hiển thị.
    /// </summary>
    [Serializable]
    public class Item
    {
        [SerializeField] private Sprite sprite; // hình đại diện
        public Sprite Sprite => sprite;
    }
}