// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//
// disable once
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable HeuristicUnreachableCode
// ReSharper disable PossibleNullReferenceException
// ReSharper disable RedundantAssignment

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using ImpressionsFileFormats.EngText;

namespace Emperor.non_UI_code
{
	internal static class EmperorEngTextEdit
	{
		internal static void _EditResolutionString(string FilePath, string OutputDirectory, ushort ResWidth, ushort ResHeight,
			in EmperorExeAttributes ExeAttributes)
		{
			try
			{
				string engTextPath = $"{FilePath}/EmperorText.eng";
				if (File.Exists(engTextPath))
				{
					EngText engText;
					bool shouldRun = true;
					#if DEBUG
					shouldRun = false;
					#endif
					string messages;
					bool wasSuccessful;
					using (FileStream engTextFileStream = new FileStream(engTextPath, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						engText = new EngText(engTextFileStream, Game.Emperor, in ExeAttributes._CharEncoding,
							in ExeAttributes._EngTextDefaultStringCount, in ExeAttributes._EngTextDefaultWordCount,
							out messages, out wasSuccessful);
					}

					if (messages.Length != 0)
					{
						MessageBox.Show(messages);
					}

					if (!wasSuccessful)
					{
						return;
					}

					// First, adjust the offset for every string group to accomodate excess NULLs being removed from the exported file
					for (int i = 0; i < 1000; ++i)
					{
						// If this iteration is for the last group used in the file, stop because:
						// 1. The end of the file has an extra NULL.
						// 2. This group and the subsequent ones don't need adjusting, since there are no other used groups afterwards.
						if (i >= engText.FileHeader.GroupCount - 1)
						{
							engText.StringGroupIndexes[i].ExcessNullsRead = 0;
							break;
						}

						if (engText.StringGroupIndexes[i].ExcessNullsRead == 0)
							continue;

						// If there are excess NULLs, adjust the offset of every subsequent group in use backwards by the number of NULLs removed.
						for (int j = i; j < 1000; ++j)
						{
							if (engText.StringGroupIndexes[j].StringCountOrIsGroupUsed == 0)
								continue;

							if (j >= engText.FileHeader.GroupCount)
								break;

							engText.StringGroupIndexes[j].StringDataOffset -= engText.StringGroupIndexes[i].ExcessNullsRead;
						}

						// Zero the Excess NULLs value, as it is no longer needed and may cause confusion.
						engText.StringGroupIndexes[i].ExcessNullsRead = 0;
					}

					// Replace the resolution option's string with the new resolution values, and note the difference in lengths.
					int oldStringLength = engText.AllStringsByGroup[42].StringsInGroup[4].Length;

					switch (engText.AllStringsByGroup[1].StringsInGroup[0])
					{
						case "Fichier":
							// French language.
							engText.AllStringsByGroup[42].StringsInGroup[4] = $"Résolution {ResWidth.ToString()} x {ResHeight.ToString()}";
							break;

						case "File":
							// English language.
							engText.AllStringsByGroup[42].StringsInGroup[4] = $"{ResWidth.ToString()} x {ResHeight.ToString()} resolution";
							break;

						default:
							// Unknown language, so just default to English.
							engText.AllStringsByGroup[42].StringsInGroup[4] = $"{ResWidth.ToString()} x {ResHeight.ToString()} resolution";
							break;
					}

					int stringLengthChange = engText.AllStringsByGroup[42].StringsInGroup[4].Length - oldStringLength;

					// Adjust the offset for every string group to accomodate the new length of the resolution string
					for (int i = 43; i < 1000; ++i)
					{
						if (i >= engText.FileHeader.GroupCount)
							break;

						engText.StringGroupIndexes[i].StringDataOffset += stringLengthChange;
					}

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

					// Finally, write the edited data into a new EmperorText.eng file.
					using (FileStream engTextFileStream = new FileStream($"{OutputDirectory}/EmperorText.eng", FileMode.Create))
					{
						engText.Write(engTextFileStream);
					}
				}
				else
				{
					MessageBox.Show(StringsDatabase._EmperorEngTextEditFileNotFound);
				}
			}
			catch (Exception e)
			{
				MessageBox.Show($"{StringsDatabase._EmperorEngTextEditExceptionOccurred}\n{e}");
			}
		}
	}
}
