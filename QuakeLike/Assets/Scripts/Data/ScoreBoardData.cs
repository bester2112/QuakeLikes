using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScoreBoardData : IComparer<ScoreBoardData>
{
    public string username;
    public int kills;
    public int deaths;
    public bool active = false;

    public ScoreBoardData(string username, int kills, int deaths)
    {
        this.username = username;
        this.kills = kills;
        this.deaths = deaths;
        this.active = true;
    }

    public int Compare(ScoreBoardData x, ScoreBoardData y)
    {
        if (x == null) return 1;
        if (y == null) return -1;

        if (!y.active)
        {
            return -1;
        }

        int killDiff = y.kills - x.kills;
        if (killDiff != 0)
        {
            return killDiff;
        }
        return x.deaths - y.deaths;
    }
}
