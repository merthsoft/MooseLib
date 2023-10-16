using Merthsoft.Moose.MooseEngine.Defs;
using Merthsoft.Moose.MooseEngine.GameObjects;
using Merthsoft.Moose.MooseEngine.Interface;
using Merthsoft.Moose.MooseEngine.Noise;
using MonoGame.Extended.Tweening;
using MonoGame.Extended.ViewportAdapters;
using System.Linq.Expressions;

namespace Merthsoft.Moose.MooseEngine;

public abstract class MooseGame : Game
{
    public static MooseGame Instance { get; protected set; } = null!;
    public static Random Random { get; protected set; } = null!;
    public static int RandomSeed { get; protected set; } = (int)DateTime.Now.Ticks;

    public Tweener Tweener { get; protected set; } = new();
    public MooseContentManager ContentManager { get; protected set; } = null!; // Set in initialize
    public OrthographicCamera MainCamera { get; protected set; } = null!; // Set in load content

    protected bool ShouldQuit { get; set; } = false;
    protected GraphicsDeviceManager Graphics { get; set; } = null!;

    public SpriteBatch SpriteBatch = null!; // Set in load 

    public IMap MainMap => ActiveMaps.First();
    public IList<IMap> ActiveMaps = new List<IMap>();

    public Dictionary<string, Def> Defs { get; } = new Dictionary<string, Def>();

    public List<MouseState> PreviousMouseStates { get; } = new();
    public MouseState PreviousMouseState => PreviousMouseStates[^1];
    public MouseState CurrentMouseState { get; private set; }

    public List<KeyboardState> PreviousKeyStates { get; } = new();
    public KeyboardState PreviousKeyState => PreviousKeyStates[^1];

    public KeyboardState CurrentKeyState { get; private set; }
    
    public bool IsActiveAndMouseInBounds
    {
        get
        {
            var (mx, my) = CurrentMouseState.Position;
            return IsActive && mx >= 0 && mx < ScreenWidth && my >= 0 && my < ScreenHeight;
        }
    }

    public Vector2 WorldMouse { get; private set; }

    private readonly List<GameObjectBase> Objects = new();
    public IList<GameObjectBase> ReadObjects => Objects;
    private readonly Queue<GameObjectBase> ObjectsToAdd = new();

    private readonly Dictionary<Type, string> DefaultRenderers = new();
    private readonly Dictionary<string, ILayerRenderer> RendererDictionary = new();

    public virtual int MapHeight => MainMap?.Height ?? 0;

    public virtual int MapWidth => MainMap?.Width ?? 0;

    public virtual int TileWidth => MainMap?.TileWidth ?? 0;

    public virtual int TileHeight => MainMap?.TileHeight ?? 0;

    public virtual Size2 TileSize => MainMap?.TileSize ?? new(0, 0); // TODO: Cache

    public virtual Vector2 HalfTileSize => MainMap?.HalfTileSize ?? new Vector2(0, 0); // TODO: Cache

    public virtual Dictionary<string, RenderHook>? DefaultRenderHooks { get; set; } = null;

    public int ScreenWidth => GraphicsDevice.Viewport.Width;
    public int ScreenHeight => GraphicsDevice.Viewport.Height;
    public Vector2 ScreenSize => new(ScreenWidth, ScreenHeight);

    public Color DefaultBackgroundColor { get; set; } = Color.DarkCyan;

    protected FramesPerSecondCounter FramesPerSecondCounter { get; } = new();


    public MooseGame()
    {
        Instance = this;

        IsMouseVisible = true;
        Graphics = new GraphicsDeviceManager(this);
        Random = new(RandomSeed);
        SimplexNoise.Seed = RandomSeed;
    }

    public void SetSeed(int seed)
    {
        RandomSeed = seed;
        Random = new(RandomSeed);
        SimplexNoise.Seed = RandomSeed;
    }

    protected virtual StartupParameters Startup()
        => new()
        {
            BlendState = BlendState.AlphaBlend,
            SamplerState = SamplerState.PointClamp,
            ScreenWidth = 800,
            ScreenHeight = 800,
            IsFullscreen = false,
            RandomSeed = null,
            StateStackSize = 10,
            CameraRectangle = null,
            DefaultBackgroundColor = Color.DarkCyan
        };

