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
		public EngTextStringGroupAndData[] StringGroups;

		public EngText(Stream InputData, GameName GameName, GameCharacterEncoding GameCharacterEncoding, ref string ErrorMessages, out bool WasSuccessful)
		{
			using (BufferedStream bufferedStream = new BufferedStream(InputData, 8192))
			{
				using (BinaryReader binaryReader = new BinaryReader(bufferedStream))
				{
					FileHeader = new EngTextHeader(binaryReader, GameName, ref ErrorMessages, out bool wasSuccessful);

					if (wasSuccessful)
					{
						StringGroups = new EngTextStringGroupAndData[1000];
						for (int i = 0; i < StringGroups.Length; ++i)
						{
							StringGroups[i] = new EngTextStringGroupAndData(binaryReader, GameCharacterEncoding, in FileHeader.IsNewFileFormat);
						}

						uint numStringsRead = 0;
						uint numWordsRead = 0;
						for (int i = 0; i < StringGroups.Length; ++i)
						{
							if (StringGroups[i].StringCountOrIsGroupUsed == 0)
							{
								continue;
							}

							int numberOfStringsInGroup;
							if (FileHeader.IsNewFileFormat)
							{
								numberOfStringsInGroup = StringGroups[i].StringCountOrIsGroupUsed;
							}
							else
							{
								numberOfStringsInGroup = int.MaxValue;
							}

							for (int j = 0; j < numberOfStringsInGroup; ++j)
							{
								readStringUntilNullEncountered(binaryReader, StringGroups[i], ref numStringsRead, ref numWordsRead, out string readString);
								StringGroups[i].Strings.Add(readString);
							}
						}

						WasSuccessful = true;
					}
					else
					{
						StringGroups = new EngTextStringGroupAndData[0];
						WasSuccessful = false;
					}
				}
			}
		}

		private void readStringUntilNullEncountered(BinaryReader BinaryReader, EngTextStringGroupAndData CurrentGroup,
			ref uint NumStringsRead, ref uint NumWordsRead, out string ReadString)
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
				}
				else
				{
					if (readByte[0] == 0x00)
					{
						haveReachedNullCharacters = true;
					}
					else
					{
						if (readByte[0] == 0x20)
						{
							lastCharacterWasSpace = true;
						}
						else if (lastCharacterWasSpace)
						{
							++NumWordsRead;
							lastCharacterWasSpace = false;
						}

						stringBuilder.Append(CurrentGroup.StringCharacterEncoding.GetChars(readByte));
					}
				}
			}

			ReadString = stringBuilder.ToString();
			++NumWordsRead;
			++NumStringsRead;
		}
	}

	public enum GameName : byte
	{
		Caesar3 = 1,
		Pharaoh = 2,
		Zeus	= 3,
		Emperor = 4
	}

	public enum GameCharacterEncoding : byte
	{
		Win1252 = 1,
		Win1250 = 2,
		Win1251 = 3,
		Win0950 = 4
	}
}
