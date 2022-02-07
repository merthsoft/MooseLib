namespace Merthsoft.Moose.MooseEngine.Ui.Controls
{
    public abstract class FlowPanel : Panel
    {
        public FlowPanel(Window window) 
            : base(window) { }
        public FlowPanel(Window window, int x, int y, int w, int h) 
            : base(window, x, y, w, h) { }

        public override void PreControlUpdate(UpdateParameters updateParameters)
        {
            if (Controls.Length > 0)
                Flow();
        }
        
        protected abstract void Flow();
    }
}