    protected override void Initialize()
    {
        ContentManager = new MooseContentManager(this, Content, GraphicsDevice);
        var initialization = Startup();
        IsMouseVisible = initialization.IsMouseVisible;

        GraphicsDevice.BlendState = initialization.BlendState ?? BlendState.AlphaBlend;
        GraphicsDevice.SamplerStates[0] = initialization.SamplerState ?? SamplerState.PointClamp;

        Graphics.PreferredBackBufferWidth = initialization.ScreenWidth;
        Graphics.PreferredBackBufferHeight = initialization.ScreenHeight;
        Graphics.IsFullScreen = initialization.IsFullscreen;
        Graphics.ApplyChanges();

        if (initialization.CameraRectangle != null)
        {
            var cameraRect = initialization.CameraRectangle.Value;
            var cameraViewport = new BoxingViewportAdapter(Window, GraphicsDevice, cameraRect.Width, cameraRect.Height);
            MainCamera = new OrthographicCamera(cameraViewport) { Origin = cameraRect.Location.ToVector2() };
        } else
            MainCamera = new OrthographicCamera(GraphicsDevice) { Origin = Vector2.Zero };

        if (initialization.RandomSeed != null)
            Random = new(initialization.RandomSeed.Value);

        for (var i = 0; i < initialization.StateStackSize; i++)
        {
            PreviousMouseStates.Add(default);
            PreviousKeyStates.Add(default);
        }

        DefaultBackgroundColor = initialization.DefaultBackgroundColor;

        base.Initialize();
    }

    protected void SetScreenSize(int? width = null, int? height = null, bool? isFullScreen = null)
    {
        Graphics.PreferredBackBufferWidth = width ?? Graphics.PreferredBackBufferWidth;
        Graphics.PreferredBackBufferHeight = height ?? Graphics.PreferredBackBufferHeight;
        Graphics.IsFullScreen = isFullScreen ?? Graphics.IsFullScreen;
        Graphics.ApplyChanges();
    }

    protected abstract void Load();
    protected virtual void PostLoad() { }

    protected override void LoadContent()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        
        Load();

        Defs.ForEach(kvp => kvp.Value.LoadContent(ContentManager));
        RendererDictionary.Values.ForEach(r => r.LoadContent(ContentManager));

