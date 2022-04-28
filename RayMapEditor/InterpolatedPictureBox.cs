using System.Drawing.Drawing2D;

namespace RayMapEditor;
internal class InterpolatedPictureBox : PictureBox
{
    public InterpolationMode InterpolationMode { get; set; } = InterpolationMode.NearestNeighbor;

    protected override void OnPaint(PaintEventArgs paintEventArgs)
    {
        paintEventArgs.Graphics.InterpolationMode = InterpolationMode;
        base.OnPaint(paintEventArgs);
    }
}
