// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

namespace Zeus_and_Poseidon.non_UI_code
{
	/// <summary>
	/// Holds code used to patch the slow animation bugs that Zeus experiences on modern CPUs.
	/// </summary>
	internal static class ZeusSlowAnimFixes
	{
		/// <summary>
		/// Gets the list of offsets that need to be patched to fix the animation bugs then patches them.
		/// </summary>
		/// <param name="ExeAttributes">Struct that specifies various details about the detected Zeus.exe</param>
		/// <param name="ZeusExeData">Byte array that contains the binary data contained within the supplied Zeus.exe</param>
		internal static void _hexEditExeAnims(ExeAttributes ExeAttributes, ref byte[] ZeusExeData)
		{
			if (ZeusExeDefinitions._FillAnimHexOffsetTable(ExeAttributes, out int[] animHexOffsetTable))
			{
				// To explain what this code does, I'll quote what Pecunia (who discovered how to fix these bugs) said to me:
				// https://www.wsgf.org/phpBB3/viewtopic.php?p=172648#p172648
				//
				// Some animation basics:
				// - Real-time animations are based on (a multiple of) a 20 millisecond update cycle
				// - Every 20ms, a flag is set for that render-cycle, indicating that the animation can be advanced
				// - Some figures in the game, notably gods and urchin collectors, use these (real-time) animations to advance their (game-time) actions
				//
				// This worked well in 2000 when there was one render-cycle per 20ms, such that an animation update coincided with a "game time" advance.
				// With modern hardware, we have multiple render cycles per 20ms interval, causing the game ticks to 'miss' their animation update and thus act incredibly slow.
				//
				// The 0x64(100) is the parameter that tells the game: "advance this figure ONLY if the animation should advance as well".
				// Since that no longer works properly with today's hardware, I changed it to "0", meaning: "always advance". Somehow this worked perfectly well gameplay-wise,
				// so it was an easy patch to make. Finding this quirk, however, took me about half a year of reading assembly, plus a year of trying to get GOG to incorporate
				// my fixes in the official distribution ("our techs will look at it soon", "really, it'll be soon"), before giving up and releasing it myself.
				//
				for (byte i = 0; i < animHexOffsetTable.Length; i++)
				{
					ZeusExeData[animHexOffsetTable[i]] = 0x00;
				}
			}
		}
	}
}
