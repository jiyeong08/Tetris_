using UnityEngine;
using System;
using UnityEngine.UI;

public class re_MainPage : MonoBehaviour
{
    [SerializeField] private Button soloBtn;
    [SerializeField] private Button dualBtn;
    [SerializeField] private Button tripleBtn;
    [SerializeField] private Button exitBtn;
    [SerializeField] private Button itemBtn;
    
    private AudioSource mainBgm;

    private Image itemImg;
    private bool itemMode = false;

    public event Action<int, bool> ButtonClick;

    public void Init()
    {
        mainBgm = GetComponent<AudioSource>();
        mainBgm.clip = Resources.Load<AudioClip>("Audio/main_bgm");

        itemImg = itemBtn.GetComponent<Image>();
        
        soloBtn.onClick.AddListener(() => onStartButtonClicked(1));
        dualBtn.onClick.AddListener(() => onStartButtonClicked(2));
        tripleBtn.onClick.AddListener(() => onStartButtonClicked(3));
        exitBtn.onClick.AddListener(onExitBtnClicked);
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

    private void onStartButtonClicked(int players)
    {
        ButtonClick?.Invoke(players, itemMode);
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

    private void onExitBtnClicked()
    {
        Application.Quit(); 
    }
}
