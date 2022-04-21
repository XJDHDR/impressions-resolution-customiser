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
	public struct EngTextStringTable
	{
		/// <summary>
		/// The strings stored in the likewise indexed <see cref="EngTextGroupIndex"/>.
		/// </summary>
		public string[] StringsInGroup;

		public EngTextStringTable(BinaryReader BinaryReader, StringBuilder Messages, int NextGroupOffset, in string[] EmptyArray,
			ref EngTextHeader Header, ref EngTextGroupIndex GroupIndex, ref int NumStringsRead, ref int NumWordsRead,
			out bool WasSuccessful)
		{
			List<string> tempGroupStringsContainer;

			switch (GroupIndex.StringCountOrIsGroupUsed)
			{
				case 0:
					// There are no strings in this group. Therefore, use the provided 0 length array and immediately stop.
					StringsInGroup = EmptyArray;
					WasSuccessful = true;
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
				GroupIndex.StringCountOrIsGroupUsed :				// If new format, Index has number of strings in group.
				int.MaxValue;										// If old format, just read strings until next offset.

			for (int i = 0; i < numberOfStringsInGroup; ++i)
			{
				readStringUntilNullEncountered(BinaryReader, ref Header, ref NumStringsRead,
					ref NumWordsRead, ref GroupIndex.ExcessNullsRead, out string readString);
				tempGroupStringsContainer.Add(readString);

				// Check whether the stream reader has moved past the next group's offset
				if (BinaryReader.BaseStream.Position > NextGroupOffset)
				{
					// The reader has gone past the offset for the next group, meaning
					// the string that was just read runs past the group's boundary.
					// This means the EngText file is not in a working state.
					// Therefore, stop reading strings and report the error.

					if (Header.IsNewFileFormat)
					{
						Messages.Append($"Error: The string reading code went past the start of the next String Group (at offset: {NextGroupOffset.ToString()}) ");
						Messages.Append($"after reading {i} strings, and is currently at offset {BinaryReader.BaseStream.Position.ToString()}. ");
						Messages.Append("The last string was supposed to end at the start of the next group at ");
						Messages.Append($"offset {NextGroupOffset.ToString()}, not after that offset.\n");
						Messages.Append("This indicates a corrupt EngText file and reading can not continue.\n\n");
					}
					else
					{
						Messages.Append($"Error: The string reading code went past the start of the next String Group (at offset: {NextGroupOffset.ToString()}) ");
						Messages.Append("before reading the number of strings that are supposed to be in this group ");
						Messages.Append($"({i} of {numberOfStringsInGroup.ToString()} strings have been read so far), ");
						Messages.Append($"and is currently at offset {BinaryReader.BaseStream.Position.ToString()}.\n");
						Messages.Append("This indicates a corrupt EngText file and reading can not continue.\n\n");
					}

					StringsInGroup = tempGroupStringsContainer.ToArray();
					WasSuccessful = false;
					return;
				}

				if (!Header.IsNewFileFormat)
				{
					// If this is the old format, check for when the end of the group has been reached.
					if (BinaryReader.BaseStream.Position == NextGroupOffset)
					{
						// The start of the next group has been encountered.
						// Thus, stop reading strings for this group and move on to the next.
						break;
					}
				}
			}

			StringsInGroup = tempGroupStringsContainer.ToArray();
			WasSuccessful = true;
		}

		private static void readStringUntilNullEncountered(BinaryReader BinaryReader, ref EngTextHeader Header,
			ref int NumStringsRead, ref int NumWordsRead, ref int ExcessNullsRead,
			out string ReadString)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool lastCharacterWasSpace = false;
			bool haveReachedNullCharacters = false;

			for (int i = 0; i < int.MaxValue; ++i)
			{
				// Immediately abort if the end of the stream has been encountered
				if (BinaryReader.BaseStream.Position >= BinaryReader.BaseStream.Length)
				{
					break;
				}

				byte[] readByte = BinaryReader.ReadBytes(1);

				if (haveReachedNullCharacters)
				{
					// It's possible for a string to be terminated by multiple NULLs. Therefore, keep reading until the next non-NULL is found.
					if (readByte[0] != 0x00)
					{
						BinaryReader.BaseStream.Position -= 1;
						break;
					}

					// If there is more than 1 NULL, count the number so that the Group Index values can be adjusted.
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
