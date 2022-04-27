// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

namespace Emperor.non_UI_code
{
	/// <summary>
	/// Struct which describes the offsets that need to be patched to change the game's resolution.
	/// </summary>
	internal struct EmperorResHexOffsetTable
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


		/// <summary>
		/// Supplies a ResHexOffsetTable struct specifying the offsets that need to be patched, based on which version of Emperor.exe was supplied.
		/// </summary>
		/// <param name="EmperorExeAttributes">Struct that specifies various details about the detected Emperor.exe</param>
		/// <param name="WasSuccessful">
		///		True if "_exeLangAndDistrib" matches one that this program knows about and knows the offsets that need to be patched.
		///		False if the EXE is not recognised.
		/// </param>
		internal EmperorResHexOffsetTable(EmperorExeAttributes EmperorExeAttributes, out bool WasSuccessful)
		{
			switch (EmperorExeAttributes._SelectedExeLangAndDistrib)
			{
				case ExeLangAndDistrib.GogEnglish:
					_ResWidth = 0x12AA6D;
					_ResHeight = 0x12AA72;
					_MainMenuViewportWidth = 0x12532A;
					_MainMenuViewportHeight = 0x125342;
					_FixMoneyPopDateTextPosWidth = 0x1B5C6A;
					_FixTopMenuBarBackgroundPosWidth = 0x1BDF70;
					_ViewportWidth = 0x13BE8D;
					_ViewportHeightMult = 0x13BE98;
					_ViewportWidthMult = 0x13BE9A;
					_SidebarRenderLimitWidth = 0x1B4E22;
					_FixSidebarCityMapRotateButton = 0x13A5DA;
					_FixSidebarCityMapRotateIcon = 0x13A6CA;
					_FixSidebarCityMapGoalsOverviewWorldMapMessagesIcons = 0x13AA0A;
					_FixSidebarCityMapGoalsOverviewWorldMapMessagesButtons = 0x13AA3A;
					_SidebarLeftEdgeStartWidth = 0x1B4E2E;
					_UnknownWidth = 0x12AD53;
					_UnknownHeight = 0x12AD5D;
					_DrawFunction1Address = 0x7570;
					_DrawFunction2Address = 0x12D60;

					_FixBottomBarLengthNewCodeInsertPoint = 0x1BDF9C;
					_FixBottomBarLengthNewCode = defineFixBottomBarLengthNewCode();

					_FixBottomBarLengthFinalJumpDest = 0x1BE038;
					_NewCodeForUiJumpLocation = 0x13A7E3;
					_NewCodeForUiInsertionLocation = 0x3A8060;
					_NewCodeForUiBytes = defineNewCodeForUiBytes();
					WasSuccessful = true;
					return;

				case ExeLangAndDistrib.CdEnglish:
					_ResWidth = 0x12B66D;
					_ResHeight = 0x12B672;
					_MainMenuViewportWidth = 0x0;
					_MainMenuViewportHeight = 0x0;
					_FixMoneyPopDateTextPosWidth = 0x0;
					_FixTopMenuBarBackgroundPosWidth = 0x0;
					_ViewportWidth = 0x0;
					_ViewportHeightMult = 0x0;
					_ViewportWidthMult = 0x0;
					_SidebarRenderLimitWidth = 0x0;
					_FixSidebarCityMapRotateButton = 0x0;
					_FixSidebarCityMapRotateIcon = 0x0;
					_FixSidebarCityMapGoalsOverviewWorldMapMessagesIcons = 0x0;
					_FixSidebarCityMapGoalsOverviewWorldMapMessagesButtons = 0x0;
					_SidebarLeftEdgeStartWidth = 0x0;
					_UnknownWidth = 0x0;
					_UnknownHeight = 0x0;
					_DrawFunction1Address = 0x0;
					_DrawFunction2Address = 0x0;
					_FixBottomBarLengthNewCodeInsertPoint = 0x0;
					_FixBottomBarLengthNewCode = null;
					_FixBottomBarLengthFinalJumpDest = 0x0;
					_NewCodeForUiJumpLocation = 0x0;
					_NewCodeForUiInsertionLocation = 0x3A80E0;
					_NewCodeForUiBytes = new byte[]
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
					};
					WasSuccessful = true;
					return ;

				// Unrecognised EXE
				case ExeLangAndDistrib.NotRecognised:
				default:
					_ResWidth = 0;
					_ResHeight = 0;
					_MainMenuViewportWidth = 0;
					_MainMenuViewportHeight = 0;
					_FixMoneyPopDateTextPosWidth = 0;
					_FixTopMenuBarBackgroundPosWidth = 0;
					_ViewportWidth = 0;
					_ViewportHeightMult = 0;
					_ViewportWidthMult = 0;
					_SidebarRenderLimitWidth = 0;
					_FixSidebarCityMapRotateButton = 0;
					_FixSidebarCityMapRotateIcon = 0;
					_FixSidebarCityMapGoalsOverviewWorldMapMessagesIcons = 0;
					_FixSidebarCityMapGoalsOverviewWorldMapMessagesButtons = 0;
					_SidebarLeftEdgeStartWidth = 0;
					_UnknownWidth = 0;
					_UnknownHeight = 0;
					_DrawFunction1Address = 0;
					_DrawFunction2Address = 0;
					_FixBottomBarLengthNewCodeInsertPoint = 0;
					_FixBottomBarLengthNewCode = null;
					_FixBottomBarLengthFinalJumpDest = 0;
					_NewCodeForUiJumpLocation = 0;
					_NewCodeForUiInsertionLocation = 0;
					_NewCodeForUiBytes = null;
					WasSuccessful = false;
					return;
			}
		}

		private static byte[] defineFixBottomBarLengthNewCode() =>
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
			};

		private static byte[] defineNewCodeForUiBytes() =>
			new byte[] {
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
			};
	}
}
