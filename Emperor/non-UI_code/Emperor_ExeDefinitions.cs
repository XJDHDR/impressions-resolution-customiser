// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using Emperor.non_UI_code.Crc32;

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
		/// <param name="_EmperorExeLocation_">String that defines the location of Emperor.exe</param>
		/// <param name="_EmperorExeData_">Byte array that contains the binary data contained within the supplied Emperor.exe</param>
		/// <param name="_ExeAttributes_">Struct that specifies various details about the detected Emperor.exe</param>
		/// <returns>
		/// True if the CRC for the selected Emperor.exe matches one that this program knows about and knows the offsets that need to be patched.
		/// False if the EXE is not recognised.
		/// </returns>
		internal static bool _GetAndCheckExeChecksum(string _EmperorExeLocation_, out byte[] _EmperorExeData_, out ExeAttributes _ExeAttributes_)
		{
			string[] _hexStringTable_ = {
				"00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "0a", "0b", "0c", "0d", "0e", "0f",
				"10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "1a", "1b", "1c", "1d", "1e", "1f",
				"20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "2a", "2b", "2c", "2d", "2e", "2f",
				"30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "3a", "3b", "3c", "3d", "3e", "3f",
				"40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "4a", "4b", "4c", "4d", "4e", "4f",
				"50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "5a", "5b", "5c", "5d", "5e", "5f",
				"60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "6a", "6b", "6c", "6d", "6e", "6f",
				"70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "7a", "7b", "7c", "7d", "7e", "7f",
				"80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "8a", "8b", "8c", "8d", "8e", "8f",
				"90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "9a", "9b", "9c", "9d", "9e", "9f",
				"a0", "a1", "a2", "a3", "a4", "a5", "a6", "a7", "a8", "a9", "aa", "ab", "ac", "ad", "ae", "af",
				"b0", "b1", "b2", "b3", "b4", "b5", "b6", "b7", "b8", "b9", "ba", "bb", "bc", "bd", "be", "bf",
				"c0", "c1", "c2", "c3", "c4", "c5", "c6", "c7", "c8", "c9", "ca", "cb", "cc", "cd", "ce", "cf",
				"d0", "d1", "d2", "d3", "d4", "d5", "d6", "d7", "d8", "d9", "da", "db", "dc", "dd", "de", "df",
				"e0", "e1", "e2", "e3", "e4", "e5", "e6", "e7", "e8", "e9", "ea", "eb", "ec", "ed", "ee", "ef",
				"f0", "f1", "f2", "f3", "f4", "f5", "f6", "f7", "f8", "f9", "fa", "fb", "fc", "fd", "fe", "ff"
			};

			_EmperorExeData_ = File.ReadAllBytes(_EmperorExeLocation_);

			// First, create a CRC32 Checksum of the EXE's data, excluding the first 4096 bytes.
			uint gameExeCrc32Checksum = SliceBy16.Crc32(0x1000, _EmperorExeData_);

			switch (gameExeCrc32Checksum)
			{
				// English GOG version
				case 0x8bc98c83:
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
						"- English GOG version"
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

						0x12AD53,
						0x12AD5D,

						0x7570,
						0x12D60,

						0x1BDF9C,
						new byte[] {
							// To draw a graphic element in Emperor requires that 6 values be pushed onto the stack. The contents
							// of the memory address 0x1C42130 must also be copied into the ECX register. Then call the
							// 1st drawing function. After that, push the EAX register onto the stack and call the 2nd drawing
							// function. Finally, finish off by adding 0x18 to the ESP register.
							0xBB, 0x00, 0x00, 0x00, 0x00,		// mov ebx, 0x00	index 1-4	- Set left edge position to <city viewport width> - Set at runtime
							0x81, 0xEB, 0x1E, 0x03, 0x00, 0x00,	// sub ebx, 0x31E				- Subtract 798 (bottom bar's length) from left edge position.

							0xB9, 0x30, 0x21, 0xC4, 0x01,		// mov ecx, 0x1C42130
							0x6A, 0x00,							// push 0						- Seems to move the texture inside the element horizontally.
							0x6A, 0x00,							//								- Unknown: Didn't see anything change after editing.
							0x6A, 0x00,							//								- Unknown: Didn't see anything change after editing.
							0x68, 0x00, 0x00, 0x00, 0x00,		// push 0x00		index 23-26	- Location of element's top edge - Set at runtime
							0x53,								// push ebx						- Location of element's left edge
							0x68, 0xD6, 0x02, 0x00, 0x00,		// push 0x2D6					- The ID for the UI element to draw - 0x2D6 is the bottom border bar.
							0xE8, 0x00, 0x00, 0x00, 0x00,		// call 0x00		index 34-37	- Call the 1st drawing function - Set at runtime
							0x50,								// push eax
							0xE8, 0x00, 0x00, 0x00, 0x00,		// call 0x00		index 40-43	- Call the 2nd drawing function - Set at runtime
							0x83, 0xC4, 0x18,					// add esp, 0x18

							0x83, 0xFB, 0x00,					// cmp ebx, 0x00				- Compare EBX against 0 - the coordinate for the window's left edge.
							0x7F, 0xD1,							// jg 0xD1						- Jump back 47 bytes to sub and draw another bar if ebx is greater than 0.

							0xBB, 0x01, 0x00, 0x00, 0x00,		// mov ebx, 0x01				- Set EBX register back to it's initial value.
							0xEB, 0x00							// jmp 0x00			index 58	- Jump forward to address of code that runs after this - Set at runtime
						},
						0x1BE038,

						0x13A7E3,
						0x3A8060,
						new byte[]
						{
							// Menubar size: 798x39 - The periwinkle roof tiles and their shadows take the top ~16px.
							// Set the height counters to it's initial value
							0xB8, 0x00, 0x00, 0x00, 0x00,		// mov eax, 0x00	index 1-4	- Set bar top edge start loc. to <window height - (39 + 16)> - Set at runtime

							// Set the vertical position counter to the correct position for the first bar on the current row.
							// Also subtract the length of a bar from that counter and save the current height value to memory.
							0xBB, 0x00, 0x00, 0x00, 0x00,		// mov ebx, 0x00	index 6-9	- Set bar left edge start loc. to <city viewport width> - Set at runtime
							0x81, 0xEB, 0x1E, 0x03, 0x00, 0x00,	// sub ebx, 0x31E				- Subtract 798 (menu bar's length) from left edge position.
							0xA3, 0xB0, 0x8C, 0x3F, 0x01,		// mov seg:off_0x013F8CB0, eax	- Copy EAX to backup memory address as draw func overwrites register.

							// Draw a menubar
							0xB9, 0x30, 0x21, 0xC4, 0x01,		// mov ecx, 0x1C42130
							0x6A, 0x00,							// push 0						- Seems to move the texture inside the element horizontally.
							0x6A, 0x00,							//								- Unknown: Didn't see anything change after editing.
							0x6A, 0x00,							//								- Unknown: Didn't see anything change after editing.
							0x50,								// push eax						- Location of element's top edge
							0x53,								// push ebx						- Location of element's left edge
							0x68, 0x7E, 0x02, 0x00, 0x00,		// push 0x27E					- The ID for the UI element to draw - 0x27E is the menu bar.
							0xE8, 0x00, 0x00, 0x00, 0x00,		// call 0x00		index 40-43	- Call the 1st drawing function - Set at runtime
							0x50,								// push eax
							0xE8, 0x00, 0x00, 0x00, 0x00,		// call 0x00		index 46-49	- Call the 2nd drawing function - Set at runtime
							0x83, 0xC4, 0x18,					// add esp, 0x18

							// Check if the positioning of the next bar graphic is correct and adjust if necessary.
							// Also check if all the menubars required have been drawn.
							0xA1, 0xB0, 0x8C, 0x3F, 0x01,		// mov eax, seg:off_0x013F8CB0	- Restore EAX's value from backup memory address.
							0x3D, 0x00, 0x00, 0x00, 0x00,		// cmp eax, 0x00	index 59-62	- Check EAX against <viewport height - 16 + 8> - Set at runtime
							0x7C, 0x0A,							// jl 0x0A						- Jump forward 10 to next check if above check's result = less than.
							0x83, 0xFB, 0x00,					// cmp ebx, 0x00				- Otherwise, compare EBX against 0.
							0x7F, 0xC4,							// jg 0xC4						- Jump back 60 bytes to draw prep if above check's result = greater than.
							0x83, 0xE8, 0x17,					// sub eax, 0x17				- Otherwise, subtract 23 (menu bar's height - tiles) from left edge position.
							0xEB, 0xBA,							// jmp 0xBA						- Jump back 70 bytes to draw prep & position left edge at start of new row.

							0x3D, 0x00, 0x00, 0x00, 0x00,		// cmp eax, 0x00				- Check EAX against 0.
							0x74, 0x07,							// je 0x07						- Jump forward 7 bytes to next compare if above check's result = equal.
							0xB8, 0x00, 0x00, 0x00, 0x00,		// mov eax, 0x00				- Otherwise, set menu bar's top edge to top of screen.
							0xEB, 0xAC,							// jmp 0xAC						- Jump back 84 bytes to draw prep & position left edge at start of new row.

							0x81, 0xFB, 0x1E, 0x03, 0x00, 0x00,	// cmp ebx, 0x31E				- Compare EBX against 798 (length of menubar).
							0x7D, 0xA9,							// jge 0xA9						- Jump back 87 bytes to next check if above check = greater than or equal.


							// sidebar rect size: 226x310
							// Set the initial values for positioning the first sidebar graphic filler.
							0xB8, 0xCA, 0x01, 0x00, 0x00,		// mov eax, 0x1CA				- Set bar top edge start loc. to 458: The (sidebar's height - elem's width).
							0xBB, 0x00, 0x00, 0x00, 0x00,		// mov ebx, 0x00  index 103-106	- Set bar left edge start loc. to <city viewport width> - Set at runtime

							// Move the drawing position to the next spot in the current column.
							0x05, 0x36, 0x01, 0x00, 0x00,		// add eax, 0x136				- Add 310 (sidebar element's height) to top edge position.
							0xA3, 0xB0, 0x8C, 0x3F, 0x01,		// mov seg:off_0x013F8CB0, eax	- Copy EAX to backup memory address as draw func overwrites register.

							// Draw a sidebar graphic.
							0xB9, 0x30, 0x21, 0xC4, 0x01,		// mov ecx, 0x1C42130
							0x6A, 0x00,							// push 0						- Seems to move the texture inside the element horizontally.
							0x6A, 0x00,							//								- Unknown: Didn't see anything change after editing.
							0x6A, 0x00,							//								- Unknown: Didn't see anything change after editing.
							0x50,								// push eax						- Location of element's top edge
							0x53,								// push ebx						- Location of element's left edge
							0x68, 0x74, 0x02, 0x00, 0x00,		// push 0x274					- The UI element ID - 0x274 is the dark-brown bottom portion of sidebar.
							0xE8, 0x00, 0x00, 0x00, 0x00,		// call 0x00	  index 136-139	- Call the 1st drawing function - Set at runtime
							0x50,								// push eax
							0xE8, 0x00, 0x00, 0x00, 0x00,		// call 0x00	  index 142-145	- Call the 2nd drawing function - Set at runtime
							0x83, 0xC4, 0x18,					// add esp, 0x18

							// Check if the positioning of the next graphic is correct and adjust if necessary.
							// Also check if all the sidebar pieces required have been drawn.
							0xA1, 0xB0, 0x8C, 0x3F, 0x01,		// mov eax, seg:off_0x013F8CB0	- Restore EAX's value from backup memory address.
							0x81, 0xFB, 0x00, 0x00, 0x00, 0x00,	// cmp ebx, 0x00  index 156-159	- Compare EBX against <window width> - Set at runtime
							0x7F, 0x14,							// jg 0x14						- Jump forward 20 bytes to cleanup code if above check = greater than.
							0x3D, 0x00, 0x00, 0x00, 0x00,		// cmp eax, 0x00  index 163-166	- Check EAX against <window height - 310> - Set at runtime
							0x7C, 0xC2,							// jl 0xC2						- Jump back 62 bytes to draw prep if above check's result = less than.
							0x81, 0xC3, 0xE2, 0x00, 0x00, 0x00,	// add ebx, 0xE2				- Add 226 (sidebar element's width) to left edge position.
							0xB8, 0x00, 0x00, 0x00, 0x00,		// mov eax, 0x00				- Set bar top edge start loc. to 0: The top of the window.
							0xEB, 0xBA,							// jmp 0xBA						- Jump back 70 bytes to draw logic.

							// Cleanup and jump back into the original code.
							0x33, 0xDB,							// xor ebx, ebx					- Set EBX register back to it's initial value (0).
							0x8B, 0x0D, 0x0C, 0x8E, 0x3F, 0x01,	// mov ecx, 0x13F8E0C			- Run the command that the jump into this code overwrote.
							0xE9, 0x00, 0x00, 0x00, 0x00		// jmp 0x00		  index 191-194	- Jump back into the original code - Set at runtime
						}
					);
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

						0x0,
						0x0,

						0x0,
						0x0,

						0x0,
						new byte[] {},
						0x0,

						0x0,
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

			internal readonly int _UnknownWidth;
			internal readonly int _UnknownHeight;

			internal readonly int _DrawFunction1Address;
			internal readonly int _DrawFunction2Address;

			internal readonly int _FixBottomBarLengthNewCodeInsertPoint;
			internal readonly byte[] _FixBottomBarLengthNewCode;
			internal readonly int _FixBottomBarLengthFinalJumpDest;

			internal readonly int _NewCodeForUiJumpLocation;
			internal readonly int _NewCodeForUiInsertionLocation;
			internal readonly byte[] _NewCodeForUiBytes;

			public ResHexOffsetTable(int _ResWidth_, int _ResHeight_, int _MainMenuViewportWidth_, int _MainMenuViewportHeight_,
				int _FixMoneyPopDateTextPosWidth_, int _FixTopMenuBarBackgroundPosWidth_, int _ViewportWidth_, int _ViewportHeightMult_,
				int _ViewportWidthMult_, int _SidebarRenderLimitWidth_, int _FixSidebarCityMapRotateButton_, int _FixSidebarCityMapRotateIcon_,
				int _FixSidebarCityMapGoalsOverviewWorldMapMessagesIcons_, int _FixSidebarCityMapGoalsOverviewWorldMapMessagesButtons_,
				int _SidebarLeftEdgeStartWidth_, int _UnknownWidth_, int _UnknownHeight_,
				int _DrawFunction1Address_, int _DrawFunction2Address_,
				int _FixBottomBarLengthNewCodeInsertPoint_, byte[] _FixBottomBarLengthNewCode_, int _FixBottomBarLengthFinalJumpDest_,
				int _NewCodeForUiJumpLocation_, int _NewCodeForUiInsertionLocation_, byte[] _NewCodeForUiBytes_)
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
				_UnknownWidth = _UnknownWidth_;
				_UnknownHeight = _UnknownHeight_;
				_DrawFunction1Address = _DrawFunction1Address_;
				_DrawFunction2Address = _DrawFunction2Address_;
				_FixBottomBarLengthNewCodeInsertPoint = _FixBottomBarLengthNewCodeInsertPoint_;
				_FixBottomBarLengthNewCode = _FixBottomBarLengthNewCode_;
				_FixBottomBarLengthFinalJumpDest = _FixBottomBarLengthFinalJumpDest_;
				_NewCodeForUiJumpLocation = _NewCodeForUiJumpLocation_;
				_NewCodeForUiInsertionLocation = _NewCodeForUiInsertionLocation_;
				_NewCodeForUiBytes = _NewCodeForUiBytes_;
			}
		}
	}
}
