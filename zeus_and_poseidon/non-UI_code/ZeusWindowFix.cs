// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

namespace Zeus_and_Poseidon.non_UI_code
{
	/// <summary>
	/// Holds code used to fix a quirk that prevents the game from being set to run in windowed mode.
	/// </summary>
	internal readonly struct ZeusWindowFix
	{
		private readonly int winFixOffset;
		private readonly byte winFixNewByte;

		internal ZeusWindowFix(ZeusExeAttributes ExeAttributes, out bool WasSuccessful)
		{
			switch (ExeAttributes._SelectedExeLangAndDistrib)
			{
				case ExeLangAndDistrib.GogAndSteamEnglish:
					winFixOffset = 0x33E7E;
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
		/// <param name="ZeusExeData">Byte array that contains the binary data contained within the supplied Emperor.exe</param>
		internal void _hexEditWindowFix(ref byte[] ZeusExeData) =>
			// At this address, the original code had a conditional jump (jl) that activates if the value stored in the EAX register is less than the value stored in the ECX.
			// This patch changes this byte into an unconditional jump.
			// I have no idea what the values represent, what code runs if the condition is false (EAX is greater than ECX) or why the widescreen mods cause
			// EAX to be greater than ECX. All I know is that it makes Windowed mode work.
			ZeusExeData[winFixOffset] = winFixNewByte;
	}
}
