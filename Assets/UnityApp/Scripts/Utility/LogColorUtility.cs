using UnityEngine;

public static class LogColorUtility
{
    // ��չ���������ڸ��ַ���������ɫ
    public static string SetColor(this object text, Color color)
    {
        // �� Color ת��ʮ������
        string hexColor = ColorUtility.ToHtmlStringRGB(color);
        return $"<color=#{hexColor}>{text}</color>";
    }

    // ��ѡ��֧�� Debug.Log ��ɫ���
    public static void Log(string text)
    {
        Debug.Log(text);
    }

    public static void Log(string text, Color color)
    {
        Debug.Log(text.SetColor(color));
    }

    public static void LogFormat(string format, params object[] args)
    {
        Debug.Log(string.Format(format, args));
    }
}
