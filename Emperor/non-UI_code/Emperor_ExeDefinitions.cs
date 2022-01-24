// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using System;
using System.IO;
using System.Windows;
using Soft160.Data.Cryptography;

namespace Emperor.non_UI_code
{
	/// <summary>
	/// Struct that specifies various details about the Emperor.exe given to the program.
	/// </summary>
	internal struct ExeAttributes
	{
		internal ExeLangAndDistrib _SelectedExeLangAndDistrib;
		internal bool _IsDiscVersion;
	}

	/// <summary>
	/// Used to define the various versions of Emperor.exe that this program recognises and knows what offsets to patch.
	/// </summary>
	internal enum ExeLangAndDistrib
	{
		NotRecognised = 0,
		GogEnglish = 1,
		CdEnglish = 3
	}

	/// <summary>
	/// Struct that specifies various details about the Emperor.exe given to the program.
	/// </summary>
	internal static class EmperorExeDefinitions
	{
		/// <summary>
		/// Copies the contents of Emperor.exe into a byte array for editing then calculates a CRC32 hash for the contents of that array.
		/// After that, compares that CRC to a list of known CRCs to determine which distribution of this game is being patched.
		/// </summary>
		/// <param name="_ZeusExeLocation_">String that defines the location of Emperor.exe</param>
		/// <param name="_ZeusExeData_">Byte array that contains the binary data contained within the supplied Emperor.exe</param>
		/// <param name="_ExeAttributes_">Struct that specifies various details about the detected Emperor.exe</param>
		/// <returns>
		/// True if the CRC for the selected Emperor.exe matches one that this program knows about and knows the offsets that need to be patched.
		/// False if the EXE is not recognised.
		/// </returns>
		internal static bool _GetAndCheckExeChecksum(string _ZeusExeLocation_, out byte[] _ZeusExeData_, out ExeAttributes _ExeAttributes_)
		{
			_ZeusExeData_ = File.ReadAllBytes(_ZeusExeLocation_);

			uint _fileHash_ = CRC.Crc32(_ZeusExeData_, 0, _ZeusExeData_.Length);

			switch (_fileHash_)
			{
				// English GOG version
				case 0xFD9CF46F:
					_ExeAttributes_ = new ExeAttributes
					{
						_SelectedExeLangAndDistrib = ExeLangAndDistrib.GogEnglish,
						_IsDiscVersion = false,
					};
					return true;

				/*// English CD version
				case 0xA8A1AE71:
					ExeAttributes = new ExeAttributes
					{
						SelectedExeLangAndDistrib = ExeLangAndDistrib.CD_English,
						IsDiscVersion = true,
					};
					return true;*/		// Comment this out until CD version is working

				// Unrecognised EXE
				default:
					string[] _messageLines_ = new string[]
					{
						"Emperor.exe was not recognised. Only the following unmodified distributions and languages are currently supported:",
						"- English GOG version (hash: ?)"
						//,"- English CD version"	// Comment this out until CD version works
					};
					MessageBox.Show(string.Join(Environment.NewLine, _messageLines_));
					_ExeAttributes_ = new ExeAttributes
					{
						_SelectedExeLangAndDistrib = ExeLangAndDistrib.NotRecognised,
						_IsDiscVersion = false,
					};
					return false;
			}
		}

