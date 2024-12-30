using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Setup : UICanvas
{
    public Text CoinsText;
    private void Update()
    {
        if (CoinsText != null) 
        {
            CoinsText.text = CoinManager.Instance.GetCoins().ToString();
        }
    }
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
        UIManager.Instance.SwitchRenderMode(true);
    }
    public void RemoveBtn()
    {
        Player.Instance.RemoveItem();
    }
}
