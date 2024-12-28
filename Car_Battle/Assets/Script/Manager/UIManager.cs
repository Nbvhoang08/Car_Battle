using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    // Start is called before the first frame update
    [SerializeField] private List<UICanvas> uiCanvases; // Danh sách các UI canvas có sẵn
    [SerializeField] private Canvas canvas;
    [SerializeField] Camera cam;

    protected override void Awake()
    {
        base.Awake();
        InitializeUICanvases();
        canvas = GetComponent<Canvas>();
    }
    void Start()
    {
        OpenUI<Setup>();
        SwitchRenderMode(true);
       
    }
    public void SwitchRenderMode(bool isCamMode)
    {
        if (canvas == null)
        {
            canvas = GetComponent<Canvas>();
            if (canvas == null)
            {
                return;
            }
        }

        if (isCamMode)
        {
            // Chuyển sang Screen Space - Camera
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main; // Lấy Main Camera

        }
        else
        {
            // Chuyển sang Screen Space - Overlay
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.worldCamera = null; // Không cần Camera ở chế độ Overlay
        }
    }
    // Khởi tạo tất cả UI Canvas, đặt chúng ở trạng thái không hoạt động
    private void InitializeUICanvases()
    {
        foreach (var canvas in uiCanvases)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    // Mở một UI cụ thể
    public T OpenUI<T>() where T : UICanvas
    {
        T canvas = GetUI<T>();
        if (canvas != null)
        {
            canvas.Setup();
            canvas.Open();
        }

        return canvas;
    }

    // Mở một UI với vị trí cha tùy chỉnh
    public T OpenUI<T>(Transform customParent) where T : UICanvas
    {
        T canvas = GetUI<T>();
        if (canvas != null)
        {
            /* canvas.transform.SetParent(customParent, false);*/
            canvas.Setup();
            canvas.Open();
        }

        return canvas;
    }

    // Đóng UI sau một khoảng thời gian
    public void CloseUI<T>(float time) where T : UICanvas
    {
        T canvas = GetUI<T>();
        if (canvas != null)
        {
            canvas.Close(time);
        }
    }

    // Đóng UI ngay lập tức
    public void CloseUIDirectly<T>() where T : UICanvas
    {
        T canvas = GetUI<T>();
        if (canvas != null)
        {
            canvas.CloseDirectly();
        }
    }

    // Kiểm tra xem một UI có đang mở không
    public bool IsUIOpened<T>() where T : UICanvas
    {
        T canvas = GetUI<T>();
        return canvas != null && canvas.gameObject.activeSelf;
    }

    // Lấy một UI cụ thể từ danh sách
    public T GetUI<T>() where T : UICanvas
    {
        return uiCanvases.Find(c => c is T) as T;
    }

    // Kích hoạt một UI cụ thể
    public void ActiveUI<T>() where T : UICanvas
    {
        T canvas = GetUI<T>();
        if (canvas != null)
        {
            canvas.gameObject.SetActive(true);
        }
    }


    // Đóng tất cả các UI đang mở
    public void CloseAll()
    {
        foreach (var canvas in uiCanvases)
        {
            if (canvas.gameObject.activeSelf)
            {
                canvas.Close(0);
            }
        }
    }
}
