using UnityEngine;
using UnityEngine.SceneManagement;

public class Lose : UICanvas
{
    public void MenuBtn()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Home");
        UIManager.Instance.SwitchRenderMode(true);
        UIManager.Instance.CloseUI<Lose>(0.3f);
        SoundManager.Instance.PlayClickSound();

    }
}
