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
		/// <param name="_ExeAttributes_">Struct that specifies various details about the detected Emperor.exe</param>
		/// <param name="_EmperorExeData_">Byte array that contains the binary data contained within the supplied Emperor.exe</param>
		internal static void _MakeChanges(ExeAttributes _ExeAttributes_, ref byte[] _EmperorExeData_)
		{
			LimitOffsets _limitOffsets_ = new LimitOffsets(_ExeAttributes_, out bool _wasSuccessful_);

			if (_wasSuccessful_)
			{
				_EmperorExeData_[_limitOffsets_._LimitOffset1 + 0] = 0xA0;
				_EmperorExeData_[_limitOffsets_._LimitOffset1 + 1] = 0x0F;
				_EmperorExeData_[_limitOffsets_._LimitOffset2 + 0] = 0xA0;
				_EmperorExeData_[_limitOffsets_._LimitOffset2 + 1] = 0x0F;
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
			/// <param name="_ExeAttributes_">Struct that specifies various details about the detected Emperor.exe</param>
			/// <param name="_WasSuccessful_">Set to True if the EXE edition was recognised. False otherwise.</param>
			internal LimitOffsets(ExeAttributes _ExeAttributes_, out bool _WasSuccessful_)
			{
				switch (_ExeAttributes_._SelectedExeLangAndDistrib)
				{
					case ExeLangAndDistrib.GogEnglish:
						_LimitOffset1 = 0x2CD20;
						_LimitOffset2 = 0x2CD64;
						_WasSuccessful_ = true;
						break;

					case ExeLangAndDistrib.CdEnglish:
						_LimitOffset1 = 0;
						_LimitOffset2 = 0;
						_WasSuccessful_ = true;
						break;

					case ExeLangAndDistrib.NotRecognised:
					default:
						_LimitOffset1 = 0;
						_LimitOffset2 = 0;
						_WasSuccessful_ = false;
						break;
				}
			}
		}
	}
}
