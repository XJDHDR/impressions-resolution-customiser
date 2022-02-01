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
using Zeus_and_Poseidon.non_UI_code;

namespace Zeus_and_Poseidon
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
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
		private const ulong MAX_RESOLUTION_HEIGHT = 5760;
		private const ulong MAX_RESOLUTION_WIDTH = 23412;

		private bool exeCreationBusy;
		private string zeusExePath;

		public MainWindow()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Code that runs when the "Generate EXE" button is clicked. Checks whether the two inputted resolution values are valid.
		/// If they are, pass them and the state of the checkboxes to the "ProcessZeusExe" function.
		/// </summary>
		private void GenerateExe_Click(object _Sender_, RoutedEventArgs _E_)
		{
			if (exeCreationBusy)
			{
				return;
			}

			exeCreationBusy = true;
			if (!float.TryParse(ResWidth.Text, NumberStyles.None, CultureInfo.InvariantCulture, out float _resWidthPreTests_))
			{
				MessageBox.Show("An error occurred while trying to convert the typed in Horizontal Resolution from a string. Make sure you only typed in digits.");
			}
			else if (!float.TryParse(ResHeight.Text, NumberStyles.None, CultureInfo.InvariantCulture, out float _resHeightPreTests_))
			{
				MessageBox.Show("An error occurred while trying to convert the typed in Vertical Resolution from a string. Make sure you only typed in digits.");
			}
			else if (_resWidthPreTests_ < 800)
			{
				MessageBox.Show("The desired Horizontal Resolution is less than 800, which is the absolute minimum width the game's window can be. " +
				                "Please type in a number which is at least 800.");
			}
			else if (_resWidthPreTests_ > MAX_RESOLUTION_WIDTH)
			{
				MessageBox.Show($"The desired Horizontal Resolution is greater than {MAX_RESOLUTION_WIDTH}, which is not allowed. Please type in a number " +
				                $"which is less than {MAX_RESOLUTION_WIDTH}.");
			}
			else if (_resWidthPreTests_ % 4 != 0)
			{
				MessageBox.Show("The desired Horizontal Resolution is not divisible by 4. Please type in a number which is.");
			}
			else if (_resHeightPreTests_ < 600)
			{
				MessageBox.Show("The desired Vertical Resolution is less than 600, which is the absolute minimum height the game's window can be. " +
				                "Please type in a number which is at least 600.");
			}
			else if (_resHeightPreTests_ > MAX_RESOLUTION_HEIGHT)
			{
				MessageBox.Show($"The desired Vertical Resolution is greater than {MAX_RESOLUTION_HEIGHT}, which is not allowed. Please type in a number " +
				                $"which is less than {MAX_RESOLUTION_HEIGHT}.");
			}
			else
			{
				bool _fixAnimations_ = ApplyAnimationFix.IsChecked ?? false;
				bool _fixWindowed_ = ApplyWindowFix.IsChecked ?? false;
				bool _resizeImages_ = ResizeImages.IsChecked ?? false;
				bool _stretchImages_ = StretchImages.IsChecked ?? false;
				ZeusMakeChanges._ProcessZeusExe(zeusExePath, Convert.ToUInt16(_resWidthPreTests_),
					Convert.ToUInt16(_resHeightPreTests_), _fixAnimations_, _fixWindowed_, _resizeImages_, _stretchImages_);
			}
			exeCreationBusy = false;
		}

		/// <summary>
		/// Code that runs when the "Select Zeus.exe" button is clicked.
		/// Opens a file selection dialog to allow the user to select a Zeus.exe to patch.
		/// </summary>
		private void SelectExe_Click(object _Sender_, RoutedEventArgs _E_)
		{
			OpenFileDialog _openFileDialog_ = new OpenFileDialog
			{
				CheckFileExists = true,
				CheckPathExists = true,
				Filter = "Zeus.exe|Zeus.exe|All files (*.*)|*.*",
				Title = "Please select the Zeus.exe you want to patch."
			};
			if (_openFileDialog_.ShowDialog() == true)
			{
				zeusExePath = _openFileDialog_.FileName;
			}
		}

		/// <summary>
		/// Code that runs when the "HelpMe" button is clicked.
		/// Opens a MessageBox containing helpful information for the user.
		/// </summary>
		private void HelpMe_Click(object _Sender_, RoutedEventArgs _E_)
		{
			string[] _messageLines_ = {
				"Help menu for the Zeus & Poseidon Resolution Customiser utility",
				"",
				"",
				"This program modifies a supplied copy of Zeus.exe to allow you to change what resolution the game runs at when the \"1024x768\" option " +
				"is selected in the game's display menu. It also allows applying a number of patches to the game and resizing the game's background images.",
				"",
				"This program requires an unmodified copy of the game's EXE to work. It can be supplied in three ways. You can place this program in the same location " +
				"you have installed the game. Alternatively, you can copy the required game files into the \"base_files\" folder. Finally, you can use the file picker " +
				"button to choose where the game's files are. In any case, the modified files will be placed in the \"patched_files\" folder afterwards.",
				"Do note that any existing \"patched_files\" folder and it's contents will be deleted before generating the new patched files.",
				"",
				"",
				"Resolution Width: This text box allows you to specify the horizontal component of your desired resolution. If your screen is in landscape, " +
				$"this is the bigger number. Note that this number must be divisible by 4 as well as between 800 and {MAX_RESOLUTION_WIDTH}, both inclusive.",
				"",
				"Resolution Height: This text box allows you to specify the vertical component of your desired resolution. If your screen is in landscape, " +
				$"this is the smaller number. Note that this number must be between 600 and {MAX_RESOLUTION_HEIGHT}, both inclusive.",
				"",
				"Apply Animation Fixes: This tickbox tells this program to fix Zeus' slow gods and urchin quay animations on modern CPUs if enabled.",
				"",
				"Apply Windowed Mode Fixes: This tickbox tells this program to fix a bug in Zeus which means that the game can't be switched into windowed mode.",
				"",
				"Resize Images: This tickbox tells this program to resize the various JPEGs the game uses as background images. This resizing is required for these images" +
				"to display properly at the new resolutions. All of the images need to be in a \"DATA\" folder that is in the same place as the selected Zeus.exe.",
				"Since this is the most computationally intensive operation this program does, it is recommended that you only keep this option enabled if you need the" +
				"resized images. If you already have images of the correct dimensions, feel free to disable this option.",
				"",
				"Stretch menu images to fit window: By default, this program keeps menu images at their original sizes and adds a black background around the images to " +
				"fill the gaps between it and the game window's edges. This option changes that behaviour and tells this program to stretch the images to fit the window ",
				"instead.",
				"Note that this option can only be selected if the \"Resize Images\" tickbox is checked.",
				"",
				"Select Zeus.exe: This button opens a file picker that lets you specify the location of a Zeus.exe that you want to modify.",
				"",
				"Generate EXE: Once you have provided all the required information and selected the desired options, click on this button to generate a patched Zeus.exe " +
				"and optionally resized images. All of these will be placed in the \"patched_files\" folder next to this program."
			};
			MessageBox.Show(string.Join(Environment.NewLine, _messageLines_));
		}

		/// <summary>
		/// Make all text in a textbox selected when clicked on.
		/// </summary>
		private void AllTextBoxes_GotFocus(object _Sender_, RoutedEventArgs _E_)
		{
			TextBox _textBox_ = (TextBox)_Sender_;
			_textBox_.Dispatcher.BeginInvoke(new Action(() => _textBox_.SelectAll()));
		}

		/// <summary>
		/// Event that fires when the any time the "Resize Images" checkbox is ticked. Used to enable the "Stretch menu images" control.
		/// </summary>
		private void ResizeImages_Checked(object _Sender_, RoutedEventArgs _EventArgs_)
		{
			if (StretchImages != null)
			{
				StretchImages.IsEnabled = true;
			}
		}

		/// <summary>
		/// Event that fires when the any time the "Resize Images" checkbox is unticked. Used to disable the "Stretch menu images" control.
		/// </summary>
		private void ResizeImages_Unchecked(object _Sender_, RoutedEventArgs _EventArgs_) {
			if (StretchImages != null)
			{
				StretchImages.IsEnabled = false;
				StretchImages.IsChecked = false;
			}
		}
	}
}