        PostLoad();
    }

    public ILayerRenderer GetRenderer(string rendererKey)
        => RendererDictionary[rendererKey];

    public TRenderer GetRenderer<TRenderer>(string rendererKey)
        where TRenderer : ILayerRenderer
        => (TRenderer)RendererDictionary[rendererKey];

    public ILayerRenderer AddDefaultRenderer<TLayer>(string rendererKey, ILayerRenderer renderer)
        where TLayer : ILayer
    {
        DefaultRenderers[typeof(TLayer)] = rendererKey;
        return AddRenderer(rendererKey, renderer);
    }

    public TRenderer AddRenderer<TRenderer>(string rendererKey, TRenderer renderer) where TRenderer : ILayerRenderer
        => (TRenderer)(RendererDictionary[rendererKey] = renderer);

    public void SetDefaultRenderer<TRenderer>(string rendererKey) where TRenderer : ILayer
        => DefaultRenderers[typeof(TRenderer)] = rendererKey;

    public TObject AddObject<TObject>(TObject gameObject, IMap? parentMap = null) where TObject : GameObjectBase
    {
        ObjectsToAdd.Enqueue(gameObject);
        gameObject.SetMap(parentMap ?? MainMap);
        return gameObject;
    }

    public void RemoveDef<TDef>(TDef def) where TDef : Def
        => Defs.Remove(def.DefName);

    public void RemoveDefs<TDef>() where TDef : Def
    {
        foreach (var d in Defs.Where(pair => pair.Value is TDef).ToList())
            Defs.Remove(d.Key);
    }

    public TDef AddDef<TDef>(TDef def) where TDef : Def
    {
        Defs[def.DefName] = def;
        return def;
    }

    public static TDef? GetDef<TDef>() where TDef : Def
        => GetDefs<TDef>().FirstOrDefault();

    public static TDef GetDef<TDef>(string defName) where TDef : Def
        => ((Instance.Defs.GetValueOrDefault(defName) ?? Def.Empty) as TDef)!;

    public static IEnumerable<TDef> GetDefs<TDef>() where TDef : Def
        => Instance.Defs.Values.OfType<TDef>();

    protected virtual void PreUpdate(GameTime gameTime) { return; }
    protected virtual bool PreRenderUpdate(GameTime gameTime) => true;
    protected virtual bool PreObjectsUpdate(GameTime gameTime) => true;
    protected virtual void PostObjectsUpdate(GameTime gameTime) { return; }
    protected virtual bool PreMapUpdate(GameTime gameTime) => true;
    protected virtual void PostUpdate(GameTime gameTime) { return; }

    protected override void Update(GameTime gameTime)
    {
        PreviousMouseStates.RemoveAt(0);
        PreviousMouseStates.Add(CurrentMouseState);
        CurrentMouseState = Mouse.GetState();
        
        PreviousKeyStates.RemoveAt(0);
        PreviousKeyStates.Add(CurrentKeyState);
        CurrentKeyState = Keyboard.GetState();

        WorldMouse = MainCamera.ScreenToWorld(CurrentMouseState.Position.X, CurrentMouseState.Position.Y).GetFloor();
        Tweener?.Update(gameTime.GetElapsedSeconds());
        
        PreUpdate(gameTime);

        if (PreRenderUpdate(gameTime))
            foreach (var renderer in RendererDictionary.Values)
                renderer.Update(this, gameTime);

        if (PreMapUpdate(gameTime) && ActiveMaps.Any())
            foreach (var map in ActiveMaps)
            {
                foreach (var layer in map.Layers)
                {
                    layer.Update(gameTime);
                    if (layer is IObjectLayer objectLayer)
                        foreach (var obj in objectLayer.Objects)
                            objectLayer.ObjectUpdate(obj);
                }
                map.Update(this, gameTime);
            }

        if (PreObjectsUpdate(gameTime))
            foreach (var obj in Objects.Where(o => o.PreUpdate(this, gameTime)))
            {
                obj.Update(this, gameTime);
                obj.PostUpdate(this, gameTime);
            }

        foreach (var obj in Objects)
        {
            if (obj.ParentMap != null && obj.ParentMap.LayerMap[obj.Layer] is IObjectLayer layer)
            {
                if (obj.Remove)
                {
                    layer.RemoveObject(obj);
                    obj.OnRemove();
                }
            }
        }

        Objects.RemoveAll(obj => obj.Remove);

        foreach (var obj in ObjectsToAdd)
        {
            Objects.Add(obj);
            var layer = obj.ParentMap?.LayerMap[obj.Layer] as IObjectLayer;
            layer?.AddObject(obj);
            obj.OnAdd();
            obj.Update(this, gameTime);
        }

        ObjectsToAdd.Clear();

        PostObjectsUpdate(gameTime);

        PostUpdate(gameTime);

        if (ShouldQuit)
            Exit();

        FramesPerSecondCounter.Update(gameTime);
    }

    protected void MarkAllObjectsForRemoval()
        => Objects.ForEach(o => o.Remove = true);

    protected override void Draw(GameTime gameTime)
    {
        FramesPerSecondCounter.Draw(gameTime);
        Draw(gameTime, DefaultRenderHooks);
    }

    protected virtual bool PreClear(GameTime gameTime) => true;
    protected virtual bool PreMapDraw(GameTime gameTime)
    {
        var ret = true;
        foreach (var map in ActiveMaps)
        {
            for (var layerIndex = 0; layerIndex < map.Layers.Count; layerIndex++)
            {
                var layer = map.Layers[layerIndex];
                var layerName = layer.Name;
                var rendererKey = layer.RendererKey;
                var layerType = layer.GetType()!;
                ILayerRenderer? renderer = null;
                if (rendererKey == null)
                    rendererKey = DefaultRenderers.GetValueOrDefault(layerType)
                                    ?? (DefaultRenderers.FirstOrDefault(r => layerType.IsAssignableTo(r.Key)).Value);
                if (rendererKey != null)
                    renderer = RendererDictionary.GetValueOrDefault(rendererKey);

                ret = ret && (renderer?.PreDraw(this, gameTime, layer) ?? true);
            }
        }

        return ret;
    }
    protected virtual void PostDraw(GameTime gameTime) { return; }

    protected void Draw(GameTime gameTime, Dictionary<string, RenderHook>? renderHooks)
    {
        var transformMatrix = MainCamera.GetViewMatrix();

        if (PreMapDraw(gameTime))
        {
            if (PreClear(gameTime))
                GraphicsDevice.Clear(DefaultBackgroundColor);

            foreach (var map in ActiveMaps)
                DrawMap(map, gameTime, renderHooks, transformMatrix);
        }

        PostDraw(gameTime);
    }

    private void DrawMap(IMap map, GameTime gameTime, Dictionary<string, RenderHook>? renderHooks, Matrix transformMatrix)
    {
        for (var layerIndex = 0; layerIndex < map.Layers.Count; layerIndex++)
        {
            var layer = map.Layers[layerIndex];
            var layerName = layer.Name;
            var hookTuple = renderHooks?.GetValueOrDefault(layerName);
            var layerType = layer.GetType()!;
            ILayerRenderer? renderer = null;
            
            var rendererKey = layer.RendererKey;
            if (rendererKey == null)
                rendererKey = DefaultRenderers.GetValueOrDefault(layerType) 
                                ?? (DefaultRenderers.FirstOrDefault(r => layerType.IsAssignableTo(r.Key)).Value);
            if (rendererKey != null)
                renderer = RendererDictionary.GetValueOrDefault(rendererKey);

            renderer?.Begin(transformMatrix);
            hookTuple?.PreHook?.Invoke(layerName);

            if (!layer.IsHidden)
                renderer?.Draw(this, gameTime, layer, renderer.DrawOffset);

            hookTuple?.PostHook?.Invoke(layerName);
            renderer?.End();
        }
    }

    public bool WasMouseMoved()
        => CurrentMouseState.X != PreviousMouseState.X
        || CurrentMouseState.Y != PreviousMouseState.Y;

    public int GetScrollWheelDelta()
        => CurrentMouseState.ScrollWheelValue - PreviousMouseState.ScrollWheelValue;

    public bool WasLeftMouseJustPressed()
        => CurrentMouseState.LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton == ButtonState.Released;

    public bool WasMiddleMouseJustPressed()
        => CurrentMouseState.MiddleButton == ButtonState.Pressed && PreviousMouseState.MiddleButton == ButtonState.Released;

    public bool WasRightMouseJustPressed()
        => CurrentMouseState.RightButton == ButtonState.Pressed && PreviousMouseState.RightButton == ButtonState.Released;

    public bool IsLeftMouseDown()
        => CurrentMouseState.LeftButton == ButtonState.Pressed;

    public bool IsMiddleMouseDown()
        => CurrentMouseState.MiddleButton == ButtonState.Pressed;

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

    public bool WasKeyJustPressed(params Keys[] keys)
        => keys.Any(WasKeyJustPressed);

    public bool WasKeyJustReleased(Keys key)
        => CurrentKeyState.IsKeyUp(key) && PreviousKeyState.IsKeyDown(key);

    public bool WasKeyJustReleased(params Keys[] keys)
        => keys.Any(WasKeyJustReleased);

    public bool IsKeyDown(Keys key)
        => CurrentKeyState.IsKeyDown(key);

    public bool IsKeyDown(params Keys[] keys)
        => keys.Any(IsKeyDown);

    public bool WasKeyDown(Keys key)
        => PreviousKeyState.IsKeyDown(key);

    public bool WasKeyDown(params Keys[] keys)
        => keys.Any(WasKeyDown);

    public bool IsKeyHeld(Keys key)
        => CurrentKeyState.IsKeyDown(key) && PreviousKeyState.IsKeyDown(key);

    public bool IsKeyHeld(params Keys[] keys)
        => keys.Any(IsKeyHeld);

    public bool IsKeyHeldLong(Keys key)
        => IsKeyHeld(key) && PreviousKeyStates[^2].IsKeyDown(key) && PreviousKeyStates[^3].IsKeyDown(key) && PreviousKeyStates[^4].IsKeyDown(key) && PreviousKeyStates[^5].IsKeyDown(key);

    public bool IsKeyHeldLong(params Keys[] keys)
        => keys.Any(IsKeyHeldLong);

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

    protected override void Dispose(bool disposing)
    {
        Tweener?.Dispose();
        SpriteBatch?.Dispose();
        GraphicsDevice?.Dispose();
        base.Dispose(disposing);
    }

    public Tween<TMember> Tween<TTarget, TMember>(
        TTarget target,
        Expression<Func<TTarget, TMember>> expression,
        TMember toValue,
        float duration,
        float delay = 0f,
        Action<Tween>? onEnd = null,
        Action<Tween>? onBegin = null,
        int repeatCount = 0,
        float repeatDelay = 0f,
        bool autoReverse = false,
        Func<float, float>? easingFunction = null) where TTarget : class where TMember : struct
    {
        var tween = Tweener.TweenTo(target, expression, toValue, duration, delay);
        if (onEnd != null)
            tween.OnEnd(onEnd);
        if (onBegin != null)
            tween.OnBegin(onBegin);

        if (repeatCount > 0)
            tween.Repeat(repeatCount, repeatDelay);
        else if (repeatCount < 0)
            tween.RepeatForever(repeatDelay);

        if (autoReverse)
            tween.AutoReverse();

        if (easingFunction != null)
            tween.Easing(easingFunction);

        return tween;
    }

    public float RandomSingle()
        => Random.NextSingle();

    public int RandomInt()
        => Random.Next();

    public int RandomInt(int min, int max)
        => Random.Next(min, max);

    public int RandomInt(int max)
        => Random.Next(max);

    public double RandomDouble() 
        => Random.NextDouble();
}
