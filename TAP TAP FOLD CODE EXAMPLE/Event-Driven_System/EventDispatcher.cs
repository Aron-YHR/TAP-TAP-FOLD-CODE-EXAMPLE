using System;
using System.Collections.Generic;


public static class EventDispatcher 
{
    private static Dictionary<string, List<Action<object>>> _eventTable = new();

    public static void Register(string eventName, Action<object> callBack)
    {
        if (!_eventTable.ContainsKey(eventName))
        {
            _eventTable[eventName] = new List<Action<object>>();
        }

        if (!_eventTable[eventName].Contains(callBack))
        {
            _eventTable[eventName].Add(callBack);
        }
    }

    public static void Unregister(string eventName, Action<object> callBack)
    {
        if(_eventTable.TryGetValue(eventName, out var list))
        {
            list.Remove(callBack);
            if(list.Count == 0)
            {
                _eventTable.Remove(eventName);
            }
        }
    }

    public static void Dispatch(string eventName, object param = null)
    {
        if(_eventTable.TryGetValue(eventName, out var list))
        {
            for(int i = list.Count - 1; i >= 0; i--)
            {
                list[i]?.Invoke(param);
            }
        }
    }

    public static void Clear()=>_eventTable.Clear();
}

public static class EventNames
{
    public const string Fold_Music_Play = nameof(Fold_Music_Play);
    public const string Scrap_Change = nameof(Scrap_Change);
    public const string Combo_Change = nameof(Combo_Change);
    public const string Star_Change = nameof(Star_Change);
    public const string OrigamiFinish = nameof(OrigamiFinish);
    public const string FinalScrap_Change = nameof(FinalScrap_Change);
    public const string Tap_Mistake = nameof(Tap_Mistake);
    public const string ScoreScreen_Change = nameof(ScoreScreen_Change);
    public const string StepIndex_Change = nameof(StepIndex_Change);
    public const string FoldLines_Change = nameof(FoldLines_Change);

    //public const string Complete_Music_Play = nameof(Complete_Music_Play);
}

public class EventSender
{
    //Send out play SFX message with different musics 
    public static void SendSFXChange(MusicType musicType)
    {

        EventDispatcher.Dispatch(EventNames.Fold_Music_Play, musicType);


    }

    //Scrap change message
    public static void SendScrapChange(int scrap)
    {
        EventDispatcher.Dispatch(EventNames.Scrap_Change,scrap);
    }

    //Scrap combo
    public static void SendComboChange(int scrap)
    {
        EventDispatcher.Dispatch(EventNames.Combo_Change, scrap);
    }

    //Star change message
    public static void SendStarChange(StarType stars)
    {
        EventDispatcher.Dispatch(EventNames.Star_Change, stars);
    }

    //origami is finished message
    public static void SendOrigamiIsFinished(OrigamiType origamiType)
    {
        EventDispatcher.Dispatch(EventNames.OrigamiFinish, origamiType);
    }

    public static void SendFinalScraps(int scrap)
    {
        EventDispatcher.Dispatch(EventNames.FinalScrap_Change, scrap);
    }

    public static void SendTapMistake()
    {
       
        EventDispatcher.Dispatch(EventNames.Tap_Mistake);
    }

    public static void SendStepIndexChange(int index)
    {
        EventDispatcher.Dispatch(EventNames.StepIndex_Change, index);
    }

    public static void SendFoldLinesChange(int num)
    {
        EventDispatcher.Dispatch(EventNames.FoldLines_Change, num);
    }

    /*public static void SendScoreScreenUpdate(ScoreScreenData scoreScreenData)
    {
        EventDispatcher.Dispatch(EventNames.ScoreScreen_Change, scoreScreenData);
    }*/
}
