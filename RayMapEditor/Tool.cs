namespace RayMapEditor;
public enum Tool
{
    Pen,
    Line,
    Rectangle,
    RectangleFill,
    Ellipse,
    EllipseFill,
    Circle,
    CircleFill,
    Flood,
    EyeDropper
}

public static class ToolExtensions
{
    public static Image GetIcon(this Tool t)
        => t switch
        {
            Tool.Pen => Resources.icon_pen,
            Tool.Line => Resources.icon_line,
            Tool.Rectangle => Resources.icon_rectangle,
            Tool.RectangleFill => Resources.icon_rectanglefill,
            Tool.Ellipse => Resources.icon_ellipse,
            Tool.EllipseFill => Resources.icon_ellipsefill,
            Tool.Circle => Resources.icon_circle,
            Tool.CircleFill => Resources.icon_circlefill,
            Tool.Flood => Resources.icon_flood,
            Tool.EyeDropper => Resources.icon_eyedropper,
            _ => Resources.icon_pattern
        };
}