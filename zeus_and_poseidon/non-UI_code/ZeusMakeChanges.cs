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
		/// <param name="ExeDirectory">Optionally contains the location of the Zeus.exe selected by the UI's file selection dialog.</param>
		/// <param name="ResWidth">The width value of the resolution inputted into the UI.</param>
		/// <param name="ResHeight">The height value of the resolution inputted into the UI.</param>
		/// <param name="FixAnimations">Whether the "Apply Animation Fixes" checkbox is selected or not.</param>
		/// <param name="FixWindowed">Whether the "Apply Windowed Mode Fixes" checkbox is selected or not.</param>
		/// <param name="PatchEngText">Whether the "Patch EmperorText.eng" checkbox is selected or not.</param>
		/// <param name="ResizeImages">Whether the "Resize Images" checkbox is selected or not.</param>
		/// <param name="StretchImages">Whether the "Stretch menu images to fit window" checkbox is selected or not.</param>
		internal static void _ProcessZeusExe(string ExeDirectory, ushort ResWidth, ushort ResHeight,
			bool FixAnimations, bool FixWindowed, bool PatchEngText, bool ResizeImages, bool StretchImages)
		{
			try
			{
				if (!File.Exists($"{ExeDirectory}/Zeus.exe"))
				{
					// User didn't select a folder using the selection button.
					if (File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}base_files/Zeus.exe"))
					{
						// Check if the user has placed the Zeus data files in the "base_files" folder.
						ExeDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}base_files";
					}
					else if (File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}Zeus.exe") && (AppDomain.CurrentDomain.FriendlyName != "Zeus.exe"))
					{
						// As a last resort, check if the Zeus data files are in the same folder as this program.
						ExeDirectory = AppDomain.CurrentDomain.BaseDirectory;
					}
					else
					{
						MessageBox.Show(StringsDatabase._ZeusMakeChangesExeNotFound);
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
					MessageBox.Show(StringsDatabase._ZeusMakeChangesPathTooLongException);
					return;
				}
				catch (IOException)
				{
					MessageBox.Show(StringsDatabase._ZeusMakeChangesIoException);
					return;
				}
				catch (UnauthorizedAccessException)
				{
					MessageBox.Show(StringsDatabase._ZeusMakeChangesUnauthorizedAccessException);
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
						byte* classQn = stackalloc byte[] { 90, 101, 117, 115, 95, 97, 110, 100, 95, 80, 111, 115, 101, 105, 100, 111,
							110, 46, 110, 111, 110, 95, 85, 73, 95, 99, 111, 100, 101, 46, 67, 114, 99, 51, 50, 46, 77, 97, 105, 110,
							69, 120, 101, 73, 110, 116, 101, 103, 114, 105, 116, 121 };
						byte* methodQn = stackalloc byte[] { 95, 67, 104, 101, 99, 107 };
						Type type = Type.GetType(Marshal.PtrToStringAnsi(new IntPtr(classQn), 52));
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

				byte[] zeusExeData = File.ReadAllBytes($"{ExeDirectory}/Zeus.exe");
				ZeusExeAttributes exeAttributes = new ZeusExeAttributes(zeusExeData, out bool wasSuccessful);

				if (!wasSuccessful)
					return;

				ZeusResolutionEdits._hexEditExeResVals(ResWidth, ResHeight, in exeAttributes, ref zeusExeData,
					out ushort viewportWidth, out ushort viewportHeight);

				if (FixAnimations)
				{
					ZeusSlowAnimFixes slowAnimFixes = new ZeusSlowAnimFixes(in exeAttributes, out bool slowAnimExeRecognised);

					if (slowAnimExeRecognised)
						slowAnimFixes._hexEditExeAnims(ref zeusExeData);
				}

				if (FixWindowed)
				{
					ZeusWindowFix windowFixData = new ZeusWindowFix(exeAttributes, out bool windowFixExeRecognised);

					if (windowFixExeRecognised)
						windowFixData._hexEditWindowFix(ref zeusExeData);
				}

				if (PatchEngText)
				{
					ZeusEngTextEdit._EditResolutionString(ExeDirectory, patchedFilesFolder,
						ResWidth, ResHeight, in exeAttributes);
				}

				if (ResizeImages)
				{
					ZeusResizeImages resizeImages = new ZeusResizeImages(ResWidth, ResHeight, viewportWidth, viewportHeight,
						StretchImages, ExeDirectory, patchedFilesFolder, in exeAttributes, out bool jpegCodecFound);

					if (jpegCodecFound)
						resizeImages._CreateResizedImages();
				}

				File.WriteAllBytes(patchedFilesFolder + "/Zeus.exe", zeusExeData);
				MessageBox.Show($"{StringsDatabase._ZeusMakeChangesExeSuccessfullyPatched} {patchedFilesFolder}");
			}
			catch (Exception e)
			{
				MessageBox.Show($"{StringsDatabase._ZeusMakeChangesUnhandledException}:\n\n{e}",
					StringsDatabase._ZeusMakeChangesUnhandledExceptionTitle);
			}
		}
	}
}
