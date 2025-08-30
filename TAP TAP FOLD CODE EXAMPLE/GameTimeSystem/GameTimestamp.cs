using System;
using UnityEngine;

[Serializable]
public class GameTimestamp 
{
    //public int year;
    //public int month;
    public int day;
    public int hour;
    public int minute;

    public GameTimestamp(int day, int hour, int minute) 
    {
        this.day = day;
        this.hour = hour;
        this.minute = minute;
    }

    public void UpdateClock()
    {
        minute++; 

        if(minute >= 60)
        {
            minute = 0;
            hour++;

        }

        if(hour >= 24)
        {
            hour = 0;

            day++;
        }

        if (day > 30)
        {
            day = 1;

            //seasons change if need
        }
    }

    public void GetDayOfTheWeek()
    {
        DayOfWeek dayOfWeek = DateTime.Now.DayOfWeek;
    }

    public static int hoursToMinutes(int hour)
    {
        return hour * 60;
    }

    public static int DaysToHours(int days)
    {
        return days * 24;
    }

   
}
