using UnityEngine;
using System;
using UnityEngine.UI;

public class MainPage : MonoBehaviour
{
    [SerializeField] private Button soloBtn;
    [SerializeField] private Button dualBtn;
    [SerializeField] private Button exitBtn;
    [SerializeField] private Button itemBtn;
    
    private AudioSource mainBgm;

    private Image itemImg;
    private bool itemMode = false;

    public event Action<Enum, bool> ButtonClick;

    public void Init()
    {
        mainBgm = GetComponent<AudioSource>();
        mainBgm.clip = Resources.Load<AudioClip>("Audio/main_bgm");

        itemImg = itemBtn.GetComponent<Image>();
        
        soloBtn.onClick.AddListener(() => onButtonClicked(State.Solo));
        dualBtn.onClick.AddListener(() => onButtonClicked(State.Dual));
        exitBtn.onClick.AddListener(() =>onButtonClicked(State.Exit));
        itemBtn.onClick.AddListener(onItemModeButtonClicked);
        
        gameObject.SetActive(false);
    }
    
    public void Show()
    {
        gameObject.SetActive(true);
        mainBgm.Play();
    }

    public void Hide()
    {
        mainBgm.Stop();
        gameObject.SetActive(false);
    }

    private void onButtonClicked(State clickedBtn)
    {
        ButtonClick?.Invoke(clickedBtn, itemMode);
    }

    private void onItemModeButtonClicked()
    {
        if (itemMode)
        {
            itemImg.sprite = Resources.Load<Sprite>("Images/item_off");
            itemMode = false;
        }
        else
        {
            itemImg.sprite = Resources.Load<Sprite>("Images/item_on");
            itemMode = true;
        }
    }

}