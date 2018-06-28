 [![license](https://img.shields.io/github/license/unterrainerinformatik/MonoGame-Textbox.svg?maxAge=2592000)](http://unlicense.org)  [![Twitter Follow](https://img.shields.io/twitter/follow/throbax.svg?style=social&label=Follow&maxAge=2592000)](https://twitter.com/throbax)  

# General

This section contains various useful projects that should help your development-process.  

This section of our GIT repositories is free. You may copy, use or rewrite every single one of its contained projects to your hearts content.  
In order to get help with basic GIT commands you may try [the GIT cheat-sheet][coding] on our [homepage][homepage].  

This repository located on our  [homepage][homepage] is private since this is the master- and release-branch. You may clone it, but it will be read-only.  
If you want to contribute to our repository (push, open pull requests), please use the copy on github located here: [the public github repository][github]  

# MonoGame-DXGraphicCardTester

This is a little project that allows for rapidly switching resolutions in order to get records of DXExceptions that might be thrown. .

> **If you like this repo, please don't forget to star it.**
> **Thank you.**



## Usage

Download and run the latest release of this project.

When running the program just follow the on-screen instructions.
It will generate a log-file that will contain information about the outcome of the tests.

### General Information

In very rare cases a customer would get some DXExceptions originating from SharpDX (as far as I can tell). Because of the nature or that library the debugging options are somewhat limited even if that happens on your dev-rig. So it's very hard to gather information about that.

Here is such a report:

#### Report

SharpDXEception at SwapChain.ResizeBuffers()...

```c#
SharpDX.SharpDXException: HRESULT: [0x887A0001], Module: [SharpDX.DXGI], ApiCode: [DXGI_ERROR_INVALID_CALL/InvalidCall], Message: The application made a call that is invalid. Either the parameters of the call or the state of some object was incorrect.
Enable the D3D debug layer in order to see details via debug messages.

   at SharpDX.Result.CheckError()
   at SharpDX.DXGI.SwapChain.ResizeBuffers(Int32 bufferCount, Int32 width, Int32 height, Format newFormat, SwapChainFlags swapChainFlags)
   at Microsoft.Xna.Framework.Graphics.GraphicsDevice.CreateSizeDependentResources()
   at Microsoft.Xna.Framework.Graphics.GraphicsDevice.Reset()
   at Microsoft.Xna.Framework.Graphics.GraphicsDevice.Reset(PresentationParameters presentationParameters)
   at Microsoft.Xna.Framework.GraphicsDeviceManager.ApplyChanges()
   at Utilities.Utils.InitGraphicsMode(Game game, GraphicsDeviceManager graphicsDeviceManager, Int32 width, Int32 height, Boolean isFullScreen, Single targetElapsedIntervalInMillis, Boolean isSynchronizeWithVerticalRetrace, Boolean isFixedTimeStep, Boolean isPreferMultiSampling, SurfaceFormat preferredBackBufferFormat, DepthFormat preferredDepthFormat, Nullable`1 zero)
   at ThrobaxTD.Screens.OptionsGraphicsMenuScreen.OnSaveAndBackMenuEntrySelected(Object sender, PlayerIndexEventArgs e)
```

MG build develop 3.7.0.1549 (latest) on windows build on windows machine (OS: Windows 10 GPU: 750 TI).

Happens immediately after my init-code calls Apply();

The resolution I set here is advertised by his graphics card and needless to say that I cannot reproduce the bug on my rig.

Here is my code:

```c#
/// <summary>
///     Attempt to set the display mode to the desired resolution.  Iterates
///     through the display capabilities of the default graphics adapter to
///     determine if the graphics adapter supports the requested resolution.
///     If so, the resolution is set and the function returns
///     <see langword="true" />.  If not, no change is made and the function
///     returns <see langword="false" />.
/// </summary>
/// <param name="game">The game.</param>
/// <param name="graphicsDeviceManager">The graphics.</param>
/// <param name="width">Desired screen width.</param>
/// <param name="height">Desired screen height.</param>
/// <param name="isFullScreen">
///     True if you wish to go to Full Screen,
///     <see langword="false" /> for Windowed Mode.
/// </param>
/// <param name="targetElapsedIntervalInMillis">The target elapsed interval in milliseconds.</param>
/// <param name="isSynchronizeWithVerticalRetrace">
///     if set to <c>true</c> [is synchronize with vertical retrace].
/// </param>
/// <param name="isFixedTimeStep">
///     if set to <c>true</c> [is fixed time step].
/// </param>
/// <param name="isPreferMultiSampling">
///     if set to <c>true</c> [is prefer multi sampling].
/// </param>
/// <param name="preferredBackBufferFormat">The preferred back buffer format.</param>
/// <param name="preferredDepthFormat">The preferred depth format.</param>
/// <param name="zero">
///     The left upper corner of the bound-rectangle. If null, we will take the left-upper corner of the
///     screen the window is currently on.
/// </param>
/// <returns></returns>
public static Rectangle? InitGraphicsMode(Game game, GraphicsDeviceManager graphicsDeviceManager, int width,
  int height,
  bool isFullScreen, float targetElapsedIntervalInMillis = 1000f / 60f,
  bool isSynchronizeWithVerticalRetrace = true, bool isFixedTimeStep = false,
  bool isPreferMultiSampling = true,
  SurfaceFormat preferredBackBufferFormat = SurfaceFormat.Color,
  DepthFormat preferredDepthFormat = DepthFormat.None, Point? zero = null)
{
	try {
		// Form.ActiveForm really gets the form of the currently active window. We want the game's window.
		var f = (Form) Control.FromHandle(game.Window.Handle);
		if (f == null)
			return null;
        
		graphicsDeviceManager.PreferredBackBufferFormat = preferredBackBufferFormat;
		graphicsDeviceManager.PreferredDepthStencilFormat = preferredDepthFormat;
		// This tells MonoGame to not switch the mode of the graphics-card directly, but to scale the window.
		graphicsDeviceManager.HardwareModeSwitch = false;
		// This tells MonoGame to fetch pre DX9c devices as well.
		graphicsDeviceManager.GraphicsProfile = GraphicsProfile.Reach;
		// Enables anti-aliasing.
		graphicsDeviceManager.PreferMultiSampling = isPreferMultiSampling;
		// Set to true to set the draw-restriction to the refresh-rate of the monitor.
		graphicsDeviceManager.SynchronizeWithVerticalRetrace = isSynchronizeWithVerticalRetrace;
		// If set to false, you are unbinding update and draw thus allowing them to be called separately.
		game.IsFixedTimeStep = isFixedTimeStep;
		// Determines how often update will be called (works only if IsFixedTimeStep is true.
		// 60 times per second is the default value.
		game.TargetElapsedTime = TimeSpan.FromMilliseconds(targetElapsedIntervalInMillis);

		Screen screen;
		if (zero.HasValue && isFullScreen)
		{
			screen = Screen.FromPoint(new System.Drawing.Point(zero.Value.X, zero.Value.Y));
		}
		else
		{
			screen = Screen.FromControl(f);
			zero = new Point(screen.Bounds.X, screen.Bounds.Y);
		}

		var x = screen.Bounds.Width;
		var y = screen.Bounds.Height;

		width = Math.Min(width, x);
		height = Math.Min(height, y);

		if (!isFullScreen)
		{
			x = width + 16;
			y = height + 38;
			zero = zero + new Point((screen.Bounds.Width - width) / 2, (screen.Bounds.Height - height) / 2);
		}

		game.Window.IsBorderless = isFullScreen;

#if LINUX
		Debug.DLog("getting form...");
		Form form = Control.FromHandle(game.Window.Handle).FindForm();
		if (form != null)
		{
			Debug.DLog("setting location...");
			form.Location = new System.Drawing.Point(x, y);
		}
		else
		{
			Debug.DLog("form was null.");
		}
#else
		game.Window.Position = zero.Value;
#endif

		graphicsDeviceManager.PreferredBackBufferWidth = width;
		graphicsDeviceManager.PreferredBackBufferHeight = height;
		graphicsDeviceManager.IsFullScreen = isFullScreen;
		graphicsDeviceManager.ApplyChanges();

		f.SetDesktopBounds(zero.Value.X, zero.Value.Y, x, y);
		return new Rectangle(zero.Value.X, zero.Value.Y, width, height);
    }
	catch (Exception)
	{
		return null;
	}
}
```



#### Used Tools

* [Resharper](https://www.jetbrains.com/resharper/)

* [MonoGame](http://www.monogame.net/)

* The font is called Arsenal and you can find it here [Fontsquirrel - Arsenal](https://www.fontsquirrel.com/fonts/arsenal?q%5Bterm%5D=arsenal&q%5Bsearch_check%5D=Y)

  It has an insane amount of supported languages (and therefore glyphs) and is under the SIL Open Font License v1.10. So it's safe to use in your games.




[homepage]: http://www.unterrainer.info
[coding]: http://www.unterrainer.info/Home/Coding
[github]: https://github.com/UnterrainerInformatik/MonoGame-DXGraphicCardTester