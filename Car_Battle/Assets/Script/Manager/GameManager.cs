using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Enemy enemy;
    public bool GameOver;
    private void Start()
    {
        GameOver = false;
    }
    public void CheckWinCondition()
    {
        if (enemy.currentHP <= 0)
        {
            Debug.Log("You Win!"); // Player wins
            // Gọi các hàm hoặc xử lý kết thúc game ở đây
            EndGame(true);
        }
    }

    public void CheckLoseCondition()
    {
        if (Player.Instance.currentHP <= 0)
        {
            Debug.Log("You Lose!"); // Player loses
            // Gọi các hàm hoặc xử lý kết thúc game ở đây
            EndGame(false);
        }
       
    }

    private void Update()
    {
        if(GameOver) return;
        // Kiểm tra điều kiện thắng hoặc thua liên tục
        CheckWinCondition();
        CheckLoseCondition();
  
    }

    private void EndGame(bool isWin)
    {
        if (isWin)
        {
            // Xử lý khi thắng (chuyển scene, hiển thị UI thắng, v.v.)
            UIManager.Instance.OpenUI<Victory>();
            Debug.Log("Game Over: Victory!");
        
        }
        else
        {
            UIManager.Instance.OpenUI<Lose>();
            // Xử lý khi thua (chuyển scene, hiển thị UI thua, v.v.)
            Debug.Log("Game Over: Defeat!");
        }

        // Có thể dừng thời gian để kết thúc game
        Time.timeScale = 0;
        GameOver = true;
    }
}
