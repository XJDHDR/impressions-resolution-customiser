// This code is part of the Impressions Resolution Customiser project
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using System;
using System.IO;
using System.Windows;

namespace Emperor
{
	/// <summary>
	/// Base class used to interact with the code patching classes, figure out what version of the game is being patched and prepare the environment before patching.
	/// </summary>
	class Emperor_MakeChanges
	{
		/// <summary>
		/// Checks if there is a Emperor.exe selected or available to patch, prepares the "patched_files" folder for the patched files
		/// then calls the requested patching functions.
		/// </summary>
		/// <param name="EmperorExeLocation">Optionally contains the location of the Emperor.exe selected by the UI's file selection dialog.</param>
		/// <param name="ResWidth">The width value of the resolution inputted into the UI.</param>
		/// <param name="ResHeight">The height value of the resolution inputted into the UI.</param>
		/// <param name="FixWindowed">Whether the "Apply Windowed Mode Fixes" checkbox is selected or not.</param>
		/// <param name="ResizeImages">Whether the "Resize Images" checkbox is selected or not.</param>
		internal static void ProcessEmperorExe(string EmperorExeLocation, ushort ResWidth, ushort ResHeight, bool FixWindowed, bool ResizeImages)
		{
			if (!File.Exists(EmperorExeLocation))
			{
				// User didn't select a folder using the selection button.
				if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"base_files\Emperor.exe"))
				{
					// Check if the user has placed the Zeus data files in the "base_files" folder.
					EmperorExeLocation = AppDomain.CurrentDomain.BaseDirectory + @"base_files\Emperor.exe";
				}
				else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Emperor.exe") && (AppDomain.CurrentDomain.FriendlyName != "Emperor.exe"))
				{
					// As a last resort, check if the Zeus data files are in the same folder as this program.
					EmperorExeLocation = AppDomain.CurrentDomain.BaseDirectory + "Emperor.exe";
				}
				else
				{
					MessageBox.Show("Emperor.exe does not exist in either the selected location or either of the automatically scanned locations. " +
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

			if (Emperor_ExeDefinitions.GetAndCheckExeChecksum(EmperorExeLocation, out byte[] _emperorExeData, out ExeAttributes _exeAttributes))
			{
				Emperor_ResolutionEdits._hexEditExeResVals(ResWidth, ResHeight, _exeAttributes, ref _emperorExeData);

				if (FixWindowed)
				{
					Emperor_WindowFix._hexEditWindowFix(_exeAttributes, ref _emperorExeData);
				}
				if (ResizeImages)
				{
					Emperor_ResizeImages.CreateResizedImages(EmperorExeLocation, ResWidth, ResHeight, _patchedFilesFolder);
				}

				File.WriteAllBytes(_patchedFilesFolder + "/Emperor.exe", _emperorExeData);
				MessageBox.Show("Your patched Emperor.exe has been successfully created in " + _patchedFilesFolder);
			}
		}
	}
}
