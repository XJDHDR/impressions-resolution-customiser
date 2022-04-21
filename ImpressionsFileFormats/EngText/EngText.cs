// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using System.IO;
using System.Text;

namespace ImpressionsFileFormats.EngText
{
	public struct EngText
	{
		public EngTextHeader FileHeader;
		public EngTextGroupIndex[] StringGroupIndexes;
		public EngTextStringTable[] AllStringsByGroup;

		/// <summary>
		/// Used to read the data in an EngText file into a usable format.
		/// </summary>
		/// <param name="InputData">A Stream pointing to the EngText data.</param>
		/// <param name="GameName">The name of the game that the EngText data belongs to.</param>
		/// <param name="GameCharEncoding">the character Encoding used by the game and EngText data.</param>
		/// <param name="Messages">Outputs any error messages or warnings generated by the reading code.</param>
		/// <param name="WasSuccessful">True if the EngText file was successfully read. False if an error occurred.</param>
		public EngText(Stream InputData, Game GameName, CharEncodingTables GameCharEncoding, int DefaultStringCount, int DefaultWordCount,
			out string Messages, out bool WasSuccessful)
		{
			using (BufferedStream bufferedStream = new BufferedStream(InputData, 8192))
			{
				using (BinaryReader binaryReader = new BinaryReader(bufferedStream))
				{
					StringBuilder messagesSb = new StringBuilder();

					// Create the header.
					FileHeader = new EngTextHeader(binaryReader, GameName, GameCharEncoding, ref messagesSb,
						out bool wasSuccessful);

					if (wasSuccessful)
					{
						// Create a GroupIndex struct for each of the 1000 String Groups.
						StringGroupIndexes = new EngTextGroupIndex[1000];
						for (int i = 0; i < 1000; ++i)
						{
							StringGroupIndexes[i] = new EngTextGroupIndex(binaryReader);
						}

						// Next, create a Strings struct for each of the 1000 Groups.
						// Also, keep track of the total number of strings and words, to check header's accuracy.
						int numStringsRead = 0;
						int numWordsRead = 0;
						AllStringsByGroup = new EngTextStringTable[1000];
						string[] emptyArray = new string[0];
						for (int i = 0; i < 1000; ++i)
						{
							int nextGroupOffset = i >= FileHeader.GroupCount - 1 ?
								(int)binaryReader.BaseStream.Length :
								StringGroupIndexes[i + 1].StringDataOffset + 8028;

							AllStringsByGroup[i] = new EngTextStringTable(binaryReader, messagesSb, nextGroupOffset, in emptyArray,
								ref FileHeader, ref StringGroupIndexes[i], ref numStringsRead, ref numWordsRead,
								out wasSuccessful);

							if (!wasSuccessful)
							{
								// Since the last string reader run encountered an error, cancel reading the remaining strings.
								Messages = messagesSb.ToString();
								WasSuccessful = false;
								return;
							}
						}

						// Check if the number of strings read matches the count noted in the Header. Correct if not.
						if (numStringsRead != FileHeader.StringCount && numStringsRead != DefaultStringCount)
						{
							messagesSb.Append($"Warning: The EngText Header says that there are {FileHeader.StringCount.ToString()} strings present, ");
							messagesSb.Append($"but {numStringsRead} were read instead. This discrepancy has been corrected\n\n");
							messagesSb.Append("This can be caused by modifications to the EngText file.\n\n");
							FileHeader.StringCount = numStringsRead;
						}

						// Check if the number of words read matches the count noted in the Header. Correct if not.
						if (numWordsRead != FileHeader.WordCount && numWordsRead != DefaultWordCount)
						{
							messagesSb.Append($"Warning: The EngText Header says that there are {FileHeader.WordCount.ToString()} strings present, ");
							messagesSb.Append($"but {numWordsRead} were read instead. This discrepancy has been corrected\n\n");
							messagesSb.Append("This can be caused by modifications to the EngText file.\n\n");
							FileHeader.WordCount = numWordsRead;
						}

						Messages = messagesSb.ToString();
						WasSuccessful = true;
					}
					else
					{
						StringGroupIndexes = new EngTextGroupIndex[0];
						AllStringsByGroup = new EngTextStringTable[0];
						Messages = messagesSb.ToString();
						WasSuccessful = false;
					}
				}
			}
		}

		/// <summary>
		/// Writes the contents of this EngText struct into a provided Stream.
		/// </summary>
		/// <param name="Output">A stream that the EngText data will be written to.</param>
		public void Save(Stream Output)
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(Output, FileHeader.StringCharEncoding))
			{
				// Write the new file's header
				binaryWriter.Write(FileHeader.FileSignature);
				binaryWriter.Write(FileHeader.GroupCount);
				binaryWriter.Write(FileHeader.StringCount);
				binaryWriter.Write(FileHeader.WordCount);

				// Next, write the file's 1000 String Group Indexes
				for (int i = 0; i < 1000; ++i)
				{
					binaryWriter.Write(StringGroupIndexes[i].StringDataOffset);
					binaryWriter.Write(StringGroupIndexes[i].StringCountOrIsGroupUsed);
				}

				// After that, write the String Table
				for (int i = 0; i < 1000; ++i)
				{
					if (i >= FileHeader.GroupCount)
					{
						break;
					}

					for (int j = 0; j < AllStringsByGroup[i].StringsInGroup.Length; ++j)
					{
						byte[] stringBytes = FileHeader.StringCharEncoding.GetBytes(AllStringsByGroup[i].StringsInGroup[j]);
						binaryWriter.Write(stringBytes);
						binaryWriter.Write((byte)0);
					}
				}

				// Finally, add a NULL byte to the end of the file
				binaryWriter.Write((byte)0);
			}
		}
	}

	public enum Game : byte
	{
		Caesar3 = 1,
		Pharaoh = 2,
		Zeus	= 3,
		Emperor = 4
	}

	public enum CharEncodingTables : byte
	{
		/// <summary> West European languages </summary>
		Win1252 = 1,
		/// <summary> Polish </summary>
		Win1250 = 2,
		/// <summary> Russian </summary>
		Win1251 = 3,
		/// <summary> Korean </summary>
		Win0950 = 4
	}
}
