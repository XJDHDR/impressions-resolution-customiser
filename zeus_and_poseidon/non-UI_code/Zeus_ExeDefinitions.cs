// This code is part of the Impressions Resolution Customiser project
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using Soft160.Data.Cryptography;
using System;
using System.IO;
using System.Windows;

namespace Zeus_and_Poseidon
{
	/// <summary>
	/// Struct that specifies various details about the Zeus.exe given to the program.
	/// </summary>
	internal struct ExeAttributes
	{
		internal ExeLangAndDistrib SelectedExeLangAndDistrib;
		internal bool IsDiscVersion;
		internal bool IsPoseidonInstalled;
	}

	/// <summary>
	/// Used to define the various versions of Zeus.exe that this program recognises and knows what offsets to patch. 
	/// </summary>
	internal enum ExeLangAndDistrib
	{
		Not_Recognised = 0,
		GOG_English = 1
	}

	/// <summary>
	/// This class contains all of the code that specifies EXE specific data which varies based on the version of Zeus that is being patched. 
	/// Hypothetically, adding support for additional versions of the game will only require editing this class.
	/// </summary>
	internal class Zeus_ExeDefinitions
	{
		/// <summary>
		/// Copies the contents of Zeus.exe into a byte array for editing then calculates a CRC32 hash for the contents of that array. 
		/// After that, compares that CRC to a list of known CRCs to determine which distribution of this game is being patched.
		/// </summary>
		/// <param name="ZeusExeLocation">String that defines the location of Zeus.exe</param>
		/// <param name="ZeusExeData">Byte array that contains the binary data contained within the supplied Zeus.exe</param>
		/// <param name="ExeAttributes">Struct that specifies various details about the detected Zeus.exe</param>
		/// <returns>
		/// True if the CRC for the selected Zeus.exe matches one that this program knows about and knows the offsets that need to be patched.
		/// False if the EXE is not recognised.
		/// </returns>
		internal static bool GetAndCheckExeChecksum(string ZeusExeLocation, out byte[] ZeusExeData, out ExeAttributes ExeAttributes)
		{
			ZeusExeData = File.ReadAllBytes(ZeusExeLocation);

			uint _fileHash = CRC.Crc32(ZeusExeData, 0, ZeusExeData.Length);

			switch (_fileHash)
			{
				case 0x90B9CF84:        // English GOG version
					ExeAttributes = new ExeAttributes
					{
						SelectedExeLangAndDistrib = ExeLangAndDistrib.GOG_English,
						IsDiscVersion = false,
						IsPoseidonInstalled = true
					};
					return true;

				default:                // Unrecognised EXE
					string[] _messageLines = new string[]
					{
						"Zeus.exe was not recognised. Only the following unmodified distributions and languages are currently supported:",
						"- English GOG version with Poseidon expansion"
					};
					MessageBox.Show(string.Join(Environment.NewLine, _messageLines));
					ExeAttributes = new ExeAttributes
					{
						SelectedExeLangAndDistrib = ExeLangAndDistrib.Not_Recognised,
						IsDiscVersion = false,
						IsPoseidonInstalled = false
					};
					return false;
			}
		}

		/// <summary>
		/// Supplies a ResHexOffsetTable struct specifying the offsets that need to be patched, based on which version of Zeus.exe was supplied.
		/// </summary>
		/// <param name="ExeAttributes">Struct that specifies various details about the detected Zeus.exe</param>
		/// <param name="ResHexOffsetTable">Struct containing the offset for the supplied Zeus.exe that needs patching.</param>
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
						_resWidth = 0x10BA29,
						_resHeight = 0x10BA2E,
						_mainMenuViewportWidth = 0x1051FA,
						_mainMenuViewportHeight = 0x105212,

						_fixMoneyPopDateTextPosWidth = 0x18EF7C,
						_fixTopMenuBarBackgroundPosWidth = 0x19EBFB,

						_viewportWidth = 0x11CC1E,
						_viewportHeightMult = 0x11CC29,
						_viewportWidthMult = 0x11CC2B,

						_fixCompBottomBlackBarWidth = 0x18E2F7,
						_fixPushBottomBlackBarWidth = 0x18E30A,
						_sidebarRenderLimitWidth = 0x18E2BE,
						_sidebarLeftEdgeStartWidth = 0x18E2CA,

