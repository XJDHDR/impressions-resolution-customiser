

using common_and_non_UI_code;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace zeus_and_poseidon
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private string _zeusExePath;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void GenerateExe_Click(object sender, RoutedEventArgs e)
		{
			if (!Single.TryParse(ResWidth.Text, NumberStyles.None, CultureInfo.InvariantCulture, out float _resWidthPreTests))
			{
				MessageBox.Show("An error occurred while trying to convert the typed in Horizontal Resolution from a string. Make sure you only typed in digits.");
			}
			else if (!Single.TryParse(ResHeight.Text, NumberStyles.None, CultureInfo.InvariantCulture, out float _resHeightPreTests))
			{
				MessageBox.Show("An error occurred while trying to convert the typed in Vertical Resolution from a string. Make sure you only typed in digits.");
			}
			else if (_resWidthPreTests < 2)
			{
				MessageBox.Show("The desired Horizontal Resolution is less than 2, which is not allowed. Please type in a number which is greater than 2.");
			}
			else if (_resWidthPreTests > ushort.MaxValue)
			{
				MessageBox.Show("The desired Horizontal Resolution is greater than " + ushort.MaxValue + ", which is not allowed. Please type in a number which is less than " + ushort.MaxValue + ".");
			}
			else if (_resWidthPreTests % 2 != 0)
			{
				MessageBox.Show("The desired Horizontal Resolution is not divisible by 2. Please type in a number which is a power of 2.");
			}
			else if (_resHeightPreTests < 2)
			{
				MessageBox.Show("The desired Vertical Resolution is less than 2, which is not allowed. Please type in a number which is greater than 2.");
			}
			else if (_resHeightPreTests > ushort.MaxValue)
			{
				MessageBox.Show("The desired Vertical Resolution is greater than " + ushort.MaxValue + ", which is not allowed. Please type in a number which is less than " + ushort.MaxValue + ".");
			}
			else if (_resHeightPreTests % 2 != 0)
			{
				MessageBox.Show("The desired Vertical Resolution is not divisible by 2. Please type in a number which is a power of 2.");
			}
			else
			{
				Zeus_HexEditing.HexEditZeusExe(_zeusExePath, Convert.ToUInt16(_resWidthPreTests), Convert.ToUInt16(_resHeightPreTests));
			}
		}
	}
}
