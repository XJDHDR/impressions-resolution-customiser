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
		internal readonly bool _IsDiscVersion;
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
			// "cracked" or pirated versions of Zeus. Nor are you permitted to modify this method or any other method for the purpose of
			// allowing the program to continue the patching process if the "default" case runs or "ExeLangAndDistrib" is set to "NotRecognised".
			// Nor are you permitted to make any other changes that would allow or cause a pirated version of Zeus to be patched.
			switch (gameExeCrc32Checksum)
			{
				// English GOG and Steam versions
				case 0xe5e22ec1:
					_SelectedExeLangAndDistrib = ExeLangAndDistrib.GogAndSteamEnglish;
					_IsDiscVersion = false;
					_IsPoseidonInstalled = true;
					_CharEncoding = CharEncodingTables.Win1252;
					_EngTextDefaultStringCount = 0;
					_EngTextDefaultWordCount = 0;
					WasSuccessful = true;
					return;

				// Unrecognised EXE
				default:
					string[] messageLines = {
						"Zeus.exe was not recognised.",
						"",
						"Only the following unmodified distributions and languages are currently supported:",
						"- English GOG version with Poseidon expansion",
						"- English Steam version with Poseidon expansion",
						"",
						"If you are using one of the listed versions, please ensure that the EXE has not been modified.",
						"If you are not, please do request that support be added, especially if you can provide info on how I can get a copy of your version."
					};
					MessageBox.Show(string.Join(Environment.NewLine, messageLines));
					_SelectedExeLangAndDistrib = ExeLangAndDistrib.NotRecognised;
					_IsDiscVersion = false;
					_IsPoseidonInstalled = false;
					_CharEncoding = CharEncodingTables.Win1252;
					_EngTextDefaultStringCount = 0;
					_EngTextDefaultWordCount = 0;
					WasSuccessful = false;
					return;
			}
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
