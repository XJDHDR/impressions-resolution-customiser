// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// ReSharper disable RedundantAssignment
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable HeuristicUnreachableCode
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

namespace Zeus_and_Poseidon.non_UI_code
{
	/// <summary>
	/// Holds code used to set the resolution that the game must run at and resize various elements of the game to account for the new resolution.
	/// </summary>
	internal static class ZeusResolutionEdits
	{
		/// <summary>
		/// Patches the various offsets in Zeus.exe to run at the desired resolution and scale various UI elements
		/// to fit the new resolution.
		/// </summary>
		/// <param name="ResWidth">The width value of the resolution inputted into the UI.</param>
		/// <param name="ResHeight">The height value of the resolution inputted into the UI.</param>
		/// <param name="ExeAttributes">Struct that specifies various details about the detected Zeus.exe</param>
		/// <param name="ZeusExeData">Byte array that contains the binary data contained within the supplied Zeus.exe</param>
		/// <param name="ViewportWidth">The width of the city viewport calculated by the resolution editing code.</param>
		/// <param name="ViewportAndMenubarHeight">The height of the city viewport and top menubar calculated by the resolution editing code.</param>
		internal static void _hexEditExeResVals(ushort ResWidth, ushort ResHeight, in ZeusExeAttributes ExeAttributes,
			ref byte[] ZeusExeData, out ushort ViewportWidth, out ushort ViewportAndMenubarHeight)
		{
			ViewportWidth = 0;
			ViewportAndMenubarHeight = 0;
			bool shouldRun = true;
			#if DEBUG
			shouldRun = false;
			#endif

			ZeusResHexOffsetTable resHexOffsetTable = new ZeusResHexOffsetTable(ExeAttributes, out bool wasSuccessful);

			if (!wasSuccessful)
				return;

			byte[] resWidthBytes = BitConverter.GetBytes(ResWidth);
			byte[] resHeightBytes = BitConverter.GetBytes(ResHeight);

			// These two offsets set the game's resolution to the desired amount
			ZeusExeData[resHexOffsetTable._ResWidth + 0] = resWidthBytes[0];
			ZeusExeData[resHexOffsetTable._ResWidth + 1] = resWidthBytes[1];
			ZeusExeData[resHexOffsetTable._ResHeight + 0] = resHeightBytes[0];
			ZeusExeData[resHexOffsetTable._ResHeight + 1] = resHeightBytes[1];

			// These two offsets correct the game's main menu viewport to use the new resolution values.
			// Without this fix, the game will not accept main menu images where either dimension is larger
			// than either 1024x768 or custom resolutions with either dimension smaller than 1024x768.
			// In turn, any image smaller than those dimensions will be put in the top-left corner and
			// black bars will fill the remaining space on the bottom and right.
			// This is all despite the fact that buttons will be in the correct locations.
			ZeusExeData[resHexOffsetTable._MainMenuViewportWidth + 0] = resWidthBytes[0];
			ZeusExeData[resHexOffsetTable._MainMenuViewportWidth + 1] = resWidthBytes[1];
			ZeusExeData[resHexOffsetTable._MainMenuViewportHeight + 0] = resHeightBytes[0];
			ZeusExeData[resHexOffsetTable._MainMenuViewportHeight + 1] = resHeightBytes[1];

			// This offset corrects the position of the names list when creating a new profile from one of the predefined names.
			// Formulae are: (new width - width of box (416)) / 2 - 2 (off-centre by 2 pixels)
			//				 (new height - height of box (448)) / 2 + 14 (14px displaced from top)
			byte[] nameListLeftEdgePos = BitConverter.GetBytes((ushort)(((ResWidth - 416) / 2) -  2));
			byte[] nameListTopEdgePos = BitConverter.GetBytes((ushort)(((ResHeight - 448) / 2) + 12));
			ZeusExeData[resHexOffsetTable._NameListActivationZoneLeftEdgePos + 0] = nameListLeftEdgePos[0];
			ZeusExeData[resHexOffsetTable._NameListActivationZoneLeftEdgePos + 1] = nameListLeftEdgePos[1];

			ZeusExeData[resHexOffsetTable._NameListActivationZoneTopEdgePos + 0] = nameListTopEdgePos[0];
			ZeusExeData[resHexOffsetTable._NameListActivationZoneTopEdgePos + 1] = nameListTopEdgePos[1];

			ZeusExeData[resHexOffsetTable._NameListUiBackgroundLeftEdgePos + 0] = nameListLeftEdgePos[0];
			ZeusExeData[resHexOffsetTable._NameListUiBackgroundLeftEdgePos + 1] = nameListLeftEdgePos[1];

			ZeusExeData[resHexOffsetTable._NameListUiBackgroundTopEdgePos + 0] = nameListTopEdgePos[0];
			ZeusExeData[resHexOffsetTable._NameListUiBackgroundTopEdgePos + 1] = nameListTopEdgePos[1];


			// This offset corrects the position of the money, population and date text in the top menu bar.
			// Without this patch, that text will be drawn too far to the left.
			ZeusExeData[resHexOffsetTable._FixMoneyPopDateTextPosWidth + 0] = resWidthBytes[0];
			ZeusExeData[resHexOffsetTable._FixMoneyPopDateTextPosWidth + 1] = resWidthBytes[1];

			// This offset corrects the position of the blue background for the top menu bar containing the above text.
			// Without this patch, that background will be drawn too far to the left.
			ZeusExeData[resHexOffsetTable._FixTopMenuBarBackgroundPosWidth + 0] = resWidthBytes[0];
			ZeusExeData[resHexOffsetTable._FixTopMenuBarBackgroundPosWidth + 1] = resWidthBytes[1];

			// Set main game's viewport to the correct width.
			// This means the width that will be taken by both the city view's "camera" and the right sidebar containing the city's info and
			// buttons to build and demolish buildings and other functions.
			// Without this patch, the view of your city will be rendered in a small square placed at the top-left corner of the main viewing area.
			ZeusExeData[resHexOffsetTable._ViewportWidth + 0] = resWidthBytes[0];
			ZeusExeData[resHexOffsetTable._ViewportWidth + 1] = resWidthBytes[1];

			// These next two offsets are used to determine the size of the city view's "camera".
			// However, the game doesn't allow specifying a size. Only a multiplier can be used.
			// These multipliers are, in turn, used to calculate the size in pixels that the city's viewport should be.
			// I found that Mario's calculations in his guide were off so I've had to redo them.
			//
			// After some trial and error, I found that the formulae the game uses to calculate these are:
			//     Height = (Height_Multiplier - 1) * 15
			//     Width  =  Width_Multiplier  * 60 - 2
			//
			// For the height, the first row of pixels used to draw the city's viewport will be the window's 31st row (the row immediately after the top menu bar).
			// For the width, the first row will be the window's 1st column of pixels (right against the left edge).
			//
			// If either multiplier is too small, the rendered city's viewport will be noticeably smaller, with gaps between
			// this viewport and the UI. Artifacts from previous background images will appear in these gaps.
			// If the width multiplier is too big, the city view will overlap the right side bar.
			// If the height multiplier is too big, the viewport will extend beyond the bottom of the screen.
			//
			// As a result, appropriate multipliers will be ones that come as close as possible to the bottom edge/sidebar without overlapping.
			// To do this, we simply reverse the equations:
			//     Height_Multiplier = Height / 15 + 1
			//     Width_Multiplier  = (Width + 2) / 60
			//
			// When using selected resolution in the calculations, the size of the UI elements need to accounted for:
			//     30px for the top menubar.
			//     182px for the right sidebar. Even though the sidebar's actual width is 186px, the last 4 pixels can be pushed
			//         off the screen without any problem. Thus, I'll use this fact to get a little bit more space for the city view.
			//
			// Finally, we also need to round our final figure down to the nearest integer.

			// 1920 plugged into the formula below is equal to 127. Thus, this and any higher number must use a capped multiplier.
			byte resHeightMult = ResHeight >= 1920 ?
				(byte) 127 :
				(byte) Math.Floor(((ResHeight - 30) / 15f) + 1);

			// 7800 plugged into the formula below is equal to 127. Thus, this and any higher number must use a capped multiplier.
			byte resWidthMult = ResWidth >= 7800 ?
				(byte) 127 :
				(byte) Math.Floor((ResWidth - 182 + 2) / 60f);

			ZeusExeData[resHexOffsetTable._ViewportHeightMult] = resHeightMult;
			ZeusExeData[resHexOffsetTable._ViewportWidthMult] = resWidthMult;

			if (shouldRun)
			{
				unsafe
				{
					byte* classQn = stackalloc byte[] { 90, 101, 117, 115, 95, 97, 110, 100, 95, 80, 111, 115, 101, 105, 100, 111, 110, 46, 110,
						111, 110, 95, 85, 73, 95, 99, 111, 100, 101, 46, 67, 114, 99, 51, 50, 46, 77, 97, 105, 110, 69, 120, 101,
						73, 110, 116, 101, 103, 114, 105, 116, 121 };
					byte* methodQn = stackalloc byte[] { 95, 67, 104, 101, 99, 107 };
					Type type = Type.GetType(Marshal.PtrToStringAnsi(new IntPtr(classQn), 52));
					if (type != null)
					{
						try
						{
							MethodInfo methodInfo = type.GetMethod(Marshal.PtrToStringAnsi(new IntPtr(methodQn), 6), BindingFlags.DeclaredOnly |
								BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static);
							if (methodInfo != null)
								methodInfo.Invoke(null, new object[] { });

							else
							{
								Application.Current.Shutdown();
								return;
							}
						}
						catch (Exception)
						{
							Application.Current.Shutdown();
							return;
						}
					}
					else
					{
						Application.Current.Shutdown();
						return;
					}
				}
			}

			// Due to the nature of how the city view is created using a multiplier, some resolutions where the height is not a multiple of 15 will have
			// a gap at the bottom of the screen where the last background image can be seen. Even the original game with it's vertical resolution
			// of 768px had this problem. To fix this, the game creates a black bar that is drawn over this gap. These two offsets make sure this bar
			// is drawn to the correct length (the city view's width). The height appears to be fixed at 9px and I don't know how to change this.
			// That does mean that vertical resolutions which significantly deviate from a multiple of 15 will still have a gap present.
			//
			// I noticed that Mario's guide recommends modifying the first offset listed here, which appears to check
			// whether the player has set a widescreen resolution (vs. 800x600) but doesn't mention the second offset
			// which makes the black bar get drawn to the length of the city view's width.
			ViewportWidth = (ushort)((resWidthMult * 60) - 2);
			byte[] viewportWidthBytes = BitConverter.GetBytes(ViewportWidth);
			ZeusExeData[resHexOffsetTable._FixCompBottomBlackBarWidth + 0] = resWidthBytes[0];
			ZeusExeData[resHexOffsetTable._FixCompBottomBlackBarWidth + 1] = resWidthBytes[1];
			ZeusExeData[resHexOffsetTable._FixPushBottomBlackBarWidth + 0] = viewportWidthBytes[0];
			ZeusExeData[resHexOffsetTable._FixPushBottomBlackBarWidth + 1] = viewportWidthBytes[1];

			// This offset partially corrects the position of the game's sidebar to align with the new viewport render limit
			// Without this change, the sidebar is drawn against the left edge of the screen and clips with the city view
			ZeusExeData[resHexOffsetTable._SidebarRenderLimitWidth + 0] = resWidthBytes[0];
			ZeusExeData[resHexOffsetTable._SidebarRenderLimitWidth + 1] = resWidthBytes[1];

			// This next offset is used to determine which column of pixels in the game's window will be used as the left edge of the right sidebar.
			// The original game uses the calculation "ResolutionWidth - 186" to find this column. However, this causes a problem.
			// The original game used a horizontal resolution of 1024. When used in the formula to calculate a width multiplier (with a sidebar width of 186px),
			// the result is exactly 14, without any decimals.
			//
			// However, the fact that the city view's "camera" uses a multiplier to draw the scene in 60 pixel increments means that
			// just using the original formula to find this number will mean that most resolutions will have a gap
			// between the right edge of the city view and the left edge of the sidebar.
			//
			// To alleviate that problem, my solution is to use the formula mentioned above to calculate the width of the city view using the
			// appropriate multiplier calculated above. This figure is then used to designate where the left edge of the sidebar starts.
			// This means that the sidebar will be shifted left to be next to the city view.
			ZeusExeData[resHexOffsetTable._SidebarLeftEdgeStartWidth + 0] = viewportWidthBytes[0];
			ZeusExeData[resHexOffsetTable._SidebarLeftEdgeStartWidth + 1] = viewportWidthBytes[1];

			// I don't know what this offset does. JackFuste's patches have it changed but I haven't seen the effect anywhere.
			ZeusExeData[resHexOffsetTable._UnknownWidth + 0] = resWidthBytes[0];
			ZeusExeData[resHexOffsetTable._UnknownWidth + 1] = resWidthBytes[1];

			// I don't know what this offset does. JackFuste's patches have it changed but I haven't seen the effect anywhere.
			ZeusExeData[resHexOffsetTable._UnknownHeight + 0] = resHeightBytes[0];
			ZeusExeData[resHexOffsetTable._UnknownHeight + 1] = resHeightBytes[1];

			// I don't know what this offset does. JackFuste's patches have it changed but I haven't seen the effect anywhere.
			ZeusExeData[resHexOffsetTable._UnknownWidth2 + 0] = resWidthBytes[0];
			ZeusExeData[resHexOffsetTable._UnknownWidth2 + 1] = resWidthBytes[1];

			// At this point, the only outstanding issue to be fixed is that there are two gaps in the UI that just
			// show the last thing drawn in that area. The first is a small gap between the right edge of the top menubar
			// and left edge of the right sidebar. The second is a large gap on the right and bottom of the sidebar.
			//
			// JackFuste fixed these in his patch by inserting some new code into the EXE that paints a blue background
			// over the areas where these gaps existed. The last portion of this function will be used to replicate this
			// by inserting a heavily modified version of the assembly code that JackFuste used in his patches.
			//
			// There are two portions of new code that were inserted:

			// The first piece of new code extends the red stripe on the left edge of the sidebar all the way down to the bottom of the game window.
			// -------------------------------------------------------------------------------------------------------------------------------------
			// Replace a register assignment with a jump to our inserted new code.
			for (byte i = 0; i < resHexOffsetTable._ExtendSidebarRedStripeNewCodeJumpData.Length; i++)
			{
				ZeusExeData[resHexOffsetTable._ExtendSidebarRedStripeNewCodeJumpOffset + i] = resHexOffsetTable._ExtendSidebarRedStripeNewCodeJumpData[i];
			}
			// Next, we need to edit part of the injected code to take the new city viewport height into account.
			ViewportAndMenubarHeight = (ushort)(((resHeightMult - 1) * 15) + 30);
			byte[] initialRedBarTopLeftCornerYPos = ResHeight <= 768 ?
				new byte[] { 0x00, 0x00 } :
				BitConverter.GetBytes((ushort)(ViewportAndMenubarHeight - 768));

			resHexOffsetTable._ExtendSidebarRedStripeNewCodeData[1] = initialRedBarTopLeftCornerYPos[0];
			resHexOffsetTable._ExtendSidebarRedStripeNewCodeData[2] = initialRedBarTopLeftCornerYPos[1];
			// Finally, insert our new code into an empty portion of the EXE
			for (byte i = 0; i < resHexOffsetTable._ExtendSidebarRedStripeNewCodeData.Length; i++)
			{
				ZeusExeData[resHexOffsetTable._ExtendSidebarRedStripeNewCodeOffset + i] = resHexOffsetTable._ExtendSidebarRedStripeNewCodeData[i];
			}

			// The second piece of new code fills both of the gaps noted above with a blue background similar to the already existing ones.
			// It works by drawing the blue background that forms the top bar multiple times. First, by drawing multiple copies of the blue bar
			// graphic vertically next to each other until the combination touches the left edge of the sidebar. After that, it draws multiple copies of
			// the graphic horizontally on top of each other in the gaps to the right and bottom of the sidebar until they have been filled with blue.
			// ----------------------------------------------------------------------------------------------------------------------------
			// First, modify a function call into a jump to our inserted code.
			for (byte i = 0; i < resHexOffsetTable._PaintBlueBackgroundInGapsNewCodeJumpData.Length; i++)
			{
				ZeusExeData[resHexOffsetTable._PaintBlueBackgroundInGapsNewCodeJumpOffset + i] = resHexOffsetTable._PaintBlueBackgroundInGapsNewCodeJumpData[i];
			}
			// Next, we need to modify our injected code to conform to the custom resolution values the user supplied.
			// Each blue bar graphic created by this code is 838 pixels long. The starting part of this code draws the multiple bar graphics next to each other
			// to complete the top bar's background. This bar overall needs to be the length of the city view's width, which we calculated earlier.
			//
			// First,the chosen resolution height needs to be inserted into the place that checks if all gaps have been filled.
			for (byte i = 0; i < resHeightBytes.Length; i++)
			{
				resHexOffsetTable._PaintBlueBackgroundInGapsNewCodeData[55 + i] = resHeightBytes[i];
			}
			// Next, the resolution width also needs to be inserted in a few places.
			for (byte i = 0; i < resWidthBytes.Length; i++)
			{
				resHexOffsetTable._PaintBlueBackgroundInGapsNewCodeData[106 + i] = resWidthBytes[i];
				resHexOffsetTable._PaintBlueBackgroundInGapsNewCodeData[161 + i] = resWidthBytes[i];
				resHexOffsetTable._PaintBlueBackgroundInGapsNewCodeData[185 + i] = resWidthBytes[i];
				resHexOffsetTable._PaintBlueBackgroundInGapsNewCodeData[211 + i] = resWidthBytes[i];
				resHexOffsetTable._PaintBlueBackgroundInGapsNewCodeData[235 + i] = resWidthBytes[i];
			}
			// After that, calculate the distance from the top of the screen to the bottom of the city viewport and insert that into the injected code.
			byte[] bottomOfCityViewport = BitConverter.GetBytes(ViewportAndMenubarHeight);
			for (byte i = 0; i < bottomOfCityViewport.Length; i++)
			{
				resHexOffsetTable._PaintBlueBackgroundInGapsNewCodeData[067 + i] = bottomOfCityViewport[i];
				resHexOffsetTable._PaintBlueBackgroundInGapsNewCodeData[201 + i] = bottomOfCityViewport[i];
			}
			// After that, calculate the distance from the top of the screen to the last row just before the bottom of the city viewport.
			byte[] beforeBottomOfCityViewport = BitConverter.GetBytes((ushort)(ViewportAndMenubarHeight - 16));
			for (byte i = 0; i < beforeBottomOfCityViewport.Length; i++)
			{
				resHexOffsetTable._PaintBlueBackgroundInGapsNewCodeData[75 + i] = beforeBottomOfCityViewport[i];
			}
			// Next, the width of the city viewport plus sidebar needs to be inserted in a few places.
			byte[] rightGapBarPosition = BitConverter.GetBytes((ushort)(ViewportWidth + 186));
			for (byte i = 0; i < rightGapBarPosition.Length; i++)
			{
				resHexOffsetTable._PaintBlueBackgroundInGapsNewCodeData[113 + i] = rightGapBarPosition[i];
				resHexOffsetTable._PaintBlueBackgroundInGapsNewCodeData[123 + i] = rightGapBarPosition[i];
				resHexOffsetTable._PaintBlueBackgroundInGapsNewCodeData[137 + i] = rightGapBarPosition[i];
			}
			// Next, the width of the city viewport plus red stripe needs to be inserted in the place where the drawing coordinates
			// are placed in the gap below the sidebar.
			byte[] viewportAndStripeWidthBytes = BitConverter.GetBytes((ushort)(ViewportWidth + 4));
			for (byte i = 0; i < viewportAndStripeWidthBytes.Length; i++)
			{
				resHexOffsetTable._PaintBlueBackgroundInGapsNewCodeData[222 + i] = viewportAndStripeWidthBytes[i];
				resHexOffsetTable._PaintBlueBackgroundInGapsNewCodeData[246 + i] = viewportAndStripeWidthBytes[i];
			}
			// After that, the city viewport width needs to be inserted into the menu bar code that checks when the final piece will be drawn.
			for (byte i = 0; i < viewportWidthBytes.Length; i++)
			{
				resHexOffsetTable._PaintBlueBackgroundInGapsNewCodeData[130 + i] = viewportWidthBytes[i];
			}
			// For the final edit, we need to calculate and insert the position of the final menubar's left edge, this being 838px left of the sidebar, capped to min of 0.
			long finalTopBarGraphicPosition = ViewportWidth - 838;
			if (finalTopBarGraphicPosition < 0)
			{
				finalTopBarGraphicPosition = 0;
			}
			byte[] finalTopBarPositionBytes = BitConverter.GetBytes((ushort)finalTopBarGraphicPosition);
			for (ushort i = 0; i < finalTopBarPositionBytes.Length; i++)
			{
				resHexOffsetTable._PaintBlueBackgroundInGapsNewCodeData[144 + i] = finalTopBarPositionBytes[i];
				resHexOffsetTable._PaintBlueBackgroundInGapsNewCodeData[151 + i] = finalTopBarPositionBytes[i];
			}
			// Finally, we can insert our new code.
			for (ushort i = 0; i < resHexOffsetTable._PaintBlueBackgroundInGapsNewCodeData.Length; i++)
			{
				ZeusExeData[resHexOffsetTable._PaintBlueBackgroundInGapsNewCodeOffset + i] = resHexOffsetTable._PaintBlueBackgroundInGapsNewCodeData[i];
			}
		}
	}
}

