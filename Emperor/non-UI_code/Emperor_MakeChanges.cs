// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using System;
using System.IO;
using System.Windows;

namespace Emperor.non_UI_code
{
	/// <summary>
	/// Base class used to interact with the code patching classes, figure out what version of the game is being patched and prepare the environment before patching.
	/// </summary>
	internal static class EmperorMakeChanges
	{
		/// <summary>
		/// Checks if there is a Emperor.exe selected or available to patch, prepares the "patched_files" folder for the patched files
		/// then calls the requested patching functions.
		/// </summary>
		/// <param name="_EmperorExeLocation_">Optionally contains the location of the Emperor.exe selected by the UI's file selection dialog.</param>
		/// <param name="_ResWidth_">The width value of the resolution inputted into the UI.</param>
		/// <param name="_ResHeight_">The height value of the resolution inputted into the UI.</param>
		/// <param name="_FixWindowed_">Whether the "Apply Windowed Mode Fixes" checkbox is selected or not.</param>
		/// <param name="_ResizeImages_">Whether the "Resize Images" checkbox is selected or not.</param>
		/// <param name="_IncreaseSpriteLimit_">Whether the "Double Sprite Limits" checkbox is selected or not.</param>
		internal static void _ProcessEmperorExe(string _EmperorExeLocation_, ushort _ResWidth_, ushort _ResHeight_,
			bool _FixWindowed_, bool _ResizeImages_, bool _IncreaseSpriteLimit_)
		{
			if (!File.Exists(_EmperorExeLocation_))
			{
				// User didn't select a folder using the selection button.
				if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"base_files\Emperor.exe"))
				{
					// Check if the user has placed the Zeus data files in the "base_files" folder.
					_EmperorExeLocation_ = AppDomain.CurrentDomain.BaseDirectory + @"base_files\Emperor.exe";
				}
				else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Emperor.exe") && (AppDomain.CurrentDomain.FriendlyName != "Emperor.exe"))
				{
					// As a last resort, check if the Zeus data files are in the same folder as this program.
					_EmperorExeLocation_ = AppDomain.CurrentDomain.BaseDirectory + "Emperor.exe";
				}
				else
				{
					MessageBox.Show("Emperor.exe does not exist in either the selected location or either of the automatically scanned locations. " +
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

			if (EmperorExeDefinitions._GetAndCheckExeChecksum(_EmperorExeLocation_, out byte[] _emperorExeData_, out ExeAttributes _exeAttributes_))
			{
				EmperorResolutionEdits._hexEditExeResVals(_ResWidth_, _ResHeight_, _exeAttributes_, ref _emperorExeData_,
					out ushort _viewportWidth_, out ushort _viewportHeight_);

				if (_FixWindowed_)
				{
					EmperorWindowFix._hexEditWindowFix(_exeAttributes_, ref _emperorExeData_);
				}
				if (_ResizeImages_)
				{
					EmperorResizeImages._CreateResizedImages(_EmperorExeLocation_, _ResWidth_, _ResHeight_, _viewportWidth_, _viewportHeight_, _patchedFilesFolder_);
				}
				if (_IncreaseSpriteLimit_)
				{
					EmperorSpriteLimitChanger._MakeChanges(_exeAttributes_, ref _emperorExeData_);
				}

				File.WriteAllBytes(_patchedFilesFolder_ + "/Emperor.exe", _emperorExeData_);
				MessageBox.Show("Your patched Emperor.exe has been successfully created in " + _patchedFilesFolder_);
			}
		}
	}
}
