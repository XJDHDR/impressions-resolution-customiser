// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using System;
using System.IO;
using System.Windows;

namespace Zeus_and_Poseidon.non_UI_code
{
	/// <summary>
	/// Base class used to interact with the code patching classes, figure out what version of the game is being patched and prepare the environment before patching.
	/// </summary>
	internal static class ZeusMakeChanges
	{
		/// <summary>
		/// Checks if there is a Zeus.exe selected or available to patch, prepares the "patched_files" folder for the patched files
		/// then calls the requested patching functions.
		/// </summary>
		/// <param name="_ZeusExeLocation_">Optionally contains the location of the Zeus.exe selected by the UI's file selection dialog.</param>
		/// <param name="_ResWidth_">The width value of the resolution inputted into the UI.</param>
		/// <param name="_ResHeight_">The height value of the resolution inputted into the UI.</param>
		/// <param name="_FixAnimations_">Whether the "Apply Animation Fixes" checkbox is selected or not.</param>
		/// <param name="_FixWindowed_">Whether the "Apply Windowed Mode Fixes" checkbox is selected or not.</param>
		/// <param name="_ResizeImages_">Whether the "Resize Images" checkbox is selected or not.</param>
		internal static void _ProcessZeusExe(string _ZeusExeLocation_, ushort _ResWidth_, ushort _ResHeight_, bool _FixAnimations_, bool _FixWindowed_, bool _ResizeImages_)
		{
			if (!File.Exists(_ZeusExeLocation_))
			{
				// User didn't select a folder using the selection button.
				if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"base_files\Zeus.exe"))
				{
					// Check if the user has placed the Zeus data files in the "base_files" folder.
					_ZeusExeLocation_ = AppDomain.CurrentDomain.BaseDirectory + @"base_files\Zeus.exe";
				}
				else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Zeus.exe") && (AppDomain.CurrentDomain.FriendlyName != "Zeus.exe"))
				{
					// As a last resort, check if the Zeus data files are in the same folder as this program.
					_ZeusExeLocation_ = AppDomain.CurrentDomain.BaseDirectory + "Zeus.exe";
				}
				else
				{
					MessageBox.Show("Zeus.exe does not exist in either the selected location or either of the automatically scanned locations. " +
						"Please ensure that you have either selected the correct place, placed this program in the correct place or " +
						"placed the correct files in the \"base_files\" folder.");
					return;
				}
			}

			string _patchedFilesFolder_ = AppDomain.CurrentDomain.BaseDirectory + "patched_files";
			try
			{
				if (Directory.Exists(_patchedFilesFolder_))
				{
					Directory.Delete(_patchedFilesFolder_, true);
				}
				Directory.CreateDirectory(_patchedFilesFolder_);
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

			if (ZeusExeDefinitions._GetAndCheckExeChecksum(_ZeusExeLocation_, out byte[] _zeusExeData_, out ExeAttributes _exeAttributes_))
			{
				ZeusResolutionEdits._hexEditExeResVals(_ResWidth_, _ResHeight_, _exeAttributes_, ref _zeusExeData_);

				if (_FixAnimations_)
				{
					ZeusSlowAnimFixes._hexEditExeAnims(_exeAttributes_, ref _zeusExeData_);
				}
				if (_FixWindowed_)
				{
					ZeusWindowFix._hexEditWindowFix(_exeAttributes_, ref _zeusExeData_);
				}
				if (_ResizeImages_)
				{
					ZeusResizeImages._CreateResizedImages(_ZeusExeLocation_, _exeAttributes_, _ResWidth_, _ResHeight_, _patchedFilesFolder_);
				}

				File.WriteAllBytes(_patchedFilesFolder_ + "/Zeus.exe", _zeusExeData_);
				MessageBox.Show("Your patched Zeus.exe has been successfully created in " + _patchedFilesFolder_);
			}
		}
	}
}
