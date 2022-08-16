using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timer
{

    public timer(float totalTime)
    {
        this.totalTime = totalTime;
        this.currentTime = 0;
    }

    public void restartTimer()
    {
        this.currentTime = 0;
    }
    float totalTime;
    private float currentTime;

    public void updateTimer()
    {
        currentTime += Time.deltaTime;
    }

    public bool isCompleted()
    {
        return currentTime >= totalTime;
    }
}
