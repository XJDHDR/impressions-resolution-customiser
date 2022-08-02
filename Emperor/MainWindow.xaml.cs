// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using Microsoft.Win32;
using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Emperor.non_UI_code;

namespace Emperor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		// Because _resHeightMult and _resWidthMult (in the EmperorResolutionEdits class) are 8 bit signed integers, they can't go higher than 127.
		// The ExeDefinitions class caps these multipliers to a maximum of 127. The following formulae show what
		// the maximum size of the game's area can be (that being the city viewport, top menubar and sidebar together):
		// _maxResolutionHeight = (127 - 1) * 20 + 40 = 2560
		// _maxResolutionWidth = 127 * 80 + 226 - 2 = 10384
		//
		// If a higher resolution than these are requested, my custom code will add some background to cover up the gaps that would be present otherwise.
		// However, higher resolutions will cause the playable portions of the game's window to take up a progressively smaller part of the screen.
		// As a result, I will cap these numbers at 2x higher than the calculated figures above. This means that the game's playable area will
		// always take up at least 25% of the screen.
		internal const ulong MAX_RESOLUTION_HEIGHT = 5120;
		internal const ulong MAX_RESOLUTION_WIDTH = 20768;

		private bool exeCreationBusy;
		private string emperorExeDirectory;

		public MainWindow() =>
			InitializeComponent();

		/// <summary>
		/// Code that runs when the "Generate EXE" button is clicked. Checks whether the two inputted resolution values are valid.
		/// If they are, pass them and the state of the checkboxes to the "ProcessEmperorExe" function.
		/// </summary>
		private void GenerateExe_Click(object Sender, RoutedEventArgs EventArgs)
		{
			if (exeCreationBusy)
			{
				return;
			}

			exeCreationBusy = true;
			if (!float.TryParse(ResWidth.Text, NumberStyles.None, CultureInfo.InvariantCulture, out float resWidthPreTests))
			{
				MessageBox.Show(StringsDatabase._GenerateExeClickWidthTryParseFailed);
			}
			else if (!float.TryParse(ResHeight.Text, NumberStyles.None, CultureInfo.InvariantCulture, out float resHeightPreTests))
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
				bool fixWindowed = ApplyWindowFix.IsChecked ?? false;
				bool patchEngTxt = PatchEngText.IsChecked ?? false;
				bool resizeImages = ResizeImages.IsChecked ?? false;
				bool stretchImages = StretchImages.IsChecked ?? false;
				bool increaseSpriteLimit = IncreaseSpriteLimits.IsChecked ?? false;
				EmperorMakeChanges._ProcessEmperorExe(emperorExeDirectory, Convert.ToUInt16(resWidthPreTests),
					Convert.ToUInt16(resHeightPreTests), fixWindowed, patchEngTxt, resizeImages, stretchImages, increaseSpriteLimit);
			}
			exeCreationBusy = false;
		}

		/// <summary>
		/// Code that runs when the "Select Emperor.exe" button is clicked.
		/// Opens a file selection dialog to allow the user to select an Emperor.exe to patch.
		/// </summary>
		private void SelectExe_Click(object Sender, RoutedEventArgs EventArgs)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				CheckFileExists = true,
				CheckPathExists = true,
				Filter = "Emperor.exe|Emperor.exe|All files (*.*)|*.*",
				Title = StringsDatabase._SelectExeClickTitle
			};
			if (openFileDialog.ShowDialog() == true)
			{
				StringBuilder pathStringBuilder = new StringBuilder(openFileDialog.FileName);
				pathStringBuilder.Replace(@"\\", "/");
				pathStringBuilder.Replace($"{openFileDialog.SafeFileName}", "");
				pathStringBuilder.Replace("/", "", pathStringBuilder.Length - 2, 1);
				emperorExeDirectory = pathStringBuilder.ToString();
			}
		}

		/// <summary>
		/// Make all text in a textbox selected when clicked on.
		/// </summary>
		private void AllTextBoxes_GotFocus(object Sender, RoutedEventArgs EventArgs)
		{
			TextBox textBox = (TextBox)Sender;
			textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
		}

		/// <summary>
		/// Event that fires any time the "Resize Images" checkbox is ticked. Used to enable the "Stretch menu images" control.
		/// </summary>
		private void ResizeImages_Checked(object Sender, RoutedEventArgs EventArgs)
		{
			if (StretchImages != null)
			{
				StretchImages.IsEnabled = true;
			}
		}

		/// <summary>
		/// Event that fires when the any time the "Resize Images" checkbox is unticked. Used to disable the "Stretch menu images" control.
		/// </summary>
		private void ResizeImages_Unchecked(object Sender, RoutedEventArgs EventArgs)
		{
			if (StretchImages != null)
			{
				StretchImages.IsEnabled = false;
				StretchImages.IsChecked = false;
			}
		}

		/// <summary>
		/// Code that runs when the "HelpMe" button is clicked.
		/// Opens a MessageBox containing helpful information for the user.
		/// </summary>
		private void HelpMe_Click(object Sender, RoutedEventArgs EventArgs) =>
			MessageBox.Show(string.Join(Environment.NewLine, StringsDatabase._HelpMeClickMessage));

		/// <summary>
		/// Code that runs when the "ApplyWindowFix_Help" button is clicked. Describes what the "ApplyWindowFix" option does.
		/// </summary>
		private void ApplyWindowFix_Help_Click(object Sender, RoutedEventArgs EventArgs) =>
			MessageBox.Show(string.Join(Environment.NewLine, StringsDatabase._ApplyWindowFixHelpClickMessage));

		private void PatchEngText_Help_Click(object Sender, RoutedEventArgs EventArgs) =>
			MessageBox.Show(string.Join(Environment.NewLine, StringsDatabase._PatchEngTextHelpClickMessage));

		private void ResizeImages_Help_Click(object Sender, RoutedEventArgs EventArgs) =>
			MessageBox.Show(string.Join(Environment.NewLine, StringsDatabase._ResizeImagesHelpClickMessage));

		private void StretchImages_Help_Click(object Sender, RoutedEventArgs EventArgs) =>
			MessageBox.Show(string.Join(Environment.NewLine, StringsDatabase._StretchImagesHelpClickMessage));

		private void IncreaseSpriteLimits_Help_Click(object Sender, RoutedEventArgs EventArgs) =>
			MessageBox.Show(string.Join(Environment.NewLine, StringsDatabase._IncreaseSpriteLimitsHelpClickMessage));
	}
}
