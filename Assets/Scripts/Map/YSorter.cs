using UnityEngine;
using UnityEngine.Rendering; // cần cho SortingGroup

public class YSorter : MonoBehaviour
{
    public int offset = 0;       // bù trừ nếu muốn player luôn trội hơn 1 chút
    public int multiplier = 100; // độ phân giải sắp xếp

    SortingGroup sg;
    SpriteRenderer sr;

    void Awake()
    {
        sg = GetComponent<SortingGroup>();
        sr = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        int order = -(int)(transform.position.y * multiplier) + offset;

        if (sg != null)          // nếu có Sorting Group (nhân vật nhiều sprite)
            sg.sortingOrder = order;
        else if (sr != null)     // nhân vật 1 sprite
            sr.sortingOrder = order;
    }
}
