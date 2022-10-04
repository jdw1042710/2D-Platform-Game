using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int totalScore;
    public int stageScore;
    public int stageIndex;
    public GameObject[] Stages;
    public PlayerMove Player;
    //UI
    public Image[] UIHealth;
    public TextMeshProUGUI UIPoint;
    public TextMeshProUGUI UIStage;
    public GameObject UIRestartBtn;


    public void GetScore(GameObject item) {
        bool isBronze = item.gameObject.name.Contains("Bronze");
        bool isSilver = item.gameObject.name.Contains("Silver");
        bool isGold = item.gameObject.name.Contains("Gold");

        if (isBronze) {
            stageScore += 50;
        } else if (isSilver) {
            stageScore += 100;
        } else if (isGold) {
            stageScore += 300;
        }
        UIPoint.text = $"{totalScore + stageScore}";
    }

    public void NextStage() {
        Stages[stageIndex].SetActive(false);
        stageIndex++;
        //Change Stage
        if (stageIndex >= Stages.Length) {
            // time stop
            Time.timeScale = 0;
            // Result UI
            Debug.Log("게임종료");
            
            TextMeshProUGUI btnText = UIRestartBtn.GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = "Clear!";
            UIRestartBtn.SetActive(true);
            return;
        }

        Player.ReSpawn();
        Stages[stageIndex].SetActive(true);

        UIStage.text = $"STAGE {stageIndex + 1}";

        //Score 
        totalScore += stageScore;
        stageScore = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag) {
            case "Player":
                Player.HPDown();
                Player.ReSpawn();
                break;
        }
    }

    public void HPDown() {
        UIHealth[Player.HP].color = new Color(1, 1, 1, 0);
    }

    public void OnDie() {
        UIRestartBtn.SetActive(true);
    }

    public void Restart() {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
