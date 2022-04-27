// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using Microsoft.Win32;
using System;
using System.Globalization;
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
		private const ulong MAX_RESOLUTION_HEIGHT = 5120;
		private const ulong MAX_RESOLUTION_WIDTH = 20768;

		private bool exeCreationBusy;
		private string emperorExePath;

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
				MessageBox.Show("An error occurred while trying to convert the typed in Horizontal Resolution from a string. Make sure you only typed in digits.");
			}
			else if (!float.TryParse(ResHeight.Text, NumberStyles.None, CultureInfo.InvariantCulture, out float resHeightPreTests))
			{
				MessageBox.Show("An error occurred while trying to convert the typed in Vertical Resolution from a string. Make sure you only typed in digits.");
			}
			else if (resWidthPreTests < 800)
			{
				MessageBox.Show("The desired Horizontal Resolution is less than 800, which is the absolute minimum width the game's window can be. " +
				                "Please type in a number which is at least 800.");
			}
			else if (resWidthPreTests > MAX_RESOLUTION_WIDTH)
			{
				MessageBox.Show($"The desired Horizontal Resolution is greater than {MAX_RESOLUTION_WIDTH.ToString()}, which is not allowed. Please type in a number " +
				                $"which is less than {MAX_RESOLUTION_WIDTH.ToString()}.");
			}
			else if (resWidthPreTests % 4 != 0)
			{
				MessageBox.Show("The desired Horizontal Resolution is not divisible by 4. Please type in a number which is.");
			}
			else if (resHeightPreTests < 600)
			{
				MessageBox.Show("The desired Vertical Resolution is less than 600, which is the absolute minimum height the game's window can be. " +
				                "Please type in a number which is at least 600.");
			}
			else if (resHeightPreTests > MAX_RESOLUTION_HEIGHT)
			{
				MessageBox.Show($"The desired Vertical Resolution is greater than {MAX_RESOLUTION_HEIGHT.ToString()}, which is not allowed. Please type in a number " +
				                $"which is less than {MAX_RESOLUTION_HEIGHT.ToString()}.");
			}
			else
			{
				bool fixWindowed = ApplyWindowFix.IsChecked ?? false;
				bool patchEngTxt = PatchEngText.IsChecked ?? false;
				bool resizeImages = ResizeImages.IsChecked ?? false;
				bool stretchImages = StretchImages.IsChecked ?? false;
				bool increaseSpriteLimit = IncreaseSpriteLimits.IsChecked ?? false;
				EmperorMakeChanges._ProcessEmperorExe(emperorExePath, Convert.ToUInt16(resWidthPreTests),
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
				Title = "Please select the Emperor.exe you want to patch."
			};
			if (openFileDialog.ShowDialog() == true)
			{
				emperorExePath = openFileDialog.FileName.Replace(openFileDialog.SafeFileName, "");
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
		private void HelpMe_Click(object Sender, RoutedEventArgs EventArgs)
		{
			string[] messageLines = {
				"Help menu for the Emperor Resolution Customiser utility",
				"",
				"",
				"This program modifies a supplied copy of Emperor.exe to allow you to change what resolution the game runs at when the \"1024x768\" option " +
				"is selected in the game's display menu. It also allows applying a number of patches to the game and resizing the game's background images.",
				"",
				"This program requires an unmodified copy of the game's EXE to work. It can be supplied in three ways. You can place this program in the same location " +
				"you have installed the game. Alternatively, you can copy the required game files into the \"base_files\" folder. Finally, you can use the file picker " +
				"button to choose where the game's files are. In any case, the modified files will be placed in the \"patched_files\" folder afterwards. ",
				"Do note that any existing \"patched_files\" folder and it's contents will be deleted before generating the new patched files.",
				"",
				"",
				"Resolution Width: This text box allows you to specify the horizontal component of your desired resolution. If your screen is in landscape, " +
				$"this is the bigger number. Note that this number must be divisible by 4 as well as between 800 and {MAX_RESOLUTION_WIDTH.ToString()}, both inclusive.",
				"",
				"Resolution Height: This text box allows you to specify the vertical component of your desired resolution. If your screen is in landscape, " +
				$"this is the smaller number. Note that this number must be between 600 and {MAX_RESOLUTION_HEIGHT.ToString()}, both inclusive.",
				"",
				"Select Emperor.exe: This button opens a file picker that lets you specify the location of a Emperor.exe that you want to modify.",
				"",
				"Generate EXE: Once you have provided all the required information and selected the desired options, click on this button to generate a patched Emperor.exe " +
				"and optionally resized images. All of these will be placed in the \"patched_files\" folder next to this program."
			};
			MessageBox.Show(string.Join(Environment.NewLine, messageLines));
		}

		/// <summary>
		/// Code that runs when the "ApplyWindowFix_Help" button is clicked. Describes what the "ApplyWindowFix" option does.
		/// </summary>
		private void ApplyWindowFix_Help_Click(object Sender, RoutedEventArgs EventArgs)
		{
			string[] messageLines = {
				"Apply Windowed Mode Fixes:",
				"This tickbox tells this program to fix a bug in Emperor which means that the game can't be switched into windowed mode.",
			};
			MessageBox.Show(string.Join(Environment.NewLine, messageLines));
		}

		private void PatchEngText_Help_Click(object Sender, RoutedEventArgs EventArgs)
		{
			string[] messageLines = {
				"Patch EmperorText.eng:",
				"The game populates the Display Settings menu using strings from EmperorText.eng.",
				"By default, the string for the modified resolution option says \"1024 x 768 resolution\" and the menu option will continue " +
				"saying this even after the EXE has been patched to use a different resolution. When this option is ticked, the string read from " +
				"this file will be modified to instead identify the setting as the patched resolution (e.g. it will say \"1920 x 1080 resolution\" " +
				"if you typed in those numbers).",
				"To use this option, you need to have a copy of \"EmperorText.eng\" in the same folder as the \"Emperor.exe\" file " +
				"you selected for patching."
			};
			MessageBox.Show(string.Join(Environment.NewLine, messageLines));
		}

		private void ResizeImages_Help_Click(object Sender, RoutedEventArgs EventArgs)
		{
			string[] messageLines = {
				"Resize Images:",
				"This tickbox tells this program to resize the various JPEGs the game uses as background images. This resizing is required for these images " +
				"to display properly at the new resolutions. All of the images need to be in a \"DATA\" folder that is in the same place as the selected Emperor.exe. ",
				"Since this is the most computationally intensive operation this program does, it is recommended that you only keep this option enabled if you need the " +
				"resized images. If you already have images of the correct dimensions, feel free to disable this option.",
			};
			MessageBox.Show(string.Join(Environment.NewLine, messageLines));
		}

		private void StretchImages_Help_Click(object Sender, RoutedEventArgs EventArgs)
		{
			string[] messageLines = {
				"Stretch menu images to fit window:",
				"By default, this program keeps menu images at their original sizes and adds a black background around the images to " +
				"fill the gaps between it and the game window's edges. This option changes that behaviour and tells this program to stretch the images to fit the window ",
				"instead.",
				"Note that this option can only be selected if the \"Resize Images\" tickbox is checked.",
			};
			MessageBox.Show(string.Join(Environment.NewLine, messageLines));
		}

		private void IncreaseSpriteLimits_Help_Click(object Sender, RoutedEventArgs EventArgs)
		{
			string[] messageLines = {
				"Double Sprite Limits:",
				"This tickbox tells the program to increase the game's sprite limit from 4000 to 8000. This is essentially the exact same patch created " +
				"by Vadim_Panenko on Mod DB, except that it will work with any of the custom resolutions this program supports. There are two things you must be aware of:",
				"1. The game might experience a bit of slowdown after building more buildings than the original limit permitted. Vadim also reported that the game " +
				"\"flies out\" if the limit is raised further than double.",
				"2. For obvious reasons, loading a save where this limit has been exceeded on an EXE that doesn't have this raised limit can cause issues.",
				"For these reasons, this option is disabled by default and it is recommended that you only enable it if you need it.",
			};
			MessageBox.Show(string.Join(Environment.NewLine, messageLines));
		}
	}
}
