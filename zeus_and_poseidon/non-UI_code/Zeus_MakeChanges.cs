// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using System;
using System.IO;
using System.Reflection;
using System.Text;
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
		/// <param name="ZeusExeLocation">Optionally contains the location of the Zeus.exe selected by the UI's file selection dialog.</param>
		/// <param name="ResWidth">The width value of the resolution inputted into the UI.</param>
		/// <param name="ResHeight">The height value of the resolution inputted into the UI.</param>
		/// <param name="FixAnimations">Whether the "Apply Animation Fixes" checkbox is selected or not.</param>
		/// <param name="FixWindowed">Whether the "Apply Windowed Mode Fixes" checkbox is selected or not.</param>
		/// <param name="ResizeImages">Whether the "Resize Images" checkbox is selected or not.</param>
		/// <param name="StretchImages">Whether the "Stretch menu images to fit window" checkbox is selected or not.</param>
		internal static void _ProcessZeusExe(string ZeusExeLocation, ushort ResWidth, ushort ResHeight,
			bool FixAnimations, bool FixWindowed, bool ResizeImages, bool StretchImages)
		{
			if (!File.Exists(ZeusExeLocation))
			{
				// User didn't select a folder using the selection button.
				if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"base_files\Zeus.exe"))
				{
					// Check if the user has placed the Zeus data files in the "base_files" folder.
					ZeusExeLocation = AppDomain.CurrentDomain.BaseDirectory + @"base_files\Zeus.exe";
				}
				else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Zeus.exe") && (AppDomain.CurrentDomain.FriendlyName != "Zeus.exe"))
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

			string patchedFilesFolder = AppDomain.CurrentDomain.BaseDirectory + "patched_files";
			try
			{
				if (Directory.Exists(patchedFilesFolder))
				{
					Directory.Delete(patchedFilesFolder, true);
				}
				Directory.CreateDirectory(patchedFilesFolder);
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

#if !DEBUG
			byte[] classQN = { 90, 101, 117, 115, 95, 97, 110, 100, 95, 80, 111, 115, 101, 105, 100, 111, 110, 46, 110,
				111, 110, 95, 85, 73, 95, 99, 111, 100, 101, 46, 67, 114, 99, 51, 50, 46, 77, 97, 105, 110, 69, 120, 101,
				73, 110, 116, 101, 103, 114, 105, 116, 121 };
			byte[] methodQN = { 95, 67, 104, 101, 99, 107 };
			Type _type_ = Type.GetType(Encoding.ASCII.GetString(classQN));
			if (_type_ != null)
			{
				try
				{
					MethodInfo methodInfo = _type_.GetMethod(Encoding.ASCII.GetString(methodQN), BindingFlags.DeclaredOnly |
						BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static);
					methodInfo.Invoke(null, new object[] { });
				}
				catch (Exception)
				{
					Application.Current.Shutdown();
					return;
				}
			}
			else
			{
				Application.Current.Shutdown();
				return;
			}
#endif

			if (ZeusExeDefinitions._GetAndCheckExeChecksum(ZeusExeLocation, out byte[] zeusExeData, out ExeAttributes exeAttributes))
			{
				ZeusResolutionEdits._hexEditExeResVals(ResWidth, ResHeight, exeAttributes, ref zeusExeData);

				if (FixAnimations)
				{
					ZeusSlowAnimFixes._hexEditExeAnims(exeAttributes, ref zeusExeData);
				}
				if (FixWindowed)
				{
					ZeusWindowFix._hexEditWindowFix(exeAttributes, ref zeusExeData);
				}
				if (ResizeImages)
				{
					ZeusResizeImages._CreateResizedImages(ZeusExeLocation, exeAttributes, ResWidth, ResHeight, StretchImages, patchedFilesFolder);
				}

				File.WriteAllBytes(patchedFilesFolder + "/Zeus.exe", zeusExeData);
				MessageBox.Show("Your patched Zeus.exe has been successfully created in " + patchedFilesFolder);
			}
		}
	}
}
