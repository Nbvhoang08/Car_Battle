using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Lose : UICanvas
{
    public void MenuBtn()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Home");
        UIManager.Instance.SwitchRenderMode(true);
        UIManager.Instance.CloseUI<Lose>(0.3f);
        SoundManager.Instance.PlayClickSound();
        StartCoroutine(ResetPlayer());
        CoinManager.Instance.RemoveCoins(100);
    }
    IEnumerator ResetPlayer()
    {
        yield return new WaitForSeconds(0.5f);
        Player.Instance.rb.useGravity = false;
        Player.Instance.rb.isKinematic = true;
        Player.Instance.currentHP = Player.Instance.hp;
        Player.Instance.transform.position = new Vector3(0, 1, 0.5f);
        Player.Instance.transform.rotation = Quaternion.identity;
        Player.Instance.RemoveItem();
        UIManager.Instance.SwitchRenderMode(true);
    }
}