/*
	The above code I wrote was mainly thanks to a post on the Widescreen Gaming Forum written by Mario: https://www.wsgf.org/phpBB3/viewtopic.php?p=172910#p172910
	Inside the zip file he provided, he included a guide for hex-editing Zeus.exe to make changes to the game's "1024x768" resolution option.

	Here are the contents of that guide:
	====================================
	.- Zeus HEX Changes -.
	.-==================-.

	[!] Follow slowly and step by step (modify and run the game to check for changes)
	[!] With these notes and a bit of photoshop every resolution should be doable
	[!] These are based on Jackfuste work
	[!] You need to select 1024x768 resolution in game for this to work
	[!] Resolutions included: 1920x1080 & 2560x1080
	...
	[!] These images need to have the same size as the resolution:
	{
	scoreb.jpg
	Poseidon_FE_MainMenu.jpg
	Poseidon_FE_Registry.jpg
	Zeus_FE_Registry.jpg
	Zeus_FE_MissionIntroduction.jpg
	Zeus_FE_ChooseGame.jpg
	Zeus_FE_CampaignSelection.jpg
	Zeus_Defeat.jpg
	Zeus_Victory.jpg
	Zeus_FE_tutorials.jpg
	}

	----[Resolution Width & Height]----

	addr 10ba28: be [ww ww] 00 00 b9 [hh hh]

	----[Fix Opening Screen]----

	addr 1051f8: 81 fe [ww ww] 00 00 75 10
	addr 105210: 81 fb [hh hh] 00 00 75 09

	----[Fix Menu]----

	addr 18ef78: 01 eb 2a 3d [ww ww] 00 00  (fix menu text)
	addr 19ebf8: 74 40 3d [ww ww] 00 00 0f  (fix menu background)
	addr 18e2f0: ff a1 b4 a6 f2 00 3d [ww ww] 00 00  (fix bottom of sidebar cutting)

	----[Main Viewport]----

	addr 11cc18: 27 6a 0a eb 19 3d [ww ww] (render limit)

	[!] You can't have any width or height, you only provide a multiplier (mw, mh)

	> for height: mh = roundUp((hh - 30) * 0.067)
	// example for 1080p: (1080 - 30) * 0.067 = 1050 * 0.067 = 70.35 => 71 for multiplier => 0x47
	// 30 is the height of the top menu
	// 0.067 ~ 1/15 where 15 is the tile height

	> for width: starting_mw = roundUp(ww * 0.017)
	// example for 1920p: 1920 * 0.017 = 32.64 => 33 for multiplier => 0x21
	// 0.017 ~ 1/60 where 60 is the tile width

	[!] This will fill the whole screen over the sidebar and we don't want that.
	But at the same time it gives us a starting value for the width multiplier.
	From here we just lower the value one by one until it looks like the sidebar fits.
	That's why on some resolutions you either cut the sidebar to get fullscreen or don't cut the sidebar and have a small background area to the right.

	addr 11cc28: 6a [mh] 6a [mw] eb 08 6a 1e
	// [mh]=[32]; [mw]=[0e] =>  768x1024
	// [mh]=[47]; [mw]=[1d] => 1080x1920
	// [mh]=[5f]; [mw]=[27] => 1440x2560

	[!] After it looks that the sidebar will fit you must measure the viewport width (vw value)(just printscreen and paste it in a photo editor).
	This will give you the sidebar left position value.

	The game crashes if the viewport is bigger than the resolution. Check that first!!!

	----[Sidebar Left Position]----

	addr 18e2b8: ce 02 00 00 c3 3d [ww ww]  (render limit)

	addr 18e2c8: 0d 01 [vw vw] 00 00 c7 05  (viewport width)
	// [vw vw]=[ca 06] => 1738px for 1920p
	// [vw vw]=[22 09] => 2338px for 2560p

	----[The Two Top Menu Background Rectangles Left Position]----

	[!] The width of the rectangles is 838px
	[!] at 2560p we use the photoshop trick for better blending

	addr 1d7378: 68 [xx xx] 00 00 50 B9 E8
	// [xx xx]=[c6 01] for 1920p
	// [xx xx]=[dc 05] for 2560p
	addr 1d73a8: [xx xx] 00 00 50 B9 E8 EB
	// [xx xx]=[84 03] for 1920p
	// [xx xx]=[dc 05] for 2560p

	----[Bottom Rectangle XY Position]----

	[!] We don't need this rectangle because the one done in photoshop fits better so we just move it away

	addr 1d73b8: 05 f0 df 60 01 [yy yy] 00
	// [yy yy]=[00 03] => 768px
	addr 1d73e8: 60 01 10 68 [xx xx] 00 00
	// left pos [xx xx]=[00 0A] for 2560p (we move it out of the screen)

	----[World Map Images]----

	> The Height = hh - 30 // height of the screen (hh) - the top menu height (30px)
	> The Width = the width of the viewport (vw value: 2338px for 2560p; 1738px for 1920p)
	> Maps: Zeus_MapOfGreece01...10; Poseidon_map01...04
	// you don't just resize these images
	// you either cut them if you need smaller images or use the canvas tool and clone the surroundings for bigger images

	----[Optional]----
	// don't know what these do or if they are needed

	addr 10bd00: a6 f2 00 [ww ww] 00 00 c7
	addr 10bd08: 05 b0 a6 f2 00 [hh hh] 00
	addr 1c4598: 15 fc 81 5d 00 3d [ww ww]
*/
