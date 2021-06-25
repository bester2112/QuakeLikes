using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerUIController : MonoBehaviour
{

    public LocalPlayerController playerController;

    public Text fpsCounter;

    public Text speedometer;
    public Text timer;

    public Text topKills;
    public Text ownKills;

    public GameObject GameHUD;
    public GameObject ScoreBoard;

    public GameObject[] columns;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (LocalPlayerController.alive)
        {
            GameHUD.SetActive(true);
            ScoreBoard.SetActive(false);
            updateGameHud();
        }
        else
        {
            GameHUD.SetActive(false);
            ScoreBoard.SetActive(true);
            updateScoreBoard();
        }

        if (Input.GetKey(KeyCode.Tab))
        {
            ScoreBoard.SetActive(true);
            updateScoreBoard();
        }
    }

    private void updateGameHud()
    {
        fpsCounter.text = "FPS: " + (1.0f / Time.deltaTime);

        int speed = (int)(playerController.getSpeed() * 45.72f);

        if (speed > 0)
        {
            speedometer.text = speed.ToString();
        }
        else
        {
            speedometer.text = "0";
        }

        float remainingTime = playerController.timer.getRemainingTime();
        if (remainingTime > 0.0f)
        {
            timer.text = remainingTime.ToString();
        }
        else
        {
            timer.text = "";
        }

        int maxKills = 0;
        ConnectedPlayerData topKiller = ClientGameInstance.players[ConnectionToServer.instance.myId];
        ConnectedPlayerData own = ClientGameInstance.players[ConnectionToServer.instance.myId];

        foreach (ConnectedPlayerData cpd in ClientGameInstance.players.Values)
        {
            if (cpd != null)
            {
                if (cpd.kills > maxKills)
                {
                    topKiller = cpd;
                    maxKills = cpd.kills;
                }
                if (cpd.kills == maxKills)
                {
                    if (cpd.deaths < topKiller.deaths)
                    {
                        topKiller = cpd;
                    }
                }
            }
        }

        topKills.text = topKiller.kills + " " + topKiller.username;
        ownKills.text = own.kills + " " + own.username;
    }

    private void updateScoreBoard()
    {
        ScoreBoardData[] scoreBoardColumns = new ScoreBoardData[16];

        int used = 0;
        foreach (ConnectedPlayerData cpd in ClientGameInstance.players.Values)
        {
            if (cpd != null)
            {
                scoreBoardColumns[used] = new ScoreBoardData(cpd.username, cpd.kills, cpd.deaths);
                used++;
            }
        }

        Array.Sort(scoreBoardColumns, new ScoreBoardData("",0,0));

        for (int i = 0; i < scoreBoardColumns.Length; i++)
        {
            if (scoreBoardColumns[i] != null)
            {
                columns[i].SetActive(true);
                Text[] items = columns[i].GetComponentsInChildren<Text>();

                items[1].text = scoreBoardColumns[i].username;
                items[2].text = scoreBoardColumns[i].kills.ToString();
                items[3].text = scoreBoardColumns[i].deaths.ToString();
            }
            else
            {
                columns[i].SetActive(false);
            }
        }
        //Debug.Log(ClientGameInstance.players.Values.Count + " " + scoreBoardColumns.Length);
    }
}