		/// <summary>
		/// Supplies a ResHexOffsetTable struct specifying the offsets that need to be patched, based on which version of Emperor.exe was supplied.
		/// </summary>
		/// <param name="_ExeAttributes_">Struct that specifies various details about the detected Emperor.exe</param>
		/// <param name="_ResHexOffsetTable_">Struct containing the offset for the supplied Emperor.exe that needs patching.</param>
		/// <returns>
		/// True if "_exeLangAndDistrib" matches one that this program knows about and knows the offsets that need to be patched.
		/// False if the EXE is not recognised.
		/// </returns>
		internal static bool _FillResHexOffsetTable(ExeAttributes _ExeAttributes_, out ResHexOffsetTable _ResHexOffsetTable_)
		{
			switch (_ExeAttributes_._SelectedExeLangAndDistrib)
			{
				case ExeLangAndDistrib.GogEnglish:
					_ResHexOffsetTable_ = new ResHexOffsetTable(
						0x12AA6D,
						0x12AA72,

						0x12532A,
						0x125342,

						0x1B5C6A,
						0x1BDF70,

						0x13BE8D,
						0x13BE98,
						0x13BE9A,

						0x1B4E22,
						0x13A5DA,
						0x13A6CA,
						0x13AA0A,
						0x13AA3A,
						0x1B4E2E,

						0x1BDFC9,
						0x1BDFA8,
						0x1BDFDC,

						0x12AD53,
						0x12AD5D,

						0x8170,
						0x13960,

						0x13A841,
						new byte[] {0xE9, 0x9A, 0xD8, 0x26, 0x00},

						0x3A80E0,
						new byte[]
						{
							// To draw a graphic element in Emperor requires that 6 values be pushed onto the stack. The contents
							// of the memory address 0x1C42130 must also be copied into the ECX register. Then call the
							// 1st drawing function. After that, push the EAX register onto the stack and call the 2nd drawing
							// function. Finally, finish off by adding 0x18 to the ESP register.
							0x6A, 0x00,							// push 0
							0x6A, 0x00,
							0x6A, 0x00,
							0x68, 0x4E, 0x02, 0x00, 0x00,		// push 0x24E					- Vertical location of element's top-left corner?
							0x68, 0x4E, 0x05, 0x00, 0x00,		// push 0x54E					- Horizontal location of element's top-left corner?
							0x68, 0x74, 0x02, 0x00, 0x00,		// push 0x274
							0xB9, 0x30, 0x21, 0xC4, 0x01,		// mov ecx, off_0x1C42130
				/* 26 */	0xE8, 0x71, 0xF4, 0xC5, 0xFF,		// call rel 0xFFC5F471 (0x8170)
							0x50,								// push eax
							0xE8, 0x5B, 0xAC, 0xC6, 0xFF,		// call rel 0xFFC6AC5B (0x13960)
							0x83, 0xC4, 0x18,					// add esp, 0x18

							0x6A, 0x00,							// push 0
							0x6A, 0x00,
							0x6A, 0x00,
							0x6A, 0x00,
				/* 48 */	0x68, 0x00, 0x03, 0x00, 0x00,		// push 0x300
							0x68, 0x7E, 0x02, 0x00, 0x00,		// push 0x27E
							0xB9, 0x30, 0x21, 0xC4, 0x01,		// mov ecx, off_0x1C42130
							0xE8, 0x4C, 0xF4, 0xC5, 0xFF,		// call rel 0xFFC5F44C (0x8170)
							0x50,								// push eax
							0xE8, 0x36, 0xAC, 0xC6, 0xFF,		// call rel 0xFFC6AC36 (0x13960)
							0x83, 0xC4, 0x18,					// add esp, 0x18
							0x6A, 0x00,							// push 0
							0x6A, 0x00,
							0x6A, 0x00,
				/* 83 */	0x68, 0xC0, 0x01, 0x00, 0x00,		// push 0x1C0
							0x68, 0x30, 0x06, 0x00, 0x00,		// push 0x630
							0x68, 0x81, 0x02, 0x00, 0x00,		// push 0x281
							0xB9, 0x30, 0x21, 0xC4, 0x01,		// mov ecx, off_0x1C42130
							0xE8, 0x24, 0xF4, 0xC5, 0xFF,		// call rel 0xFFC5F424 (0x8170)
							0x50,								// push eax
							0xE8, 0x0E, 0xAC, 0xC6, 0xFF,		// call rel 0xFFC6AC0E (0x13960)
							0x83, 0xC4, 0x18,					// add esp, 0x18
							0x6A, 0x00,							// push 0
							0x6A, 0x00,
							0x6A, 0x00,
							0x6A, 0x00,
							0x68, 0x30, 0x06, 0x00, 0x00,		// push 0x630
							0x68, 0x81, 0x02, 0x00, 0x00,		// push 0x281
							0xB9, 0x30, 0x21, 0xC4, 0x01,		// mov ecx, off_0x1C42130
							0xE8, 0xFF, 0xF3, 0xC5, 0xFF,		// call rel 0xFFC5F3FF (0x8170)
							0x50,								// push eax
							0xE8, 0xE9, 0xAB, 0xC6, 0xFF,		// call rel 0xFFC6ABE9 (0x13960)
							0x83, 0xC4, 0x18,					// add esp, 0x18
							0x6A, 0x00,							// push 0
							0x6A, 0x00,
							0x6A, 0x00,
							0x68, 0x10, 0x04, 0x00, 0x00,		// push 0x410
							0x68, 0x80, 0x02, 0x00, 0x00,		// push 0x280
							0x68, 0xD6, 0x02, 0x00, 0x00,		// push 0x2D6
							0xB9, 0x30, 0x21, 0xC4, 0x01,		// mov ecx, off_0x1C42130
							0xE8, 0xD7, 0xF3, 0xC5, 0xFF,		// call rel 0xFFC5F3D7 (0x8170)
							0x50,								// push eax
							0xE8, 0xC1, 0xAB, 0xC6, 0xFF,		// call rel 0xFFC6ABC1 (0x13960)
							0x83, 0xC4, 0x18,					// add esp, 0x18
							0xE8, 0xC9, 0xD4, 0xE0, 0xFF,		// call rel 0xFFE0D4C9 (0x1B6270)
							0xE9, 0x9A, 0x26, 0xD9, 0xFF		// jmp rel 0xFFD9269A
						});
					return true;

				case ExeLangAndDistrib.CdEnglish:
					_ResHexOffsetTable_ = new ResHexOffsetTable(
						0x12B66D,
						0x12B672,

						0x0,
						0x0,

						0x0,
						0x0,

						0x0,
						0x0,
						0x0,

						0x0,
						0x0,
						0x0,
						0x0,
						0x0,
						0x0,

						0x1BEBC9,
						0x1BEBA8,
						0x1BEBDC,

						0x0,
						0x0,

						0x0,
						0x0,

						0x13A841,
						new byte[] {0xE9, 0x9A, 0xD8, 0x26, 0x00},

						0x3A80E0,
						new byte[]
						{
							0x6A, 0x00, // push 0
							0x6A, 0x00,
							0x6A, 0x00,
							0x68, 0x4E, 0x02, 0x00, 0x00, // push 0x24E
							0x68, 0x4E, 0x05, 0x00, 0x00, // push 0x54E
							0x68, 0x74, 0x02, 0x00, 0x00, // push 0x274
							0xB9, 0x30, 0x21, 0xC4, 0x01, // mov ecx, off_0x1C42130
							0xE8, 0x71, 0xF4, 0xC5, 0xFF, // call rel 0xFFC5F471 (0x8170)
							0x50, // push eax
							0xE8, 0x5B, 0xAC, 0xC6, 0xFF, // call rel 0xFFC6AC5B (0x13960)
							0x83, 0xC4, 0x18, // add esp, 0x18
							0x6A, 0x00, // push 0
							0x6A, 0x00,
							0x6A, 0x00,
							0x6A, 0x00,
							0x68, 0x00, 0x03, 0x00, 0x00, // push 0x300
							0x68, 0x7E, 0x02, 0x00, 0x00, // push 0x27E
							0xB9, 0x30, 0x21, 0xC4, 0x01, // mov ecx, off_0x1C42130
							0xE8, 0x4C, 0xF4, 0xC5, 0xFF, // call rel 0xFFC5F44C (0x8170)
							0x50, // push eax
							0xE8, 0x36, 0xAC, 0xC6, 0xFF, // call rel 0xFFC6AC36 (0x13960)
							0x83, 0xC4, 0x18, // add esp, 0x18
							0x6A, 0x00, // push 0
							0x6A, 0x00,
							0x6A, 0x00,
							0x68, 0xC0, 0x01, 0x00, 0x00, // push 0x1C0
							0x68, 0x30, 0x06, 0x00, 0x00, // push 0x630
							0x68, 0x81, 0x02, 0x00, 0x00, // push 0x281
							0xB9, 0x30, 0x21, 0xC4, 0x01, // mov ecx, off_0x1C42130
							0xE8, 0x24, 0xF4, 0xC5, 0xFF, // call rel 0xFFC5F424 (0x8170)
							0x50, // push eax
							0xE8, 0x0E, 0xAC, 0xC6, 0xFF, // call rel 0xFFC6AC0E (0x13960)
							0x83, 0xC4, 0x18, // add esp, 0x18
							0x6A, 0x00, // push 0
							0x6A, 0x00,
							0x6A, 0x00,
							0x6A, 0x00,
							0x68, 0x30, 0x06, 0x00, 0x00, // push 0x630
							0x68, 0x81, 0x02, 0x00, 0x00, // push 0x281
							0xB9, 0x30, 0x21, 0xC4, 0x01, // mov ecx, off_0x1C42130
							0xE8, 0xFF, 0xF3, 0xC5, 0xFF, // call rel 0xFFC5F3FF (0x8170)
							0x50, // push eax
							0xE8, 0xE9, 0xAB, 0xC6, 0xFF, // call rel 0xFFC6ABE9 (0x13960)
							0x83, 0xC4, 0x18, // add esp, 0x18
							0x6A, 0x00, // push 0
							0x6A, 0x00,
							0x6A, 0x00,
							0x68, 0x10, 0x04, 0x00, 0x00, // push 0x410
							0x68, 0x80, 0x02, 0x00, 0x00, // push 0x280
							0x68, 0xD6, 0x02, 0x00, 0x00, // push 0x2D6
							0xB9, 0x30, 0x21, 0xC4, 0x01, // mov ecx, off_0x1C42130
							0xE8, 0xD7, 0xF3, 0xC5, 0xFF, // call rel 0xFFC5F3D7 (0x8170)
							0x50, // push eax
							0xE8, 0xC1, 0xAB, 0xC6, 0xFF, // call rel 0xFFC6ABC1 (0x13960)
							0x83, 0xC4, 0x18, // add esp, 0x18
							0xE8, 0xC9, 0xD4, 0xE0, 0xFF, // call rel 0xFFE0D4C9 (0x1B6270)
							0xE9, 0x9A, 0x26, 0xD9, 0xFF // jmp rel 0xFFD9269A
						});
					return true;

				// Unrecognised EXE
				case ExeLangAndDistrib.NotRecognised:
				default:
					_ResHexOffsetTable_ = new ResHexOffsetTable();
					return false;
			}
		}

