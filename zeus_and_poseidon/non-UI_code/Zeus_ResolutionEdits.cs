// This code is part of the Impressions Resolution Customiser project
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using System;
using static Zeus_and_Poseidon.Zeus_ExeDefinitions;

namespace Zeus_and_Poseidon
{
	/// <summary>
	/// Holds code used to set the resolution that the game must run at and resize various elements of the game to account for the new resolution.
	/// </summary>
	internal class Zeus_ResolutionEdits
	{
		/// <summary>
		/// Patches the various offsets in Zeus.exe to run at the desired resolution and scale various UI elements 
		/// to fit the new resolution.
		/// </summary>
		/// <param name="ResWidth">The width value of the resolution inputted into the UI.</param>
		/// <param name="ResHeight">The height value of the resolution inputted into the UI.</param>
		/// <param name="ExeAttributes">Struct that specifies various details about the detected Zeus.exe</param>
		/// <param name="ZeusExeData">Byte array that contains the binary data contained within the supplied Zeus.exe</param>
		internal static void _hexEditExeResVals(ushort ResWidth, ushort ResHeight, ExeAttributes ExeAttributes, ref byte[] ZeusExeData)
		{
			if (FillResHexOffsetTable(ExeAttributes, out ResHexOffsetTable _resHexOffsetTable))
			{
				byte[] _resWidthBytes = BitConverter.GetBytes(ResWidth);
				byte[] _resHeightBytes = BitConverter.GetBytes(ResHeight);

				// These two offsets set the game's resolution to the desired amount
				ZeusExeData[_resHexOffsetTable._resWidth + 0] = _resWidthBytes[0];
				ZeusExeData[_resHexOffsetTable._resWidth + 1] = _resWidthBytes[1];
				ZeusExeData[_resHexOffsetTable._resHeight + 0] = _resHeightBytes[0];
				ZeusExeData[_resHexOffsetTable._resHeight + 1] = _resHeightBytes[1];

				// These two offsets correct the game's main menu viewport to use the new resolution values.
				// Without this fix, the game will not accept main menu images where either dimension is larger
				// than either 1024x768 or custom resolutions with either dimension smaller than 1024x768.
				// In turn, any image smaller than those dimensions will be put in the top-left corner and
				// black bars will fill the remaining space on the bottom and right.
				// This is all despite the fact that buttons will be in the correct locations.
				ZeusExeData[_resHexOffsetTable._mainMenuViewportWidth + 0] = _resWidthBytes[0];
				ZeusExeData[_resHexOffsetTable._mainMenuViewportWidth + 1] = _resWidthBytes[1];
				ZeusExeData[_resHexOffsetTable._mainMenuViewportHeight + 0] = _resHeightBytes[0];
				ZeusExeData[_resHexOffsetTable._mainMenuViewportHeight + 1] = _resHeightBytes[1];

				// This offset corrects the position of the money, population and date text in the top menu bar.
				// Without this patch, that text will be drawn too far to the left.
				ZeusExeData[_resHexOffsetTable._fixMoneyPopDateTextPosWidth + 0] = _resWidthBytes[0];
				ZeusExeData[_resHexOffsetTable._fixMoneyPopDateTextPosWidth + 1] = _resWidthBytes[1];

				// This offset corrects the position of the blue background for the top menu bar containing the above text.
				// Without this patch, that background will be drawn too far to the left.
				ZeusExeData[_resHexOffsetTable._fixTopMenuBarBackgroundPosWidth + 0] = _resWidthBytes[0];
				ZeusExeData[_resHexOffsetTable._fixTopMenuBarBackgroundPosWidth + 1] = _resWidthBytes[1];

				// Set main game's viewport to the correct width.
				// This means the width that will be taken by both the city view's "camera" and the right sidebar containing the city's info and 
				// buttons to build and demolish buildings and other functions.
				// Without this patch, the view of your city will be rendered in a small square placed at the top-left corner of the main viewing area.
				ZeusExeData[_resHexOffsetTable._viewportWidth + 0] = _resWidthBytes[0];
				ZeusExeData[_resHexOffsetTable._viewportWidth + 1] = _resWidthBytes[1];

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
				byte _resHeightMult;
				// 1920 plugged into the formula below is equal to 127. Thus, this and any higher number must use a capped multiplier.
				if (ResHeight >= 1920)
				{
					_resHeightMult = 127;
				}
				else
				{
					_resHeightMult = (byte)Math.Floor((ResHeight - 30) / 15f + 1); // fs are required. Otherwise, compiler error CS0121 occurrs.
				}
				byte _resWidthMult;
				// 7800 plugged into the formula below is equal to 127. Thus, this and any higher number must use a capped multiplier.
				if (ResWidth >= 7800)
				{
					_resWidthMult = 127;
				}
				else
				{
					_resWidthMult = (byte)Math.Floor((ResWidth - 182 + 2) / 60f); // fs are required. Otherwise, compiler error CS0121 occurrs.
				}
				ZeusExeData[_resHexOffsetTable._viewportHeightMult] = _resHeightMult;
				ZeusExeData[_resHexOffsetTable._viewportWidthMult] = _resWidthMult;

				// Due to the nature of how the city view is created using a multiplier, some resolutions where the height is not a multiple of 15 will have
				// a gap at the bottom of the screen where the last background image can be seen. Even the original game with it's vertical resolution
				// of 768px had this problem. To fix this, the game creates a black bar that is drawn over this gap. These two offsets make sure this bar
				// is drawn to the correct length (the city view's width). The height appears to be fixed at 9px and I don't know how to change this.
				// That does mean that vertical resolutions which significantly deviate from a multiple of 15 will still have a gap present.
				// 
				// I noticed that Mario's guide recommends modifying the first offset listed here, which appears to check
				// whether the player has set a widescreen resolution (vs. 800x600) but doesn't mention the second offset
				// which makes the black bar get drawn to the length of the city view's width.
				ushort _cityViewWidth = (ushort)(_resWidthMult * 60 - 2);
				byte[] _viewportWidthBytes = BitConverter.GetBytes(_cityViewWidth);
				ZeusExeData[_resHexOffsetTable._fixCompBottomBlackBarWidth + 0] = _resWidthBytes[0];
				ZeusExeData[_resHexOffsetTable._fixCompBottomBlackBarWidth + 1] = _resWidthBytes[1];
				ZeusExeData[_resHexOffsetTable._fixPushBottomBlackBarWidth + 0] = _viewportWidthBytes[0];
				ZeusExeData[_resHexOffsetTable._fixPushBottomBlackBarWidth + 1] = _viewportWidthBytes[1];

				// This offset partially corrects the position of the game's sidebar to align with the new viewport render limit
				// Without this change, the sidebar is drawn against the left edge of the screen and clips with the city view
				ZeusExeData[_resHexOffsetTable._sidebarRenderLimitWidth + 0] = _resWidthBytes[0];
				ZeusExeData[_resHexOffsetTable._sidebarRenderLimitWidth + 1] = _resWidthBytes[1];

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
				ZeusExeData[_resHexOffsetTable._sidebarLeftEdgeStartWidth + 0] = _viewportWidthBytes[0];
				ZeusExeData[_resHexOffsetTable._sidebarLeftEdgeStartWidth + 1] = _viewportWidthBytes[1];

				// I don't know what this offset does. JackFuste's patches have it changed but I haven't seen the effect anywhere.
				ZeusExeData[_resHexOffsetTable._unknownWidth + 0] = _resWidthBytes[0];
				ZeusExeData[_resHexOffsetTable._unknownWidth + 1] = _resWidthBytes[1];

				// I don't know what this offset does. JackFuste's patches have it changed but I haven't seen the effect anywhere.
				ZeusExeData[_resHexOffsetTable._unknownHeight + 0] = _resHeightBytes[0];
				ZeusExeData[_resHexOffsetTable._unknownHeight + 1] = _resHeightBytes[1];

				// I don't know what this offset does. JackFuste's patches have it changed but I haven't seen the effect anywhere.
				ZeusExeData[_resHexOffsetTable._unknownWidth2 + 0] = _resWidthBytes[0];
				ZeusExeData[_resHexOffsetTable._unknownWidth2 + 1] = _resWidthBytes[1];

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
				for (byte _i = 0; _i < _resHexOffsetTable._extendSidebarRedStripeNewCodeJumpData.Length; _i++)
				{
					ZeusExeData[_resHexOffsetTable._extendSidebarRedStripeNewCodeJumpOffset + _i] = _resHexOffsetTable._extendSidebarRedStripeNewCodeJumpData[_i];
				}
				// Next, we need to edit part of the injected code to take the new city viewport height into account.
				ushort _cityViewAndMenubarHeight = (ushort)((_resHeightMult - 1) * 15 + 30);
				byte[] _initialRedBarTopLeftCornerYPos;
				if (ResHeight <= 768)
				{
					_initialRedBarTopLeftCornerYPos = new byte[] { 0x00, 0x00 };
				}
				else
				{
					_initialRedBarTopLeftCornerYPos = BitConverter.GetBytes((ushort)(_cityViewAndMenubarHeight - 768));
				}
				_resHexOffsetTable._extendSidebarRedStripeNewCodeData[1] = _initialRedBarTopLeftCornerYPos[0];
				_resHexOffsetTable._extendSidebarRedStripeNewCodeData[2] = _initialRedBarTopLeftCornerYPos[1];
				// Finally, insert our new code into an empty portion of the EXE
				for (byte _i = 0; _i < _resHexOffsetTable._extendSidebarRedStripeNewCodeData.Length; _i++)
				{
					ZeusExeData[_resHexOffsetTable._extendSidebarRedStripeNewCodeOffset + _i] = _resHexOffsetTable._extendSidebarRedStripeNewCodeData[_i];
				}

				// The second piece of new code fills both of the gaps noted above with a blue background similar to the already existing ones.
				// It works by drawing the blue background that forms the top bar multiple times. First, by drawing multiple copies of the blue bar
				// graphic vertically next to each other until the combination touches the left edge of the sidebar. After that, it draws multiple copies of
				// the graphic horizontally on top of each other in the gaps to the right and bottom of the sidebar until they have been filled with blue.
				// ----------------------------------------------------------------------------------------------------------------------------
				// First, modify a function call into a jump to our inserted code.
				for (byte _i = 0; _i < _resHexOffsetTable._paintBlueBackgroundInGapsNewCodeJumpData.Length; _i++)
				{
					ZeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCodeJumpOffset + _i] = _resHexOffsetTable._paintBlueBackgroundInGapsNewCodeJumpData[_i];
				}
				// Next, we need to modify our injected code to conform to the custom resolution values the user supplied.
				// Each blue bar graphic created by this code is 838 pixels long. The starting part of this code draws the multiple bar graphics next to each other
				// to complete the top bar's background. This bar overall needs to be the length of the city view's width, which we calculated earlier.
				//
				// First,the chosen resolution height needs to be inserted into the place that checks if all gaps have been filled.
				for (byte _i = 0; _i < _resHeightBytes.Length; _i++)
				{
					_resHexOffsetTable._paintBlueBackgroundInGapsNewCodeData[55 + _i] = _resHeightBytes[_i];
				}
				// Next, the resolution width also needs to be inserted in a few places.
				for (byte _i = 0; _i < _resWidthBytes.Length; _i++)
				{
					_resHexOffsetTable._paintBlueBackgroundInGapsNewCodeData[106 + _i] = _resWidthBytes[_i];
					_resHexOffsetTable._paintBlueBackgroundInGapsNewCodeData[161 + _i] = _resWidthBytes[_i];
					_resHexOffsetTable._paintBlueBackgroundInGapsNewCodeData[185 + _i] = _resWidthBytes[_i];
					_resHexOffsetTable._paintBlueBackgroundInGapsNewCodeData[211 + _i] = _resWidthBytes[_i];
					_resHexOffsetTable._paintBlueBackgroundInGapsNewCodeData[235 + _i] = _resWidthBytes[_i];
				}
				// After that, calculate the distance from the top of the screen to the bottom of the city viewport and insert that into the injected code.
				byte[] _bottomOfCityViewport = BitConverter.GetBytes(_cityViewAndMenubarHeight);
				for (byte _i = 0; _i < _bottomOfCityViewport.Length; _i++)
				{
					_resHexOffsetTable._paintBlueBackgroundInGapsNewCodeData[067 + _i] = _bottomOfCityViewport[_i];
					_resHexOffsetTable._paintBlueBackgroundInGapsNewCodeData[201 + _i] = _bottomOfCityViewport[_i];
				}
				// After that, calculate the distance from the top of the screen to the last row just before the bottom of the city viewport.
				byte[] _beforeBottomOfCityViewport = BitConverter.GetBytes((ushort)(_cityViewAndMenubarHeight - 16));
				for (byte _i = 0; _i < _beforeBottomOfCityViewport.Length; _i++)
				{
					_resHexOffsetTable._paintBlueBackgroundInGapsNewCodeData[75 + _i] = _beforeBottomOfCityViewport[_i];
				}
				// Next, the width of the city viewport plus sidebar needs to be inserted in a few places.
				byte[] _rightGapBarPosition = BitConverter.GetBytes((ushort)(_cityViewWidth + 186));
				for (byte _i = 0; _i < _rightGapBarPosition.Length; _i++)
				{
					_resHexOffsetTable._paintBlueBackgroundInGapsNewCodeData[113 + _i] = _rightGapBarPosition[_i];
					_resHexOffsetTable._paintBlueBackgroundInGapsNewCodeData[123 + _i] = _rightGapBarPosition[_i];
					_resHexOffsetTable._paintBlueBackgroundInGapsNewCodeData[137 + _i] = _rightGapBarPosition[_i];
				}
				// Next, the width of the city viewport plus red stripe needs to be inserted in the place where the drawing coordinates
				// are placed in the gap below the sidebar.
				byte[] _viewportAndStripeWidthBytes = BitConverter.GetBytes((ushort)(_cityViewWidth + 4));
				for (byte _i = 0; _i < _viewportAndStripeWidthBytes.Length; _i++)
				{
					_resHexOffsetTable._paintBlueBackgroundInGapsNewCodeData[222 + _i] = _viewportAndStripeWidthBytes[_i];
					_resHexOffsetTable._paintBlueBackgroundInGapsNewCodeData[246 + _i] = _viewportAndStripeWidthBytes[_i];
				}
				// After that, the city viewport width needs to be inserted into the menu bar code that checks when the final piece will be drawn.
				for (byte _i = 0; _i < _viewportWidthBytes.Length; _i++)
				{
					_resHexOffsetTable._paintBlueBackgroundInGapsNewCodeData[130 + _i] = _viewportWidthBytes[_i];
				}
				// For the final edit, we need to calculate and insert the position of the final menubar's left edge, this being 838px left of the sidebar, capped to min of 0.
				long _finalTopBarGraphicPosition = _cityViewWidth - 838;
				if (_finalTopBarGraphicPosition < 0)
				{
					_finalTopBarGraphicPosition = 0;
				}
				byte[] _finalTopBarPositionBytes = BitConverter.GetBytes((ushort)_finalTopBarGraphicPosition);
				for (ushort _i = 0; _i < _finalTopBarPositionBytes.Length; _i++)
				{
					_resHexOffsetTable._paintBlueBackgroundInGapsNewCodeData[144 + _i] = _finalTopBarPositionBytes[_i];
					_resHexOffsetTable._paintBlueBackgroundInGapsNewCodeData[151 + _i] = _finalTopBarPositionBytes[_i];
				}
				// Finally, we can insert our new code.
				for (ushort _i = 0; _i < _resHexOffsetTable._paintBlueBackgroundInGapsNewCodeData.Length; _i++)
				{
					ZeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCodeOffset + _i] = _resHexOffsetTable._paintBlueBackgroundInGapsNewCodeData[_i];
				}
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
	This will give you the sidebar left postion value.

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
