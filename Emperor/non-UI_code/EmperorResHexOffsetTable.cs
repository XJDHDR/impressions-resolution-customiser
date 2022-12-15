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
	internal readonly struct EmperorResHexOffsetTable
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
					// There is one line (mov ecx, <constant>) that is different for each language.
					// For English, it needs to be: 0x1C42130
					_FixBottomBarLengthNewCode[12] = 0x30;
					_FixBottomBarLengthNewCode[13] = 0x21;
					_FixBottomBarLengthNewCode[14] = 0xC4;
					_FixBottomBarLengthNewCode[15] = 0x01;
					_FixBottomBarLengthFinalJumpDest = 0x1BE038;

					_NewCodeForUiJumpLocation = 0x13A7E3;
					_NewCodeForUiInsertionLocation = 0x3A8060;
					_NewCodeForUiBytes = defineNewCodeForUiBytes();
					// There are two lines (mov ecx, <constant>) here.
					// Same as before, for English, it needs to be: 0x1C42130
					_NewCodeForUiBytes[22] = 0x30;
					_NewCodeForUiBytes[23] = 0x21;
					_NewCodeForUiBytes[24] = 0xC4;
					_NewCodeForUiBytes[25] = 0x01;
					_NewCodeForUiBytes[118] = 0x30;
					_NewCodeForUiBytes[119] = 0x21;
					_NewCodeForUiBytes[120] = 0xC4;
					_NewCodeForUiBytes[121] = 0x01;
					// Finally, the command overwritten by the jump into new code pushes a constant into the ECX register.
					// This constant is different for each language. For English, the command is: mov ecx, 0x13F8E0C
					_NewCodeForUiBytes[186] = 0x0C;
					_NewCodeForUiBytes[187] = 0x8E;
					_NewCodeForUiBytes[188] = 0x3F;
					_NewCodeForUiBytes[189] = 0x01;
					WasSuccessful = true;
					return;

				case ExeLangAndDistrib.CdEnglish:
					_ResWidth = 0x12B66D;
					_ResHeight = 0x12B672;
					_MainMenuViewportWidth = 0x125F2A;
					_MainMenuViewportHeight = 0x125F42;
					_FixMoneyPopDateTextPosWidth = 0x1B686A;
					_FixTopMenuBarBackgroundPosWidth = 0x1BEB70;
					_ViewportWidth = 0x13CA8D;
					_ViewportHeightMult = 0x13CA98;
					_ViewportWidthMult = 0x13CA9A;
					_SidebarRenderLimitWidth = 0x1B5A22;
					_FixSidebarCityMapRotateButton = 0x13B1DA;
					_FixSidebarCityMapRotateIcon = 0x13B2CA;
					_FixSidebarCityMapGoalsOverviewWorldMapMessagesIcons = 0x13B60A;
					_FixSidebarCityMapGoalsOverviewWorldMapMessagesButtons = 0x13B63A;
					_SidebarLeftEdgeStartWidth = 0x1B5A2E;
					_UnknownWidth = 0x12B953;
					_UnknownHeight = 0x12B95D;
					_DrawFunction1Address = 0x8170;
					_DrawFunction2Address = 0x13960;

					_FixBottomBarLengthNewCodeInsertPoint = 0x1BEB9C;
					_FixBottomBarLengthNewCode = defineFixBottomBarLengthNewCode();
					// There is one line (mov ecx, <constant>) that is different for each language.
					// For English, it needs to be: 0x1C42130
					_FixBottomBarLengthNewCode[12] = 0x30;
					_FixBottomBarLengthNewCode[13] = 0x21;
					_FixBottomBarLengthNewCode[14] = 0xC4;
					_FixBottomBarLengthNewCode[15] = 0x01;
					_FixBottomBarLengthFinalJumpDest = 0x1BEC38;

					_NewCodeForUiJumpLocation = 0x13B3E3;
					_NewCodeForUiInsertionLocation = 0x3A8C60;
					_NewCodeForUiBytes = defineNewCodeForUiBytes();
					// There are two lines (mov ecx, <constant>) here.
					// Same as before, for English, it needs to be: 0x1C42130
					_NewCodeForUiBytes[22] = 0x30;
					_NewCodeForUiBytes[23] = 0x21;
					_NewCodeForUiBytes[24] = 0xC4;
					_NewCodeForUiBytes[25] = 0x01;
					_NewCodeForUiBytes[118] = 0x30;
					_NewCodeForUiBytes[119] = 0x21;
					_NewCodeForUiBytes[120] = 0xC4;
					_NewCodeForUiBytes[121] = 0x01;
					// Finally, the command overwritten by the jump into new code pushes a constant into the ECX register.
					// This constant is different for each language. For English, the command is: mov ecx, 0x13F8E0C
					_NewCodeForUiBytes[186] = 0x0C;
					_NewCodeForUiBytes[187] = 0x8E;
					_NewCodeForUiBytes[188] = 0x3F;
					_NewCodeForUiBytes[189] = 0x01;
					WasSuccessful = true;
					return ;

				case ExeLangAndDistrib.CdFrench:
					_ResWidth = 0x12B40D;
					_ResHeight = 0x12B412;
					_MainMenuViewportWidth = 0x125B9A;
					_MainMenuViewportHeight = 0x125BB2;
					_FixMoneyPopDateTextPosWidth = 0x1B6E9A;
					_FixTopMenuBarBackgroundPosWidth = 0x1BF1C0;
					_ViewportWidth = 0x13C80D;
					_ViewportHeightMult = 0x13C818;
					_ViewportWidthMult = 0x13C81A;
					_SidebarRenderLimitWidth = 0x1B6052;
					_FixSidebarCityMapRotateButton = 0x13AF7A;
					_FixSidebarCityMapRotateIcon = 0x13B06A;
					_FixSidebarCityMapGoalsOverviewWorldMapMessagesIcons = 0x13B3AA;
					_FixSidebarCityMapGoalsOverviewWorldMapMessagesButtons = 0x13B3DA;
					_SidebarLeftEdgeStartWidth = 0x1B605E;
					_UnknownWidth = 0x12B6F3;
					_UnknownHeight = 0x12B6FD;
					_DrawFunction1Address = 0x8170;
					_DrawFunction2Address = 0x13A50;

					_FixBottomBarLengthNewCodeInsertPoint = 0x1BF1EC;
					_FixBottomBarLengthNewCode = defineFixBottomBarLengthNewCode();
					// There is one line (mov ecx, <constant>) that is different for each language.
					// For French, it needs to be: 0x1C43960
					_FixBottomBarLengthNewCode[12] = 0x60;
					_FixBottomBarLengthNewCode[13] = 0x39;
					_FixBottomBarLengthNewCode[14] = 0xC4;
					_FixBottomBarLengthNewCode[15] = 0x01;
					_FixBottomBarLengthFinalJumpDest = 0x1BF288;

					_NewCodeForUiJumpLocation = 0x13B183;
					_NewCodeForUiInsertionLocation = 0x3A9800;
					_NewCodeForUiBytes = defineNewCodeForUiBytes();
					// There are two lines (mov ecx, <constant>) here.
					// Same as before, for French, it needs to be: 0x1C43960
					_NewCodeForUiBytes[22] = 0x60;
					_NewCodeForUiBytes[23] = 0x39;
					_NewCodeForUiBytes[24] = 0xC4;
					_NewCodeForUiBytes[25] = 0x01;
					_NewCodeForUiBytes[118] = 0x60;
					_NewCodeForUiBytes[119] = 0x39;
					_NewCodeForUiBytes[120] = 0xC4;
					_NewCodeForUiBytes[121] = 0x01;
					// Finally, the command overwritten by the jump into new code pushes a constant into the ECX register.
					// This constant is different for each language. For French, the command is: mov ecx, 0x13FA63C
					_NewCodeForUiBytes[186] = 0x3C;
					_NewCodeForUiBytes[187] = 0xA6;
					_NewCodeForUiBytes[188] = 0x3F;
					_NewCodeForUiBytes[189] = 0x01;
					WasSuccessful = true;
					return ;

				case ExeLangAndDistrib.CdItalian:
					_ResWidth = 0x12B6DD;		//0x12B40D	+2D0
					_ResHeight = 0x12B6E2;		//0x12B412	+2D0
					_MainMenuViewportWidth = 0x125E7A;		// 0x125B9A	+2E0
					_MainMenuViewportHeight = 0x125E92;		// 0x125BB2	+2E0
					_FixMoneyPopDateTextPosWidth = 0x1B6FBA;	// 0x1B6E9A	+120
					_FixTopMenuBarBackgroundPosWidth = 0x1BF2D0;	// 0x1BF1C0	+110
					_ViewportWidth = 0x13CADD;		// 0x13C80D	+2D0
					_ViewportHeightMult = 0x13CAE8;	// 0x13C818	+2D0
					_ViewportWidthMult = 0x13CAEA;	// 0x13C81A	+2D0
					_SidebarRenderLimitWidth = 0x1B6172;	// 0x1B6052	+120
					_FixSidebarCityMapRotateButton = 0x13B24A;	// 0x13AF7A	+2D0
					_FixSidebarCityMapRotateIcon = 0x13B33A;	// 0x13B06A	+2D0
					_FixSidebarCityMapGoalsOverviewWorldMapMessagesIcons = 0x13B67A;	// 0x13B3AA	+2D0
					_FixSidebarCityMapGoalsOverviewWorldMapMessagesButtons = 0x13B6AA;	// 0x13B3DA	+2D0
					_SidebarLeftEdgeStartWidth = 0x1B617E;	// 0x1B605E	+120
					_UnknownWidth = 0x12B9C3;	// 0x12B6F3	+2D0
					_UnknownHeight = 0x12B9CD;	// 0x12B6FD
					_DrawFunction1Address = 0x8190;	// 0x8170
					_DrawFunction2Address = 0x139E0;// 0x13A50

					_FixBottomBarLengthNewCodeInsertPoint = 0x1BF2FC;	// 0x1BF1EC	+110
					_FixBottomBarLengthNewCode = defineFixBottomBarLengthNewCode();
					// There is one line (mov ecx, <constant>) that is different for each language.
					// For Italian, it needs to be: 0x1C43920
					_FixBottomBarLengthNewCode[12] = 0x20;
					_FixBottomBarLengthNewCode[13] = 0x39;
					_FixBottomBarLengthNewCode[14] = 0xC4;
					_FixBottomBarLengthNewCode[15] = 0x01;
					_FixBottomBarLengthFinalJumpDest = 0x1BF398;

					_NewCodeForUiJumpLocation = 0x13B453;	// 0x13B183	+2D0
					_NewCodeForUiInsertionLocation = 0x3A96B0;	// 0x3A9800
					_NewCodeForUiBytes = defineNewCodeForUiBytes();
					// There are two lines (mov ecx, <constant>) here.
					// Same as before, for Italian, it needs to be: 0x1C43920
					_NewCodeForUiBytes[22] = 0x20;
					_NewCodeForUiBytes[23] = 0x39;
					_NewCodeForUiBytes[24] = 0xC4;
					_NewCodeForUiBytes[25] = 0x01;
					_NewCodeForUiBytes[118] = 0x20;
					_NewCodeForUiBytes[119] = 0x39;
					_NewCodeForUiBytes[120] = 0xC4;
					_NewCodeForUiBytes[121] = 0x01;
					// Finally, the command overwritten by the jump into new code pushes a constant into the ECX register.
					// This constant is different for each language. For Italian, the command is: mov ecx, 0x13FA5FC
					_NewCodeForUiBytes[186] = 0xFC;
					_NewCodeForUiBytes[187] = 0xA5;
					_NewCodeForUiBytes[188] = 0x3F;
					_NewCodeForUiBytes[189] = 0x01;
					WasSuccessful = true;
					return;

				case ExeLangAndDistrib.CdSpanish:
					_ResWidth = 0x12B79D;//		0x12B40D - forward 0x390
					_ResHeight = 0x12B7A2;//	0x12B412 - forward 0x390
					_MainMenuViewportWidth = 0x125E9A;//	0x125B9A - forward 0x300
					_MainMenuViewportHeight = 0x125EB2;//	0x125BB2 - forward 0x300
					_FixMoneyPopDateTextPosWidth = 0x1B706A;//	0x1B6E9A - forward 0x1D0
					_FixTopMenuBarBackgroundPosWidth = 0x1BF380;//	0x1BF1C0 - forward 0x1C0
					_ViewportWidth = 0x13CB9D;//	0x13C80D - forward 0x390
					_ViewportHeightMult = 0x13CBA8;//	0x13C818 - forward 0x390
					_ViewportWidthMult = 0x13CBAA;//	0x13C81A - forward 0x390
					_SidebarRenderLimitWidth = 0x1B6222;//	0x1B6052 - forward 0x1D0
					_FixSidebarCityMapRotateButton = 0x13B30A;//	0x13AF7A - forward 0x390
					_FixSidebarCityMapRotateIcon = 0x13B3FA;//	0x13B06A - forward 0x390
					_FixSidebarCityMapGoalsOverviewWorldMapMessagesIcons = 0x13B73A;//	0x13B3AA - forward 0x390
					_FixSidebarCityMapGoalsOverviewWorldMapMessagesButtons = 0x13B76A;//	0x13B3DA - forward 0x390
					_SidebarLeftEdgeStartWidth = 0x1B622E;//	0x1B605E - forward 0x1D0
					_UnknownWidth = 0x12BA83;//		0x12B6F3 - forward 0x390
					_UnknownHeight = 0x12BA8D;//		0x12B6FD - forward 0x390
					_DrawFunction1Address = 0x8190;//		0x8170 - forward 0x20
					_DrawFunction2Address = 0x139F0;//		0x13A50 - back 0x60

					_FixBottomBarLengthNewCodeInsertPoint = 0x1BF3AC;//		0x1BF1EC - forward 0x1C0
					_FixBottomBarLengthNewCode = defineFixBottomBarLengthNewCode();
					// There is one line (mov ecx, <constant>) that is different for each language.
					// For Spanish, it needs to be: 0x1C43900
					_FixBottomBarLengthNewCode[12] = 0x00;
					_FixBottomBarLengthNewCode[13] = 0x39;
					_FixBottomBarLengthNewCode[14] = 0xC4;
					_FixBottomBarLengthNewCode[15] = 0x01;
					_FixBottomBarLengthFinalJumpDest = 0x1BF448;//		0x1BF288 - forward 0x1C0

					_NewCodeForUiJumpLocation = 0x13B513;//		0x13B183 - forward 0x390
					_NewCodeForUiInsertionLocation = 0x3A9740;
					_NewCodeForUiBytes = defineNewCodeForUiBytes();
					// There are two lines (mov ecx, <constant>) here.
					// Same as before, for Spanish, it needs to be: 0x1C43900
					_NewCodeForUiBytes[22] = 0x00;
					_NewCodeForUiBytes[23] = 0x39;
					_NewCodeForUiBytes[24] = 0xC4;
					_NewCodeForUiBytes[25] = 0x01;
					_NewCodeForUiBytes[118] = 0x60;
					_NewCodeForUiBytes[119] = 0x39;
					_NewCodeForUiBytes[120] = 0xC4;
					_NewCodeForUiBytes[121] = 0x01;
					// Finally, the command overwritten by the jump into new code pushes a constant into the ECX register.
					// This constant is different for each language. For Spanish, the command is: mov ecx, 0x13FA5DC
					_NewCodeForUiBytes[186] = 0xDC;
					_NewCodeForUiBytes[187] = 0xA5;
					_NewCodeForUiBytes[188] = 0x3F;
					_NewCodeForUiBytes[189] = 0x01;
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

				0xB9, 0x00, 0x00, 0x00, 0x00,		// mov ecx, 0x00	index 12-15	- Constant used for unknown purposes - Set in Constructor
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
				0xB9, 0x00, 0x00, 0x00, 0x00,		// mov ecx, 0x00	index 22-25	- Constant used for unknown purposes - Set in Constructor
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
				0xB9, 0x00, 0x00, 0x00, 0x00,		// mov ecx, 0x00  index 118-121	- Constant used for unknown purposes - Set in Constructor
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
				0x8B, 0x0D, 0x00, 0x00, 0x00, 0x00,	// mov ecx, 0x00  index 186-189 - Run the command that the jump into this code overwrote - Set in Constructor
				0xE9, 0x00, 0x00, 0x00, 0x00		// jmp 0x00		  index 191-194	- Jump back into the original code - Set at runtime
			};
	}
}
