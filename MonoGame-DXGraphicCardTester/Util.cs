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
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame_DXGraphicCardTester
{
	public static class Util
	{
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
			try
			{
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
	}
}