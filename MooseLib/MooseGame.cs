using Merthsoft.MooseEngine.Defs;
using Merthsoft.MooseEngine.GameObjects;
using Merthsoft.MooseEngine.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Merthsoft.MooseEngine
{
    public abstract class MooseGame : Game
    {
        protected Random Random = new();

        protected bool ShouldQuit { get; set; } = false;

        public MooseContentManager ContentManager { get; set; }
        
        public OrthographicCamera MainCamera = null!;
        public SpriteBatch SpriteBatch = null!;

        public GraphicsDeviceManager Graphics { get; set; }

        public IMap MainMap = null!;

        public IDictionary<string, Def> Defs { get; } = new Dictionary<string, Def>();

        public MouseState PreviousMouseState { get; private set; }

        public MouseState CurrentMouseState { get; private set; }

        public KeyboardState PreviousKeyState { get; private set; }

        public KeyboardState CurrentKeyState { get; private set; }

        public Vector2 WorldMouse { get; private set; }

        private readonly SortedSet<GameObjectBase> Objects = new();
        public IReadOnlyList<GameObjectBase> ReadObjects => Objects.ToList().AsReadOnly();
        private readonly Queue<GameObjectBase> ObjectsToAdd = new();

        private readonly Dictionary<string, ILayerRenderer> Renderers = new();

        public virtual int MapHeight => MainMap.Height;
               
        public virtual int MapWidth => MainMap.Width;
               
        public virtual int TileWidth => MainMap.TileWidth;
               
        public virtual int TileHeight => MainMap.TileHeight;
               
        public virtual Size2 TileSize => MainMap.TileSize; // TODO: Cache
               
        public virtual Vector2 HalfTileSize => MainMap.HalfTileSize; // TODO: Cache

        public Dictionary<int, RenderHook>? DefaultRenderHooks { get; } = new();

        public int ScreenWidth => GraphicsDevice.Viewport.Width;
        public int ScreenHeight => GraphicsDevice.Viewport.Height;

        public Color DefaultBackgroundColor { get; set; } = Color.DarkCyan;

        public MooseGame()
        {
            ContentManager = new MooseContentManager(Content);
            IsMouseVisible = true;
            Graphics = new GraphicsDeviceManager(this);
        }

        protected virtual StartupParameters Startup()
            => new()
            {
                BlendState = BlendState.AlphaBlend,
                SamplerState = SamplerState.PointClamp,
                ScreenWidth = 800,
                ScreenHeight = 800,
                Fullscreen = false,
                RandomSeed = null,
            };

        protected override void Initialize()
        {
            var initialization = Startup();
            GraphicsDevice.BlendState = initialization.BlendState ?? BlendState.AlphaBlend;
            GraphicsDevice.SamplerStates[0] = initialization.SamplerState ?? SamplerState.PointClamp;
            
            Graphics.PreferredBackBufferWidth = initialization.ScreenWidth;
            Graphics.PreferredBackBufferHeight = initialization.ScreenHeight;
            Graphics.IsFullScreen = initialization.Fullscreen;
            Graphics.ApplyChanges();

            if (initialization.RandomSeed != null)
                Random = new(initialization.RandomSeed.Value);

            base.Initialize();
        }

        protected void SetScreenSize(int? width = null, int? height = null, bool? isFullScreen = null)
        {
            Graphics.PreferredBackBufferWidth = width ?? Graphics.PreferredBackBufferWidth;
            Graphics.PreferredBackBufferHeight = height ?? Graphics.PreferredBackBufferHeight;
            Graphics.IsFullScreen = isFullScreen ?? Graphics.IsFullScreen;
            Graphics.ApplyChanges();
        }

        protected virtual void Load() { }
        protected virtual void PostLoad() { }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            MainCamera = new OrthographicCamera(GraphicsDevice) { Origin = new(0, 0) };

            Load();

            Defs.AsParallel().ForEach(kvp => kvp.Value.LoadContent(ContentManager));
            
            foreach (var renderer in Renderers.Values)
                renderer.Load(MainMap);

            PostLoad();
        }

        public void AddRenderer(string rendererKey, ILayerRenderer renderer)
            => Renderers[rendererKey] = renderer;

        public TObject AddObject<TObject>(TObject gameObject, IMap? parentMap = null) where TObject : GameObjectBase
        {
            ObjectsToAdd.Enqueue(gameObject);
            gameObject.OnAdd(parentMap ?? MainMap);
            return gameObject;
        }

        public TDef AddDef<TDef>(TDef def) where TDef : Def
        {
            Defs[def.DefName] = def;
            return def;
        }

        public TDef GetDef<TDef>(string defName) where TDef : Def
            => ((Defs.GetValueOrDefault(defName) ?? Def.Empty) as TDef)!;

        protected virtual void PreRenderUpdate(GameTime gameTime) { return; }
        protected virtual void PreObjectsUpdate(GameTime gameTime) { return; }
        protected virtual void PostObjectsUpdate(GameTime gameTime) { return; }
        protected virtual void PreMapUpdate(GameTime gameTime) { return; }
        protected virtual void PostUpdate(GameTime gameTime) { return; }

        protected override void Update(GameTime gameTime)
        {
            PreviousMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
            PreviousKeyState = CurrentKeyState;
            CurrentKeyState = Keyboard.GetState();

            WorldMouse = MainCamera.ScreenToWorld(CurrentMouseState.Position.X, CurrentMouseState.Position.Y).GetFloor();

            PreRenderUpdate(gameTime);
            foreach (var renderer in Renderers.Values)
                renderer.Update(gameTime);

            PreObjectsUpdate(gameTime);
            foreach (var obj in Objects)
                obj.Update(gameTime);
            PostObjectsUpdate(gameTime);

            foreach (var obj in Objects)
                if (obj.RemoveFlag && obj.ParentMap != null && obj.ParentMap.Layers[obj.Layer] is IObjectLayer layer)
                {
                    layer.RemoveObject(obj);
                    obj.OnRemove();
                }

            Objects.RemoveWhere(obj => obj.RemoveFlag);

            foreach (var obj in ObjectsToAdd)
            {
                var map = obj.ParentMap
                    ?? throw new Exception("Object not added to a map.");
                var layer = map.Layers[obj.Layer] as IObjectLayer
                    ?? throw new Exception("Cannot add object to non-object layer");
                
                layer.AddObject(obj);
                Objects.Add(obj);
            }

            ObjectsToAdd.Clear();

            PreMapUpdate(gameTime);
            MainMap?.Update(gameTime);

            PostUpdate(gameTime);

            if (ShouldQuit)
                base.Exit();
        }

        protected void MarkAllObjectsForRemoval()
            => Objects.ForEach(o => o.RemoveFlag = true);

        protected override void Draw(GameTime gameTime)
            => Draw(gameTime, DefaultRenderHooks);

        protected void Draw(GameTime gameTime, IDictionary<int,  RenderHook>? renderHooks)
        {
            GraphicsDevice.Clear(DefaultBackgroundColor);

            if (MainMap == null)
                return;

            var transformMatrix = MainCamera.GetViewMatrix();

            for (var layerIndex = 0; layerIndex < MainMap.Layers.Count; layerIndex++)
            {
                var hookTuple = renderHooks?.GetValueOrDefault(layerIndex);
                var layer = MainMap.Layers[layerIndex];
                var renderer = Renderers[layer.RendererKey];

                renderer.Begin(transformMatrix: transformMatrix, blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
                hookTuple?.PreHook?.Invoke(layerIndex);
                renderer.Draw(gameTime, layer, layerIndex);
                hookTuple?.PostHook?.Invoke(layerIndex);
                renderer.End();
            }
        }

        public bool WasLeftMouseJustPressed()
            => CurrentMouseState.LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton == ButtonState.Released;

        public bool WasRightMouseJustPressed()
            => CurrentMouseState.RightButton == ButtonState.Pressed && PreviousMouseState.RightButton == ButtonState.Released;

        public bool IsLeftMouseDown()
            => CurrentMouseState.LeftButton == ButtonState.Pressed;

        public bool IsRightMouseDown()
            => CurrentMouseState.RightButton == ButtonState.Pressed;

        public int GetPressedKeyCount()
            => CurrentKeyState.GetPressedKeyCount();

        public Keys[] GetPressedKeys()
            => CurrentKeyState.GetPressedKeys();

        public int GetPreviousPressedKeyCount()
            => PreviousKeyState.GetPressedKeyCount();

        public Keys[] GetPreviousPressedKeys()
            => PreviousKeyState.GetPressedKeys();

        public bool WasKeyJustPressed(Keys key)
            => CurrentKeyState.IsKeyDown(key) && PreviousKeyState.IsKeyUp(key);

        public bool WasKeyJustReleased(Keys key)
            => CurrentKeyState.IsKeyUp(key) && PreviousKeyState.IsKeyDown(key);

        public bool IsKeyDown(Keys key)
            => CurrentKeyState.IsKeyDown(key);

        public bool WasKeyDown(Keys key)
            => PreviousKeyState.IsKeyDown(key);

        public bool IsKeyHeld(Keys key)
            => CurrentKeyState.IsKeyDown(key) && PreviousKeyState.IsKeyDown(key);

        public void ZoomIn(float deltaZoom)
            => MainCamera.ZoomIn(1f);

        public void ZoomOut(float deltaZoom)
            => MainCamera.ZoomOut(deltaZoom);

        public void MoveCamera(Vector2 direction)
            => MainCamera.Move(direction);

        public void Rotate(float deltaRadians)
            => MainCamera.Rotate(deltaRadians);

        public void MoveCameraTo(Vector2 destination)
            => MainCamera.LookAt(destination);
    }
}
