using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : Singleton<CoinManager>
{
    private const string CoinsKey = "Coins"; // Khóa lưu trữ trong PlayerPrefs
    /// <summary>
    /// Thêm số lượng coin.
    /// </summary>
    /// <param name="amount">Số coin muốn thêm.</param>
    public void AddCoins(int amount)
    {
        int currentCoins = PlayerPrefs.GetInt(CoinsKey, 500); // Lấy số coin hiện tại
        currentCoins += amount; // Cộng thêm số coin
        PlayerPrefs.SetInt(CoinsKey, currentCoins); // Lưu lại số coin mới
        PlayerPrefs.Save(); // Lưu thay đổi xuống bộ nhớ
    }

    /// <summary>
    /// Giảm số lượng coin.
    /// </summary>
    /// <param name="amount">Số coin muốn giảm.</param>
    public void RemoveCoins(int amount)
    {
        int currentCoins = PlayerPrefs.GetInt(CoinsKey, 500); // Lấy số coin hiện tại
        currentCoins = Mathf.Max(0, currentCoins - amount); // Đảm bảo không giảm xuống dưới 0
        PlayerPrefs.SetInt(CoinsKey, currentCoins); // Lưu lại số coin mới
        Debug.Log("tinh tien");
        PlayerPrefs.Save(); // Lưu thay đổi xuống bộ nhớ
    }

    /// <summary>
    /// Lấy số coin hiện tại.
    /// </summary>
    /// <returns>Số coin hiện tại được lưu trữ.</returns>
    public int GetCoins()
    {
        return PlayerPrefs.GetInt(CoinsKey, 500); // Lấy giá trị coin hiện tại
    }
}
