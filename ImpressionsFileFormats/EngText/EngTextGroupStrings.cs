// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ImpressionsFileFormats.EngText
{
	public struct EngTextGroupStrings
	{
		/// <summary>
		/// The strings stored in the likewise indexed <see cref="EngTextGroupIndex"/>.
		/// </summary>
		public string[] GroupStrings;

		public EngTextGroupStrings(BinaryReader BinaryReader, in string[] EmptyArray,
			ref EngTextHeader Header, ref EngTextGroupIndex GroupIndex,
			ref uint NumStringsRead, ref uint NumWordsRead, ref uint ExcessNullsRead)
		{
			// NOTE: DO NOT MODIFY ANYTHING IN Header OR GroupIndex. The Ref keyword is there for performance reasons.
			List<string> tempGroupStringsContainer;

			switch (GroupIndex.StringCountOrIsGroupUsed)
			{
				case 0:
					// There are no strings in this group. Therefore, use the provided 0 length array and stop.
					GroupStrings = EmptyArray;
					return;

				case 1:
					tempGroupStringsContainer = Header.IsNewFileFormat ?
						new List<string>(1) :	// File is new format. Therefore, group has only 1 string.
						new List<string>();				// File is old format. Therefore, number of strings is unknown.
					break;

				default:
					// Otherwise, this is the new file format and the value indicates the number of strings present.
					// Initialise the String List with that number of entries.
					tempGroupStringsContainer = new List<string>(GroupIndex.StringCountOrIsGroupUsed);
					break;
			}

			int numberOfStringsInGroup = Header.IsNewFileFormat ?
				GroupIndex.StringCountOrIsGroupUsed :
				int.MaxValue;

			for (int i = 0; i < numberOfStringsInGroup; ++i)
			{
				readStringUntilNullEncountered(BinaryReader, Header, ref NumStringsRead,
					ref NumWordsRead, ref ExcessNullsRead, out string readString);
				tempGroupStringsContainer.Add(readString);

				// If this is the old format, check for when the end of the group has been reached.
				if (!Header.IsNewFileFormat)
				{
					// TODO: Correct this
					if (BinaryReader.BaseStream.Position == GroupIndex.StringDataOffset + 8028)
					{
						break;
					}
				}
			}

			GroupStrings = tempGroupStringsContainer.ToArray();
		}

		private static void readStringUntilNullEncountered(BinaryReader BinaryReader, EngTextHeader Header,
			ref uint NumStringsRead, ref uint NumWordsRead, ref uint ExcessNullsRead, out string ReadString)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool lastCharacterWasSpace = false;
			bool haveReachedNullCharacters = false;

			for (int i = 0; i < int.MaxValue; ++i)
			{
				byte[] readByte = BinaryReader.ReadBytes(1);

				if (haveReachedNullCharacters)
				{
					// It's possible for a string to be terminated by multiple NULLs. Therefore, keep reading until the next non-NULL is found.
					if (readByte[0] != 0x00)
					{
						BinaryReader.BaseStream.Position -= 1;
						break;
					}

					// If there are excess NULLs, count the number so that the Group Index values can be adjusted.
					++ExcessNullsRead;
				}
				else
				{
					if (readByte[0] == 0x00)
					{
						// If a NULL was read, the end of the string was encountered. Make a note so that
						// the start of the next string can be located.
						haveReachedNullCharacters = true;
					}
					else
					{
						// If a space was encountered, use it to count the number of words read so far.
						if (readByte[0] == 0x20)
						{
							lastCharacterWasSpace = true;
						}
						else if (lastCharacterWasSpace)
						{
							++NumWordsRead;
							lastCharacterWasSpace = false;
						}

						stringBuilder.Append(Header.StringCharEncoding.GetChars(readByte));
					}
				}
			}

			ReadString = stringBuilder.ToString();
			++NumWordsRead;
			++NumStringsRead;
		}
	}
}
