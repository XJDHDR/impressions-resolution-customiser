// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//
//

using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ImpressionsFileFormats.EngText {

	/// <summary>
	/// Storage class for <see cref="EngTextReaderWriter"/> group data.
	/// </summary>
	public struct EngTextStringGroupAndData
	{
		/// <summary>
		/// The offset from start of the String Data where this group begins.
		/// </summary>
		public int StringDataOffset;

		/// <summary>
		/// The offset from start of the file where this group begins. This will be <see cref="StringDataOffset"/> + 8028
		/// (length of header + all 1000 groups).
		/// </summary>
		public int FileOffset;

		/// <summary>
		/// If the game is "Caesar 3" or "Pharaoh", this will be 0 if the group is unused and 1 if it is used.
		/// If the game is "Zeus" or "Emperor", this will be equal to the number of strings in the group.
		/// </summary>
		public int StringCountOrIsGroupUsed;

		/// <summary>
		/// The strings stored in this <see cref="EngTextStringGroupAndData"/>.
		/// </summary>
		public List<string> Strings;

		public Encoding StringCharacterEncoding;

		public EngTextStringGroupAndData(BinaryReader BinaryReader, GameCharacterEncoding GameCharacterEncoding, in bool IsNewFileFormat)
		{
			StringDataOffset = BinaryReader.ReadInt32();
			FileOffset = StringDataOffset + 8028;
			StringCountOrIsGroupUsed = BinaryReader.ReadInt32();

			switch (StringCountOrIsGroupUsed)
			{
				case 0:
					// There are no strings in this group. Therefore, initialise the String List for 0 strings.
					Strings = new List<string>(0);
					break;

				case 1:
					Strings = IsNewFileFormat ?
						new List<string>(1) :	// File is new format. Therefore, group has only 1 string.
						new List<string>();				// File is old format. Therefore, number of strings is unknown.
					break;

				default:
					// Otherwise, this is the new file format and the value indicates the number of strings present.
					// Initialise the String List with that number of entries.
					Strings = new List<string>(StringCountOrIsGroupUsed);
					break;
			}

			switch (GameCharacterEncoding)
			{
				case GameCharacterEncoding.Win1252:
				default:
					StringCharacterEncoding = Encoding.GetEncoding(1252);
					break;

				case GameCharacterEncoding.Win1250:
					StringCharacterEncoding = Encoding.GetEncoding(1250);
					break;

				case GameCharacterEncoding.Win1251:
					StringCharacterEncoding = Encoding.GetEncoding(1251);
					break;

				case GameCharacterEncoding.Win0950:
					StringCharacterEncoding = Encoding.GetEncoding(950);
					break;
			}
		}
	}
}
