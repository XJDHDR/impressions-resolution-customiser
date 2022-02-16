// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//
//

namespace ImpressionsFileFormats.EngText {
	/// <summary>
	/// Storage class for <see cref="EngTextReaderWriter"/> header data.
	/// </summary>
	public class EngTextHeader {

		/// <summary>
		/// Magic string that identifies the file.
		/// </summary>
		public string Magic { get; set; }

		/// <summary>
		/// Number of in-use <see cref="EngTextStringGroup"/>s in the file.
		/// </summary>
		public int GroupCount { get; set; }

		/// <summary>
		/// Number of individual strings in the file.
		/// </summary>
		public int StringCount { get; set; }

		/// <summary>
		/// Number of individual words in the file. May be innacurate according to bvschaik.
		/// </summary>
		public int WordCount { get; set; }
	}
}
