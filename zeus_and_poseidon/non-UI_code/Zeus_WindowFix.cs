// This code is part of the Impressions Resolution Customiser project
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using static zeus_and_poseidon.Zeus_MakeChanges;

namespace zeus_and_poseidon
{
	/// <summary>
	/// Holds code used to fix a bug that prevents the game from being set to run in windowed mode.
	/// </summary>
	class Zeus_WindowFix
	{
		/// <summary>
		/// Gets the offset that needs to be patched to fix the windowed mode bug then patches it.
		/// </summary>
		/// <param name="_exeLangAndDistrib">Enum that specifies which version of Zeus.exe was detected.</param>
		/// <param name="_zeusExeData">Byte array that contains the binary data contained within the supplied Zeus.exe</param>
		internal static void _hexEditWindowFix(ExeLangAndDistrib _exeLangAndDistrib, ref byte[] _zeusExeData)
		{
			// At this address, the original code had a conditional jump (jl) that activates if the value stored in the EAX register is less than the value stored in the ECX.
			// This patch changes this byte into an unconditional jump.
			// I have no idea what the values represent, what code runs if the condition is false (EAX is greater than ECX) or why the widescreen mods cause
			// EAX to be greater than ECX. All I know is that it makes Windowed mode work.
			if (_identifyWinFixOffset(_exeLangAndDistrib, out int _winFixOffset))
			{
				_zeusExeData[_winFixOffset] = 0xEB;
			}
		}

		/// <summary>
		/// Supplies an int specifying the offset that needs to be patched, based on which version of Zeus.exe was supplied.
		/// </summary>
		/// <param name="_exeLangAndDistrib">Enum that specifies which version of Zeus.exe was detected.</param>
		/// <param name="_animHexOffsetTable">Int containing the offset for the supplied Zeus.exe that needs patching.</param>
		/// <returns>
		/// True if "_exeLangAndDistrib" matches one that this program knows about and knows the offsets that need to be patched.
		/// False if the EXE is not recognised.
		/// </returns>
		private static bool _identifyWinFixOffset(ExeLangAndDistrib _exeLangAndDistrib, out int _winFixOffset)
		{
			switch ((byte)_exeLangAndDistrib)
			{
				case 1:         // English GOG version
					_winFixOffset = 0x212606;
					return true;

				default:        // Unrecognised EXE
					_winFixOffset = 0;
					return false;
			}
		}
	}
}
