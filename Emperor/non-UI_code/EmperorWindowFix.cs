// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

namespace Emperor.non_UI_code
{
	/// <summary>
	/// Holds code used to fix a quirk that prevents the game from being set to run in windowed mode.
	/// </summary>
	internal readonly struct EmperorWindowFix
	{
		private readonly int winFixOffset;
		private readonly byte winFixNewByte;

		internal EmperorWindowFix(EmperorExeAttributes ExeAttributes, out bool WasSuccessful)
		{
			switch (ExeAttributes._SelectedExeLangAndDistrib)
			{
				case ExeLangAndDistrib.GogEnglish:
					winFixOffset = 0x4C62E;
					winFixNewByte = 0xEB;
					WasSuccessful = true;
					return;

				case ExeLangAndDistrib.CdEnglish:
					winFixOffset = 0x4D22E;
					winFixNewByte = 0xEB;
					WasSuccessful = true;
					return;

				case ExeLangAndDistrib.NotRecognised:
				default:
					winFixOffset = 0;
					winFixNewByte = 0;
					WasSuccessful = false;
					return;
			}
		}

		/// <summary>
		/// Gets the offset that needs to be patched to fix the windowed mode quirk then patches it.
		/// </summary>
		/// <param name="EmperorExeData">Byte array that contains the binary data contained within the supplied Emperor.exe</param>
		internal void _hexEditWindowFix(ref byte[] EmperorExeData) =>
			// At this address, the original code had a conditional jump (jl) that activates if the value stored in the EAX register is less than the value stored in the ECX.
			// This patch changes this byte into an unconditional jump.
			// I have no idea what the values represent, what code runs if the condition is false (EAX is greater than ECX) or why the widescreen mods cause
			// EAX to be greater than ECX. All I know is that it makes Windowed mode work.
			EmperorExeData[winFixOffset] = winFixNewByte;
	}
}
