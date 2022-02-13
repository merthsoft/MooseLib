namespace Merthsoft.Moose.MooseEngine.Ui.Controls;

public class Option
{
    public string Text { get; set; } = "";
    public object? Tag { get; set; }

    public bool Selected { get; set; }

    public bool Enabled { get; set; } = true;

    public char? SplitChar { get; set; } = null;

    public string FormattedText => SplitChar == null ? Text : Text.Replace(SplitChar.Value, '\n');

    public Option(string text)
        => Text = text;

    public TTag? GetTag<TTag>()
        => (TTag?)Tag;
}
