using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Setup : UICanvas
{
    public void StartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Game");
        UIManager.Instance.OpenUI<GamePlay>();
        UIManager.Instance.CloseUI<Setup>(0.3f);
        Player.Instance.RefreshWheels();
        Player.Instance.rb.useGravity = true;
        Player.Instance.rb.isKinematic = false;
        SoundManager.Instance.PlayClickSound();
    }
    public void RemoveBtn()
    {
        Player.Instance.RemoveItem();
    }
}