						_unknownWidth = 0x10BD03,
						_unknownHeight = 0x10BD0D,
						_unknownWidth2 = 0x1C459E,

						_extendSidebarRedStripeNewCodeJumpOffset = 0x117B75,
						_extendSidebarRedStripeNewCodeJumpData = new byte[] { 0xE9, 0x1A, 0x04, 0x0C, 0x00 },

						_paintBlueBackgroundInGapsNewCodeJumpOffset = 0x19EC31,
						_paintBlueBackgroundInGapsNewCodeJumpData = new byte[] { 0xE9, 0x0A, 0x92, 0x03, 0x00 },

						_extendSidebarRedStripeNewCodeOffset = 0x1D7FA0,
						_extendSidebarRedStripeNewCodeData = new byte[]
						{
							0xA3,0xF4,0xDF,0x60,0x01,0xE8,0xA6,0x4F, 0xFD,0xFF,0xA1,0xF4,0xDF,0x60,0x01,0x8B, 0x0D,0x34,0xFF,0x0D,0x01,0x6A,0x00,0x6A,
							0x00,0x6A,0x00,0x6A,0x00,0x51,0x50,0xB9, 0xE8,0xEB,0x2A,0x01,0xE8,0x87,0x4F,0xFD, 0xFF,0xE9,0xB8,0xFB,0xF3,0xFF
						},

