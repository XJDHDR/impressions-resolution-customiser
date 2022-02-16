// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//
//

using System.Collections.Generic;

namespace ImpressionsFileFormats.EngText {

	/// <summary>
	/// Storage class for <see cref="EngTextReaderWriter"/> group data.
	/// </summary>
	public class EngTextStringGroup {

		/// <summary>
		/// The ID of the group. (Usually it's the index.)
		/// </summary>
		public int ID { get; set; }

		/// <summary>
		/// Offset to the group in the file relative to the end of the indexes.
		/// </summary>
		public int Offset { get; set; }

		/// <summary>
		/// The number of strings in the group.
		/// If all <see cref="EngTextStringGroup"/>s have a <see cref="StringCount"/> of 0 or 1, then the per-group string count is
		/// unknown and you have to read as many strings as possible before reaching the next group.
		/// (Caesar 3)
		/// </summary>
		public int StringCount { get; set; }

		/// <summary>
		/// The strings stored in this <see cref="EngTextStringGroup"/>.
		/// </summary>
		public List<string> Strings { get; set; } = new List<string>();
	}
}
