using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private float timerRestartTime;

    private float remainingTime;

    private bool finished = true;

    // Start is called before the first frame update
    void Start()
    {
        remainingTime = timerRestartTime;
    }

    // Update is called once per frame
    public void Update()
    {
        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0.0f)
        {
            remainingTime = 0.0f;
            finished = true;
        }
    }

    public void startTimer(float timerRestartTime)
    {
        this.timerRestartTime = timerRestartTime;
        this.remainingTime = timerRestartTime;
        finished = false;
    }

    public bool isFinished()
    {
        return finished;
    }

    public float getRemainingTime()
    {
        return remainingTime;
    }
}
