using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tiled;
using MooseLib;
using MooseLib.BaseDriver;
using MooseLib.Interface;
using MooseLib.Tiled;
using Platformer.PlatformerGameObjects;
using System;
using System.Diagnostics;
using System.Linq;

namespace Platformer
{
    public class PlatformerGame : MooseGame
    {
        Player player = null!;

        protected override void Initialize()
        {
            base.Initialize();

            Graphics.PreferredBackBufferWidth = 960;
            Graphics.PreferredBackBufferHeight = 960;
            Graphics.ApplyChanges();
        }

        protected override void Load()
        {
            AddRenderer(TiledMooseMapRenderer.DefaultRenderKey, new TiledMooseMapRenderer(GraphicsDevice));
            AddRenderer(SpriteBatchRenderer.DefaultRenderKey, new SpriteBatchRenderer(SpriteBatch));

            InitializeMap(30, 30, 16, 16);
            MainMap.CopyFromMap(new TiledMooseMap(Content.Load<TiledMap>("Maps/testmap")));
            LoadMap();

            foreach (var layer in MainMap.Layers)
            {
                layer.RendererKey = layer switch
                {
                    ITileLayer => TiledMooseMapRenderer.DefaultRenderKey,
                    IObjectLayer => SpriteBatchRenderer.DefaultRenderKey,
                    _ => throw new Exception("Unsupported layer type"),
                };
            }

            MainCamera.ZoomIn(2f);
            MainCamera.Move(Direction.South * 160);

            AddDef(new PlayerDef());
        }

        protected override void Update(GameTime gameTime)
        {
            if (player == null)
            {
                player = new Player(GetDef<PlayerDef>("player"), position: new(32, 32), layer: 4, transformLocation: new(-8, -15) );
                AddObject(player);
            }
            

            foreach (var obj in ReadObjects.Cast<PlatformerGameObject>())
            {
                var cachedPosition = obj.WorldPosition;
                var cachedVelocity = obj.Veclocity;

                if (obj.IsEffectedByGravity)
                {
                    var blockValue = GetBlockingVectorFromWorldPosition(obj.WorldPosition + Direction.South)[1];
                    if (blockValue == 0)
                    {
                        obj.Veclocity += Direction.South;
                        if (obj.Veclocity.Y > 0)
                            obj.State = PlatformerGameObject.States.Fall;
                    }
                }

                if (obj.Veclocity.Y > 4)
                    obj.Veclocity = new(obj.Veclocity.X, 4);

                obj.Veclocity = obj.Veclocity.Round(2);

                if (obj.Veclocity != Vector2.Zero)
                {
                    var newPosition = (obj.WorldPosition + obj.Veclocity).Round(0);
                    var blocked = false;
                    var prevPosition = obj.WorldPosition;
                    foreach (var cell in FindWorldRay(cachedPosition, newPosition, true))
                    {
                        var normal = (prevPosition.X - cell.WorldPosition.X,
                                      prevPosition.Y - cell.WorldPosition.Y);
                        prevPosition = cell.WorldPosition;

                        if (cell.BlockedVector[1] == 0 || normal == (0, 0))
                            continue;

                        // TODO: Handle jumping up betrter
                        if (normal == (0, 1))
                            continue;

                        obj.Veclocity = normal switch
                        {
                            // North
                            (0, -1) => new(obj.Veclocity.X, 0),
                            // South
                            (0, 1) => new(obj.Veclocity.X, 0),
                            // East
                            (1, 0) => new(0, obj.Veclocity.Y),
                            // West
                            (-1, 0) => new(0, obj.Veclocity.Y),
                            _ => obj.Veclocity
                        };

                        if (obj.Veclocity == Vector2.Zero)
                            obj.State = PlatformerGameObject.States.Idle;
                        else if (obj.Veclocity.Y == 0)
                            obj.State = PlatformerGameObject.States.Walk;
                        else if (obj.Veclocity.Y > 0)
                            obj.State = PlatformerGameObject.States.Fall;
                        else
                            obj.State = PlatformerGameObject.States.Jump;

                        Debug.WriteLine($"Collision: direction: {normal} new veloctiy: {obj.Veclocity} new state: {obj.State}");
                        blocked = true;
                        break;
                    }

                    if (!blocked)
                        obj.WorldPosition += obj.Veclocity;
                }

                obj.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void PostUpdate(GameTime gameTime)
        {
            if (CurrentKeyState.GetPressedKeyCount() != 0 && PreviousKeyState.GetPressedKeyCount() == 0)
                Debug.WriteLine("");

            if (CurrentKeyState.IsKeyDown(Keys.Space) && !PreviousKeyState.IsKeyDown(Keys.Space) && (player.IsIdle || player.IsWalking))
            {
                player.State = PlatformerGameObject.States.Jump;
                player.Veclocity = new(player.Veclocity.X, -7);
            }

            var applyFriction = player.IsWalking ? .5f : 0;

            if (CurrentKeyState.IsKeyDown(Keys.A) || CurrentKeyState.IsKeyDown(Keys.Left))
            {
                if (player.IsIdle)
                    player.State = PlatformerGameObject.States.Walk;
                player.Veclocity += Direction.West;
                applyFriction = 0;
                player.Direction = Direction.Left;
            }

            if (CurrentKeyState.IsKeyDown(Keys.D) || CurrentKeyState.IsKeyDown(Keys.Right))
            {
                if (player.IsIdle)
                    player.State = PlatformerGameObject.States.Walk;
                player.Veclocity += Direction.East;
                applyFriction = 0;
                player.Direction = Direction.Right;
            }

            if (player.Veclocity.X > 2)
                player.Veclocity = new(2, player.Veclocity.Y);

            if (player.Veclocity.X < -2)
                player.Veclocity = new(-2, player.Veclocity.Y);

            
            if (applyFriction != 0)
                player.Veclocity = new(player.Veclocity.X - (player.Veclocity.X * applyFriction), player.Veclocity.Y);

            player.Veclocity = player.Veclocity.Round(0);

            if (player.Veclocity == Vector2.Zero)
                player.State = PlatformerGameObject.States.Idle;
        }

        protected override void Draw(GameTime gameTime)
        {
            Draw();
        }
    }
}
