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
        UIManager.Instance.CloseUI<Victory>(0.6f);
        UIManager.Instance.OpenUI<Setup>();
        SoundManager.Instance.PlayClickSound();
        StartCoroutine(ResetPlayer());
        CoinManager.Instance.AddCoins(200);
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
