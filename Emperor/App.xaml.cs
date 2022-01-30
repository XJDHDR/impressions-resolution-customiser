// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using System.Windows;
using Emperor.non_UI_code.Crc32;

namespace Emperor
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		// Please note that, as per the repo's license noted above, you are not permitted to modify anything in this class
		// in any way that violates the terms and conditions of the license.
		private void applicationStart(object sender, StartupEventArgs e)
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
