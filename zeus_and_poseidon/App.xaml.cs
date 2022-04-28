// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// ReSharper disable RedundantUsingDirective
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using System.Windows;
using Zeus_and_Poseidon.non_UI_code.Crc32;

namespace Zeus_and_Poseidon
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App
	{
		// Please note that, as per the repo's license noted above, you are not permitted to modify anything in this class
		// in any way that violates the terms and conditions of the license.
		private void applicationStart(object Sender, StartupEventArgs E)
		{
			//Disable shutdown when the dialog closes
			Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

			#if !DEBUG
			MainExeIntegrity._Check();
			#endif

			MainWindow mainWindow = new MainWindow();
			//Re-enable normal shutdown mode.
			Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
			Current.MainWindow = mainWindow;
			mainWindow.Show();
		}
	}
}