						_paintBlueBackgroundInGapsNewCodeOffset = 0x1D7E40,
						_paintBlueBackgroundInGapsNewCodeData = new byte[]
						{
							0xE8,0x0B,0x51,0xFD,0xFF,		// call sub_1ACF50		- Calls the function that draws the top bar graphic at screen coordinate
							0xBB,0x46,0x03,0x00,0x00,		// mov ebx, 346h		- Set the loop counter's value to the bar graphic's length (838px)
							0x81,0xFB,0x00,0x00,0x00,0x00,	// cmp ebx, 0h			- Compare the loop counter's value to the city view's width - set by ResolutionEdits class
							0x73,0x33,						// jnb short loc_1D7E85	- Jump forward by 51 bytes (xor ebx, ebx) if cmp didn't set the Carry Flag
							0xA1,0x64,0x94,0x2C,0x01,		// mov eax, 0x12C9464				 (i.e. ebx is >= city view's width)
							0xA1,0x30,0xC8,0x2B,0x01,		// mov eax, 0x12BC830
							0x05,0x58,0x03,0x00,0x00,		// add eax, 358h
							0x8B,0x40,0x04,					// mov eax, [eax+4]
							0x6A,0x00,						// push 0
							0x6A,0x00,
							0x6A,0x00,
							0x05,0x00,0x40,0x00,0x00,		// add eax, 4000h
							0x6A,0x00,						// push 0				- Sets the vertical position of the bar graphic's top edge right against the window's top edge
							0x53,							// push ebx				- Sets the horizontal position of current bar graphic's left edge to counter's value
							0x50,							// push eax
							0xB9,0xE8,0xEB,0x2A,0x01,		// mov ecx, offset 0x12AEBE8
							0xE8,0xD3,0x50,0xFD,0xFF,		// call sub_1ACF50		- Calls the function that draws the top bar graphic at screen coordinate
							0x81,0xC3,0x46,0x03,0x00,0x00,	// add ebx, 346h		- Increment the loop counter's value by 838
							0xEB,0xC5,						// jmp short loc_1D7E4A	- Jump back by 59 bytes (cmp ebx, <width>)
							0x33,0xDB,						// xor ebx, ebx			- Counter has done it's job so zero it
							0xA1,0x64,0x94,0x2C,0x01,
							0xA1,0x30,0xC8,0x2B,0x01,
							0x05,0x58,0x03,0x00,0x00,
							0x8B,0x40,0x04,
							0x6A,0x00,
							0x6A,0x00,
							0x6A,0x00,
							0x05,0x00,0x40,0x00,0x00,
							0x6A,0x00,
							0x68,0x00,0x00,0x00,0x00,		// push 0x0				- Sets horizontal position of the final bar graphic's left edge to 838px left of 
							0x50,							// push eax						the sidebar - set by ResolutionEdits class
							0xB9,0xE8,0xEB,0x2A,0x01,		// mov ecx, offset 0x12AEBE8
							0xE8,0x9A,0x50,0xFD,0xFF,		// call sub_1ACF50		- Calls the function that draws the top bar graphic at screen coordinate
							0xC7,0x05,0xF0,0xDF,0x60,0x01,0x00,0x00,0x00,0x00,	// mov dword_160DFF0, 0
							0xA1,0x64,0x94,0x2C,0x01,		// mov eax, 0x12C9464
							0xA1,0x30,0xC8,0x2B,0x01,
							0x05,0x58,0x03,0x00,0x00,
							0x8B,0x40,0x04,
							0x6A,0x00,
							0x6A,0x00,
							0x6A,0x00,
							0x05,0x00,0x40,0x00,0x00,
							0xFF,0x35,0xF0,0xDF,0x60,0x01,	// push dword_160DFF0	- Sets the vertical position of the bar graphic's top edge at the global var's stored value
							0x83,0x05,0xF0,0xDF,0x60,0x01,0x10,		// add dword_160DFF0, 0x10		- Draw the next bar graphic 10px lower than previous one
							0x68,0x00,0x00,0x00,0x00,		// push 0x0				- Sets the left edge position of the bar graphics used to fill the gap to the right of 
							0x50,                           //							the sidebar - set by ResolutionEdits class to city view + sidebar width
							0xB9,0xE8,0xEB,0x2A,0x01,
							0xE8,0x56,0x50,0xFD,0xFF,		// call sub_1ACF50		- Calls the function that draws the top bar graphic at screen coordinate
							0x81,0x3D,0xF0,0xDF,0x60,0x01,0x00,0x03,0x00,0x00,	// cmp dword_160DFF0, 0x300		- Check if global var's value if less, greater than or equal to 768
							0x72,0xBA,						// jb short loc_1D7EC0	- Jump back by 70 bytes (mov eax, 0x12C9464) if Carry Flag not set (i.e. GL is less than 768)
							0xC7,0x05,0xF0,0xDF,0x60,0x01,0x00,0x03,0x00,0x00,	// mov dword_160DFF0, 300h		- Set GL to 768.
							0xA1,0x64,0x94,0x2C,0x01,		// mov eax, 0x12C9464
							0xA1,0x30,0xC8,0x2B,0x01,
							0x05,0x58,0x03,0x00,0x00,
							0x8B,0x40,0x04,
							0x6A,0x00,
							0x6A,0x00,
							0x6A,0x00,
							0x05,0x00,0x40,0x00,0x00,
							0xFF,0x35,0xF0,0xDF,0x60,0x01,	// push dword_160DFF0	- Sets the vertical position of the bar graphic's top edge at the global var's stored value
							0x83,0x05,0xF0,0xDF,0x60,0x01,0x10,		// add dword_160DFF0, 0x10		- Draw the next bar graphic 10px lower than previous one
							0x68,0x00,0x00,0x00,0x00,		// push 0x0				- Sets the left edge position of the bar graphics used to fill the gap to the right of 
							0x50,							//							the sidebar - set by ResolutionEdits class to city view + red stripe width
							0xB9,0xE8,0xEB,0x2A,0x01,
							0xE8,0x06,0x50,0xFD,0xFF,		// call sub_1ACF50		- Calls the function that draws the top bar graphic at screen coordinate
							0x81,0x3D,0xF0,0xDF,0x60,0x01,0x00,0x00,0x00,0x00,	// cmp dword_160DFF0, 0x300	- Check if global var's value if less, greater than or equal to screen height
							0x72,0xBA,						// jb short loc_1D7F10	- Jump back by 70 bytes (mov eax, 0x12C9464) if Carry Flag not set (i.e. GL is less than screen height)
							0xE9,0xDB,0x6C,0xFC,0xFF		// jmp loc_19EC36		- Jump back to the byte after our jump into this injected code as we are done here.
						}
					};
					return true;

