// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using System;
using System.IO;
using System.Windows;
using ImpressionsFileFormats.EngText;

namespace Emperor.non_UI_code
{
	internal static class EmperorEngTextEdit
	{
		internal static void _EditResolutionString(string FilePath, string OutputDirectory, ExeAttributes GameExeInfo, ushort ResWidth, ushort ResHeight)
		{
			try
			{
				if (File.Exists(FilePath + "/EmperorText.eng"))
				{
					using (FileStream engTextFileStream = new FileStream(FilePath + "/EmperorText.eng", FileMode.Open))
					{
						EngText engText = new EngText(engTextFileStream, Game.Emperor, GameExeInfo._CharEncoding, out string messages, out bool wasSuccessful);
						// TODO: String reader is bugging out while reading group index 358 or 359

						// Replace the resolution option's string with the new resolution values, and note the difference in lengths.
						int oldStringLength = engText.Strings[42].GroupStrings[4].Length;
						engText.Strings[42].GroupStrings[4] = $"{ResWidth.ToString()} x {ResHeight.ToString()} resolution";
						int stringLengthChange = engText.Strings[42].GroupStrings[4].Length - oldStringLength;
					}
				}
				else
				{
					MessageBox.Show("You selected the \"Patch EmperorText.eng\" option but there is no " +
					                "\"EmperorText.eng\" file in the folder containing Emperor.exe.\n" +
					                "Please correct this problem then try again.");
				}
			}
			catch (Exception e)
			{
				MessageBox.Show("An exception occurred while trying to patch EmperorText.eng:\n" +
				                $"{e}");
			}
		}
	}
}
