// This code is part of the Impressions Resolution Customiser project
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using Soft160.Data.Cryptography;
using System;
using System.IO;
using System.Windows;

namespace Emperor
{
	/// <summary>
	/// Struct that specifies various details about the Emperor.exe given to the program.
	/// </summary>
	internal struct ExeAttributes
	{
		internal ExeLangAndDistrib SelectedExeLangAndDistrib;
		internal bool IsDiscVersion;
	}

	/// <summary>
	/// Used to define the various versions of Emperor.exe that this program recognises and knows what offsets to patch. 
	/// </summary>
	internal enum ExeLangAndDistrib
	{
		Not_Recognised = 0,
		GOG_English = 1,
		CD_English = 3
	}

	/// <summary>
	/// Struct that specifies various details about the Emperor.exe given to the program.
	/// </summary>
	class Emperor_ExeDefinitions
	{
		/// <summary>
		/// Copies the contents of Emperor.exe into a byte array for editing then calculates a CRC32 hash for the contents of that array. 
		/// After that, compares that CRC to a list of known CRCs to determine which distribution of this game is being patched.
		/// </summary>
		/// <param name="ZeusExeLocation">String that defines the location of Emperor.exe</param>
		/// <param name="ZeusExeData">Byte array that contains the binary data contained within the supplied Emperor.exe</param>
		/// <param name="ExeAttributes">Struct that specifies various details about the detected Emperor.exe</param>
		/// <returns>
		/// True if the CRC for the selected Emperor.exe matches one that this program knows about and knows the offsets that need to be patched.
		/// False if the EXE is not recognised.
		/// </returns>
		internal static bool GetAndCheckExeChecksum(string ZeusExeLocation, out byte[] ZeusExeData, out ExeAttributes ExeAttributes)
		{
			ZeusExeData = File.ReadAllBytes(ZeusExeLocation);

			uint _fileHash = CRC.Crc32(ZeusExeData, 0, ZeusExeData.Length);

			switch (_fileHash)
			{
				case 0xFD9CF46F:        // English GOG version
					ExeAttributes = new ExeAttributes
					{
						SelectedExeLangAndDistrib = ExeLangAndDistrib.GOG_English,
						IsDiscVersion = false,
					};
					return true;

				case 0xA8A1AE71:        // English CD version
					ExeAttributes = new ExeAttributes
					{
						SelectedExeLangAndDistrib = ExeLangAndDistrib.CD_English,
						IsDiscVersion = true,
					};
					return true;

				default:                // Unrecognised EXE
					string[] _messageLines = new string[]
					{
						"Emperor.exe was not recognised. Only the following unmodified distributions and languages are currently supported:",
						"- English GOG version",
						"- English CD version"
					};
					MessageBox.Show(string.Join(Environment.NewLine, _messageLines));
					ExeAttributes = new ExeAttributes
					{
						SelectedExeLangAndDistrib = ExeLangAndDistrib.Not_Recognised,
						IsDiscVersion = false,
					};
					return false;
			}
		}

