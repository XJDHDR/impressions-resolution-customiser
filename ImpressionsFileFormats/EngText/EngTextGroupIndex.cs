// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//
//

using System.IO;

namespace ImpressionsFileFormats.EngText {

	/// <summary>
	/// Storage class for <see cref="EngText"/> group data.
	/// </summary>
	public struct EngTextGroupIndex
	{
		/// <summary>
		/// The offset from start of the String Data, which marks the start of this group's data.
		/// To calculate offset from start of file, add 8028 (length of header + all 1000 groups).
		/// </summary>
		public int StringDataOffset;

		/// <summary>
		/// If the game is "Caesar 3" or "Pharaoh", this will be 0 if the group is unused and 1 if it is used.
		/// If the game is "Zeus" or "Emperor", this will be equal to the number of strings in the group.
		/// </summary>
		public int StringCountOrIsGroupUsed;

		/// <summary>
		/// Records the number of excess NULLs encountered while reading this group's strings.
		/// Note: This data is not part of the EngText specification. It is used to indicate how much each
		/// group's <see cref="StringDataOffset"/> needs to be adjusted when writing the group.
		/// </summary>
		public int ExcessNullsRead;


		public EngTextGroupIndex(BinaryReader BinaryReader)
		{
			StringDataOffset = BinaryReader.ReadInt32();
			StringCountOrIsGroupUsed = BinaryReader.ReadInt32();
			ExcessNullsRead = 0;
		}
	}
}
