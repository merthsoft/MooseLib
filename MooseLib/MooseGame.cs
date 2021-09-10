using Merthsoft.Moose.MooseEngine.Defs;
using Merthsoft.Moose.MooseEngine.GameObjects;
using Merthsoft.Moose.MooseEngine.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Merthsoft.Moose.MooseEngine
{
    public abstract class MooseGame : Game
    {
        protected Random Random = new();

        protected bool ShouldQuit { get; set; } = false;

        public MooseContentManager ContentManager { get; set; } = null!; // Set in initialize
        
        public OrthographicCamera MainCamera = null!; // Set in load content
        public SpriteBatch SpriteBatch = null!; // Set in load content

        public GraphicsDeviceManager Graphics { get; set; } = null!;

        public IMap MainMap => ActiveMaps.Any() ? ActiveMaps[0] : NullMap.Instance;
        public IList<IMap> ActiveMaps = new List<IMap>();

        public IDictionary<string, Def> Defs { get; } = new Dictionary<string, Def>();

        public MouseState PreviousMouseState { get; private set; }

        public MouseState CurrentMouseState { get; private set; }

        public KeyboardState PreviousKeyState { get; private set; }

        public KeyboardState CurrentKeyState { get; private set; }

        public Vector2 WorldMouse { get; private set; }

        private readonly SortedSet<GameObjectBase> Objects = new();
        public IReadOnlyList<GameObjectBase> ReadObjects => Objects.ToList().AsReadOnly();
        private readonly Queue<GameObjectBase> ObjectsToAdd = new();

        private readonly Dictionary<Type, string> DefaultRenderers = new();
        private readonly Dictionary<string, ILayerRenderer> RendererDictionary = new();
        private readonly Dictionary<int, string> LayerRenderers = new();

        public virtual int MapHeight => MainMap?.Height ?? 0;
               
        public virtual int MapWidth => MainMap?.Width ?? 0;
               
        public virtual int TileWidth => MainMap?.TileWidth ?? 0;
               
        public virtual int TileHeight => MainMap?.TileHeight ?? 0;
               
        public virtual Size2 TileSize => MainMap?.TileSize ?? new(0, 0); // TODO: Cache
               
        public virtual Vector2 HalfTileSize => MainMap?.HalfTileSize ?? new Vector2(0, 0); // TODO: Cache

        public virtual IDictionary<int, RenderHook>? DefaultRenderHooks => null;

        public int ScreenWidth => GraphicsDevice.Viewport.Width;
        public int ScreenHeight => GraphicsDevice.Viewport.Height;

        public Color DefaultBackgroundColor { get; set; } = Color.DarkCyan;

        public MooseGame()
        {
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
            ContentManager = new MooseContentManager(Content, GraphicsDevice);

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

            Defs.ForEach(kvp => kvp.Value.LoadContent(ContentManager));

            PostLoad();
        }

        public ILayerRenderer GetRenderer(string rendererKey)
            => RendererDictionary[rendererKey];

        public TRenderer GetRenderer<TRenderer>(string rendererKey)
            where TRenderer : ILayerRenderer
            => (TRenderer)RendererDictionary[rendererKey];

        public void AddDefaultRenderer<TLayer>(string rendererKey, ILayerRenderer renderer, params int[] layerIndexes)
            where TLayer : ILayer
        {
            DefaultRenderers[typeof(TLayer)] = rendererKey;
            AddRenderer(rendererKey, renderer, layerIndexes);
        }

        public void AddRenderer(string rendererKey, ILayerRenderer renderer, params int[] layerIndexes)
        {
            RendererDictionary[rendererKey] = renderer;
            foreach (var layerIndex in layerIndexes)
                LayerRenderers[layerIndex] = rendererKey;
        }

        public void SetRenderer(int layerIndex, string rendererKey)
            => LayerRenderers[layerIndex] = rendererKey;

        public void SetDefaultRenderer<TRenderer>(string rendererKey) where TRenderer : ILayer
            => DefaultRenderers[typeof(TRenderer)] = rendererKey;

        public TObject AddObject<TObject>(TObject gameObject, IMap? parentMap = null) where TObject : GameObjectBase
        {
            ObjectsToAdd.Enqueue(gameObject);
            gameObject.ParentMap = parentMap ?? MainMap;
            return gameObject;
        }

        public TDef AddDef<TDef>(TDef def) where TDef : Def
        {
            Defs[def.DefName] = def;
            return def;
        }

        public TDef GetDef<TDef>(string defName) where TDef : Def
            => ((Defs.GetValueOrDefault(defName) ?? Def.Empty) as TDef)!;

        protected virtual bool PreRenderUpdate(GameTime gameTime) => true;
        protected virtual bool PreObjectsUpdate(GameTime gameTime) => true;
        protected virtual void PostObjectsUpdate(GameTime gameTime) { return; }
        protected virtual bool PreMapUpdate(GameTime gameTime) => true;
        protected virtual void PostUpdate(GameTime gameTime) { return; }

        protected override void Update(GameTime gameTime)
        {
            PreviousMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
            PreviousKeyState = CurrentKeyState;
            CurrentKeyState = Keyboard.GetState();

            WorldMouse = MainCamera.ScreenToWorld(CurrentMouseState.Position.X, CurrentMouseState.Position.Y).GetFloor();

            if (PreRenderUpdate(gameTime))
                foreach (var renderer in RendererDictionary.Values)
                    renderer.Update(gameTime);

            if (PreMapUpdate(gameTime) && ActiveMaps.Any())
                foreach (var map in ActiveMaps)
                    map.Update(gameTime);

            if (PreObjectsUpdate(gameTime))
                foreach (var obj in Objects)
                    obj.Update(gameTime);
            PostObjectsUpdate(gameTime);

            foreach (var obj in Objects.Where(obj => obj.RemoveFlag))
                if (obj.ParentMap != null && obj.ParentMap.Layers[obj.Layer] is IObjectLayer layer)
                {
                    layer.RemoveObject(obj);
                    obj.OnRemove();
                    obj.ParentMap = null;
                }

            Objects.RemoveWhere(obj => obj.RemoveFlag);

            foreach (var obj in ObjectsToAdd)
            {
                Objects.Add(obj);
                var layer = obj.ParentMap?.Layers[obj.Layer] as IObjectLayer;
                layer?.AddObject(obj);
                obj.OnAdd();
            }

            ObjectsToAdd.Clear();

            PostUpdate(gameTime);

            if (ShouldQuit)
                base.Exit();
        }

        protected void MarkAllObjectsForRemoval()
            => Objects.ForEach(o => o.RemoveFlag = true);

        protected override void Draw(GameTime gameTime)
            => Draw(gameTime, DefaultRenderHooks);

        protected virtual bool PreClear(GameTime gameTime) => true;
        protected virtual bool PreMapDraw(GameTime gameTime) => true;
        protected virtual void PostDraw(GameTime gameTime) { return; }

        protected void Draw(GameTime gameTime, IDictionary<int, RenderHook>? renderHooks)
        {
            if (PreClear(gameTime))
                GraphicsDevice.Clear(DefaultBackgroundColor);
            
            var transformMatrix = MainCamera.GetViewMatrix();

            if (PreMapDraw(gameTime) && ActiveMaps.Any())
                foreach (var map in ActiveMaps)
                    DrawMap(map, gameTime, renderHooks, transformMatrix);

            PostDraw(gameTime);
        }

        private void DrawMap(IMap map, GameTime gameTime, IDictionary<int, RenderHook>? renderHooks, Matrix transformMatrix)
        {
            for (var layerIndex = 0; layerIndex < map.Layers.Count; layerIndex++)
            {
                var hookTuple = renderHooks?.GetValueOrDefault(layerIndex);
                var layer = map.Layers[layerIndex];
                
                ILayerRenderer? renderer = null;
                var rendererKey = LayerRenderers.GetValueOrDefault(layerIndex);
                if (renderer == null)
                    rendererKey = DefaultRenderers.GetValueOrDefault(layer.GetType());
                if (rendererKey != null)
                    renderer = RendererDictionary[rendererKey];

                renderer?.Begin(transformMatrix);
                hookTuple?.PreHook?.Invoke(layerIndex);
                
                if (!layer.IsHidden)
                    renderer?.Draw(gameTime, layer, layerIndex);
                
                hookTuple?.PostHook?.Invoke(layerIndex);
                renderer?.End();
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
            => MainCamera.ZoomIn(deltaZoom);

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