		/// <summary>
		/// Supplies an int specifying the offset that needs to be patched, based on which version of Emperor.exe was supplied.
		/// </summary>
		/// <param name="_ExeAttributes_">Struct that specifies various details about the detected Emperor.exe</param>
		/// <param name="_WinFixOffset_">Int containing the offset for the supplied Emperor.exe that needs patching.</param>
		/// <returns>
		/// True if "_exeLangAndDistrib" matches one that this program knows about and knows the offsets that need to be patched.
		/// False if the EXE is not recognised.
		/// </returns>
		internal static bool _IdentifyWinFixOffset(ExeAttributes _ExeAttributes_, out int _WinFixOffset_)
		{
			switch ((byte)_ExeAttributes_._SelectedExeLangAndDistrib)
			{
				case 1:         // English GOG version
					_WinFixOffset_ = 0x4C62E;
					return true;

				case 3:         // English CD version
					_WinFixOffset_ = 0x4D22E;
					return true;

				default:        // Unrecognised EXE
					_WinFixOffset_ = 0;
					return false;
			}
		}

		/// <summary>
		/// Struct which describes the offsets that need to be patched to change the game's resolution.
		/// </summary>
		internal struct ResHexOffsetTable
		{
			internal readonly int _ResWidth;
			internal readonly int _ResHeight;
			internal readonly int _MainMenuViewportWidth;
			internal readonly int _MainMenuViewportHeight;

