// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using System;
using System.Globalization;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using Zeus_and_Poseidon.non_UI_code;
using Zeus_and_Poseidon.ViewModels;

namespace Zeus_and_Poseidon.Models
{
	public static class CentralManager
	{
		// Because _resHeightMult and _resWidthMult (in the Zeus_ResolutionEdits class) are 8 bit signed integers, they can't go higher than 127.
		// The ExeDefinitions class caps these multipliers to a maximum of 127. The following formulae show what
		// the maximum size of the game's area can be (that being the city viewport, top menubar and sidebar together):
		// Max Height = (127 - 1) * 15 + 30 = 1920
		// Max Width = 127 * 60 + 186 - 2 = 7804
		//
		// If a higher resolution than these are requested, my custom code will add some background to cover up the gaps that would be present otherwise.
		// However, higher resolutions will cause the playable portions of the game's window to take up a progressively smaller part of the screen.
		// As a result, I will cap these numbers at 3x higher than the calculated figures above. This means that the game's playable area will
		// always take up at least 11.1% of the screen.
		internal const ulong MAX_RESOLUTION_HEIGHT = 5760;
		internal const ulong MAX_RESOLUTION_WIDTH = 23412;

		private static string exeLocation;


		// ==== Private methods ====
		[ExecuteFromViewModelConstructor]
		private static void registerForChangeLanguageEvent()
		{
			MainWindowVm.Instance.GenerateExeButtonClickedEventHandler += generateExeButtonClicked;
			MainWindowVm.Instance.HelpMeButtonClickedEventHandler += helpMeClicked;
			MainWindowVm.Instance.SelectExeButtonClickedEventHandler += selectExeButtonClicked;

			MainWindowVm.Instance.ApplyAnimationFixHelpButtonClickedEventHandler += applyAnimationFixHelpButtonClicked;
			MainWindowVm.Instance.ApplyWindowFixHelpButtonClickedEventHandler += applyWindowFixHelpButtonClicked;
			MainWindowVm.Instance.PatchEngTextHelpButtonClickedEventHandler += patchEngTextHelpButtonClicked;
			MainWindowVm.Instance.ResizeImagesHelpButtonClickedEventHandler += resizeImagesHelpButtonClicked;
			MainWindowVm.Instance.StretchImagesHelpButtonClickedEventHandler += stretchImagesHelpButtonClicked;
		}

		/// <summary>
		/// Code that runs when the "Generate EXE" button is clicked. Checks whether the two inputted resolution values are valid.
		/// If they are, pass them and the state of the checkboxes to the "ProcessZeusExe" function.
		/// </summary>
		private static void generateExeButtonClicked()
		{
			if (!float.TryParse(MainWindowVm.Instance.ResolutionWidth, NumberStyles.None, CultureInfo.InvariantCulture, out float resWidthPreTests))
			{
				MessageBox.Show(StringsDatabase._GenerateExeClickWidthTryParseFailed);
			}
			else if (!float.TryParse(MainWindowVm.Instance.ResolutionHeight, NumberStyles.None, CultureInfo.InvariantCulture, out float resHeightPreTests))
			{
				MessageBox.Show(StringsDatabase._GenerateExeClickHeightTryParseFailed);
			}
			else if (resWidthPreTests < 800)
			{
				MessageBox.Show(StringsDatabase._GenerateExeClickWidthLessThanMinimum);
			}
			else if (resWidthPreTests > MAX_RESOLUTION_WIDTH)
			{
				MessageBox.Show(StringsDatabase._GenerateExeClickWidthMoreThanMaximum);
			}
			else if (resWidthPreTests % 4 != 0)
			{
				MessageBox.Show(StringsDatabase._GenerateExeClickWidthNotDivisibleBy4);
			}
			else if (resHeightPreTests < 600)
			{
				MessageBox.Show(StringsDatabase._GenerateExeClickHeightLessThanMinimum);
			}
			else if (resHeightPreTests > MAX_RESOLUTION_HEIGHT)
			{
				MessageBox.Show(StringsDatabase._GenerateExeClickHeightMoreThanMaximum);
			}
			else
			{
				bool fixWindowed = MainWindowVm.Instance.ApplyWindowModeFixesIsChecked;
				bool patchEngTxt = MainWindowVm.Instance.PatchEngTextIsChecked;
				bool resizeImages = MainWindowVm.Instance.ResizeImagesIsChecked;
				bool stretchImages = MainWindowVm.Instance.StretchImagesIsChecked;
				bool fixAnimations = MainWindowVm.Instance.ApplyAnimationFixIsChecked;
				ZeusMakeChanges._ProcessZeusExe(exeLocation, Convert.ToUInt16(resWidthPreTests),
					Convert.ToUInt16(resHeightPreTests), fixAnimations, fixWindowed, patchEngTxt, resizeImages, stretchImages);
			}
		}

		/// <summary>
		/// Code that runs when the "HelpMe" button is clicked.
		/// Opens a MessageBox containing helpful information for the user.
		/// </summary>
		private static void helpMeClicked() =>
			MessageBox.Show(string.Join(Environment.NewLine, StringsDatabase._HelpMeClickMessage));

		/// <summary>
		/// Code that runs when the "Select Zeus.exe" button is clicked.
		/// Opens a file selection dialog to allow the user to select an Zeus.exe to patch.
		/// </summary>
		private static void selectExeButtonClicked()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				CheckFileExists = true,
				CheckPathExists = true,
				Filter = "Zeus.exe|Zeus.exe|All files (*.*)|*.*",
				Title = StringsDatabase._SelectExeClickTitle
			};
			if (openFileDialog.ShowDialog() == true)
			{
				StringBuilder pathStringBuilder = new StringBuilder(openFileDialog.FileName);
				pathStringBuilder.Replace(@"\\", "/");
				pathStringBuilder.Replace($"{openFileDialog.SafeFileName}", "");
				pathStringBuilder.Replace("/", "", pathStringBuilder.Length - 2, 1);
				exeLocation = pathStringBuilder.ToString();
			}
		}

		private static void applyAnimationFixHelpButtonClicked() =>
			MessageBox.Show(string.Join(Environment.NewLine, StringsDatabase._ApplyAnimationFixHelpClickMessage));

		/// <summary>
		/// Code that runs when the "ApplyWindowFix_Help" button is clicked. Describes what the "ApplyWindowFix" option does.
		/// </summary>
		private static void applyWindowFixHelpButtonClicked() =>
			MessageBox.Show(string.Join(Environment.NewLine, StringsDatabase._ApplyWindowFixHelpClickMessage));

		private static void patchEngTextHelpButtonClicked() =>
			MessageBox.Show(string.Join(Environment.NewLine, StringsDatabase._PatchEngTextHelpClickMessage));

		private static void resizeImagesHelpButtonClicked() =>
			MessageBox.Show(string.Join(Environment.NewLine, StringsDatabase._ResizeImagesHelpClickMessage));

		private static void stretchImagesHelpButtonClicked() =>
			MessageBox.Show(string.Join(Environment.NewLine, StringsDatabase._StretchImagesHelpClickMessage));
	}
}
