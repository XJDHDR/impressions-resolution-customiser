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
	public struct EngText
	{
		public EngTextHeader FileHeader;
		public EngTextGroupIndex[] StringGroups;
		public EngTextGroupStrings[] Strings;

		public EngText(Stream InputData, GameName GameName, CharEncodingTables GameCharEncoding,
			ref string ErrorMessages, out bool WasSuccessful)
		{
			using (BufferedStream bufferedStream = new BufferedStream(InputData, 8192))
			{
				using (BinaryReader binaryReader = new BinaryReader(bufferedStream))
				{
					// Create the header.
					FileHeader = new EngTextHeader(binaryReader, GameName, GameCharEncoding, ref ErrorMessages,
						out bool wasSuccessful);

					if (wasSuccessful)
					{
						// Create a GroupIndex struct for each of the 1000 String Groups.
						StringGroups = new EngTextGroupIndex[1000];
						for (int i = 0; i < 1000; ++i)
						{
							StringGroups[i] = new EngTextGroupIndex(binaryReader);
						}

						// Next, create a Strings struct for each of the 1000 Groups.
						// Also, keep track of the total number of strings and words, to check header's accuracy.
						uint numStringsRead = 0;
						uint numWordsRead = 0;
						Strings = new EngTextGroupStrings[1000];
						string[] emptyArray = new string[0];
						for (int i = 0; i < 1000; ++i)
						{
							Strings[i] = new EngTextGroupStrings(binaryReader, in emptyArray,
								ref FileHeader, ref StringGroups[i], ref numStringsRead, ref numWordsRead);
						}

						WasSuccessful = true;
					}
					else
					{
						StringGroups = new EngTextGroupIndex[0];
						WasSuccessful = false;
					}
				}
			}
		}
	}

	public enum GameName : byte
	{
		Caesar3 = 1,
		Pharaoh = 2,
		Zeus	= 3,
		Emperor = 4
	}

	public enum CharEncodingTables : byte
	{
		Win1252 = 1,
		Win1250 = 2,
		Win1251 = 3,
		Win0950 = 4
	}
}