			internal readonly int _FixMoneyPopDateTextPosWidth;
			internal readonly int _FixTopMenuBarBackgroundPosWidth;

			internal readonly int _ViewportWidth;
			internal readonly int _ViewportHeightMult;
			internal readonly int _ViewportWidthMult;

			internal readonly int _SidebarRenderLimitWidth;
			internal readonly int _FixSidebarCityMapRotateButton;
			internal readonly int _FixSidebarCityMapRotateIcon;
			internal readonly int _FixSidebarCityMapGoalsOverviewWorldMapMessagesIcons;
			internal readonly int _FixSidebarCityMapGoalsOverviewWorldMapMessagesButtons;
			internal readonly int _SidebarLeftEdgeStartWidth;

			internal readonly int _FixBottomBarLength;
			internal readonly int _UnknownBottomBarTweak1;
			internal readonly int _UnknownBottomBarTweak2;

			internal readonly int _UnknownWidth;
			internal readonly int _UnknownHeight;

			internal readonly int _DrawFunction1Address;
			internal readonly int _DrawFunction2Address;

			internal readonly int _ExtendOriginalUiNewCodeJumpOffset;
			internal readonly byte[] _ExtendOriginalUiNewCodeJumpData;
			internal readonly int _ExtendOriginalUiNewCodeOffset;
			internal readonly byte[] _ExtendOriginalUiNewCodeData;

