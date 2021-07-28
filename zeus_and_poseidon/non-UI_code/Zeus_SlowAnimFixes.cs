// This code is part of the Impressions Resolution Customiser project
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using static zeus_and_poseidon.Zeus_MakeChanges;

namespace zeus_and_poseidon
{
	/// <summary>
	/// Holds code used to patch the slow animation bugs that Zeus experiences on modern CPUs.
	/// </summary>
	class Zeus_SlowAnimFixes
	{
		internal static void _hexEditExeAnims(ExeLangAndDistrib _exeLangAndDistrib, ref byte[] _zeusExeData)
		{
			if (_fillAnimHexOffsetTable(_exeLangAndDistrib, out int[] _animHexOffsetTable))
			{
				// To explain what this code does, I'll quote what Pecunia, who discovered how to fix these bugs, said to me:
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
				// so it was an easy patch to make. Finding this bug, however, took me about half a year of reading assembly, plus a year of trying to get GOG to incorporate
				// my fixes in the official distribution ("our techs will look at it soon", "really, it'll be soon"), before giving up and releasing it myself.
				//
				for (byte i = 0; i < _animHexOffsetTable.Length; i++)
				{
					_zeusExeData[_animHexOffsetTable[i]] = 0x00;
				}
			}
		}

		private static bool _fillAnimHexOffsetTable(ExeLangAndDistrib _exeLangAndDistrib, out int[] _animHexOffsetTable)
		{
			switch ((byte)_exeLangAndDistrib)
			{
				case 1:         // English GOG version
					_animHexOffsetTable = new int[] { 0x30407, 0xB395D, 0xB3992, 0xB5642, 0xB5AED, 0xB5DE5, 0xB65FF, 0xB69B7, 0xB91D6, 0xB9AB2, 0xB9AFB, 0xB9B7C,
						0xB9DB1, 0xBA007, 0xBAC20, 0xBAC31, 0xBAC42, 0xBAC53, 0xBB1F4, 0xBB381, 0xBB3E5, 0xBB40B, 0xBB431, 0xBB457, 0xBB47D, 0xBB4A3, 0xBB4C9,
						0xBB4EC, 0xBB50F, 0xBB532, 0xBB593, 0xBB5AD, 0xBB5C7, 0xBB5E4, 0xBB656, 0xBD331, 0xBD349, 0xBD3B2, 0xBDC62, 0xBDC7F, 0xBDC9C, 0xBDCB9,
						0xBDD2F, 0xBDDD7, 0xBDE5A, 0xBDE9F, 0xBDEE4, 0xBDF29, 0xBDF6E, 0xBDFB3, 0xBDFF8, 0xBE03D, 0xBE082, 0xBE0C7, 0xBFC43, 0xBFDF8, 0xBFF47,
						0xC26D1, 0xC2740, 0xC28E3, 0xC2904, 0xC2BD8, 0xC3A78, 0xC8415, 0xC84FC, 0xC9DEC, 0xC9E80, 0xCB1D7, 0xCB1F0, 0xCB23F };
					return true;

				default:        // Unrecognised EXE
					_animHexOffsetTable = new int[1];
					return false;
			}
		}
	}
}
