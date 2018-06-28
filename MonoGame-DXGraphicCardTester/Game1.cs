using System.Text;
using InputStateManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameDemoTools;

namespace MonoGame_DXGraphicCardTester
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		private const string IMAGE_COPYRIGHT =
			"Image by Michael Beckwith\nAuthor: https://www.flickr.com/people/78207463@N04 \n" +
			"Source: https://www.flickr.com/photos/78207463@N04/8226826999/ \n'Inside St Kentigerns RC Church.'\n" +
			"Located in Blackpool, Lancashire, England, UK.";

		private SpriteBatch spriteBatch;
		private InputManager Input { get; } = new InputManager();

		private SpriteFont Font { get; set; }
		private Texture2D Image { get; set; }
		private Point Resolution { get; } = new Point(1280, 720);

		public Game1()
		{
			var graphics = new GraphicsDeviceManager(this);
			graphics.PreferMultiSampling = true;
			graphics.PreferredBackBufferWidth = Resolution.X;
			graphics.PreferredBackBufferHeight = Resolution.Y;
			graphics.IsFullScreen = false;
			graphics.PreparingDeviceSettings += PrepareDeviceSettings;
			graphics.SynchronizeWithVerticalRetrace = true;

			IsMouseVisible = true;
			IsFixedTimeStep = true;

			Content.RootDirectory = "Content";
			Debug.Log("Program started.");
		}

		void PrepareDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
		{
			e.GraphicsDeviceInformation.GraphicsProfile = GraphicsProfile.Reach;
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);
			Font = Content.Load<SpriteFont>("AnonymousPro8");
			Image = Content.Load<Texture2D>("image");
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
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

			}
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			DrawImage();
			DrawText(gameTime);
			base.Draw(gameTime);
		}

		private void DrawImage()
		{
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			spriteBatch.Draw(Image, new Rectangle(0, 0, Resolution.X, Resolution.Y), Color.White);
			spriteBatch.End();
		}

		private void DrawText(GameTime gameTime)
		{
			var c = Demo.GetLerpColor(gameTime);

			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			spriteBatch.DrawString(Font, BuildText(), new Vector2(10, 10), c);
			var s = Font.MeasureString(IMAGE_COPYRIGHT);
			spriteBatch.DrawString(Font, IMAGE_COPYRIGHT, new Vector2(30f, Resolution.Y - s.Y - 30f), c);
			spriteBatch.End();
		}

		private string BuildText()
		{
			var sb = new StringBuilder();
			sb.Append($"This will test your display-adapter.\n");
			sb.Append($"A report named 'report.txt' will be generated right next to the executable of this program.\n\n\n");
			sb.Append($"Press <SPACE> to switch to next resolution!\n\n");
			sb.Append($"  TEST 1/12\n");
			sb.Append($"    current resolution: 800x600 fullscreen\n");
			sb.Append($"       next resolution: 800x600 fullscreen\n");
			sb.Append("\n\n\nPress <ESC> to exit!");
			return sb.ToString();
		}
	}
}
