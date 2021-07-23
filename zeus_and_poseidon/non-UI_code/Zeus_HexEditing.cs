using Soft160.Data.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace zeus_and_poseidon
{
	class Zeus_HexEditing
	{
		public static void ProcessZeusExe(string ZeusExeLocation, ushort ResWidth, ushort ResHeight, bool FixAnimations, bool FixWindowed, bool ResizeImages)
		{
			if (!File.Exists(ZeusExeLocation))
			{
				// User didn't select a folder using the selection button.
				if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "base_files/Zeus.exe"))
				{
					// Check if the user has placed the Zeus data files in the "base_files" folder.
					ZeusExeLocation = AppDomain.CurrentDomain.BaseDirectory + "base_files/Zeus.exe";
				}
				else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Zeus.exe"))
				{
					// As a last resort, check if the Zeus data files are in the same folder as this program.
					ZeusExeLocation = AppDomain.CurrentDomain.BaseDirectory + "Zeus.exe";
				}
				else
				{
					MessageBox.Show("Zeus.exe does not exist in either the selected location or either of the automatically scanned locations. " +
						"Please ensure that you have either selected the correct place, placed this program in the correct place or " +
						"placed the correct files in the \"base_files\" folder.");
					return;
				}
			}

			string _patchedFilesFolder = AppDomain.CurrentDomain.BaseDirectory + "patched_files";
			try
			{
				if (Directory.Exists(_patchedFilesFolder))
				{
					Directory.Delete(_patchedFilesFolder, true);
				}
				Directory.CreateDirectory(_patchedFilesFolder);
			}
			catch (PathTooLongException)
			{
				MessageBox.Show("A PathTooLong Exception occurred while trying to work on the \"patched_files\" folder next to this program's EXE. " +
					"Please exit and move the program somewhere with a shorter path length (the Downloads folder is a good choice).");
				return;
			}
			catch (IOException)
			{
				MessageBox.Show("An IO Exception occurred while trying to work on the \"patched_files\" folder next to this program's EXE. " +
					"Please close any other programs using that folder, make sure the folder and it's contents are not marked Read-only and/or " +
					"manually delete any files or folders named \"patched_files\".");
				return;
			}
			catch (UnauthorizedAccessException)
			{
				MessageBox.Show("This program must be run from a location that it is allowed to write files to. " +
					"Please exit and move the program somewhere that you have write permissions available (the Downloads folder is a good choice).");
				return;
			}

			if (_getAndCheckExeChecksum(ZeusExeLocation, out byte[] _zeusExeData, out ExeLangAndDistrib _exeLangAndDistrib))
			{
				_hexEditExeResVals(ResWidth, ResHeight, _exeLangAndDistrib, ref _zeusExeData);

				if (FixAnimations)
				{
					_hexEditExeAnims(_exeLangAndDistrib, ref _zeusExeData);
				}
				if (FixWindowed)
				{
					_hexEditWindowFix(_exeLangAndDistrib, ref _zeusExeData);
				}
				if (ResizeImages)
				{

				}

				File.WriteAllBytes(_patchedFilesFolder + "/Zeus.exe", _zeusExeData);
				MessageBox.Show("Your patched Zeus.exe has been successfully created in " + _patchedFilesFolder);
			}
		}

		private static void _hexEditExeResVals(ushort _resWidth, ushort _resHeight, ExeLangAndDistrib _exeLangAndDistrib, ref byte[] _zeusExeData)
		{
			if (_fillResHexOffsetTable(_exeLangAndDistrib, out ResHexOffsetTable _resHexOffsetTable))
			{
				byte[] _resWidthBytes = BitConverter.GetBytes(_resWidth);
				byte[] _resHeightBytes = BitConverter.GetBytes(_resHeight);

				// These two offsets set the game's resolution to the desired amount
				_zeusExeData[_resHexOffsetTable._resWidth] = _resWidthBytes[0];
				_zeusExeData[_resHexOffsetTable._resWidth + 1] = _resWidthBytes[1];
				_zeusExeData[_resHexOffsetTable._resHeight] = _resHeightBytes[0];
				_zeusExeData[_resHexOffsetTable._resHeight + 1] = _resHeightBytes[1];
				
				// These two offsets correct the game's main menu viewport to use the new resolution values.
				// Without this fix, the game will not accept main menu images where either dimension is larger
				// than either 1024x768 or custom resolutions with either dimension smaller than 1024x768.
				// In turn, any image smaller than those dimensions will be put in the top-left corner and
				// black bars will fill the remaining space on the bottom and right.
				// This is all despite the fact that buttons will be in the correct locations.
				_zeusExeData[_resHexOffsetTable._mainMenuViewportWidth]			= _resWidthBytes[0];
				_zeusExeData[_resHexOffsetTable._mainMenuViewportWidth + 1]		= _resWidthBytes[1];
				_zeusExeData[_resHexOffsetTable._mainMenuViewportHeight]		= _resHeightBytes[0];
				_zeusExeData[_resHexOffsetTable._mainMenuViewportHeight + 1]	= _resHeightBytes[1];
				
				// This offset corrects the position of the money, population and date text in the top menu bar.
				// Without this patch, that text will be drawn too far to the left.
				_zeusExeData[_resHexOffsetTable._fixMoneyPopDateTextPosWidth]			 = _resWidthBytes[0];
				_zeusExeData[_resHexOffsetTable._fixMoneyPopDateTextPosWidth + 1]		 = _resWidthBytes[1];

				// This offset corrects the position of the blue background for the top menu bar containing the above text.
				// Without this patch, that background will be drawn too far to the left.
				_zeusExeData[_resHexOffsetTable._fixTopMenuBarBackgroundPosWidth]		= _resWidthBytes[0];
				_zeusExeData[_resHexOffsetTable._fixTopMenuBarBackgroundPosWidth + 1]	= _resWidthBytes[1];

				// No idea what this does. Moves a black square at the bottom of the screen around.
				_zeusExeData[_resHexOffsetTable._fixSidebarBottomWidth]		 = _resWidthBytes[0];	// fix bottom of sidebar
				_zeusExeData[_resHexOffsetTable._fixSidebarBottomWidth + 1]  = _resWidthBytes[1];
				
				// Set main game's viewport to the correct width.
				// This means the width that will be taken by both the city view's "camera" and the right sidebar containing the city's info and 
				// buttons to build and demolish buildings and other functions.
				// Without this patch, the view of your city will be rendered in a small square placed at the top-left corner of the main viewing area.
				_zeusExeData[_resHexOffsetTable._viewportWidth]		= _resWidthBytes[0];
				_zeusExeData[_resHexOffsetTable._viewportWidth + 1] = _resWidthBytes[1];

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
				byte _resHeightMult = (byte)Math.Floor((_resHeight - 30) / 15.0 + 1);   // .0s are required. Otherwise,
				byte _resWidthMult  = (byte)Math.Floor((_resWidth - 182 + 2) / 60.0);	// compiler error CS0121 occurrs.
				_zeusExeData[_resHexOffsetTable._viewportHeightMult] = _resHeightMult;
				_zeusExeData[_resHexOffsetTable._viewportWidthMult]  = _resWidthMult;
				
				// This offset partially corrects the position of the game's sidebar to align with the new viewport render limit
				// Without this change, the sidebar is drawn against the left edge of the screen and clips with the city view
				_zeusExeData[_resHexOffsetTable._sidebarRenderLimitWidth]		= _resWidthBytes[0];
				_zeusExeData[_resHexOffsetTable._sidebarRenderLimitWidth + 1]	= _resWidthBytes[1];
				
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
				byte[] _viewportWidthBytes = BitConverter.GetBytes(Convert.ToUInt16(_resWidthMult * 60 - 2));
				_zeusExeData[_resHexOffsetTable._sidebarLeftEdgeStartWidth]		= _viewportWidthBytes[0];
				_zeusExeData[_resHexOffsetTable._sidebarLeftEdgeStartWidth + 1] = _viewportWidthBytes[1];

				// I don't know what this offset does. JackFuste's patches have it changed but I haven't seen the effect anywhere.
				_zeusExeData[_resHexOffsetTable._unknownWidth]		= _resWidthBytes[0];
				_zeusExeData[_resHexOffsetTable._unknownWidth + 1]  = _resWidthBytes[1];

				// I don't know what this offset does. JackFuste's patches have it changed but I haven't seen the effect anywhere.
				_zeusExeData[_resHexOffsetTable._unknownHeight]		= _resHeightBytes[0];
				_zeusExeData[_resHexOffsetTable._unknownHeight + 1] = _resHeightBytes[1];

				// I don't know what this offset does. JackFuste's patches have it changed but I haven't seen the effect anywhere.
				_zeusExeData[_resHexOffsetTable._unknownWidth2]		= _resWidthBytes[0];
				_zeusExeData[_resHexOffsetTable._unknownWidth2 + 1] = _resWidthBytes[1];

				// At this point, the only outstanding issue to be fixed is that there are two gaps in the UI that holds
				// the last displayed background image. The first is a small gap between the right edge of the top menubar
				// and left edge of the right sidebar. The second is a large gap on the right and bottom of the sidebar.
				//
				// JackFuste fixed these in his patch by inserting some new code into the EXE that paints a blue background 
				// over the areas where these gaps existed. The last portion of this function will be used to replicate this insertion.
				// There are two portions of new code that were inserted.
				
				// The first piece of new code extends the red stripe on the left edge of the sidebar all the way down to the bottom of the game window.
				// -----------------------------------------------------------------------------------------
				// First, we need to shift some of the original code at this offset 3 bytes forward to make space for a change.
				// Fortunately, there are a string of NOPs starting at offset 0x117BE6 that can be overwritten to accomodate this shift.
				for (byte i = 115; i > 3; i--)
				{
					_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCodeJump + i] = _zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCodeJump + i - 3];
				}
				// Next, convert a "push 0" instruction into a "push 0x1B0" instruction.
				_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCodeJump]	 = 0x68;
				_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCodeJump + 1] = 0xB0;
				_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCodeJump + 2] = 0x01;
				_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCodeJump + 3] = 0x00;
				_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCodeJump + 4] = 0x00;
				// Next, we must replace a function call with a jump to our inserted code.
				_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCodeJump + 12] = 0xE9;
				_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCodeJump + 13] = 0x23;
				_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCodeJump + 14] = 0x04;
				_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCodeJump + 15] = 0x0C;
				// In the shifted code, there were a number of jump commands to other offsets. We need to patch these commands to point to the new correct locations.
				_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCodeJump +  85] -= 3;
				_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCodeJump +  90] -= 3;
				_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCodeJump + 108] -= 3;
				// Finally, insert our new code into an empty portion of the EXE
				_patchInFirstNewCode(_resHexOffsetTable, ref _zeusExeData);
				
				// The second piece of new code fills both of the gaps noted above with a blue background similar to the already existing ones.
				// ---------------------------------------
				// First, modify a function call into a jump to our inserted code.
				_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCodeJump]	   = 0xE9;
				_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCodeJump + 1] = 0x1E;
				_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCodeJump + 2] = 0x87;
				_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCodeJump + 3] = 0x03;
				// After that, insert our new code.
				_paintBlueBackgroundInGapsNewCode(_resHexOffsetTable, ref _zeusExeData);
			}
		}

		private static void _patchInFirstNewCode(ResHexOffsetTable _resHexOffsetTable, ref byte[] _zeusExeData)
		{
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode]	  = 0xA3;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 1]  = 0xF4;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 2]  = 0xDF;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 3]  = 0x60;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 4]  = 0x01;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 5]  = 0xE8;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 6]  = 0x9D;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 7]  = 0x4F;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 8]  = 0xFD;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 9]  = 0xFF;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 10] = 0xA1;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 11] = 0xF4;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 12] = 0xDF;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 13] = 0x60;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 14] = 0x01;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 15] = 0x8B;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 16] = 0x0D;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 17] = 0x34;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 18] = 0xFF;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 19] = 0x0D;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 20] = 0x01;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 21] = 0x6A;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 22] = 0x00;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 23] = 0x6A;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 24] = 0x00;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 25] = 0x6A;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 26] = 0x00;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 27] = 0x6A;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 28] = 0x00;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 29] = 0x51;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 30] = 0x50;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 31] = 0xB9;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 32] = 0xE8;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 33] = 0xEB;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 34] = 0x2A;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 35] = 0x01;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 36] = 0xE8;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 37] = 0x7E;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 38] = 0x4F;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 39] = 0xFD;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 40] = 0xFF;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 41] = 0xE9;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 42] = 0xAF;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 43] = 0xFB;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 44] = 0xF3;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 45] = 0xFF;
			_zeusExeData[_resHexOffsetTable._extendSidebarRedStripeCode + 46] = 0x90;
		}

		private static void _paintBlueBackgroundInGapsNewCode(ResHexOffsetTable _resHexOffsetTable, ref byte[] _zeusExeData)
		{
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode]		 = 0xCC;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 1]	 = 0xCC;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 2]	 = 0xCC;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 3]	 = 0xE8;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 4]	 = 0xF7;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 5]	 = 0x5B;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 6]	 = 0xFD;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 7]	 = 0xFF;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 8]	 = 0xA1;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 9]	 = 0x64;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 10]  = 0x94;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 11]  = 0x2C;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 12]  = 0x01;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 13]  = 0xA1;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 14]  = 0x30;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 15]  = 0xC8;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 16]  = 0x2B;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 17]  = 0x01;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 18]  = 0x05;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 19]  = 0x58;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 20]  = 0x03;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 21]  = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 22]  = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 23]  = 0x8B;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 24]  = 0x40;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 25]  = 0x04;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 26]  = 0x6A;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 27]  = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 28]  = 0x6A;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 29]  = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 30]  = 0x6A;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 31]  = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 32]  = 0x05;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 33]  = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 34]  = 0x40;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 35]  = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 36]  = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 37]  = 0x6A;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 38]  = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 39]  = 0x68;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 40]  = 0x0F;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 41]  = 0x02;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 42]  = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 43]  = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 44]  = 0x50;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 45]  = 0xB9;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 46]  = 0xE8;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 47]  = 0xEB;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 48]  = 0x2A;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 49]  = 0x01;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 50]  = 0xE8;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 51]  = 0xC8;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 52]  = 0x5B;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 53]  = 0xFD;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 54]  = 0xFF;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 55]  = 0xA1;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 56]  = 0x64;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 57]  = 0x94;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 58]  = 0x2C;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 59]  = 0x01;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 60]  = 0xA1;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 61]  = 0x30;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 62]  = 0xC8;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 63]  = 0x2B;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 64]  = 0x01;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 65]  = 0x05;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 66]  = 0x58;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 67]  = 0x03;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 68]  = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 69]  = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 70]  = 0x8B;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 71]  = 0x40;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 72]  = 0x04;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 73]  = 0x6A;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 74]  = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 75]  = 0x6A;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 76]  = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 77]  = 0x6A;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 78]  = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 79]  = 0x05;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 80]  = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 81]  = 0x40;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 82]  = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 83]  = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 84]  = 0x6A;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 85]  = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 86]  = 0x68;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 87]  = 0x1C;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 88]  = 0x02;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 89]  = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 90]  = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 91]  = 0x50;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 92]  = 0xB9;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 93]  = 0xE8;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 94]  = 0xEB;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 95]  = 0x2A;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 96]  = 0x01;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 97]  = 0xE8;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 98]  = 0x99;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 99]  = 0x5B;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 100] = 0xFD;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 101] = 0xFF;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 102] = 0xC7;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 103] = 0x05;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 104] = 0xF0;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 105] = 0xDF;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 106] = 0x60;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 107] = 0x01;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 108] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 109] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 110] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 111] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 112] = 0xA1;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 113] = 0x64;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 114] = 0x94;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 115] = 0x2C;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 116] = 0x01;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 117] = 0xA1;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 118] = 0x30;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 119] = 0xC8;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 120] = 0x2B;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 121] = 0x01;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 122] = 0x05;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 123] = 0x58;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 124] = 0x03;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 125] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 126] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 127] = 0x8B;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 128] = 0x40;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 129] = 0x04;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 130] = 0x6A;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 131] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 132] = 0x6A;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 133] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 134] = 0x6A;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 135] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 136] = 0x05;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 137] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 138] = 0x40;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 139] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 140] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 141] = 0xFF;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 142] = 0x35;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 143] = 0xF0;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 144] = 0xDF;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 145] = 0x60;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 146] = 0x01;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 147] = 0x83;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 148] = 0x05;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 149] = 0xF0;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 150] = 0xDF;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 151] = 0x60;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 152] = 0x01;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 153] = 0x10;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 154] = 0x68;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 155] = 0x1C;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 156] = 0x06;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 157] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 158] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 159] = 0x50;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 160] = 0xB9;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 161] = 0xE8;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 162] = 0xEB;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 163] = 0x2A;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 164] = 0x01;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 165] = 0xE8;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 166] = 0x55;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 167] = 0x5B;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 168] = 0xFD;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 169] = 0xFF;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 170] = 0x81;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 171] = 0x3D;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 172] = 0xF0;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 173] = 0xDF;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 174] = 0x60;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 175] = 0x01;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 176] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 177] = 0x03;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 178] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 179] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 180] = 0x75;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 181] = 0xBA;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 182] = 0xC7;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 183] = 0x05;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 184] = 0xF0;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 185] = 0xDF;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 186] = 0x60;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 187] = 0x01;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 188] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 189] = 0x03;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 190] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 191] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 192] = 0xA1;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 193] = 0x64;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 194] = 0x94;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 195] = 0x2C;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 196] = 0x01;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 197] = 0xA1;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 198] = 0x30;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 199] = 0xC8;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 200] = 0x2B;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 201] = 0x01;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 202] = 0x05;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 203] = 0x58;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 204] = 0x03;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 205] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 206] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 207] = 0x8B;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 208] = 0x40;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 209] = 0x04;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 210] = 0x6A;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 211] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 212] = 0x6A;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 213] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 214] = 0x6A;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 215] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 216] = 0x05;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 217] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 218] = 0x40;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 219] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 220] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 221] = 0xFF;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 222] = 0x35;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 223] = 0xF0;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 224] = 0xDF;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 225] = 0x60;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 226] = 0x01;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 227] = 0x83;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 228] = 0x05;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 229] = 0xF0;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 230] = 0xDF;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 231] = 0x60;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 232] = 0x01;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 233] = 0x10;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 234] = 0x68;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 235] = 0x68;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 236] = 0x05;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 237] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 238] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 239] = 0x50;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 240] = 0xB9;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 241] = 0xE8;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 242] = 0xEB;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 243] = 0x2A;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 244] = 0x01;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 245] = 0xE8;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 246] = 0x05;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 247] = 0x5B;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 248] = 0xFD;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 249] = 0xFF;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 250] = 0x81;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 251] = 0x3D;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 252] = 0xF0;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 253] = 0xDF;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 254] = 0x60;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 255] = 0x01;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 256] = 0x90;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 257] = 0x03;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 258] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 259] = 0x00;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 260] = 0x75;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 261] = 0xBA;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 262] = 0xE9;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 263] = 0xDA;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 264] = 0x77;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 265] = 0xFC;
			_zeusExeData[_resHexOffsetTable._paintBlueBackgroundInGapsNewCode + 266] = 0xFF;
		}

		private static bool _getAndCheckExeChecksum(string _zeusExeLocation, out byte[] _zeusExeData, out ExeLangAndDistrib exeLangAndDistrib)
		{
			_zeusExeData = File.ReadAllBytes(_zeusExeLocation);

			uint _fileHash = CRC.Crc32(_zeusExeData, 0, _zeusExeData.Length);

			switch (_fileHash)
			{
				case 0x90B9CF84:        // English GOG version
					exeLangAndDistrib = ExeLangAndDistrib.GOG_English;
					return true;

				default:                // Unrecognised EXE
					MessageBox.Show("Zeus.exe was not recognised. Only the English GOG version of this game is supported.");
					exeLangAndDistrib = ExeLangAndDistrib.Not_Recognised;
					return false;
			}
		}

		private static void _hexEditExeAnims(ExeLangAndDistrib _exeLangAndDistrib, ref byte[] _zeusExeData)
		{
			if (_fillAnimHexOffsetTable(_exeLangAndDistrib, out int[] _animHexOffsetTable))
			{
				for (byte i = 0; i < _animHexOffsetTable.Length; i++)
				{
					_zeusExeData[_animHexOffsetTable[i]] = 0x00;
				}
			}
		}

		private static void _hexEditWindowFix(ExeLangAndDistrib _exeLangAndDistrib, ref byte[] _zeusExeData)
		{
			// At this address, the original code had a conditional jump (jl) that activates if the value stored in the EAX register is less than the value stored in the ECX.
			// This patch changes this byte into an unconditional jump.
			// I have no idea what the values represent, what code runs if the condition is false (EAX is greater than ECX) or why the widescreen mods cause
			// EAX to be greater than ECX. All I know is that it makes Windowed mode work.
			if (_identifyWinFixOffset(_exeLangAndDistrib, out int _winFixOffset))
			{
				_zeusExeData[_winFixOffset] = 0xEB;
			}
		}

		private static bool _fillResHexOffsetTable(ExeLangAndDistrib _exeLangAndDistrib, out ResHexOffsetTable _resHexOffsetTable)
		{
			switch ((byte)_exeLangAndDistrib)
			{
				case 1:         // English GOG version
					int _resWidth = 0x10BA29;
					int _resHeight = 0x10BA2E;
					int _mainMenuViewportWidth = 0x1051FA;
					int _mainMenuViewportHeight = 0x105212;

					int _fixMoneyPopDateTextPosWidth = 0x18EF7C;
					int _fixTopMenuBarBackgroundPosWidth = 0x19EBFB;
					int _fixSidebarBottomWidth = 0x18E2F7;

					int _viewportWidth = 0x11CC1E;
					int _viewportHeightMult = 0x11CC29;
					int _viewportWidthMult = 0x11CC2B;

					int _sidebarRenderLimitWidth = 0x18E2BE;
					int _sidebarLeftEdgeStartWidth = 0x18E2CA;

					int _unknownWidth  = 0x10BD03;
					int _unknownHeight = 0x10BD0D;
					int _unknownWidth2 = 0x1C459E;

					int _codeJump1 = 0x117B75;
					int _paintBlueBackgroundInGapsNewCodeJump = 0x19EC31;

					int _codeInsertion1 = 0x1D7FA9;
					int _paintBlueBackgroundInGapsNewCode = 0x1D7351;


					_resHexOffsetTable = new ResHexOffsetTable(_resWidth, _resHeight, _mainMenuViewportWidth, _mainMenuViewportHeight, 
						_fixMoneyPopDateTextPosWidth, _fixTopMenuBarBackgroundPosWidth, _fixSidebarBottomWidth, _viewportWidth, 
						_viewportHeightMult, _viewportWidthMult, _sidebarRenderLimitWidth, _sidebarLeftEdgeStartWidth, 
						_unknownWidth, _unknownHeight, _unknownWidth2, _codeJump1, _paintBlueBackgroundInGapsNewCodeJump, _codeInsertion1, _paintBlueBackgroundInGapsNewCode);
					return true;

				default:        // Unrecognised EXE
					_resHexOffsetTable = new ResHexOffsetTable(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
					return false;
			}
		}

		private static bool _fillAnimHexOffsetTable(ExeLangAndDistrib _exeLangAndDistrib, out int[] _animHexOffsetTable)
		{
			switch ((byte)_exeLangAndDistrib)
			{
				case 1:         // English GOG version
					_animHexOffsetTable = new int[] { 0x30407, 0xB395D, 0xB3992, 0xB5642, 0xB5AED, 0xB5DE5, 0xB65FF, 0xB69B7, 0xB91D6, 0xB9AB2, 0xB9AFB, 0xB9B7C,
						0xB9DB1, 0xBA007, 0xBAC20, 0xBAC31, 0xBAC42, 0xBAC53, 0xBB1F4, 0xBB381, 0xBB3E5, 0xBB40B, 0xBB431, 0xBB457, 0xBB47D, 0xBB4A3, 0xBB4C9,
						0xBB4EC, 0xBB50F, 0xBB532, 0xBB593, 0xBB5AD, 0xBB5C7, 0xBB5E4, 0xBB656, 0xBD331, 0xBD349, 0xBD3B2, 0xBDC62, 0xBDC7F, 0xBDC9C, 0xBDCB9,
						0xBDD2F, 0xBDDD7, 0xBDE5A, 0xBDE9F, 0xBDEE4, 0xBDF29, 0xBDF6E, 0xBDFB3, 0xBDFF8, 0xBE03D, 0xBE082, 0xBE0C7, 0xBFC43, 0xBFDF8, 0xBFF47,
						0xC26D1, 0xC2740, 0xC28E3, 0xC2904, 0xC2BD8, 0xC3A78, 0xC8415, 0xC84FC, 0xC9DEC, 0xC9E80, 0xCB1D7, 0xCB1F0, 0xCB23F };
					return true;

				default:        // Unrecognised EXE
					_animHexOffsetTable = new int[1];
					return false;
			}
		}

		private static bool _identifyWinFixOffset(ExeLangAndDistrib _exeLangAndDistrib, out int _winFixOffset)
		{
			switch ((byte)_exeLangAndDistrib)
			{
				case 1:         // English GOG version
					_winFixOffset = 0x212606;
					return true;

				default:        // Unrecognised EXE
					_winFixOffset = 0;
					return false;
			}
		}

		private enum ExeLangAndDistrib
		{
			Not_Recognised = 0,
			GOG_English = 1
		}

		private readonly struct ResHexOffsetTable
		{
			internal readonly int _resWidth;
			internal readonly int _resHeight;
			internal readonly int _mainMenuViewportWidth;
			internal readonly int _mainMenuViewportHeight;

			internal readonly int _fixMoneyPopDateTextPosWidth;
			internal readonly int _fixTopMenuBarBackgroundPosWidth;
			internal readonly int _fixSidebarBottomWidth;

			internal readonly int _viewportWidth;
			internal readonly int _viewportHeightMult;
			internal readonly int _viewportWidthMult;

			internal readonly int _sidebarRenderLimitWidth;
			internal readonly int _sidebarLeftEdgeStartWidth;

			internal readonly int _unknownWidth;
			internal readonly int _unknownHeight;
			internal readonly int _unknownWidth2;

			internal readonly int _extendSidebarRedStripeCodeJump;
			internal readonly int _paintBlueBackgroundInGapsNewCodeJump;

			internal readonly int _extendSidebarRedStripeCode;
			internal readonly int _paintBlueBackgroundInGapsNewCode;


			public ResHexOffsetTable(int ResWidth, int ResHeight, int MainMenuViewportWidth, int MainMenuViewportHeight, int FixMoneyPopDateTextPosWidth,
				int FixTopMenuBarBackgroundPosWidth, int FixSidebarBottomWidth, int ViewportWidth, int ViewportHeightMult, int ViewportWidthMult, 
				int SidebarRenderLimitWidth, int SidebarLeftEdgeStartWidth, int UnknownWidth, int UnknownHeight, int UnknownWidth2,
				int CodeJump1, int CodeJump2, int CodeInsertion1, int PaintBlueBackgroundInGapsNewCode)
			{
				_resWidth = ResWidth;
				_resHeight = ResHeight;
				_mainMenuViewportWidth = MainMenuViewportWidth;
				_mainMenuViewportHeight = MainMenuViewportHeight;
				_fixMoneyPopDateTextPosWidth = FixMoneyPopDateTextPosWidth;
				_fixTopMenuBarBackgroundPosWidth = FixTopMenuBarBackgroundPosWidth;
				_fixSidebarBottomWidth = FixSidebarBottomWidth;
				_viewportWidth = ViewportWidth;
				_viewportHeightMult = ViewportHeightMult;
				_viewportWidthMult = ViewportWidthMult;
				_sidebarRenderLimitWidth = SidebarRenderLimitWidth;
				_sidebarLeftEdgeStartWidth = SidebarLeftEdgeStartWidth;
				_unknownWidth = UnknownWidth;
				_unknownHeight = UnknownHeight;
				_unknownWidth2 = UnknownWidth2;
				_extendSidebarRedStripeCodeJump = CodeJump1;
				_paintBlueBackgroundInGapsNewCodeJump = CodeJump2;
				_extendSidebarRedStripeCode = CodeInsertion1;
				_paintBlueBackgroundInGapsNewCode = PaintBlueBackgroundInGapsNewCode;
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
