// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using System;
using System.Windows;
using Emperor.non_UI_code.Crc32;
using ImpressionsFileFormats.EngText;

namespace Emperor.non_UI_code
{
	/// <summary>
	/// Struct that specifies various details about the Emperor.exe given to the program.
	/// </summary>
	internal readonly struct EmperorExeAttributes
	{
		internal readonly ExeLangAndDistrib _SelectedExeLangAndDistrib;
		internal readonly CharEncodingTables _CharEncoding;

		/// <summary>
		/// Compares the CRC for the game's EXE to a list of known CRCs to determine which distribution of this game is being patched.
		/// </summary>
		/// <param name="EmperorExeData">Byte array that holds the binary data contained within the supplied Emperor.exe</param>
		/// <param name="WasSuccessful">
		///		True if the CRC for the selected Emperor.exe matches one that this program knows about and knows the offsets that need to be patched.
		///		False if the EXE is not recognised.
		/// </param>
		internal EmperorExeAttributes(byte[] EmperorExeData, out bool WasSuccessful)
		{
			// First, create a CRC32 Checksum of the EXE's data, excluding the first 4096 bytes.
			uint gameExeCrc32Checksum = SliceBy16.Crc32(0x1000, EmperorExeData);

			// Please note that as per the software license, you are not permitted to modify this code to add the CRC for any
			// "cracked" or pirated versions of Emperor to the code paths that successfully patch the EXE.
			// Nor are you permitted to modify this method or any other method for the purpose of allowing the program to continue
			// the patching process if the "default" case runs or "ExeLangAndDistrib" is set to "NotRecognised".
			// Nor are you permitted to make any other changes that would allow or cause a pirated version of Emperor to be patched.
			switch (gameExeCrc32Checksum)
			{
				// Exes known to be modified
				case 0x26791e30:	// v1.0.1.0 - German & Spanish CD		0x39343561 - EmperorEdit
				case 0xcf46e4b6:	// v1.0.1.0 - English CD				0xbb21655b - EmperorEdit
				case 0xfb4b0a0c:	// v1.0.1.0 - French CD					0x5bbe23db - EmperorEdit
					MessageBox.Show(string.Join(Environment.NewLine, StringsDatabase._EmperorExeAttributesExeKnownModified),
						StringsDatabase._EmperorExeAttributesExeKnownModifiedMessageTitle);
					_SelectedExeLangAndDistrib = ExeLangAndDistrib.NotRecognised;
					_CharEncoding = CharEncodingTables.Win1252;
					WasSuccessful = false;
					break;

				// Exes known to be both outdated and modified
				case 0x2c2d13b9:	// v1.0.0.0 - English CD				0xa6b8e461 - EmperorEdit
				case 0x62effc86:	// v1.0.0.0 - English CD				0x68ccef0b - EmperorEdit
				case 0x90ce1021:	// v1.0.0.0 - English or Italian CD		0x1587ddc8 - EmperorEdit
				case 0xa666e0b2:	// v1.0.0.0 - Spanish CD				0x99fdb392 - EmperorEdit
					MessageBox.Show(string.Join(Environment.NewLine, StringsDatabase._EmperorExeAttributesExeKnownModifiedAndOutdated),
						StringsDatabase._EmperorExeAttributesExeKnownModifiedAndOutdatedMessageTitle);
					_SelectedExeLangAndDistrib = ExeLangAndDistrib.NotRecognised;
					_CharEncoding = CharEncodingTables.Win1252;
					WasSuccessful = false;
					break;

				// Exes known to be outdated
				case 0x15f426a8:	// v1.0.0.0 - English CD	0xf6ff4c3a - EmperorEdit
				case 0x4ca5afc6:	// v1.0.0.0 - French CD		0xba901a82 - EmperorEdit
				case 0xa2539999:	// v1.0.0.0 - Spanish CD	0xba901a82 - EmperorEdit
				case 0xafa41a01:	// v1.0.0.0 - Italian CD	0xba901a82 - EmperorEdit
					MessageBox.Show(string.Join(Environment.NewLine, StringsDatabase._EmperorExeAttributesExeKnownOutdated),
						StringsDatabase._EmperorExeAttributesExeKnownOutdatedMessageTitle);
					_SelectedExeLangAndDistrib = ExeLangAndDistrib.NotRecognised;
					_CharEncoding = CharEncodingTables.Win1252;
					WasSuccessful = false;
					break;

				// Unrecognised EXE
				default:
					MessageBox.Show(string.Join(Environment.NewLine, StringsDatabase._EmperorExeAttributesExeNotRecognised),
						StringsDatabase._EmperorExeAttributesExeNotRecognisedMessageTitle);
					_SelectedExeLangAndDistrib = ExeLangAndDistrib.NotRecognised;
					_CharEncoding = CharEncodingTables.Win1252;
					WasSuccessful = false;
					return;


				// English GOG version
				case 0x8bc98c83:	// 0x9430833c - EmperorEdit
					_SelectedExeLangAndDistrib = ExeLangAndDistrib.GogEnglish;
					_CharEncoding = CharEncodingTables.Win1252;
					WasSuccessful = true;
					return;

				// English CD version with 1.1 patch
				case 0x71af4e0e:	// 0xe814ff39 - EmperorEdit
					_SelectedExeLangAndDistrib = ExeLangAndDistrib.CdEnglish;
					_CharEncoding = CharEncodingTables.Win1252;
					WasSuccessful = true;
					return;

				// French CD version with 1.1 patch
				case 0xdbaf4fad:	// 0xa7ee624a - EmperorEdit
					_SelectedExeLangAndDistrib = ExeLangAndDistrib.CdFrench;
					_CharEncoding = CharEncodingTables.Win1252;
					WasSuccessful = true;
					return;

				// Italian CD version with 1.1 patch
				case 0x725e219a:	// 0xf6df98e1 - EmperorEdit
					_SelectedExeLangAndDistrib = ExeLangAndDistrib.CdItalian;
					_CharEncoding = CharEncodingTables.Win1252;
					WasSuccessful = true;
					break;

				// Spanish CD version with 1.1 patch
				case 0x76d2185f:	// 0xa42e2399 - EmperorEdit
					_SelectedExeLangAndDistrib = ExeLangAndDistrib.CdSpanish;
					_CharEncoding = CharEncodingTables.Win1252;
					WasSuccessful = true;
					return;

			}
		}
	}

	/// <summary>
	/// Used to define the various versions of Emperor.exe that this program recognises and knows what offsets to patch.
	/// </summary>
	internal enum ExeLangAndDistrib
	{
		NotRecognised = 0,
		GogEnglish = 1,
		CdEnglish = 3,
		CdFrench = 4,
		CdItalian = 5,
		CdSpanish = 6
	}
}
