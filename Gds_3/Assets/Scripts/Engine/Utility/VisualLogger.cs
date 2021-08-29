using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ai;
using NaughtyAttributes;

public class VisualLogger : MonoBehaviour
{
    public class LogData
    {
        public string text;
        public Color color;
        public Vector2 locationOffset;
        public Vector2 size = new Vector2(120, 50);
        public bool enabled = true;
    }
    readonly List<LogData> _logDataList = new List<LogData>();

    public LogData AddLog(Vector2 locationOffset, Color color, string text = "", bool enabled = true)
    {
        LogData data = new LogData();
        data.color = color;
        data.text = text;
        data.locationOffset = locationOffset;
        data.enabled = enabled;

        _logDataList.Add(data);

        return data;
    }

    public LogData AddLog(Vector2 locationOffset, string text = "", bool enabled = true)
    {
        return AddLog(locationOffset, Color.black, text, enabled);
    }

    public void RemoveLog(LogData log)
    {
        _logDataList.Remove(log);
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        var c = Camera.main;
        foreach (var it in _logDataList)
        {
            if (it == null || !it.enabled)
                continue;

            Vector2 loc = new Vector2(
                c.WorldToScreenPoint(transform.position).x + it.locationOffset.x,
                Screen.height - c.WorldToScreenPoint(transform.position).y + it.locationOffset.y);

            GUI.color = it.color;
            GUI.Label(new Rect(loc, it.size), it.text);
        }
    }
#endif
}
