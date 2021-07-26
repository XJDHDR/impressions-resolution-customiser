using Soft160.Data.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace zeus_and_poseidon
{
	class Zeus_MakeChanges
	{
		internal static void ProcessZeusExe(string ZeusExeLocation, ushort ResWidth, ushort ResHeight, bool FixAnimations, bool FixWindowed, bool ResizeImages)
		{
			if (!File.Exists(ZeusExeLocation))
			{
				// User didn't select a folder using the selection button.
				if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "base_files/Zeus.exe"))
				{
					// Check if the user has placed the Zeus data files in the "base_files" folder.
					ZeusExeLocation = AppDomain.CurrentDomain.BaseDirectory + "base_files/Zeus.exe";
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
					_hexEditExeAnims(_exeLangAndDistrib, ref _zeusExeData);
				}
				if (FixWindowed)
				{
					_hexEditWindowFix(_exeLangAndDistrib, ref _zeusExeData);
				}
				if (ResizeImages)
				{
					Zeus_ResizeImages.CreateResizedImages(ZeusExeLocation, ResWidth, ResHeight, _patchedFilesFolder);
				}

				File.WriteAllBytes(_patchedFilesFolder + "/Zeus.exe", _zeusExeData);
				MessageBox.Show("Your patched Zeus.exe has been successfully created in " + _patchedFilesFolder);
			}
		}

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

		private static void _hexEditExeAnims(ExeLangAndDistrib _exeLangAndDistrib, ref byte[] _zeusExeData)
		{
			if (_fillAnimHexOffsetTable(_exeLangAndDistrib, out int[] _animHexOffsetTable))
			{
				for (byte i = 0; i < _animHexOffsetTable.Length; i++)
				{
					_zeusExeData[_animHexOffsetTable[i]] = 0x00;
				}
			}
		}

		private static void _hexEditWindowFix(ExeLangAndDistrib _exeLangAndDistrib, ref byte[] _zeusExeData)
		{
			// At this address, the original code had a conditional jump (jl) that activates if the value stored in the EAX register is less than the value stored in the ECX.
			// This patch changes this byte into an unconditional jump.
			// I have no idea what the values represent, what code runs if the condition is false (EAX is greater than ECX) or why the widescreen mods cause
			// EAX to be greater than ECX. All I know is that it makes Windowed mode work.
			if (_identifyWinFixOffset(_exeLangAndDistrib, out int _winFixOffset))
			{
				_zeusExeData[_winFixOffset] = 0xEB;
			}
		}

		private static bool _fillAnimHexOffsetTable(ExeLangAndDistrib _exeLangAndDistrib, out int[] _animHexOffsetTable)
		{
			switch ((byte)_exeLangAndDistrib)
			{
				case 1:         // English GOG version
					_animHexOffsetTable = new int[] { 0x30407, 0xB395D, 0xB3992, 0xB5642, 0xB5AED, 0xB5DE5, 0xB65FF, 0xB69B7, 0xB91D6, 0xB9AB2, 0xB9AFB, 0xB9B7C,
						0xB9DB1, 0xBA007, 0xBAC20, 0xBAC31, 0xBAC42, 0xBAC53, 0xBB1F4, 0xBB381, 0xBB3E5, 0xBB40B, 0xBB431, 0xBB457, 0xBB47D, 0xBB4A3, 0xBB4C9,
						0xBB4EC, 0xBB50F, 0xBB532, 0xBB593, 0xBB5AD, 0xBB5C7, 0xBB5E4, 0xBB656, 0xBD331, 0xBD349, 0xBD3B2, 0xBDC62, 0xBDC7F, 0xBDC9C, 0xBDCB9,
						0xBDD2F, 0xBDDD7, 0xBDE5A, 0xBDE9F, 0xBDEE4, 0xBDF29, 0xBDF6E, 0xBDFB3, 0xBDFF8, 0xBE03D, 0xBE082, 0xBE0C7, 0xBFC43, 0xBFDF8, 0xBFF47,
						0xC26D1, 0xC2740, 0xC28E3, 0xC2904, 0xC2BD8, 0xC3A78, 0xC8415, 0xC84FC, 0xC9DEC, 0xC9E80, 0xCB1D7, 0xCB1F0, 0xCB23F };
					return true;

				default:        // Unrecognised EXE
					_animHexOffsetTable = new int[1];
					return false;
			}
		}

		private static bool _identifyWinFixOffset(ExeLangAndDistrib _exeLangAndDistrib, out int _winFixOffset)
		{
			switch ((byte)_exeLangAndDistrib)
			{
				case 1:         // English GOG version
					_winFixOffset = 0x212606;
					return true;

				default:        // Unrecognised EXE
					_winFixOffset = 0;
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