			public ResHexOffsetTable(int _ResWidth_, int _ResHeight_, int _MainMenuViewportWidth_, int _MainMenuViewportHeight_,
				int _FixMoneyPopDateTextPosWidth_, int _FixTopMenuBarBackgroundPosWidth_, int _ViewportWidth_, int _ViewportHeightMult_,
				int _ViewportWidthMult_, int _SidebarRenderLimitWidth_, int _FixSidebarCityMapRotateButton_, int _FixSidebarCityMapRotateIcon_,
				int _FixSidebarCityMapGoalsOverviewWorldMapMessagesIcons_, int _FixSidebarCityMapGoalsOverviewWorldMapMessagesButtons_,
				int _SidebarLeftEdgeStartWidth_, int _FixBottomBarLength_, int _UnknownBottomBarTweak1_, int _UnknownBottomBarTweak2_,
				int _UnknownWidth_, int _UnknownHeight_, int _DrawFunction1Address_, int _DrawFunction2Address_,
				int _ExtendOriginalUiNewCodeJumpOffset_, byte[] _ExtendOriginalUiNewCodeJumpData_,
				int _ExtendOriginalUiNewCodeOffset_, byte[] _ExtendOriginalUiNewCodeData_)
			{
				_ResWidth = _ResWidth_;
				_ResHeight = _ResHeight_;
				_MainMenuViewportWidth = _MainMenuViewportWidth_;
				_MainMenuViewportHeight = _MainMenuViewportHeight_;
				_FixMoneyPopDateTextPosWidth = _FixMoneyPopDateTextPosWidth_;
				_FixTopMenuBarBackgroundPosWidth = _FixTopMenuBarBackgroundPosWidth_;
				_ViewportWidth = _ViewportWidth_;
				_ViewportHeightMult = _ViewportHeightMult_;
				_ViewportWidthMult = _ViewportWidthMult_;
				_SidebarRenderLimitWidth = _SidebarRenderLimitWidth_;
				_FixSidebarCityMapRotateButton = _FixSidebarCityMapRotateButton_;
				_FixSidebarCityMapRotateIcon = _FixSidebarCityMapRotateIcon_;
				_FixSidebarCityMapGoalsOverviewWorldMapMessagesIcons = _FixSidebarCityMapGoalsOverviewWorldMapMessagesIcons_;
				_FixSidebarCityMapGoalsOverviewWorldMapMessagesButtons = _FixSidebarCityMapGoalsOverviewWorldMapMessagesButtons_;
				_SidebarLeftEdgeStartWidth = _SidebarLeftEdgeStartWidth_;
				_FixBottomBarLength = _FixBottomBarLength_;
				_UnknownBottomBarTweak1 = _UnknownBottomBarTweak1_;
				_UnknownBottomBarTweak2 = _UnknownBottomBarTweak2_;
				_UnknownWidth = _UnknownWidth_;
				_UnknownHeight = _UnknownHeight_;
				_DrawFunction1Address = _DrawFunction1Address_;
				_DrawFunction2Address = _DrawFunction2Address_;
				_ExtendOriginalUiNewCodeJumpOffset = _ExtendOriginalUiNewCodeJumpOffset_;
				_ExtendOriginalUiNewCodeJumpData = _ExtendOriginalUiNewCodeJumpData_;
				_ExtendOriginalUiNewCodeOffset = _ExtendOriginalUiNewCodeOffset_;
				_ExtendOriginalUiNewCodeData = _ExtendOriginalUiNewCodeData_;
			}
		}
	}
}
