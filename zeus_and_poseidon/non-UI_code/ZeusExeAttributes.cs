// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using System;
using System.Windows;
using ImpressionsFileFormats.EngText;
using Zeus_and_Poseidon.non_UI_code.Crc32;

namespace Zeus_and_Poseidon.non_UI_code
{
	/// <summary>
	/// Struct that specifies various details about the Zeus.exe given to the program.
	/// </summary>
	internal readonly struct ZeusExeAttributes
	{
		internal readonly ExeLangAndDistrib _SelectedExeLangAndDistrib;
		internal readonly bool _IsPoseidonInstalled;
		internal readonly CharEncodingTables _CharEncoding;
		internal readonly int _EngTextDefaultStringCount;
		internal readonly int _EngTextDefaultWordCount;

		/// <summary>
		/// Compares the CRC for the game's EXE to a list of known CRCs to determine which distribution of this game is being patched.
		/// </summary>
		/// <param name="ZeusExeData">Byte array that contains the binary data contained within the supplied Zeus.exe</param>
		/// <param name="WasSuccessful">
		///		True if the CRC for the selected Zeus.exe matches one that this program knows about and knows the offsets that need to be patched.
		///		False if the EXE is not recognised.
		/// </param>
		internal ZeusExeAttributes(byte[] ZeusExeData, out bool WasSuccessful)
		{
			// First, create a CRC32 Checksum of the EXE's data, excluding the first 4096 bytes.
			uint gameExeCrc32Checksum = SliceBy16.Crc32(0x1000, ZeusExeData);

			// Please note that as per the software license, you are not permitted to modify this code to add the CRC for any
			// "cracked" or pirated versions of Zeus to the code paths that successfully patch the EXE.
			// Nor are you permitted to modify this method or any other method for the purpose of allowing the program to continue
			// the patching process if the "default" case runs or "ExeLangAndDistrib" is set to "NotRecognised".
			// Nor are you permitted to make any other changes that would allow or cause a pirated version of Zeus to be patched.
			switch (gameExeCrc32Checksum)
			{
				// Exes known to be modified
				case 0x058deed7:	// Zeus only		- v1.1.0.0 - German
				case 0xe3469478:	// Zeus & Poseidon	- v2.1.4.0 - Polish
					MessageBox.Show(string.Join(Environment.NewLine, StringsDatabase._ZeusExeAttributesExeKnownModified),
						StringsDatabase._ZeusExeAttributesExeKnownModifiedMessageTitle);
					break;

				// Exes known to be both outdated and modified
				case 0x03901894:	// Zeus & Poseidon	- v2.0.0.2 - German
				case 0x08d8f36c:	// Zeus & Poseidon	- v2.0.0.2 - Spanish
				case 0x2bdf84ad:	// Zeus only		- v1.0.1.0 - English US
				case 0xc897c56b:	// Zeus only		- v1.0.0.0 - English US
					MessageBox.Show(string.Join(Environment.NewLine, StringsDatabase._ZeusExeAttributesExeKnownModifiedAndOutdated),
						StringsDatabase._ZeusExeAttributesExeKnownModifiedAndOutdatedMessageTitle);
					break;

				// Exes known to be outdated
				case 0x00000000:	// Zeus only ??		- v1.0.0.0 - ??
					MessageBox.Show(string.Join(Environment.NewLine, StringsDatabase._ZeusExeAttributesExeKnownOutdated),
						StringsDatabase._ZeusExeAttributesExeKnownOutdatedMessageTitle);
					break;

				default:	// Unrecognised EXE
					MessageBox.Show(string.Join(Environment.NewLine, StringsDatabase._ZeusExeAttributesExeNotRecognised),
						StringsDatabase._ZeusExeAttributesExeNotRecognisedMessageTitle);
					break;


				case 0xe5e22ec1:	// English GOG and Steam versions
					_SelectedExeLangAndDistrib = ExeLangAndDistrib.GogAndSteamEnglish;
					_IsPoseidonInstalled = true;
					_CharEncoding = CharEncodingTables.Win1252;
					_EngTextDefaultStringCount = 8248;
					_EngTextDefaultWordCount = 42379;
					WasSuccessful = true;
					return;
			}
			_SelectedExeLangAndDistrib = ExeLangAndDistrib.NotRecognised;
			_IsPoseidonInstalled = false;
			_CharEncoding = CharEncodingTables.Win1252;
			_EngTextDefaultStringCount = 0;
			_EngTextDefaultWordCount = 0;
			WasSuccessful = false;
		}
	}

	/// <summary>
	/// Used to define the various versions of Zeus.exe that this program recognises and knows what offsets to patch.
	/// </summary>
	internal enum ExeLangAndDistrib
	{
		NotRecognised = 0,
		GogAndSteamEnglish = 1
	}
}
