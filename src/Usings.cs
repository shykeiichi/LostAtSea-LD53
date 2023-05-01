global using System.Numerics;
global using Fjord;
global using Fjord.Graphics;
global using Fjord.Input;
global using Fjord.Scenes;
global using Fjord.Ui;
global using Newtonsoft.Json;
global using static SDL2.SDL;

public static class LD
{
    public static void DrawSmallFont(Vector2 pos, string text, int depth=5000)
    {
        List<char> chars = new List<char>() {
            'a',
            'b',
            'c',
            'd',
            'e',
            'f',
            'g',
            'h',
            'i',
            'j',
            'k',
            'l',
            'm',
            'n',
            'o',
            'p',
            'q',
            'r',
            's',
            't',
            'u',
            'v',
            'w',
            'x',
            'y',
            'z',
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7',
            '8',
            '9',
            '0',
            ',',
            '.',
            '<',
            '>'
        };

        int index = -1;
        foreach(char i in text)
        {
            index ++;
            if(chars.Contains(char.ToLower(i)))
            {
                new Texture("Images/SmallFont.png")
                    .SrcTextureOffset(new(3 * chars.IndexOf(char.ToLower(i)), 0, 3, 4))
                    .Size(new(3, 4))
                    .Position(new(index * 4 + pos.X, 0 + pos.Y))
                    .Depth(depth)
                    .Render();
            }
        }
    }

    public static void DrawButton(Vector2 pos, string text, Vector2 mpos, Action callback)
    {
        LD.DrawSmallFont(new(pos.X + 2, pos.Y + 2), text);

        Vector4 Color = new(235, 214, 190, 255);

        if(Helpers.PointInside(mpos, new Vector4(pos.X, pos.Y, 4 * text.Length + 4, 8)))
        {
            Color = new(205, 184, 160, 255);

            if(GlobalMouse.Pressed(MB.Left))
            {
                callback();
            }
        }

        Draw.Box(new(pos.X, pos.Y, 4 * text.Length + 4, 8), Color);
    }
}