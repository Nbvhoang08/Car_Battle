using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Victory : UICanvas
{
    // Start is called before the first frame update
    public void MenuBtn()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Home");
        UIManager.Instance.SwitchRenderMode(true);
        UIManager.Instance.CloseUI<Lose>(0.3f);
        SoundManager.Instance.PlayClickSound();

    }
}
