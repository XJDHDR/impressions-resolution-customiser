// This code is part of the Impressions Resolution Customiser project
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

namespace Emperor
{
	/// <summary>
	/// Holds code used to fix a bug that prevents the game from being set to run in windowed mode.
	/// </summary>
	class Emperor_WindowFix
	{
		/// <summary>
		/// Gets the offset that needs to be patched to fix the windowed mode bug then patches it.
		/// </summary>
		/// <param name="ExeAttributes">Struct that specifies various details about the detected Emperor.exe</param>
		/// <param name="EmperorExeData">Byte array that contains the binary data contained within the supplied Emperor.exe</param>
		internal static void _hexEditWindowFix(ExeAttributes ExeAttributes, ref byte[] EmperorExeData)
		{
			// At this address, the original code had a conditional jump (jl) that activates if the value stored in the EAX register is less than the value stored in the ECX.
			// This patch changes this byte into an unconditional jump.
			// I have no idea what the values represent, what code runs if the condition is false (EAX is greater than ECX) or why the widescreen mods cause
			// EAX to be greater than ECX. All I know is that it makes Windowed mode work.
			if (Emperor_ExeDefinitions.IdentifyWinFixOffset(ExeAttributes, out int _winFixOffset))
			{
				EmperorExeData[_winFixOffset] = 0xEB;
			}
		}
	}
}
