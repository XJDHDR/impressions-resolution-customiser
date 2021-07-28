// This code is part of the Impressions Resolution Customiser project
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using Soft160.Data.Cryptography;
using System;
using System.IO;
using System.Windows;

namespace zeus_and_poseidon
{
	/// <summary>
	/// Base class used to interact with the code patching classes, figure out what version of the game is being patched and prepare the environment before patching.
	/// </summary>
	class Zeus_MakeChanges
	{
		internal static void ProcessZeusExe(string ZeusExeLocation, ushort ResWidth, ushort ResHeight, bool FixAnimations, bool FixWindowed, bool ResizeImages)
		{
			if (!File.Exists(ZeusExeLocation))
			{
				// User didn't select a folder using the selection button.
				if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"base_files\Zeus.exe"))
				{
					// Check if the user has placed the Zeus data files in the "base_files" folder.
					ZeusExeLocation = AppDomain.CurrentDomain.BaseDirectory + @"base_files\Zeus.exe";
				}
				else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Zeus.exe"))
				{
					// As a last resort, check if the Zeus data files are in the same folder as this program.
					ZeusExeLocation = AppDomain.CurrentDomain.BaseDirectory + "Zeus.exe";
				}
				else
				{
					MessageBox.Show("Zeus.exe does not exist in either the selected location or either of the automatically scanned locations. " +
						"Please ensure that you have either selected the correct place, placed this program in the correct place or " +
						"placed the correct files in the \"base_files\" folder.");
					return;
				}
			}

			string _patchedFilesFolder = AppDomain.CurrentDomain.BaseDirectory + "patched_files";
			try
			{
				if (Directory.Exists(_patchedFilesFolder))
				{
					Directory.Delete(_patchedFilesFolder, true);
				}
				Directory.CreateDirectory(_patchedFilesFolder);
			}
			catch (PathTooLongException)
			{
				MessageBox.Show("A PathTooLong Exception occurred while trying to work on the \"patched_files\" folder next to this program's EXE. " +
					"Please exit and move the program somewhere with a shorter path length (the Downloads folder is a good choice).");
				return;
			}
			catch (IOException)
			{
				MessageBox.Show("An IO Exception occurred while trying to work on the \"patched_files\" folder next to this program's EXE. " +
					"Please close any other programs using that folder, make sure the folder and it's contents are not marked Read-only and/or " +
					"manually delete any files or folders named \"patched_files\".");
				return;
			}
			catch (UnauthorizedAccessException)
			{
				MessageBox.Show("This program must be run from a location that it is allowed to write files to. " +
					"Please exit and move the program somewhere that you have write permissions available (the Downloads folder is a good choice).");
				return;
			}

			if (_getAndCheckExeChecksum(ZeusExeLocation, out byte[] _zeusExeData, out ExeLangAndDistrib _exeLangAndDistrib))
			{
				Zeus_ResolutionEdits._hexEditExeResVals(ResWidth, ResHeight, _exeLangAndDistrib, ref _zeusExeData);

				if (FixAnimations)
				{
					Zeus_SlowAnimFixes._hexEditExeAnims(_exeLangAndDistrib, ref _zeusExeData);
				}
				if (FixWindowed)
				{
					Zeus_WindowFix._hexEditWindowFix(_exeLangAndDistrib, ref _zeusExeData);
				}
				if (ResizeImages)
				{
					Zeus_ResizeImages.CreateResizedImages(ZeusExeLocation, ResWidth, ResHeight, _patchedFilesFolder);
				}

				File.WriteAllBytes(_patchedFilesFolder + "/Zeus.exe", _zeusExeData);
				MessageBox.Show("Your patched Zeus.exe has been successfully created in " + _patchedFilesFolder);
			}
		}

		/// <summary>
		/// Copies the contents of Zeus.exe into a byte array for editing then calculates a CRC32 hash for the contents of that array. 
		/// After that, compares that CRC to a list of known CRCs to determine 
		/// </summary>
		/// <param name="_zeusExeLocation">String that defines the location of Zeus.exe</param>
		/// <param name="_zeusExeData">Byte array that contains the binary data contained within the supplied Zeus.exe</param>
		/// <param name="exeLangAndDistrib">Enum that defines what version of the Zeus.exe was detected.</param>
		/// <returns>
		/// True if the CRC for the selected Zeus.exe matches one that this program knows about and knows the offsets that need to be patched.
		/// False if the EXE is not recognised.
		/// </returns>
		private static bool _getAndCheckExeChecksum(string _zeusExeLocation, out byte[] _zeusExeData, out ExeLangAndDistrib exeLangAndDistrib)
		{
			_zeusExeData = File.ReadAllBytes(_zeusExeLocation);

			uint _fileHash = CRC.Crc32(_zeusExeData, 0, _zeusExeData.Length);

			switch (_fileHash)
			{
				case 0x90B9CF84:        // English GOG version
					exeLangAndDistrib = ExeLangAndDistrib.GOG_English;
					return true;

				default:                // Unrecognised EXE
					string[] _messageLines = new string[]
					{
						"Zeus.exe was not recognised. Only the following distributions and languages are currently supported:",
						"- GOG Zeus & Poseidon - English"
					};
					MessageBox.Show(string.Join(Environment.NewLine, _messageLines));
					exeLangAndDistrib = ExeLangAndDistrib.Not_Recognised;
					return false;
			}
		}

		internal enum ExeLangAndDistrib
		{
			Not_Recognised = 0,
			GOG_English = 1
		}
	}
}
