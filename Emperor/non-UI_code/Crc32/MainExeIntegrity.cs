// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Emperor.non_UI_code.Crc32
{
	internal static class MainExeIntegrity
	{
		private const int CHECKSUM_ADDRESS = 0x1000;
		private const int IGNORE_LENGTH = 1;

		private static byte[] mainExeData = {};

		// Please note that, as per the repo's license noted above, you are not permitted to modify anything in this class
		// in any way that violates the terms and conditions of the license.
		internal static void _Check()
		{
			if (mainExeData.Length == 0)
			{
				string location = Assembly.GetEntryAssembly()?.Location;
				if (location != null)
				{
					List<byte> mainExeDataWithCheck = new List<byte>(File.ReadAllBytes(location));
					mainExeDataWithCheck.RemoveRange(CHECKSUM_ADDRESS, IGNORE_LENGTH);
					mainExeData = mainExeDataWithCheck.ToArray();
				}
				else
				{
					MessageBox.Show(StringsDatabase._MainExeIntegrityCouldNotFindExe);
					Application.Current.Shutdown();
				}
			}

			uint exeCrc32Checksum = SliceBy16.Crc32(0x0, mainExeData);
			if (exeCrc32Checksum == 0xFEFFFEFF)
			{
				return;
			}

			MessageBox.Show(StringsDatabase._MainExeIntegrityDataCorruptionDetected);
			Application.Current.Shutdown();
		}
	}
}
