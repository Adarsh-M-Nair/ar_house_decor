using UnityEngine;

public static class UIColors
{
    public static Color Background   = HexColor("#0A0A0A");
    public static Color Surface      = HexColor("#121212");
    public static Color Gold         = HexColor("#C9A84C");
    public static Color GoldDim      = HexColor("#C9A84C44");
    public static Color TextPrimary  = HexColor("#E8E0D0");
    public static Color TextMuted    = HexColor("#555555");
    public static Color Border       = HexColor("#1E1E1E");
    public static Color DeleteRed    = HexColor("#CC4444");

    static Color HexColor(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color color);
        return color;
    }
}