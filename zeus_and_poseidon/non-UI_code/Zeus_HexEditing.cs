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
				MessageBox.Show("Zeus.exe does not exist in the selected location. Please ensure that you have selected the correct place.");
				return;
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
					"Please close any other programs using that folder, make sure the folder and it's contents are not marked Read-only or " +
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

				// These two offsets set the resolution to the desired amount
				_zeusExeData[_resHexOffsetTable._resWidth] = _resWidthBytes[0];
				_zeusExeData[_resHexOffsetTable._resWidth + 1] = _resWidthBytes[1];
				_zeusExeData[_resHexOffsetTable._resHeight] = _resHeightBytes[0];
				_zeusExeData[_resHexOffsetTable._resHeight + 1] = _resHeightBytes[1];
				
				// Fix the game's opening screen
				_zeusExeData[_resHexOffsetTable._openingScreenWidth]		= _resWidthBytes[0];
				_zeusExeData[_resHexOffsetTable._openingScreenWidth + 1]	= _resWidthBytes[1];
				_zeusExeData[_resHexOffsetTable._openingScreenHeight]		= _resHeightBytes[0];
				_zeusExeData[_resHexOffsetTable._openingScreenHeight + 1]	= _resHeightBytes[1];
				/*
				// Fix game's menus
				_zeusExeData[_resHexOffsetTable._fixMenuTextWidth]			 = _resWidthBytes[0];	// fix menu text
				_zeusExeData[_resHexOffsetTable._fixMenuTextWidth + 1]		 = _resWidthBytes[1];
				_zeusExeData[_resHexOffsetTable._fixMenuBackgroundWidth]	 = _resWidthBytes[0];	// fix menu background
				_zeusExeData[_resHexOffsetTable._fixMenuBackgroundWidth + 1] = _resWidthBytes[1];
				_zeusExeData[_resHexOffsetTable._fixSidebarBottomWidth]		 = _resWidthBytes[0];	// fix bottom of sidebar
				_zeusExeData[_resHexOffsetTable._fixSidebarBottomWidth + 1]  = _resWidthBytes[1];

				// Set viewport to the correct size
				_zeusExeData[_resHexOffsetTable._viewportWidth]		= _resWidthBytes[0];	// set viewport width
				_zeusExeData[_resHexOffsetTable._viewportWidth + 1] = _resWidthBytes[1];

				byte _resHeightMult = (byte)Math.Ceiling((_resHeight - 30) * 0.06666666666666666666666667);		// Might need to adjust these numbers by subtraction
				byte _resWidthMult  = (byte)Math.Ceiling(_resWidth * 0.017);
				_zeusExeData[_resHexOffsetTable._viewportHeightMult] = _resHeightMult;		// set viewport height multiplier
				_zeusExeData[_resHexOffsetTable._viewportWidthMult]  = _resWidthMult;		// set viewport width multiplier

				// Fix sidebar
				_zeusExeData[_resHexOffsetTable._sidebarViewportWidth]			= _resWidthBytes[0];		// fix menu text's render limit
				_zeusExeData[_resHexOffsetTable._sidebarRenderLimitWidth + 1]	= _resWidthBytes[1];

				byte[] _viewportWidthBytes;
				if (_resWidth > 182)
				{
					_viewportWidthBytes = BitConverter.GetBytes(Convert.ToUInt16(_resWidth - 182));	// Original game seems to use (width - 6). Mario's patch uses (width - 182) instead, for some reason.
				}
				else
				{
					_viewportWidthBytes = new byte[2];
					_viewportWidthBytes[0] = 0;
					_viewportWidthBytes[1] = 0;
				}
				_zeusExeData[_resHexOffsetTable._sidebarViewportWidth]		= _viewportWidthBytes[0];	// fix sidebar's viewport width
				_zeusExeData[_resHexOffsetTable._sidebarViewportWidth + 1]  = _viewportWidthBytes[1];

				*/
			}
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
					_resHexOffsetTable = new ResHexOffsetTable(0x10BA29, 0x10BA2E, 0x1051FA, 0x105212, 0x18EF7C,
						0x19EBFB, 0x18E2F7, 0x11CC1E, 0x11CC29, 0x11CC2B, 0x18E2BE, 0x18E2CA);
					return true;

				default:        // Unrecognised EXE
					_resHexOffsetTable = new ResHexOffsetTable(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
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
			internal readonly int _openingScreenWidth;
			internal readonly int _openingScreenHeight;

			internal readonly int _fixMenuTextWidth;
			internal readonly int _fixMenuBackgroundWidth;
			internal readonly int _fixSidebarBottomWidth;

			internal readonly int _viewportWidth;
			internal readonly int _viewportHeightMult;
			internal readonly int _viewportWidthMult;

			internal readonly int _sidebarRenderLimitWidth;
			internal readonly int _sidebarViewportWidth;

			public ResHexOffsetTable(int ResWidth, int ResHeight, int OpeningScreenWidth, int OpeningScreenHeight, int FixMenuTextWidth,
				int FixMenuBackgroundWidth, int FixSidebarBottomWidth, int ViewportWidth, int ViewportHeightMult, int ViewportWidthMult,
				int SidebarRenderLimitWidth, int SidebarViewportWidth)
			{
				_resWidth = ResWidth;
				_resHeight = ResHeight;
				_openingScreenWidth = OpeningScreenWidth;
				_openingScreenHeight = OpeningScreenHeight;
				_fixMenuTextWidth = FixMenuTextWidth;
				_fixMenuBackgroundWidth = FixMenuBackgroundWidth;
				_fixSidebarBottomWidth = FixSidebarBottomWidth;
				_viewportWidth = ViewportWidth;
				_viewportHeightMult = ViewportHeightMult;
				_viewportWidthMult = ViewportWidthMult;
				_sidebarRenderLimitWidth = SidebarRenderLimitWidth;
				_sidebarViewportWidth = SidebarViewportWidth;
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