				default:        // Unrecognised EXE
					ResHexOffsetTable = new ResHexOffsetTable();
					return false;
			}
		}

		/// <summary>
		/// Fills an array containing all the offsets that need to be patched, based on which version of Zeus.exe was supplied.
		/// </summary>
		/// <param name="ExeAttributes">Struct that specifies various details about the detected Zeus.exe</param>
		/// <param name="AnimHexOffsetTable">Int array containing the offsets for the supplied Zeus.exe that need patching.</param>
		/// <returns>
		/// True if "_exeLangAndDistrib" matches one that this program knows about and knows the offsets that need to be patched.
		/// False if the EXE is not recognised.
		/// </returns>
		internal static bool FillAnimHexOffsetTable(ExeAttributes ExeAttributes, out int[] AnimHexOffsetTable)
		{
			switch ((byte)ExeAttributes.SelectedExeLangAndDistrib)
			{
				case 1:         // English GOG version
					AnimHexOffsetTable = new int[] { 0x30407, 0xB395D, 0xB3992, 0xB5642, 0xB5AED, 0xB5DE5, 0xB65FF, 0xB69B7, 0xB91D6, 0xB9AB2, 0xB9AFB, 0xB9B7C,
						0xB9DB1, 0xBA007, 0xBAC20, 0xBAC31, 0xBAC42, 0xBAC53, 0xBB1F4, 0xBB381, 0xBB3E5, 0xBB40B, 0xBB431, 0xBB457, 0xBB47D, 0xBB4A3, 0xBB4C9,
						0xBB4EC, 0xBB50F, 0xBB532, 0xBB593, 0xBB5AD, 0xBB5C7, 0xBB5E4, 0xBB656, 0xBD331, 0xBD349, 0xBD3B2, 0xBDC62, 0xBDC7F, 0xBDC9C, 0xBDCB9,
						0xBDD2F, 0xBDDD7, 0xBDE5A, 0xBDE9F, 0xBDEE4, 0xBDF29, 0xBDF6E, 0xBDFB3, 0xBDFF8, 0xBE03D, 0xBE082, 0xBE0C7, 0xBFC43, 0xBFDF8, 0xBFF47,
						0xC26D1, 0xC2740, 0xC28E3, 0xC2904, 0xC2BD8, 0xC3A78, 0xC8415, 0xC84FC, 0xC9DEC, 0xC9E80, 0xCB1D7, 0xCB1F0, 0xCB23F };
					return true;

				default:        // Unrecognised EXE
					AnimHexOffsetTable = new int[1];
					return false;
			}
		}

		/// <summary>
		/// Supplies an int specifying the offset that needs to be patched, based on which version of Zeus.exe was supplied.
		/// </summary>
		/// <param name="ExeAttributes">Struct that specifies various details about the detected Zeus.exe</param>
		/// <param name="WinFixOffset">Int containing the offset for the supplied Zeus.exe that needs patching.</param>
		/// <returns>
		/// True if "_exeLangAndDistrib" matches one that this program knows about and knows the offsets that need to be patched.
		/// False if the EXE is not recognised.
		/// </returns>
		internal static bool IdentifyWinFixOffset(ExeAttributes ExeAttributes, out int WinFixOffset)
		{
			switch ((byte)ExeAttributes.SelectedExeLangAndDistrib)
			{
				case 1:         // English GOG version
					WinFixOffset = 0x33E7E;
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

			internal int _fixCompBottomBlackBarWidth;
			internal int _fixPushBottomBlackBarWidth;
			internal int _sidebarRenderLimitWidth;
			internal int _sidebarLeftEdgeStartWidth;

			internal int _unknownWidth;
			internal int _unknownHeight;
			internal int _unknownWidth2;

			internal int _extendSidebarRedStripeNewCodeJumpOffset;
			internal byte[] _extendSidebarRedStripeNewCodeJumpData;

			internal int _paintBlueBackgroundInGapsNewCodeJumpOffset;
			internal byte[] _paintBlueBackgroundInGapsNewCodeJumpData;

			internal int _extendSidebarRedStripeNewCodeOffset;
			internal byte[] _extendSidebarRedStripeNewCodeData;

			internal int _paintBlueBackgroundInGapsNewCodeOffset;
			internal byte[] _paintBlueBackgroundInGapsNewCodeData;
		}
	}
}