		/// <summary>
		/// Supplies a ResHexOffsetTable struct specifying the offsets that need to be patched, based on which version of Emperor.exe was supplied.
		/// </summary>
		/// <param name="ExeAttributes">Struct that specifies various details about the detected Emperor.exe</param>
		/// <param name="ResHexOffsetTable">Struct containing the offset for the supplied Emperor.exe that needs patching.</param>
		/// <returns>
		/// True if "_exeLangAndDistrib" matches one that this program knows about and knows the offsets that need to be patched.
		/// False if the EXE is not recognised.
		/// </returns>
		internal static bool FillResHexOffsetTable(ExeAttributes ExeAttributes, out ResHexOffsetTable ResHexOffsetTable)
		{
			switch ((byte)ExeAttributes.SelectedExeLangAndDistrib)
			{
				case 1:         // English GOG version
					ResHexOffsetTable = new ResHexOffsetTable
					{
						_resWidth = 0x12AA6D,
						_resHeight = 0x12AA72,
						_mainMenuViewportWidth = 0x12532A,
						_mainMenuViewportHeight = 0x125342,

						_fixMoneyPopDateTextPosWidth = 0x1B5C6A,
						_fixTopMenuBarBackgroundPosWidth = 0x1BDF70,

						_viewportWidth = 0x13BE8D,
						_viewportHeightMult = 0x13BE98,
						_viewportWidthMult = 0x13BE9A,

						_sidebarRenderLimitWidth = 0x1B4E22,
						_fixSidebarCityMap_RotateButton = 0x13A5DA,
						_fixSidebarCityMap_RotateIcon = 0x13A6CA,
						_fixSidebarCityMap_GoalsOverviewWorldMapMessagesIcons = 0x13AA0A,
						_fixSidebarCityMap_GoalsOverviewWorldMapMessagesButtons = 0x13AA3A,
						_sidebarLeftEdgeStartWidth = 0x1B4E2E,

						_fixBottomBarLength = 0x1BDFC9,
						_unknownBottomBarTweak1 = 0x1BDFA8,
						_unknownBottomBarTweak2 = 0x1BDFDC,

						_unknownWidth = 0x12AD53,
						_unknownHeight = 0x12AD5D,

						_extendOriginalUiNewCodeJumpOffset = 0x13A841,
						_extendOriginalUiNewCodeJumpData = new byte[] { 0xE9, 0x9A, 0xD8, 0x26, 0x00 },

						_extendOriginalUiNewCodeOffset = 0x3A80E0,
						_extendOriginalUiNewCodeData = new byte[]
						{
							0x6A,0x00,0x6A,0x00,0x6A,0x00,0x68,0x4E, 0x02,0x00,0x00,0x68,0x4E,0x05,0x00,0x00, 0x68,0x74,0x02,0x00,0x00,0xB9,0x30,0x21,
							0xC4,0x01,0xE8,0xB2,0xF4,0xC5,0xFF,0x50, 0xE8,0x9C,0xAC,0xC6,0xFF,0x83,0xC4,0x18, 0x6A,0x00,0x6A,0x00,0x6A,0x00,0x6A,0x00,
							0x68,0x00,0x03,0x00,0x00,0x68,0x7E,0x02, 0x00,0x00,0xB9,0x30,0x21,0xC4,0x01,0xE8, 0x8D,0xF4,0xC5,0xFF,0x50,0xE8,0x77,0xAC,
							0xC6,0xFF,0x83,0xC4,0x18,0x6A,0x00,0x6A, 0x00,0x6A,0x00,0x68,0xC0,0x01,0x00,0x00, 0x68,0x30,0x06,0x00,0x00,0x68,0x81,0x02,
							0x00,0x00,0xB9,0x30,0x21,0xC4,0x01,0xE8, 0x65,0xF4,0xC5,0xFF,0x50,0xE8,0x4F,0xAC, 0xC6,0xFF,0x83,0xC4,0x18,0x6A,0x00,0x6A,
							0x00,0x6A,0x00,0x6A,0x00,0x68,0x30,0x06, 0x00,0x00,0x68,0x81,0x02,0x00,0x00,0xB9, 0x30,0x21,0xC4,0x01,0xE8,0x40,0xF4,0xC5,
							0xFF,0x50,0xE8,0x2A,0xAC,0xC6,0xFF,0x83, 0xC4,0x18,0x6A,0x00,0x6A,0x00,0x6A,0x00, 0x68,0x10,0x04,0x00,0x00,0x68,0x80,0x02,
							0x00,0x00,0x68,0xD6,0x02,0x00,0x00,0xB9, 0x30,0x21,0xC4,0x01,0xE8,0x18,0xF4,0xC5, 0xFF,0x50,0xE8,0x02,0xAC,0xC6,0xFF,0x83,
							0xC4,0x18,0xE8,0x0A,0xD5,0xE0,0xFF,0xE9, 0xDB,0x26,0xD9,0xFF
						}
					};
					return true;

				case 3:         // English CD version
					ResHexOffsetTable = new ResHexOffsetTable
					{
						_resWidth = 0x12B66D,
						_resHeight = 0x12B672,
						_mainMenuViewportWidth = 0x0,
						_mainMenuViewportHeight = 0x0,

						_fixMoneyPopDateTextPosWidth = 0x0,
						_fixTopMenuBarBackgroundPosWidth = 0x0,

						_viewportWidth = 0x0,
						_viewportHeightMult = 0x0,
						_viewportWidthMult = 0x0,

						_sidebarRenderLimitWidth = 0x0,
						_fixSidebarCityMap_RotateButton = 0x0,
						_fixSidebarCityMap_RotateIcon = 0x0,
						_fixSidebarCityMap_GoalsOverviewWorldMapMessagesIcons = 0x0,
						_fixSidebarCityMap_GoalsOverviewWorldMapMessagesButtons = 0x0,
						_sidebarLeftEdgeStartWidth = 0x0,

						_fixBottomBarLength = 0x1BEBC9,
						_unknownBottomBarTweak1 = 0x1BEBA8,
						_unknownBottomBarTweak2 = 0x1BEBDC,

						_unknownWidth = 0x0,
						_unknownHeight = 0x0,

						_extendOriginalUiNewCodeJumpOffset = 0x13B441,  // Next instruction: 0x13B446
						_extendOriginalUiNewCodeJumpData = new byte[] { 0xE9, 0x59, 0xD8, 0x26, 0x00 },

						_extendOriginalUiNewCodeOffset = 0x3A8C9F,
						_extendOriginalUiNewCodeData = new byte[]
						{
							0x6A,0x00,0x6A,0x00,0x6A,0x00,0x68,0x4E, 0x02,0x00,0x00,0x68,0x4E,0x05,0x00,0x00, 0x68,0x74,0x02,0x00,0x00,0xB9,0x30,0x21,
							0xC4,0x01,0xE8,0xB2,0xF4,0xC5,0xFF,0x50, 0xE8,0x9C,0xAC,0xC6,0xFF,0x83,0xC4,0x18, 0x6A,0x00,0x6A,0x00,0x6A,0x00,0x6A,0x00,
							0x68,0x00,0x03,0x00,0x00,0x68,0x7E,0x02, 0x00,0x00,0xB9,0x30,0x21,0xC4,0x01,0xE8, 0x8D,0xF4,0xC5,0xFF,0x50,0xE8,0x77,0xAC,
							0xC6,0xFF,0x83,0xC4,0x18,0x6A,0x00,0x6A, 0x00,0x6A,0x00,0x68,0xC0,0x01,0x00,0x00, 0x68,0x30,0x06,0x00,0x00,0x68,0x81,0x02,
							0x00,0x00,0xB9,0x30,0x21,0xC4,0x01,0xE8, 0x65,0xF4,0xC5,0xFF,0x50,0xE8,0x4F,0xAC, 0xC6,0xFF,0x83,0xC4,0x18,0x6A,0x00,0x6A,
							0x00,0x6A,0x00,0x6A,0x00,0x68,0x30,0x06, 0x00,0x00,0x68,0x81,0x02,0x00,0x00,0xB9, 0x30,0x21,0xC4,0x01,0xE8,0x40,0xF4,0xC5,
							0xFF,0x50,0xE8,0x2A,0xAC,0xC6,0xFF,0x83, 0xC4,0x18,0x6A,0x00,0x6A,0x00,0x6A,0x00, 0x68,0x10,0x04,0x00,0x00,0x68,0x80,0x02,
							0x00,0x00,0x68,0xD6,0x02,0x00,0x00,0xB9, 0x30,0x21,0xC4,0x01,0xE8,0x18,0xF4,0xC5, 0xFF,0x50,0xE8,0x02,0xAC,0xC6,0xFF,0x83,
							0xC4,0x18,0xE8,0x0A,0xD5,0xE0,0xFF,0xE9, 0xDB,0x26,0xD9,0xFF
						}
					};
					return true;

				default:        // Unrecognised EXE
					ResHexOffsetTable = new ResHexOffsetTable();
					return false;
			}
		}

