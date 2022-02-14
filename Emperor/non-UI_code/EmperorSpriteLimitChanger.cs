// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

namespace Emperor.non_UI_code
{
	/// <summary>
	/// Class used to change Emperor's sprite limit.
	/// </summary>
	internal static class EmperorSpriteLimitChanger
	{
		/// <summary>
		/// Used to make the required changes to the EXE.
		/// </summary>
		/// <param name="ExeAttributes">Struct that specifies various details about the detected Emperor.exe</param>
		/// <param name="EmperorExeData">Byte array that contains the binary data contained within the supplied Emperor.exe</param>
		internal static void _MakeChanges(ExeAttributes ExeAttributes, ref byte[] EmperorExeData)
		{
			LimitOffsets limitOffsets = new LimitOffsets(ExeAttributes, out bool wasSuccessful);

			if (wasSuccessful)
			{
				EmperorExeData[limitOffsets._LimitOffset1 + 0] = 0xA0;
				EmperorExeData[limitOffsets._LimitOffset1 + 1] = 0x0F;
				EmperorExeData[limitOffsets._LimitOffset2 + 0] = 0xA0;
				EmperorExeData[limitOffsets._LimitOffset2 + 1] = 0x0F;
			}
		}

		/// <summary>
		/// Used to define the offsets the game uses to define the sprite limits.
		/// </summary>
		private readonly struct LimitOffsets
		{
			internal readonly int _LimitOffset1;
			internal readonly int _LimitOffset2;

			/// <summary>
			/// Test which version of the game is being patched and set the offset values appropriately.
			/// </summary>
			/// <param name="ExeAttributes">Struct that specifies various details about the detected Emperor.exe</param>
			/// <param name="WasSuccessful">Set to True if the EXE edition was recognised. False otherwise.</param>
			internal LimitOffsets(ExeAttributes ExeAttributes, out bool WasSuccessful)
			{
				switch (ExeAttributes._SelectedExeLangAndDistrib)
				{
					case ExeLangAndDistrib.GogEnglish:
						_LimitOffset1 = 0x2CD20;
						_LimitOffset2 = 0x2CD64;
						WasSuccessful = true;
						return;

					case ExeLangAndDistrib.CdEnglish:
						_LimitOffset1 = 0;
						_LimitOffset2 = 0;
						WasSuccessful = true;
						return;

					case ExeLangAndDistrib.NotRecognised:
					default:
						_LimitOffset1 = 0;
						_LimitOffset2 = 0;
						WasSuccessful = false;
						return;
				}
			}
		}
	}
}
