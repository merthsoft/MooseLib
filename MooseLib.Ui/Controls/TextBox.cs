using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Merthsoft.Moose.MooseEngine.Ui.Controls;

public class TextBox : Control
{
    public int Width { get; set; }
    protected int Padding => Font.MeasureString("W").X.Ceiling();

    private string text = null!;
    public string Text
    {
        get => text;
        set
        {
            text = value ?? "";

            if (text == "")
            {
                CursorPosition = 0;
                ScrollPosition = 0;
            }
        }
    }

    public int CursorPosition { get; set; }
    public int ScrollPosition { get; set; }

    public TextBox(Theme theme, int x, int y, int width) : base(theme, x, y)
        => Width = width;

    public override Vector2 CalculateSize()
        => new(Width, MeasureString("M").Y + 1);

    public override void Draw(SpriteBatch spriteBatch, Vector2 parentOffset)
    {
        var backgroundColor = Theme.ResolveBackgroundColor(UpdateParameters, Enabled);
        var borderColor = Theme.ControlBorderColor;
        var textColor = Theme.ResolveTextColor(UpdateParameters, Enabled, false);

        var (x, y) = Position + parentOffset;
        var (w, h) = CalculateSize();
        spriteBatch.FillRectangle(x, y, w, h, backgroundColor);
        spriteBatch.DrawRectangle(x, y, w, h, borderColor);

        var truncatedText = TruncateString(Text[ScrollPosition..], Width, "");
        spriteBatch.DrawString(Font, truncatedText, new(x + 3, y + 1), textColor);

        var cursorX = x + 3;
        if (CursorPosition != 0)
        {
            var subString = Text.Substring(ScrollPosition, CursorPosition - ScrollPosition);
            cursorX += MeasureString(subString).X.Ceiling();
        }
        spriteBatch.DrawLine(cursorX, y + 3, cursorX, y + h - 3, Theme.ControlPointerColor, 3);
    }

    public override void Update(UpdateParameters updateParameters)
    {
        base.Update(updateParameters);

        var previousKeyState = UpdateParameters.RawKeyState;
        var currentKeyState = updateParameters.RawKeyState;

        var previousPressedKeys = previousKeyState.GetPressedKeys();
        var currentPressedKeys = currentKeyState.GetPressedKeys();

        var newChars = new List<char>();
        var shift = false;
        var back = false;
        var left = false;
        var right = false;

        var truncatedText = TruncateString(Text[ScrollPosition..], Width - Padding, "");
        var truncatedTextLength = truncatedText.Length;
        var isTruncated = truncatedTextLength != Text.Length;

        foreach (var key in currentPressedKeys)
        {
            var keyChar = (char)key;
            shift |= key == Keys.LeftShift || key == Keys.RightShift;
            if (!previousPressedKeys.Contains(key))
            {
                if (keyChar.IsPrintableAscii())
                    newChars.Add(keyChar);

                back |= key == Keys.Back || key == Keys.Delete;
                left |= key == Keys.Left;
                right |= key == Keys.Right;
            }

            if (key == Keys.LeftAlt || key == Keys.RightAlt)
                return;
        }

        if (left)
        {
            if (CursorPosition > 0)
            {
                if (isTruncated && ScrollPosition > 0 && CursorPosition - ScrollPosition - 1 == 1)
                    ScrollPosition -= 1;
                CursorPosition -= 1;
            }
            return;
        }

        if (right)
        {
            if (CursorPosition < Text.Length)
            {
                if (isTruncated && CursorPosition - ScrollPosition == truncatedTextLength)
                    ScrollPosition += 1;
                CursorPosition += 1;
            }
            return;
        }

        if (back)
        {
            if (CursorPosition == 0 || Text.Length == 0)
                return;
            if (CursorPosition == Text.Length + 1)
                Text = Text[..(CursorPosition - 1)];
            else
                Text = Text[..(CursorPosition - 1)] + Text[CursorPosition..];
            CursorPosition -= 1;
            if (CursorPosition < 0)
                CursorPosition = 0;
            if (ScrollPosition > 0)
                ScrollPosition -= 1;
            return;
        }

        foreach (var key in newChars)
        {
            var outKey = key;

            if (char.IsLetter(key) && !shift)
                outKey = char.ToLower(key);
            // this is so dirty and won't work with other keyboard layouts
            if ((char.IsNumber(key) || char.IsSymbol(key)) && shift)
                outKey = key switch
                {
                    '1' => '!',
                    '2' => '@',
                    '3' => '#',
                    '4' => '$',
                    '5' => '%',
                    '6' => '^',
                    '7' => '&',
                    '8' => '*',
                    '9' => '(',
                    '0' => ')',
                    '-' => '_',
                    '=' => '+',
                    '[' => '{',
                    ']' => '}',
                    '\\' => '|',
                    ';' => ':',
                    '\'' => '"',
                    ',' => '<',
                    '.' => '>',
                    '/' => '?',
                    _ => ' ',
                };

            if (CursorPosition > Text.Length || Text.Length == 0)
                Text += outKey;
            else if (CursorPosition == 0)
                Text = outKey + Text;
            else
                Text = Text[..CursorPosition] + outKey + Text[CursorPosition..];

            truncatedText = TruncateString(Text[ScrollPosition..], Width - Padding, "");
            truncatedTextLength = truncatedText.Length;
            isTruncated = truncatedTextLength != Text.Length;
            if (isTruncated)
                ScrollPosition += 1;

            CursorPosition += 1;
        }

        if (updateParameters.LeftMouseClick)
        {
            var (w, h) = CalculateSize();
            var (clickX, clickY) = updateParameters.LocalMousePosition;
            if (new Rectangle(0, 0, (int)w, (int)h).Intersects(clickX, clickY))
            {
                clickX -= 3;

                CursorPosition = ScrollPosition;
                var currentLength = 0f;
                foreach (var c in Text.Skip(ScrollPosition))
                {
                    currentLength += Font.MeasureString(c.ToString()).X;
                    if (currentLength > clickX)
                        break;
                    CursorPosition++;
                }
            }
        }
    }
}
