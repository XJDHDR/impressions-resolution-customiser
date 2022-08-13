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
using Zeus_and_Poseidon.non_UI_code;

namespace Zeus_and_Poseidon
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public MainWindow() =>
			InitializeComponent();

		/// <summary>
		/// Make all text in a textbox selected when clicked on.
		/// </summary>
		private void AllTextBoxes_GotFocus(object Sender, RoutedEventArgs EventArgs)
		{
			TextBox textBox = (TextBox)Sender;
			textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
		}
	}
}
