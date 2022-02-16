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
	/// Allows for loading and -saving- of <see cref="EngText"/> files from Caesar 3, Pharaoh/Cleopatra, Zeus/Poseidon, and Emporer. Probably, much testing is needed.
	///
	/// </summary>
	public class EngText {

		/// <summary>
		/// File header.
		/// </summary>
		public EngHeader Header { get; } = new EngHeader();

		/// <summary>
		/// All available groups in this <see cref="ENG"/>.
		/// </summary>
		public List<EngTextGroup> Groups { get; } = new List<EngTextGroup>();

		/// <summary>
		/// Determines if the <see cref="EngTextGroup.StringCount"/> actually stores the number of strings instead of just being 0 or 1 (Caesar 3).
		/// </summary>
		public bool IndexWithCounts = false;

		/// <summary>
		/// Attempts to load an <see cref="EngText"/> file into memory from the input <see cref="Stream"/>.
		/// </summary>
		/// <param name="Input"></param>
		/// <returns></returns>
		public static EngText LoadEng(Stream Input) {

			EngText eng = new EngText();

			using (BinaryReader binaryReader = new BinaryReader(Input))
			{
				// Header
				eng.Header.Magic = Encoding.ASCII.GetString(binaryReader.ReadBytes(16)).Trim('\0');
				eng.Header.GroupCount = binaryReader.ReadInt32();
				eng.Header.StringCount = binaryReader.ReadInt32();
				eng.Header.WordCount = binaryReader.ReadInt32();

				// Groups
				for (int i = 0; i < 1000; i++)
				{
					int offset = binaryReader.ReadInt32();
					int stringCount = binaryReader.ReadInt32();

					if (stringCount > 0)
					{
						eng.Groups.Add(new EngTextGroup { ID = i, Offset = offset, StringCount = stringCount });
						if (stringCount > 1)
						{
							eng.IndexWithCounts = true;
						}
					}
				}

				long baseOffset = binaryReader.BaseStream.Position;
				long textSize = binaryReader.BaseStream.Length - baseOffset;

				// Strings
				for (int i = 0; i < eng.Groups.Count; i++)
				{
					EngTextGroup group = eng.Groups[i];
					long nextGroupOffset = i < eng.Groups.Count - 1 ? eng.Groups[i + 1].Offset : textSize;

					// TODO: If the reading code is working 100% properly this line should have no effect.
					// Testing is required before it is removed.
					binaryReader.BaseStream.Position = baseOffset + group.Offset;

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
								group.Strings.Add(Encoding.ASCII.GetString(bytes.ToArray()));
							}
						}
					}
					else // Handle all of the others much more simply.
					{
						for (int j = 0; j < group.StringCount; j++)
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
								group.Strings.Add(Encoding.ASCII.GetString(bytes.ToArray()));
							}
						}
					}
				}
			}

			return eng;
		}

		/// <summary>
		/// Saves an in-memory <see cref="EngText"/> to the output <see cref="Stream"/>.
		/// </summary>
		/// <param name="Output"></param>
		/// <returns></returns>
		public bool Save(Stream Output) {

			// Here you go XJDHDR, this is the fun part. ^_^;;

			return true;
		}
	}
}
