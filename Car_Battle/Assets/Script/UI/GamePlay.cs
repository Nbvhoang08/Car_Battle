using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlay : UICanvas
{
    // Start is called before the first frame update

    [SerializeField] Slider playerHp;
    [SerializeField] Slider enemyHp;
    [Header("Sound Setting")]
    public Sprite OnVolume;
    public Sprite OffVolume;
    [SerializeField] private Image buttonImage;
    void Start()
    {
        
    }
    public void SetMove(float inputMove)
    {
        Player.Instance.horizontalInput= inputMove;
    }


    // Update is called once per frame
    void Update()
    {
        UpdateButtonImage();
    }
    public void SoundBtn()
    {
        SoundManager.Instance.TurnOn = !SoundManager.Instance.TurnOn;
        UpdateButtonImage();
        SoundManager.Instance.PlayClickSound();

    }

    private void UpdateButtonImage()
    {
        if (SoundManager.Instance.TurnOn)
        {
            buttonImage.sprite = OnVolume;
        }
        else
        {
            buttonImage.sprite = OffVolume;
        }
    }
}
