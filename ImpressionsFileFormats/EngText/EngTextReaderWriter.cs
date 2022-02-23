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
	/// Allows for loading and -saving- of <see cref="EngTextReaderWriter"/> files from Caesar 3, Pharaoh/Cleopatra, Zeus/Poseidon, and Emporer. Probably, much testing is needed.
	///
	/// </summary>
	public class EngTextReaderWriter {
		/// <summary>
		/// File header.
		/// </summary>
		public EngTextHeader TextHeader { get; } = new EngTextHeader();

		/// <summary>
		/// All available groups in this <see cref="ENG"/>.
		/// </summary>
		public List<EngTextStringGroupAndData> Groups { get; } = new List<EngTextStringGroupAndData>();

		/// <summary>
		/// Determines if the <see cref="EngTextStringGroupAndData.StringCountOrIsGroupUsed"/> actually stores the number of strings instead of just being 0 or 1 (Caesar 3).
		/// </summary>
		public bool IndexWithCounts = false;

		/// <summary>
		/// Attempts to load an <see cref="EngTextReaderWriter"/> file into memory from the input <see cref="Stream"/>.
		/// </summary>
		/// <param name="Input"></param>
		/// <param name="GameName"></param>
		/// <param name="ErrorMessages"></param>
		/// <param name="EngText"></param>
		/// <param name="GameCharacterEncoding"></param>
		/// <returns></returns>
		public static EngTextReaderWriter LoadEng(Stream Input, GameName GameName, GameCharacterEncoding GameCharacterEncoding,
			ref string ErrorMessages, out EngText EngText)
		{
			EngTextReaderWriter eng = new EngTextReaderWriter();

			using (BinaryReader binaryReader = new BinaryReader(Input))
			{
				// Run the constructor that fills the field values by reading them from the input stream.
				EngText = new EngText(Input, GameName, GameCharacterEncoding, ref ErrorMessages, out bool wasSuccessful);

				// Strings
				for (int i = 0; i < eng.Groups.Count; i++)
				{
					EngTextStringGroupAndData stringGroupAndData = eng.Groups[i];
					long nextGroupOffset = i < eng.Groups.Count - 1 ?
						eng.Groups[i + 1].FileOffset :
						textSize;

					// TODO: If the reading code is working 100% properly this line should have no effect.
					// Testing is required before it is removed.
					binaryReader.BaseStream.Position = baseOffset + stringGroupAndData.FileOffset;

					// Engage Caesar 3 string reading loop.
					if (!eng.IndexWithCounts)
					{
						while (binaryReader.BaseStream.Position - baseOffset < nextGroupOffset)
						{
							List<byte> bytes = new List<byte>();
							byte currentByte = binaryReader.ReadByte();

							while (currentByte != 0)
							{
								bytes.Add(currentByte);
								currentByte = binaryReader.ReadByte();
							}

							if (bytes.Count > 0)
							{
								// TODO: Encoding support for the various game language versions will need to be implemented as this is currently only ASCII compatible.
								stringGroupAndData.Strings.Add(Encoding.ASCII.GetString(bytes.ToArray()));
							}
						}
					}
					else // Handle all of the others much more simply.
					{
						for (int j = 0; j < stringGroupAndData.StringCountOrIsGroupUsed; j++)
						{
							List<byte> bytes = new List<byte>();
							byte currentByte = binaryReader.ReadByte();
							while (currentByte != 0)
							{
								bytes.Add(currentByte);
								currentByte = binaryReader.ReadByte();
							}

							if (bytes.Count > 0)
							{
								// TODO: Encoding support for the various game language versions will need to be implemented as this is currently only ASCII compatible.
								stringGroupAndData.Strings.Add(Encoding.ASCII.GetString(bytes.ToArray()));
							}
						}
					}
				}
			}

			return eng;
		}

		/// <summary>
		/// Saves an in-memory <see cref="EngTextReaderWriter"/> to the output <see cref="Stream"/>.
		/// </summary>
		/// <param name="Output"></param>
		/// <returns></returns>
		public bool Save(Stream Output) {

			// Here you go XJDHDR, this is the fun part. ^_^;;

			return true;
		}
	}
}
