using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [Header("Menu chính")]
    public GameObject menu;

    private bool isOpen = false;

    void Start()
    {
        if (menu != null)
            menu.SetActive(false); // ẩn menu khi bắt đầu
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        isOpen = !isOpen;
        menu.SetActive(isOpen);

        //// Dừng game khi mở menu
        //Time.timeScale = isOpen ? 0f : 1f;

        //// (tuỳ chọn) bật/tắt con trỏ chuột
        //Cursor.visible = isOpen;
        //Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void ResumeGame()
    {
        isOpen = false;
        menu.SetActive(false);
        //Time.timeScale = 1f;
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }
}
