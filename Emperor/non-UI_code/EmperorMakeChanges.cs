// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable HeuristicUnreachableCode
// ReSharper disable PossibleNullReferenceException
// ReSharper disable RedundantAssignment

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
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
		/// <param name="EmperorExeDirectory">Optionally contains the location of the Emperor.exe selected by the UI's file selection dialog.</param>
		/// <param name="ResWidth">The width value of the resolution inputted into the UI.</param>
		/// <param name="ResHeight">The height value of the resolution inputted into the UI.</param>
		/// <param name="FixWindowed">Whether the "Apply Windowed Mode Fixes" checkbox is selected or not.</param>
		/// <param name="PatchEngText">Whether the "Patch EmperorText.eng" checkbox is selected or not.</param>
		/// <param name="ResizeImages">Whether the "Resize Images" checkbox is selected or not.</param>
		/// <param name="StretchImages">Whether the "Stretch menu images to fit window" checkbox is selected or not.</param>
		/// <param name="IncreaseSpriteLimit">Whether the "Double Sprite Limits" checkbox is selected or not.</param>
		internal static void _ProcessEmperorExe(string EmperorExeDirectory, ushort ResWidth, ushort ResHeight,
			bool FixWindowed, bool PatchEngText, bool ResizeImages, bool StretchImages, bool IncreaseSpriteLimit)
		{
			if (!File.Exists($"{EmperorExeDirectory}/Emperor.exe"))
			{
				// User didn't select a folder using the selection button.
				if (File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}base_files/Emperor.exe"))
				{
					// Check if the user has placed the game's data files in the "base_files" folder.
					EmperorExeDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}base_files";
				}
				else if (File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}Emperor.exe") && (AppDomain.CurrentDomain.FriendlyName != "Emperor.exe"))
				{
					// As a last resort, check if the game's data files are in the same folder as this program.
					EmperorExeDirectory = AppDomain.CurrentDomain.BaseDirectory;
				}
				else
				{
					MessageBox.Show("Emperor.exe does not exist in either the selected location or either of the automatically scanned locations.\n" +
						"Please ensure that you have done one of the following:\n" +
						"- Selected the correct location using the \"Select Emperor.exe\" button,\n" +
						"- Placed this program in the folder you installed Emperor, or\n" +
						"- Placed the correct files in the \"base_files\" folder.");
					return;
				}
			}

			string patchedFilesFolder = $"{AppDomain.CurrentDomain.BaseDirectory}patched_files";
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

			bool shouldRun = true;
			#if DEBUG
			shouldRun = false;
			#endif

			if (shouldRun)
			{
				unsafe
				{
					byte* classQn = stackalloc byte[] { 69, 109, 112, 101, 114, 111, 114, 46, 110, 111, 110, 95, 85, 73, 95, 99, 111, 100, 101, 46, 67, 114, 99,
						51, 50, 46, 77, 97, 105, 110, 69, 120, 101, 73, 110, 116, 101, 103, 114, 105, 116, 121 };
					byte* methodQn = stackalloc byte[] { 95, 67, 104, 101, 99, 107 };
					Type type = Type.GetType(Marshal.PtrToStringAnsi(new IntPtr(classQn), 42));
					if (type != null)
					{
						try
						{
							MethodInfo methodInfo = type.GetMethod(Marshal.PtrToStringAnsi(new IntPtr(methodQn), 6), BindingFlags.DeclaredOnly |
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
				}
			}

			byte[] emperorExeData = File.ReadAllBytes($"{EmperorExeDirectory}/Emperor.exe");
			EmperorExeAttributes exeAttributes = new EmperorExeAttributes(emperorExeData, out bool wasSuccessful);

			if (!wasSuccessful)
				return;

			EmperorResolutionEdits._hexEditExeResVals(ResWidth, ResHeight, in exeAttributes, ref emperorExeData,
				out ushort viewportWidth, out ushort viewportHeight);

			if (FixWindowed)
			{
				EmperorWindowFix windowFixData = new EmperorWindowFix(exeAttributes, out bool windowFixExeRecognised);

				if (windowFixExeRecognised)
					windowFixData._hexEditWindowFix(ref emperorExeData);
			}

			if (PatchEngText)
			{
				EmperorEngTextEdit._EditResolutionString(EmperorExeDirectory, patchedFilesFolder, ResWidth, ResHeight,
					in exeAttributes);
			}

			if (ResizeImages)
			{
				EmperorResizeImages resizeImages = new EmperorResizeImages(ResWidth, ResHeight, viewportWidth,
					viewportHeight, StretchImages, EmperorExeDirectory, patchedFilesFolder, out bool jpegCodecFound);

				if (jpegCodecFound)
					resizeImages._CreateResizedImages();
			}

			if (IncreaseSpriteLimit)
			{
				EmperorSpriteLimitChanger._MakeChanges(in exeAttributes, ref emperorExeData);
			}

			File.WriteAllBytes($"{patchedFilesFolder}/Emperor.exe", emperorExeData);
			MessageBox.Show($"Your patched Emperor.exe has been successfully created in {patchedFilesFolder}");
		}
	}
}
