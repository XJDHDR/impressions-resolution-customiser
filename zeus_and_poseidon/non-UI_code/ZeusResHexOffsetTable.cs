// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

namespace Zeus_and_Poseidon.non_UI_code
{
	/// <summary>
	/// Struct which describes the offsets that need to be patched to change the game's resolution.
	/// </summary>
	internal readonly struct ZeusResHexOffsetTable
	{
		internal readonly int _ResWidth;
		internal readonly int _ResHeight;
		internal readonly int _MainMenuViewportWidth;
		internal readonly int _MainMenuViewportHeight;

		internal readonly int _NameListActivationZoneLeftEdgePos;
		internal readonly int _NameListActivationZoneTopEdgePos;
		internal readonly int _NameListUiBackgroundLeftEdgePos;
		internal readonly int _NameListUiBackgroundTopEdgePos;

		internal readonly int _FixMoneyPopDateTextPosWidth;
		internal readonly int _FixTopMenuBarBackgroundPosWidth;

		internal readonly int _ViewportWidth;
		internal readonly int _ViewportHeightMult;
		internal readonly int _ViewportWidthMult;

		internal readonly int _FixCompBottomBlackBarWidth;
		internal readonly int _FixPushBottomBlackBarWidth;
		internal readonly int _SidebarRenderLimitWidth;
		internal readonly int _SidebarLeftEdgeStartWidth;

		internal readonly int _UnknownWidth;
		internal readonly int _UnknownHeight;
		internal readonly int _UnknownWidth2;

		internal readonly int _ExtendSidebarRedStripeNewCodeJumpOffset;
		internal readonly byte[] _ExtendSidebarRedStripeNewCodeJumpData;

		internal readonly int _PaintBlueBackgroundInGapsNewCodeJumpOffset;
		internal readonly byte[] _PaintBlueBackgroundInGapsNewCodeJumpData;

		internal readonly int _ExtendSidebarRedStripeNewCodeOffset;
		internal readonly byte[] _ExtendSidebarRedStripeNewCodeData;

		internal readonly int _PaintBlueBackgroundInGapsNewCodeOffset;
		internal readonly byte[] _PaintBlueBackgroundInGapsNewCodeData;

		/// <summary>
		/// Supplies a ResHexOffsetTable struct specifying the offsets that need to be patched, based on which version of Zeus.exe was supplied.
		/// </summary>
		/// <param name="ExeAttributes">Struct that specifies various details about the detected Zeus.exe</param>
		/// <param name="WasSuccessful">
		///		True if "_exeLangAndDistrib" matches one that this program knows about and knows the offsets that need to be patched.
		///		False if the EXE is not recognised.
		/// </param>
		internal ZeusResHexOffsetTable(ZeusExeAttributes ExeAttributes, out bool WasSuccessful)
		{
			switch (ExeAttributes._SelectedExeLangAndDistrib)
			{
				case ExeLangAndDistrib.GogAndSteamEnglish:         // English GOG version
					_ResWidth = 0x10BA29;
					_ResHeight = 0x10BA2E;
					_MainMenuViewportWidth = 0x1051FA;
					_MainMenuViewportHeight = 0x105212;

					_NameListActivationZoneLeftEdgePos = 0x114634;
					_NameListActivationZoneTopEdgePos = 0x114639;
					_NameListUiBackgroundLeftEdgePos = 0x19038C;
					_NameListUiBackgroundTopEdgePos = 0x190391;

					_FixMoneyPopDateTextPosWidth = 0x18EF7C;
					_FixTopMenuBarBackgroundPosWidth = 0x19EBFB;

					_ViewportWidth = 0x11CC1E;
					_ViewportHeightMult = 0x11CC29;
					_ViewportWidthMult = 0x11CC2B;

					_FixCompBottomBlackBarWidth = 0x18E2F7;
					_FixPushBottomBlackBarWidth = 0x18E30A;
					_SidebarRenderLimitWidth = 0x18E2BE;
					_SidebarLeftEdgeStartWidth = 0x18E2CA;

					_UnknownWidth = 0x10BD03;
					_UnknownHeight = 0x10BD0D;
					_UnknownWidth2 = 0x1C459E;

					_ExtendSidebarRedStripeNewCodeJumpOffset = 0x117B69;
					_ExtendSidebarRedStripeNewCodeJumpData = new byte[] {0xE9, 0x32, 0x04, 0x0C, 0x00, 0x90};

					_PaintBlueBackgroundInGapsNewCodeJumpOffset = 0x19EC31;
					_PaintBlueBackgroundInGapsNewCodeJumpData = new byte[] {0xE9, 0x0A, 0x92, 0x03, 0x00};

					_ExtendSidebarRedStripeNewCodeOffset = 0x1D7FA0;
					_ExtendSidebarRedStripeNewCodeData = fillExtendSidebarRedStripeNewCodeData();

					_PaintBlueBackgroundInGapsNewCodeOffset = 0x1D7E40;
					_PaintBlueBackgroundInGapsNewCodeData = fillPaintBlueBackgroundInGapsNewCodeData();
					WasSuccessful = true;
					return;

				/*
					Offsets for CD version??
					_NameList1 = 0x113FD4,
					_NameList2 = 0x113FD9,
					_NameList3 = 0x18FBEC,
					_NameList4 = 0x18FBF1,
				*/

				case ExeLangAndDistrib.NotRecognised:
				default:        // Unrecognised EXE
					_ResWidth = 0x10BA29;
					_ResHeight = 0;
					_MainMenuViewportWidth = 0;
					_MainMenuViewportHeight = 0;
					_NameListActivationZoneLeftEdgePos = 0;
					_NameListActivationZoneTopEdgePos = 0;
					_NameListUiBackgroundLeftEdgePos = 0;
					_NameListUiBackgroundTopEdgePos = 0;
					_FixMoneyPopDateTextPosWidth = 0;
					_FixTopMenuBarBackgroundPosWidth = 0;
					_ViewportWidth = 0;
					_ViewportHeightMult = 0;
					_ViewportWidthMult = 0;
					_FixCompBottomBlackBarWidth = 0;
					_FixPushBottomBlackBarWidth = 0;
					_SidebarRenderLimitWidth = 0;
					_SidebarLeftEdgeStartWidth = 0;
					_UnknownWidth = 0;
					_UnknownHeight = 0;
					_UnknownWidth2 = 0;
					_ExtendSidebarRedStripeNewCodeJumpOffset = 0;
					_ExtendSidebarRedStripeNewCodeJumpData = null;
					_PaintBlueBackgroundInGapsNewCodeJumpOffset = 0;
					_PaintBlueBackgroundInGapsNewCodeJumpData = null;
					_ExtendSidebarRedStripeNewCodeOffset = 0;
					_ExtendSidebarRedStripeNewCodeData = null;
					_PaintBlueBackgroundInGapsNewCodeOffset = 0;
					_PaintBlueBackgroundInGapsNewCodeData = null;
					WasSuccessful = false;
					return;
			}
		}

		private static byte[] fillExtendSidebarRedStripeNewCodeData() =>
			new byte[]
			{
				// ReSharper disable CommentTypo

				0xBB, 0x00, 0x00, 0x00,
				0x00, // mov ebx, 0			- Initial position of red stripe placer. Set by script to <city view width - stripe height>

				// Code required to run the "draw UI element" subroutine
				0x8B, 0x0D, 0x34, 0xFF, 0x0D, 0x01, // mov ecx, dw_10DFF34
				0x6A, 0x00, // push 0
				0x6A, 0x00,
				0x6A, 0x00,
				0x53, // push ebx				- Sets the current red stripe graphic's top right corner's Y position.
				0x51, // push ecx				- Sets the current red stripe graphic's top right corner's X position.
				0x50, // push eax				- I think this param sets which UI element the subroutine will draw.
				0xB9, 0xE8, 0xEB, 0x2A, 0x01, // mov ecx, offset unk_12AEBE8
				0xA3, 0xF4, 0xDF, 0x60, 0x01, // mov dw_160DFF4, eax
				0xE8, 0x8D, 0x4F, 0xFD, 0xFF, // call sub_1ACF50
				0xA1, 0xF4, 0xDF, 0x60, 0x01, // mov eax, dw_160DFF4

				// Check if the gap has been filled. Create and position another red stripe if not.
				0x81, 0xFB, 0xE8, 0x02, 0x00,
				0x00, // cmp ebx, 0x2E8 (744)	- Check if the new code has filled the area below the final stripe graphic
				0x72,
				0x08, // jb short loc_5D7FD8	- Jump forward by 8 bytes (xor ebx, ebx) if cmp set the Carry Flag
				0x81, 0xEB, 0xE8, 0x02, 0x00,
				0x00, // sub ebx, 0x2E8 (744)	- Reduce the counter by the red stripe's length (744px)
				0xEB,
				0xCD, // jmp short loc_5D7FA5	- Jump back by 51 bytes (first "mov ecx, 0x10DFF34") and build another red stripe

				// Prepare for jump back into the game's original code.
				0x33, 0xDB, // xor ebx, ebx			- Counter has done it's job so zero it
				0x8B, 0x0D, 0x34, 0xFF, 0x0D,
				0x01, // mov ecx, 0x10DFF34	- Almost done. Run this to replace the instruction our injected jump overwrote.
				0xE9, 0x8A, 0xFB, 0xF3,
				0xFF // jmp loc_117B6F		- Jump back to the byte after our jump into this injected code as we are done here.

				// ReSharper restore CommentTypo
			};

		private static byte[] fillPaintBlueBackgroundInGapsNewCodeData() =>
			new byte[]
			{
				// ReSharper disable CommentTypo

				// This block of code is injected into the EXE in a blank space. It paints a blue background over every area of the in-game UI that
				// has a gap in it due to moving and resizing the game's elements to fit the new resolution. It does this by creating copies of
				// the graphic that is used as the background for the top menubar and places those copies in such a way that they cover up the gaps.
				//
				// Throughout this section there are comments that say "Ind # <??>". These indicate that the byte immediately after that comment will
				// have the indicated index number in this byte array.
				//
				// Run the subroutine call that we overwrote in the original code.
				0xE8, 0x0B, 0x51, 0xFD,
				0xFF, // call sub_1ACF50		- sub_1ACF50 is the subroutine that draws graphics at a screen coordinate.

				// Ind # 005	// Set up the variables for creating the first middle piece for the menubar then jump to variable testing code.
				0x33, 0xDB, // xor ebx, ebx			- Set Y coordinate to 0.
				0xB8, 0x46, 0x03, 0x00, 0x00, // mov eax, 0x346		- Set X coordinate to 838.
				0xEB,
				0x27, // jmp short rel 0x27	- Jump forward by 39 bytes to POS#TEST-START (cmp ebx, <win height>) for variable tests.

				// Run the subroutine that creates the blue bar graphic to create another one
				// Ind # 014	// POS#DRAW
				0x6A, 0x00, // push 0
				0x6A, 0x00,
				0x6A, 0x00,
				0x53, // push ebx				- Sets the Y position of current bar graphic's top edge.
				0xA3, 0xF4, 0xDF, 0x60,
				0x01, // mov dw_160DFF4, eax	- The subroutine likes to modify the contents of the EAX register so make a backup.
				0x50, // push eax				- Sets the X position of current bar graphic's left edge.
				0xB8, 0x06, 0x44, 0x00,
				0x00, // mov eax, 0x4406		- Set EAX equal to 0x4406, which is the number for the top menubar.
				0x50, // push eax				- Set which UI element the subroutine will draw.
				0xB9, 0xE8, 0xEB, 0x2A,
				0x01, // mov ecx, offset 0x12AEBE8	- Not sure what this does but game crashes if this isn't here.
				0xE8, 0xE5, 0x50, 0xFD,
				0xFF, // call sub_1ACF50		- Call the subroutine that draws the top bar graphic at screen coordinates.
				0xA1, 0xF4, 0xDF, 0x60,
				0x01, // mov eax, dw_160DFF4	- Restore the backed up value into the register.

				// Ind # 048	// Move the Y coordinate across to the next column.
				0x05, 0x46, 0x03, 0x00, 0x00, // add eax, 0x346		- Add 838 to horizontal variable.

				// Start of the code that checks what state the drawing code is at and modifies the variables to place the next bar graphic in the correct place.
				//
				// This part checks if the drawing coordinates are past the bottom of the game window. Keep testing if not and break loop if so.
				// Ind # 053	// POS#TEST-START
				0x81, 0xFB, 0x00, 0x00, 0x00,
				0x00, // cmp ebx, 0x0			- Compare Y var to (window height) - set by ResolutionEdits class
				0x0F, 0x83, 0xC3, 0x00, 0x00,
				0x00, // jae near rel 0xC3	- Jump forward 195 bytes to POS#CLEAN (xor ebx, ebx) to cleanup code if more than height.

				// This part checks if the drawing point is in the empty area between the bottom of the city viewport and the bottom of the game's window.
				// Ind # 065	// This test only fires is such a region exists. If it doesn't, the window bottom edge test will fire first.
				0x81, 0xFB, 0x00, 0x00, 0x00,
				0x00, // cmp ebx, 0x0			- Compare Y var to (viewport + menubar height) - set by ResolutionEdits class
				0x73, 0x57, // jae short rel 0x57	- Jump forward 87 bytes to POS#1 (cmp eax, <window width>).

				// This part tests if the drawing point is in the last row just before the bottom of the city viewport.
				// Ind # 073	// This test ensures a smooth transition between the right gap filling code and the bottom filling section.
				0x81, 0xFB, 0x00, 0x00, 0x00,
				0x00, // cmp ebx, 0x0			- Compare Y variable to (viewport + menubar height - 0x10) - set by ResolutionEdits class
				0x73, 0x67, // jae short rel 0x67	- Jump forward 103 bytes to POS#2 (cmp eax, <window width>).

				// This part checks if the drawing point is in the empty area between the bottom of the sidebar and bottom of the city viewport.
				// Ind # 081	// This test only fires is such a region exists. If it doesn't, the above three tests will take precedence.
				0x81, 0xFB, 0x00, 0x03, 0x00,
				0x00, // cmp ebx, 0x300		- Compare Y variable to height of sidebar (768px)
				0x0F, 0x83, 0x75, 0x00, 0x00,
				0x00, // jae near rel 0x75	- Jump forward 117 bytes to POS#3 (cmp eax, <window width>).

				// This part tests if the drawing point is in the last row just before the bottom of the sidebar.
				// Ind # 093	// This test only fires is such a region exists. If it doesn't, the above three tests will take precedence.
				0x81, 0xFB, 0xF0, 0x02, 0x00,
				0x00, // cmp ebx, 0x2F0		- Compare Y var to (sidebar height - 0x10 = 752)
				0x0F, 0x83, 0x81, 0x00, 0x00,
				0x00, // jae near rel 0x81	- Jump forward 129 bytes to POS#4 (cmp eax, <window width>).


				// POS#10
				// Ind # 105	// Since all the above tests failed, that means the coordinates are either at the top of the screen or in the empty area right of the sidebar.
				0x3D, 0x00, 0x00, 0x00,
				0x00, // cmp eax, 0x0			- Compare X var to (window width) - set by ResolutionEdits class
				0x72, 0x0A, // jb short rel 0x0A	- Jump forward 10 bytes to next test if less than window width.
				// Ind # 112	// Since this point is past the screen's width, move coordinates to the first cell of the next row.
				0xB8, 0x00, 0x00, 0x00,
				0x00, // mov eax, 0x0			- Set X coordinate to <city + sidebar width> - set by ResolutionEdits class
				0x83, 0xC3, 0x10, // add ebx, 0x10		- Add 16 to Y coordinate to move to next drawing row.
				0xEB,
				0xBB, // jmp short rel 0xBB	- Jump backwards by 69 bytes to POS#TEST-START (cmp ebx, <Win height>) for variable tests.

				// This section checks if the coordinates are in the empty space right of the sidebar. If so, draw a new graphic. If not, move on to next test.
				// Ind # 122	// This only runs if there is such an empty space. If not, the above test will run first, incrementing the coordinates downwards without drawing anything.
				0x3D, 0x00, 0x00, 0x00,
				0x00, // cmp eax, 0x0			- Compare X var to (city view + sidebar width) - set by ResolutionEdits class
				0x73,
				0x8D, // jae short rel 0x8D	- Jump backwards 115 bytes to POS#DRAW if more than city + sidebar width.

				// Ind # 129	// This section checks if the coordinates are inside the sidebar. This means that the menubar is finished. If so, move the coordinates right of the sidebar.
				0x3D, 0x00, 0x00, 0x00,
				0x00, // cmp eax, 0x0			- Compare X var to (city view width) - set by ResolutionEdits class
				0x72,
				0x07, // jb short rel 0x7		- Jump forward 7 bytes to next test if less than city view width.
				// Ind # 136	// Since this point is past the city view's width, move coordinates to past the sidebar's right edge.
				0xB8, 0x00, 0x00, 0x00,
				0x00, // mov eax, 0x0			- Set X coordinate to <city + sidebar width> - set by ResolutionEdits class
				0xEB, 0xDA, // jmp short rel 0xDA	- Jump backwards 38 bytes to POS#10 (cmp eax, <window width>).

				// At this point, this means that the code is constructing the top menubar. If so, compare X against the position of the last piece of the bar.
				// Ind # 143	// If the number is greater, this means that the final segment of the bar is ready to be drawn after the X coordinate is corrected.
				0x3D, 0x00, 0x00, 0x00,
				0x00, // cmp eax, 0x0			- Compare X var to (city view width - 838) - set by ResolutionEdits class
				0x76,
				0x05, // jbe short rel 0x05	- Jump forward by 5 bytes to POS#DRAW jump and draw a middle or final segment of bar.
				0xB8, 0x00, 0x00, 0x00,
				0x00, // mov eax, 0x0			- Set X coordinate to <city view width - 838> - set by ResolutionEdits class
				0xE9, 0x6E, 0xFF, 0xFF,
				0xFF, // jmp near rel 0xFF6E	- Jump backwards by 146 bytes to POS#DRAW (cmp ebx, <Win height>) for variable tests.


				// POS#1
				// Ind # 160	// This code runs if the coordinates are inside the empty area between the bottom of the city viewport and the bottom of the screen.
				0x3D, 0x00, 0x00, 0x00,
				0x00, // cmp eax, 0x0			- Compare X var to (window width) - set by ResolutionEdits class
				0x0F, 0x82, 0x63, 0xFF, 0xFF,
				0xFF, // jb near rel 0xFF..63	- Jump backwards 157 bytes to POS#DRAW if less than window width.
				// Ind # 171	// Since this point is past the screen's width, move coordinates to the first cell of the next row.
				0xB8, 0x00, 0x00, 0x00, 0x00, // mov eax, 0x0			- Set X coordinate to 0.
				0x83, 0xC3, 0x10, // add ebx, 0x10		- Add 16 to Y coordinate to move to next drawing row.
				0xE9, 0x7D, 0xFF, 0xFF,
				0xFF, // jmp near rel 0xFF7D	- Jump backwards by 131 bytes to POS#TEST-START (cmp ebx, <Win height>) for variable tests.


				// POS#2
				// Ind # 184	// This code runs if the coordinates are in the row immediately before the empty area between the bottom of the city viewport and the bottom of the screen.
				0x3D, 0x00, 0x00, 0x00,
				0x00, // cmp eax, 0x0			- Compare X var to (window width) - set by ResolutionEdits class
				0x0F, 0x82, 0x4B, 0xFF, 0xFF,
				0xFF, // jb near rel 0xFF..4B	- Jump backwards 181 bytes to POS#DRAW if less than window width.
				// Ind # 195	// Since this point is past the screen's width, move coordinates to the first cell of the next row.
				0xB8, 0x00, 0x00, 0x00, 0x00, // mov eax, 0x0			- Set X coordinate to 0.
				0xBB, 0x00, 0x00, 0x00,
				0x00, // mov ebx, 0x0			- Set Y coordinate to <city view height> - set by ResolutionEdits class
				0xE9, 0x63, 0xFF, 0xFF,
				0xFF, // jmp near rel 0xFF63	- Jump backwards by 157 bytes to POS#TEST-START (cmp ebx, <Win height>) for variable tests.


				// POS#3
				// Ind # 210	// This code runs if the coordinates are inside the empty area under the sidebar but before the bottom of the city viewport.
				0x3D, 0x00, 0x00, 0x00,
				0x00, // cmp eax, 0x0			- Compare X var to (window width) - set by ResolutionEdits class
				0x0F, 0x82, 0x31, 0xFF, 0xFF,
				0xFF, // jb near rel 0xFF..31	- Jump backwards 207 bytes to POS#DRAW if less than window width.
				// Ind # 221	// Since this point is past the screen's width, move coordinates to the first cell of the next row.
				0xB8, 0x00, 0x00, 0x00,
				0x00, // mov eax, 0x0			- Set X coordinate to <city view + stripe widths> - set by ResolutionEdits class
				0x83, 0xC3, 0x10, // add ebx, 0x10		- Add 16 to Y coordinate to move to next drawing row.
				0xE9, 0x4B, 0xFF, 0xFF,
				0xFF, // jmp near rel 0xFF4B	- Jump backwards by 181 bytes to POS#TEST-START (cmp ebx, <Win height>) for variable tests.


				// POS#4
				// Ind # 234	// This code runs if the coordinates are in the row immediately before the empty area under the sidebar.
				0x3D, 0x00, 0x00, 0x00,
				0x00, // cmp eax, 0x0			- Compare X var to (window width) - set by ResolutionEdits class
				0x0F, 0x82, 0x19, 0xFF, 0xFF,
				0xFF, // jb near rel 0xFF..19	- Jump backwards 231 bytes to POS#DRAW if less than window width.
				// Ind # 245	// Since this point is past the screen's width, move coordinates to the first cell of the next row.
				0xB8, 0x00, 0x00, 0x00,
				0x00, // mov eax, 0x0			- Set X coordinate to <city view + stripe widths> - set by ResolutionEdits class
				0xBB, 0x00, 0x03, 0x00,
				0x00, // mov ebx, 0x300		- Set Y coordinate to 0x300 (768) - the height of the sidebar
				0xE9, 0x31, 0xFF, 0xFF,
				0xFF, // jmp near rel 0xFF31	- Jump backwards by 207 bytes to POS#TEST-START (cmp ebx, <Win height>) for variable tests.


				// Finished creating bars to fill the empty areas so cleanup and jump back into original game's code.
				// Ind # 260	// POS#CLEAN
				0x33, 0xDB, // xor ebx, ebx			- Set register back to 0.
				0xB8, 0x06, 0x44, 0x00,
				0x00, // mov eax, 0x4406		- Set register back to it's original value before running this block of code.
				0xE9, 0xE6, 0x6C, 0xFC,
				0xFF // jmp near rel 0xFFFC6CE6	- Jump back to the byte after our jump into this injected code as we are done here.

				// ReSharper restore CommentTypo
			};
	}
}
