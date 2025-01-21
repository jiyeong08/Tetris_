using TMPro;
using UnityEngine;

public class re_GamePlayUI : MonoBehaviour
{
    [Header("player1")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text lineText;
    [SerializeField] private TMP_Text bombItem;
    [SerializeField] private TMP_Text changeItem;
    
    public void Init()
    {
        gameObject.SetActive(false);
    }

    public void GameStart()
    {
        gameObject.SetActive(true);
    }

    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    public void UpdateLevel(int level)
    {
        levelText.text = $"Level: {level}";
    }

    public void UpdateLine(int line)
    {
        lineText.text = $"Line: {line}";
    }

    public void UpdateBomb(int bomb)
    {
        bombItem.text = $"Bomb: {bomb}";
    }

    public void UpdateChange(int change)
    {
        changeItem.text = $"Change: {change}";
    }

    public void GamePlayUIOff()
    {
        gameObject.SetActive(false);
    }
}
