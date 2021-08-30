namespace Merthsoft.MooseEngine.Interface
{
    public interface ILayer
    {
        string Name { get; }
        bool IsVisible { get; set;}
        string RendererKey { get; set; }
    }
}
