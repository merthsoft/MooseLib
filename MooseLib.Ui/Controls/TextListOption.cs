namespace Merthsoft.Moose.MooseEngine.Ui.Controls
{
    public class TextListOption
    {
        public string Text { get; set; } = "";

        public bool Selected { get; set; }

        public bool Enabled { get; set; } = true;

        public char? SplitChar { get; set; } = null;

        public string FormattedText => SplitChar == null ? Text : Text.Replace(SplitChar.Value, '\n');

        public TextListOption(string text)
            => Text = text;
    }
}
