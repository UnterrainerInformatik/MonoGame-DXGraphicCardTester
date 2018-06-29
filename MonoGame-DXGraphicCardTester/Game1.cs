// *************************************************************************** 
// This is free and unencumbered software released into the public domain.
// 
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
// 
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
// For more information, please refer to <http://unlicense.org>
// ***************************************************************************

using System;
using System.Text;
using InputStateManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameDemoTools;
using StateMachine;

namespace MonoGame_DXGraphicCardTester
{
	public enum State
	{
		RESOLUTION_800_X600,
		RESOLUTION_1024_X768,
		RESOLUTION_1280_X768,
		RESOLUTION_1920_X1080,
		RESOLUTION_800_X600_FULLSCREEN,
		RESOLUTION_1024_X768_FULLSCREEN,
		RESOLUTION_1280_X768_FULLSCREEN,
		RESOLUTION_1920_X1080_FULLSCREEN
	}

	public enum Trigger
	{
		SPACE
	}

	/// <summary>
	///     This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		private const string IMAGE_COPYRIGHT =
			"Image by Michael Beckwith\nAuthor: https://www.flickr.com/people/78207463@N04 \n" +
			"Source: https://www.flickr.com/photos/78207463@N04/8226826999/ \n'Inside St Kentigerns RC Church.'\n" +
			"Located in Blackpool, Lancashire, England, UK.";

		private readonly GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private InputManager Input { get; } = new InputManager();

		private SpriteFont Font { get; set; }
		private Texture2D Image { get; set; }

		private readonly Fsm<State, Trigger> fsm;
		private int counter;
		private RenderTarget2D rt;
		private Point targetResolution;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferMultiSampling = true;
			graphics.PreferredBackBufferWidth = 800;
			graphics.PreferredBackBufferHeight = 600;
			graphics.IsFullScreen = false;
			graphics.HardwareModeSwitch = false;
			graphics.PreparingDeviceSettings += PrepareDeviceSettings;
			graphics.SynchronizeWithVerticalRetrace = true;

			IsMouseVisible = true;
			IsFixedTimeStep = true;

			Content.RootDirectory = "Content";
			Debug.Log("Program started.");

			fsm = CreateFiniteStateMachine();
		}

		private void PrepareDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
		{
			e.GraphicsDeviceInformation.GraphicsProfile = GraphicsProfile.Reach;
		}

		protected override void Initialize()
		{
			base.Initialize();
			SetResolution(800, 600, false);
		}

		private void SetResolution(int width, int height, bool isFullscreen)
		{
			rt = new RenderTarget2D(GraphicsDevice, width, height);
			if (isFullscreen)
				targetResolution = new Point(GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height);
			else
				targetResolution = new Point(width, height);
			try
			{
				Util.InitGraphicsMode(this, graphics, width, height, isFullscreen, 1000f / 60f, true, false, false);
				LogResolutionChange();
				counter++;
			}
			catch (Exception e)
			{
				Debug.Log("Exception thrown!", e);
			}
		}

		private Fsm<State, Trigger> CreateFiniteStateMachine()
		{
			return Fsm<State, Trigger>.Builder(State.RESOLUTION_800_X600)
				.State(State.RESOLUTION_800_X600)
				.OnEnter(args => { SetResolution(800, 600, false); })
				.TransitionTo(State.RESOLUTION_800_X600_FULLSCREEN).On(Trigger.SPACE)
				.State(State.RESOLUTION_800_X600_FULLSCREEN)
				.OnEnter(args => { SetResolution(800, 600, true); })
				.TransitionTo(State.RESOLUTION_1024_X768).On(Trigger.SPACE).State(State.RESOLUTION_1024_X768).OnEnter(
					args => { SetResolution(1024, 768, false); })
				.TransitionTo(State.RESOLUTION_1024_X768_FULLSCREEN).On(Trigger.SPACE)
				.State(State.RESOLUTION_1024_X768_FULLSCREEN)
				.OnEnter(args => { SetResolution(1024, 768, true); })
				.TransitionTo(State.RESOLUTION_1280_X768).On(Trigger.SPACE).State(State.RESOLUTION_1280_X768).OnEnter(
					args => { SetResolution(1280, 768, false); })
				.TransitionTo(State.RESOLUTION_1280_X768_FULLSCREEN).On(Trigger.SPACE)
				.State(State.RESOLUTION_1280_X768_FULLSCREEN).OnEnter(args => { SetResolution(1280, 768, true); })
				.TransitionTo(State.RESOLUTION_1920_X1080).On(Trigger.SPACE).State(State.RESOLUTION_1920_X1080).OnEnter(
					args => { SetResolution(1920, 1080, false); })
				.TransitionTo(State.RESOLUTION_1920_X1080_FULLSCREEN).On(Trigger.SPACE)
				.State(State.RESOLUTION_1920_X1080_FULLSCREEN).OnEnter(args => { SetResolution(1920, 1080, true); })
				.Build();
		}

		private void LogResolutionChange()
		{
			Debug.Log($"current resolution: {fsm.Current.Identifier}");
		}

		/// <summary>
		///     LoadContent will be called once per game and is the place to load
		///     all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);
			Font = Content.Load<SpriteFont>("AnonymousPro8");
			Image = Content.Load<Texture2D>("image");
		}

		/// <summary>
		///     Allows the game to run logic such as updating the world,
		///     checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			Input.Update();
			if (Input.Pad().Is.Press(Buttons.Back) || Input.Key.Is.Press(Keys.Escape))
			{
				Debug.Log("Program aborted by user.");
				Exit();
			}

			HandleInput();
			base.Update(gameTime);
		}

		private void HandleInput()
		{
			if (Input.Key.Is.Press(Keys.Space))
			{
				fsm.Trigger(Trigger.SPACE);
			}
		}

		/// <summary>
		///     This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			GraphicsDevice.SetRenderTarget(rt);
			GraphicsDevice.Clear(Color.CornflowerBlue);
			DrawImage();
			DrawText(gameTime);
			base.Draw(gameTime);

			FinalDraw();
		}

		private void DrawImage()
		{
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			spriteBatch.Draw(Image, new Rectangle(0, 0, rt.Width, rt.Height), Color.White);
			spriteBatch.End();
		}

		private void DrawText(GameTime gameTime)
		{
			var c = Demo.GetLerpColor(gameTime);

			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			spriteBatch.DrawString(Font, BuildText(), new Vector2(10, 10), c);
			var s = Font.MeasureString(IMAGE_COPYRIGHT);
			spriteBatch.DrawString(Font, IMAGE_COPYRIGHT, new Vector2(30f, rt.Height - s.Y - 30f), c);
			spriteBatch.End();
		}

		private void FinalDraw()
		{
			GraphicsDevice.SetRenderTarget(null);
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			spriteBatch.Draw(rt, new Rectangle(0, 0, targetResolution.X, targetResolution.Y), Color.White);
			spriteBatch.End();
		}

		private string BuildText()
		{
			var sb = new StringBuilder();
			sb.Append("This will test your display-adapter.\n");
			sb.Append(
				"A report named 'report.txt' will be generated right next to the executable of this program.\n\n\n");
			sb.Append("Press <SPACE> to switch to next resolution!\n\n");
			sb.Append($"  TEST {counter + 1}\n");
			sb.Append($"    current resolution: {rt.Width} x {rt.Height}\n");
			if (counter == 7) sb.Append("\nYOU'RE DONE! Now close this program and get your 'report.txt' file.\n");
			sb.Append("\n\n\nPress <ESC> to exit!");
			return sb.ToString();
		}
	}
}