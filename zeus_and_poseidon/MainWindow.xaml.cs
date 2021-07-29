// This code is part of the Impressions Resolution Customiser project
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using Microsoft.Win32;
using System;
using System.Globalization;
using System.Windows;

namespace zeus_and_poseidon
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private bool _exeCreationBusy;
		private string _zeusExePath;

		public MainWindow()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Code that runs when the "Generate EXE" button is clicked. Checks whether the two inputted resolution values are valid. 
		/// If they are, pass them and the state of the checkboxes to the "ProcessZeusExe" function.
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
				else if (_resWidthPreTests < 240)
				{
					MessageBox.Show("The desired Horizontal Resolution is less than 240, which is the absolute minimum width the game's window can be. " +
						"Please type in a number which is at least 240.");
				}
				else if (_resWidthPreTests > ushort.MaxValue)
				{
					MessageBox.Show("The desired Horizontal Resolution is greater than " + ushort.MaxValue + ", which is not allowed. Please type in a number " +
						"which is less than " + ushort.MaxValue + ".");
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
				else if (_resHeightPreTests > ushort.MaxValue)
				{
					MessageBox.Show("The desired Vertical Resolution is greater than " + ushort.MaxValue + ", which is not allowed. Please type in a number " +
						"which is less than " + ushort.MaxValue + ".");
				}
				else
				{
					bool _fixAnimations = ApplyAnimationFix.IsChecked ?? false;
					bool _fixWindowed = ApplyWindowFix.IsChecked ?? false;
					bool _resizeImages = ResizeImages.IsChecked ?? false;
					Zeus_MakeChanges.ProcessZeusExe(_zeusExePath, Convert.ToUInt16(_resWidthPreTests), Convert.ToUInt16(_resHeightPreTests), _fixAnimations, _fixWindowed, _resizeImages);
				}
				_exeCreationBusy = false;
			}
		}

		/// <summary>
		/// Code that runs when the "Select Zeus.exe" button is clicked. 
		/// Opens a file selection dialog to allow the user to select a Zeus.exe to patch.
		/// </summary>
		private void SelectExe_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				CheckFileExists = true,
				CheckPathExists = true,
				Filter = "Zeus.exe|Zeus.exe|All files (*.*)|*.*",
				Title = "Please select the Zeus.exe you want to patch."
			};
			if (openFileDialog.ShowDialog() == true)
			{
				_zeusExePath = openFileDialog.FileName;
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
						"Help menu for the Zeus & Poseidon Resolution Customiser utility",
						""
			};
			MessageBox.Show(string.Join(Environment.NewLine, _messageLines));
		}
	}
}
