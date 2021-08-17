using Merthsoft.MooseEngine.Defs;
using Merthsoft.MooseEngine.GameObjects;
using Merthsoft.MooseEngine.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Merthsoft.MooseEngine
{
    public abstract class MooseGame : Game
    {
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

        public int MapHeight => MainMap.Height;

        public int MapWidth => MainMap.Width;

        public int TileWidth => MainMap.TileWidth;

        public int TileHeight => MainMap.TileHeight;

        public Size2 TileSize => MainMap.TileSize; // TODO: Cache

        public Vector2 HalfTileSize => MainMap.HalfTileSize; // TODO: Cache

        public MooseGame()
        {
            ContentManager = new MooseContentManager(Content);
            IsMouseVisible = true;
            Graphics = new GraphicsDeviceManager(this);
        }

        protected override void Initialize()
        {
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            base.Initialize();
        }

        protected abstract void Load();

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            MainCamera = new OrthographicCamera(GraphicsDevice) { Origin = new(0, 0) };

            Load();

            Defs.AsParallel().ForEach(kvp => kvp.Value.LoadContent(ContentManager));
        }

        protected void LoadMap()
        {
            foreach (var renderer in Renderers.Values)
                renderer.Load(MainMap);
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
            MainMap.Update(gameTime);

            PostUpdate(gameTime);

            if (ShouldQuit)
                base.Exit();
        }

        protected override void Draw(GameTime gameTime)
            => Draw();

        protected void Draw(params (Action<int>? preHook, Action<int>? postHook)?[] renderHooks)
        {
            GraphicsDevice.Clear(Color.DarkGray);

            var transformMatrix = MainCamera.GetViewMatrix();

            for (var layerIndex = 0; layerIndex < MainMap.Layers.Count; layerIndex++)
            {
                var hookTuple = renderHooks?.ElementAtOrDefault(layerIndex);
                var layer = MainMap.Layers[layerIndex];
                var renderer = Renderers[layer.RendererKey];

                renderer.Begin(transformMatrix: transformMatrix, blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
                hookTuple?.preHook?.Invoke(layerIndex);
                renderer.Draw(layer, layerIndex);
                hookTuple?.postHook?.Invoke(layerIndex);
                renderer.End();
            }
        }

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
    }
}
