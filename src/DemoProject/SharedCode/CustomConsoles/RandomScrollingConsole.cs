﻿using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ColorHelper = Microsoft.Xna.Framework.Color;

using SadConsole;
using SadConsole.Surfaces;
using Console = SadConsole.Console;
using System;

namespace StarterProject.CustomConsoles
{
    class RandomScrollingConsole : Console
    {
        private BasicSurface mainData;
        private SurfaceEditor messageData;
        private bool initialized;
        private bool initializedStep2;
        private bool initializedStep3;

        public RandomScrollingConsole() : base(80, 25)
        {
            messageData = new SurfaceEditor(new BasicSurface(10, 1));
            IsVisible = false;

            
            KeyboardHandler = (cons, info) =>
            {

                if (info.IsKeyDown(Keys.Left))
                    cons.TextSurface.RenderArea = new Rectangle(cons.TextSurface.RenderArea.Left - 1, cons.TextSurface.RenderArea.Top, 80, 25);

                if (info.IsKeyDown(Keys.Right))
                    cons.TextSurface.RenderArea = new Rectangle(cons.TextSurface.RenderArea.Left + 1, cons.TextSurface.RenderArea.Top, 80, 25);

                if (info.IsKeyDown(Keys.Up))
                    cons.TextSurface.RenderArea = new Rectangle(cons.TextSurface.RenderArea.Left, cons.TextSurface.RenderArea.Top - 1, 80, 25);

                if (info.IsKeyDown(Keys.Down))
                    cons.TextSurface.RenderArea = new Rectangle(cons.TextSurface.RenderArea.Left, cons.TextSurface.RenderArea.Top + 1, 80, 25);

                return true;
            };

        }

        protected override void OnVisibleChanged()
        {
            if (IsVisible && !initialized)
            {
                // Write to the message layer
                Print(0, 0, "Generating random console data, please wait...");
                initialized = true;
            }
        }

        public override void Draw(TimeSpan delta)
        {
            // These 3 render calls are a hack to get the console data generated and display a message to the user
            // Should add in async calls that let us generate these in the background... That would be cool.
            if (IsVisible)
            {
                if (!initialized)
                    base.Draw(delta);

                else if (!initializedStep2)
                {
                    initializedStep2 = true;
                    base.Draw(delta);
                }
                else if (!initializedStep3)
                {
                    base.Draw(delta);

                    // Generate the content
                    TextSurface = new BasicSurface(2000, 2000, new Rectangle(0, 0, 80, 25)); //500mb ?? why?
                    //Data = new TextSurface(2000, 2000);
                    //DataViewport = new Rectangle(0, 0, 80, 25);
                    TextSurface.RenderArea = new Rectangle(0, 0, 80, 25);

                    // Clear message data and make it transparent so that it acts as a layer
                    messageData.Fill(Color.White, Color.Transparent, 0, null);

                    // We need to set celldata to the big console data so we can use the FillWithRandom method.
                    FillWithRandomGarbage();
                    initializedStep3 = true;
                }

                else
                {
                    // Set message data information about where the viewport is located
                    //messageData.Print(0, 0, $"{ViewArea.X} , {ViewArea.Y}            ", Color.White, Color.Black);

                    // Create a faux layering system.
                    base.Draw(delta);

                    //Renderer.Render(messageData.TextSurface, new Point(0, 0));
                }
            }
        }
    }
}