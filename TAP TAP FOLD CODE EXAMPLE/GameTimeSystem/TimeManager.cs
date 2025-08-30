using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    [Header("Internal Clock")]
    [SerializeField]
    public GameTimestamp gameTimestamp;

    public float timeScale = 1.0f;

    //Observer Pattern
    //a list of objects to inform of changes to the time. time manager as observer class
    public List<ITimeTracker> listeners = new List<ITimeTracker>(); 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Initialise the time stamp
        gameTimestamp = new GameTimestamp(DateTime.Now.Day,DateTime.Now.Hour,DateTime.Now.Minute);
        StartCoroutine(TimeUpdate());
    }

    IEnumerator TimeUpdate()
    {
        while (true)
        {
            //yield return new WaitForSecondsRealtime(60f);
            yield return new WaitForSeconds(60f / timeScale);
            Tick();
        }
   
    }

    public void Tick() // A tick of the in-game time
    {
        gameTimestamp.UpdateClock();

        //Inform the listeners of the new time state
        foreach(ITimeTracker listener in listeners)
        {
            listener.ClockUpdate(gameTimestamp);
        }
    }

    //Handling Listeners
    //Add the object to the list of listeners
    public void RegisterTracker(ITimeTracker listener)
    {
        listeners.Add(listener);
    }

    //Remove the object from the list of listeners
    public void UnregisterTracker(ITimeTracker listener)
    {
        listeners.Remove(listener);
    }
}
