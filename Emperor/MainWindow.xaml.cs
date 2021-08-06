// This code is part of the Impressions Resolution Customiser project
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using Microsoft.Win32;
using System;
using System.Globalization;
using System.Windows;

namespace Emperor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		// Because _resHeightMult and _resWidthMult (in the Emperor_ResolutionEdits class) are 8 bit numbers, they can't go higher than 255.
		// This means that we must cap our resolution numbers to figures that will be the highest possible that still leaves their multipliers at 255.
		// 
		// Thus, to copy and adapt the relevant formulas for calculating a resolution from a multiplier:
		// _maxResolutionHeight = (256 - 1) * 20 + 40 - 1 = 5139
		// _maxResolutionWidth = 256 * 80 + 222 - 2 - 1 = 20699
		private const ulong _maxResolutionHeight = 5139;
		private const ulong _maxResolutionWidth = 20699;

		private bool _exeCreationBusy;
		private string _emperorExePath;

		public MainWindow()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Code that runs when the "Generate EXE" button is clicked. Checks whether the two inputted resolution values are valid. 
		/// If they are, pass them and the state of the checkboxes to the "ProcessEmperorExe" function.
		/// </summary>
		private void GenerateExe_Click(object sender, RoutedEventArgs e)
		{
			if (_exeCreationBusy != true)
			{
				_exeCreationBusy = true;
				if (!float.TryParse(ResWidth.Text, NumberStyles.None, CultureInfo.InvariantCulture, out float _resWidthPreTests))
				{
					MessageBox.Show("An error occurred while trying to convert the typed in Horizontal Resolution from a string. Make sure you only typed in digits.");
				}
				else if (!float.TryParse(ResHeight.Text, NumberStyles.None, CultureInfo.InvariantCulture, out float _resHeightPreTests))
				{
					MessageBox.Show("An error occurred while trying to convert the typed in Vertical Resolution from a string. Make sure you only typed in digits.");
				}
				else if (_resWidthPreTests < 800)
				{
					MessageBox.Show("The desired Horizontal Resolution is less than 800, which is the absolute minimum width the game's window can be. " +
						"Please type in a number which is at least 800.");
				}
				else if (_resWidthPreTests > _maxResolutionWidth)
				{
					MessageBox.Show("The desired Horizontal Resolution is greater than " + _maxResolutionWidth + ", which is not allowed. Please type in a number " +
						"which is less than " + _maxResolutionWidth + ".");
				}
				else if (_resWidthPreTests % 4 != 0)
				{
					MessageBox.Show("The desired Horizontal Resolution is not divisible by 4. Please type in a number which is.");
				}
				else if (_resHeightPreTests < 600)
				{
					MessageBox.Show("The desired Vertical Resolution is less than 600, which is the absolute minimum height the game's window can be. " +
						"Please type in a number which is at least 600.");
				}
				else if (_resHeightPreTests > _maxResolutionHeight)
				{
					MessageBox.Show("The desired Vertical Resolution is greater than " + _maxResolutionHeight + ", which is not allowed. Please type in a number " +
						"which is less than " + _maxResolutionHeight + ".");
				}
				else
				{
					bool _fixWindowed = ApplyWindowFix.IsChecked ?? false;
					bool _resizeImages = ResizeImages.IsChecked ?? false;
					Emperor_MakeChanges.ProcessEmperorExe(_emperorExePath, Convert.ToUInt16(_resWidthPreTests), Convert.ToUInt16(_resHeightPreTests), _fixWindowed, _resizeImages);
				}
				_exeCreationBusy = false;
			}
		}

		/// <summary>
		/// Code that runs when the "Select Emperor.exe" button is clicked. 
		/// Opens a file selection dialog to allow the user to select a Emperor.exe to patch.
		/// </summary>
		private void SelectExe_Click(object sender, RoutedEventArgs e)
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
				_emperorExePath = openFileDialog.FileName;
			}
		}

		/// <summary>
		/// Code that runs when the "HelpMe" button is clicked. 
		/// Opens a MessageBox containing helpful information for the user.
		/// </summary>
		private void HelpMe_Click(object sender, RoutedEventArgs e)
		{
			string[] _messageLines = new string[]
			{
				"Help menu for the Emperor Resolution Customiser utility",
				"",
				"",
				"This program modifies a supplied copy of Emperor.exe to allow you to change what resolution the game runs at when the \"1024x768\" option " +
				"is selected in the game's display menu. It also allows applying a number of patches to the game and resizing the game's background images.",
				"",
				"This program requires an unmodified copy of the game's EXE to work. It can be supplied in three ways. You can place this program in the same location " +
				"you have installed the game. Alternatively, you can copy the required game files into the \"base_files\" folder. Finally, you can use the file picker " +
				"button to choose where the game's files are. In any case, the modified files will be placed in the \"patched_files\" folder afterwards.",
				"Do note that any existing \"patched_files\" folder and it's contents will be deleted before generating the new patched files.",
				"",
				"",
				"Resolution Width: This text box allows you to specify the horizontal component of your desired resolution. If your screen is in landscape, " +
				"this is the bigger number. Note that this number must be divisible by 4 as well as between 800 and " + _maxResolutionWidth + ", both inclusive.",
				"",
				"Resolution Height: This text box allows you to specify the vertical component of your desired resolution. If your screen is in landscape, " +
				"this is the smaller number. Note that this number must be between 600 and " + _maxResolutionHeight + ", both inclusive.",
				"",
				"Apply Windowed Mode Fixes: This tickbox tells this program to fix a bug in Emperor which means that the game can't be switched into windowed mode.",
				"",
				"Resize Images: This tickbox tells this program to resize the various JPEGs the game uses as background images. This resizing is required for these images" +
				"to display properly at the new resolutions. All of the images need to be in a \"DATA\" folder that is in the same place as the selected Emperor.exe.",
				"Since this is the most computationally intensive operation this program does, it is recommended that you only keep this option enabled if you need the" +
				"resized images. If you already have images of the correct dimensions, feel free to disable this option.",
				"",
				"Select Emperor.exe: This button opens a file picker that lets you specify the location of a Emperor.exe that you want to modify.",
				"",
				"Generate EXE: Once you have provided all the required information and selected the desired options, click on this button to generate a patched Emperor.exe " +
				"and optionally resized images. All of these will be placed in the \"patched_files\" folder next to this program."
			};
			MessageBox.Show(string.Join(Environment.NewLine, _messageLines));
		}
	}
}
