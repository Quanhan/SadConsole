﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole
{
    public partial class Game
    {
        public class SadConsoleGameComponent : DrawableGameComponent
        {
            internal SadConsoleGameComponent(Game game) : base(game)
            {
                DrawOrder = 5;
                UpdateOrder = 5;
            }

            public override void Draw(GameTime gameTime)
            {
                if (Settings.DoDraw)
                {
                    Global.GameTimeRender = gameTime;
                    Global.GameTimeElapsedRender = gameTime.ElapsedGameTime.TotalSeconds;

                    // Make sure all items in the screen are drawn. (Build a list of draw calls)
                    Global.CurrentScreen?.Draw(gameTime.ElapsedGameTime);

                    // Render to the global output texture
                    GraphicsDevice.SetRenderTarget(Global.RenderOutput);
                    GraphicsDevice.Clear(Settings.ClearColor);

                    // Render each draw call
                    Global.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
                    foreach (var call in Global.DrawCalls)
                    {
                        call.Draw();
                    }

                    Global.SpriteBatch.End();
                    GraphicsDevice.SetRenderTarget(null);

                    // Clear draw calls for next run
                    Global.DrawCalls.Clear();

                    // If we're going to draw to the screen, do it.
                    if (Settings.DoFinalDraw)
                    {
                        Global.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
                        Global.SpriteBatch.Draw(Global.RenderOutput, Global.RenderRect, Color.White);
                        Global.SpriteBatch.End();
                    }
                }

                SadConsole.Game.OnDraw?.Invoke(gameTime);
            }

            public override void Update(GameTime gameTime)
            {
                if (Settings.DoUpdate)
                {
                    Global.GameTimeUpdate = gameTime;
                    Global.GameTimeElapsedUpdate = gameTime.ElapsedGameTime.TotalSeconds;

                    if (Settings.Input.DoKeyboard)
                        Global.KeyboardState.ProcessKeys(gameTime);

                    if (Settings.Input.DoMouse)
                    {
                        Global.MouseState.ProcessMouse(gameTime);

                        Global.MouseState.IsOnScreen = Settings.Input.ProcessMouseOffscreen || Global.RenderRect.Contains(Global.MouseState.ScreenLocation);
                        Global.MouseState.IsBusy = Console.ActiveConsole != null && Console.ActiveConsole.ExclusiveFocus;

                        //if (Global.MouseState.IsBusy)
                        //    Console.ActiveConsole.ProcessMouse(Global.MouseState);
                    }

                    Global.CurrentScreen?.Update(gameTime.ElapsedGameTime);


                    if (Settings.Input.DoMouse)
                    {
                        if (!Global.MouseState.IsOnConsole && Global.MouseState.LastConsole != null)
                            Global.MouseState.FlushLastConsole();
                    }

                    SadConsole.Game.OnUpdate?.Invoke(gameTime);
                }
            }
        }
    }
}