		/// <summary>
		/// Supplies an int specifying the offset that needs to be patched, based on which version of Emperor.exe was supplied.
		/// </summary>
		/// <param name="ExeAttributes">Struct that specifies various details about the detected Emperor.exe</param>
		/// <param name="WinFixOffset">Int containing the offset for the supplied Emperor.exe that needs patching.</param>
		/// <returns>
		/// True if "_exeLangAndDistrib" matches one that this program knows about and knows the offsets that need to be patched.
		/// False if the EXE is not recognised.
		/// </returns>
		internal static bool IdentifyWinFixOffset(ExeAttributes ExeAttributes, out int WinFixOffset)
		{
			switch ((byte)ExeAttributes.SelectedExeLangAndDistrib)
			{
				case 1:         // English GOG version
					WinFixOffset = 0x0;
					return true;

				case 3:         // English CD version
					WinFixOffset = 0x4D22E;
					return true;

				default:        // Unrecognised EXE
					WinFixOffset = 0;
					return false;
			}
		}

		/// <summary>
		/// Struct which describes the offsets that need to be patched to change the game's resolution.
		/// </summary>
		internal struct ResHexOffsetTable
		{
			internal int _resWidth;
			internal int _resHeight;
			internal int _mainMenuViewportWidth;
			internal int _mainMenuViewportHeight;

			internal int _fixMoneyPopDateTextPosWidth;
			internal int _fixTopMenuBarBackgroundPosWidth;

			internal int _viewportWidth;
			internal int _viewportHeightMult;
			internal int _viewportWidthMult;

			internal int _sidebarRenderLimitWidth;
			internal int _fixSidebarCityMap_RotateButton;
			internal int _fixSidebarCityMap_RotateIcon;
			internal int _fixSidebarCityMap_GoalsOverviewWorldMapMessagesIcons;
			internal int _fixSidebarCityMap_GoalsOverviewWorldMapMessagesButtons;
			internal int _sidebarLeftEdgeStartWidth;

			internal int _fixBottomBarLength;
			internal int _unknownBottomBarTweak1;
			internal int _unknownBottomBarTweak2;

			internal int _unknownWidth;
			internal int _unknownHeight;

			internal int _extendOriginalUiNewCodeJumpOffset;
			internal byte[] _extendOriginalUiNewCodeJumpData;
			internal int _extendOriginalUiNewCodeOffset;
			internal byte[] _extendOriginalUiNewCodeData;
		}
	}
}
