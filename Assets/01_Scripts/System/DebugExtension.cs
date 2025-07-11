using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public static class DebugExtension
{
    public static void ColorLog(this string message, string color)
    {
        Debug.Log($"<color={color}>{message}</color>");
    }

    public static void BoldLog(this string message)
    {
        Debug.Log($"<b>{message}</b>");
    }

    public static void ItalicLog(this string message)
    {
        Debug.Log($"<i>{message}</i>");
    }
}
