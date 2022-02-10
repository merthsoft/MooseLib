using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Merthsoft.Moose.MooseEngine.Ui.Controls
{
    public class Panel : Control, IControlContainer
    {
        public DockMode DockMode { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        protected List<Control> controlsToAdd = new();
        protected List<Control> controls = new();
        public Control[] Controls => controls.ToArray();

        public Window ParentWindow => Window;

        public BackgroundDrawingMode BackgroundDrawingMode { get; set; } = BackgroundDrawingMode.Basic;

        public Panel(Window window)
            : base(window, 0, 0)
        {
            DockMode = DockMode.Fill;
            Width = window.Width;
            Height = window.Height;
        }

        public Panel(Window window, int x, int y, int w, int h)
            : base(window, x, y)
        {
            DockMode = DockMode.None;
            Width = w;
            Height = h;
        }

        public override Vector2 CalculateSize()
            => new(Width, Height);

        public virtual IControlContainer AddControl(Control control)
        {
            controlsToAdd.Add(control);
            return this;
        }

        public virtual IControlContainer ClearControls()
        {
            controls.Clear();
            controlsToAdd.Clear();
            return this;
        }

        public virtual IControlContainer RemoveControl(Control control)
        {
            controls.Remove(control);
            controlsToAdd.Remove(control);
            return this;
        }

        public virtual IControlContainer RemoveControlAt(int index)
        {
            controls.RemoveAt(index);
            return this;
        }

        public override void Update(UpdateParameters updateParameters)
        {
            if (DockMode == DockMode.Fill)
            {
                X = 0;
                Y = 0;
                Width = Window.Width;
                Height = Window.Height;
            }

            PreControlUpdate(updateParameters);
            foreach (var c in Controls)
            {
                UpdateParameters controlUpdateParameters
                    = new(updateParameters.GameTime, updateParameters.LocalMousePosition - c.Position, updateParameters.RawMouseState, updateParameters.RawKeyState);
                if (c.Rectangle.Contains(updateParameters.LocalMousePosition) && !IsHidden && !c.IsHidden && updateParameters.MouseOver)
                {
                    controlUpdateParameters.MouseOver = true;
                    controlUpdateParameters.LeftMouseClick = updateParameters.LeftMouseClick;
                    controlUpdateParameters.RightMouseClick = updateParameters.RightMouseClick;
                    controlUpdateParameters.LeftMouseDown = updateParameters.LeftMouseDown;
                    controlUpdateParameters.RightMouseDown = updateParameters.RightMouseDown;
                }
                c.Update(controlUpdateParameters);
                c.UpdateParameters = controlUpdateParameters;
            }
            controls.AddRange(controlsToAdd);
            controlsToAdd.Clear();
            PostControlUpdate(updateParameters);
        }

        public virtual void PreControlUpdate(UpdateParameters updateParameters) { }
        public virtual void PostControlUpdate(UpdateParameters updateParameters) { }

        public override void Draw(SpriteBatch spriteBatch, Vector2 parentOffset)
        {
            if (IsHidden)
                return;

            var position = Position + parentOffset;
            position += Theme.DrawWindow(spriteBatch, Rectangle.Move(parentOffset), BackgroundDrawingMode);
            
            foreach (var c in Controls)
                if (!c.IsHidden)
                    c.Draw(spriteBatch, position);
        }
    }
}
