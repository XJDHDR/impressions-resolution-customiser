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

						_fixCompSidebarBottomWidth = 0x0,
						_fixPushSidebarBottomWidth = 0x0,
						_sidebarRenderLimitWidth = 0x1B4E22,
						_sidebarLeftEdgeStartWidth = 0x1B4E2E,

						_unknownWidth = 0x0,
						_unknownHeight = 0x0,
						_unknownWidth2 = 0x0,

						_extendSidebarRedStripeNewCodeJump = 0x0,
						_paintBlueBackgroundInGapsNewCodeJump = 0x0,

						_extendSidebarRedStripeNewCode = 0x0,
						_paintBlueBackgroundInGapsNewCode = 0x0
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

						_fixCompSidebarBottomWidth = 0x0,
						_fixPushSidebarBottomWidth = 0x0,
						_sidebarRenderLimitWidth = 0x0,
						_sidebarLeftEdgeStartWidth = 0x0,

						_unknownWidth = 0x0,
						_unknownHeight = 0x0,
						_unknownWidth2 = 0x0,

						_extendSidebarRedStripeNewCodeJump = 0x0,
						_paintBlueBackgroundInGapsNewCodeJump = 0x0,

						_extendSidebarRedStripeNewCode = 0x0,
						_paintBlueBackgroundInGapsNewCode = 0x0
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

			internal int _fixCompSidebarBottomWidth;
			internal int _fixPushSidebarBottomWidth;
			internal int _sidebarRenderLimitWidth;
			internal int _sidebarLeftEdgeStartWidth;

			internal int _unknownWidth;
			internal int _unknownHeight;
			internal int _unknownWidth2;

			internal int _extendSidebarRedStripeNewCodeJump;
			internal int _paintBlueBackgroundInGapsNewCodeJump;

			internal int _extendSidebarRedStripeNewCode;
			internal int _paintBlueBackgroundInGapsNewCode;
		}
	}
}
