// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//
//

using System.IO;
using System.Text;

namespace ImpressionsFileFormats.EngText {
	/// <summary>
	/// Storage class for <see cref="EngText"/> header data.
	/// </summary>
	public struct EngTextHeader
	{
		/// <summary>
		/// 16 byte string that constitutes the File Signature.
		/// </summary>
		public readonly byte[] FileSignature;

		/// <summary>
		/// Number of in-use <see cref="EngTextGroupIndex"/>es in the file.
		/// </summary>
		public readonly int GroupCount;

		/// <summary>
		/// Number of individual strings in the file. Not necessarily accurate but inaccurate values are corrected by my reading code.
		/// </summary>
		public int StringCount;

		/// <summary>
		/// Number of individual words in the file. Not necessarily accurate but inaccurate values are corrected by my reading code.
		/// </summary>
		public int WordCount;

		/// <summary>
		/// False if the file is in the old format used by Caesar 3 and Pharaoh.
		/// True if it is in the new format used by Zeus and Emperor.
		/// Note that this data is not part of the EngText specification. It is to assist with reading & writing the string groups.
		/// </summary>
		public readonly bool IsNewFileFormat;

		/// <summary>
		/// The <see cref="Encoding"/> used for the strings in this EngText file.
		/// Note that this data is not part of the Eng Text specification. It is here to assist with reading & writing the strings.
		/// </summary>
		public readonly Encoding StringCharEncoding;


		/// <summary>
		/// Populate the struct's fields by reading them from a BinaryReader.
		/// </summary>
		/// <param name="BinaryReader">The BinaryReader that points to the start of the Eng file's data.</param>
		/// <param name="GameName">Used to indicate the name of the game that is being patched.</param>
		/// <param name="GameCharEncoding">Indicates the string encoding used for this EngText file.</param>
		/// <param name="Messages">If an error occurred, contains messages indicating the error(s) that occurred.</param>
		/// <param name="WasSuccessful">True if the constructor completed without errors. False otherwise.</param>
		public EngTextHeader(BinaryReader BinaryReader, Game GameName, CharEncodingTables GameCharEncoding,
			ref StringBuilder Messages, out bool WasSuccessful)
		{
			FileSignature = BinaryReader.ReadBytes(16);
			GroupCount = BinaryReader.ReadInt32();
			StringCount = BinaryReader.ReadInt32();
			WordCount = BinaryReader.ReadInt32();

			// Are there more than 1000 groups present (not supported)?
			if (GroupCount > 1001)
			{
				IsNewFileFormat = false;
				Messages.Append($"The Eng file's header indicates that it has {GroupCount.ToString()} String Groups present, ");
				Messages.Append("which is more than the permitted count of 1001.");
				WasSuccessful = false;
				StringCharEncoding = Encoding.Default;
				return;
			}

			// Assign the encoding used for this EngText file's strings.
			switch (GameCharEncoding)
			{
				case CharEncodingTables.Win1252:
				default:
					StringCharEncoding = Encoding.GetEncoding(1252);
					break;

				case CharEncodingTables.Win1250:
					StringCharEncoding = Encoding.GetEncoding(1250);
					break;

				case CharEncodingTables.Win1251:
					StringCharEncoding = Encoding.GetEncoding(1251);
					break;

				case CharEncodingTables.Win0950:
					StringCharEncoding = Encoding.GetEncoding(950);
					break;
			}

			// Test whether the EngText FileSignature matches that of the game currently being processed.
			switch (FileSignature[0])
			{
				case 0x43:	// if first letter of Signature = "C" for C3
					// Is Caesar 3 the game being worked on?
					if (GameName != Game.Caesar3)
					{
						fileSignatureErrorMessageCreation(1, ref Messages, out IsNewFileFormat, out WasSuccessful);
						return;
					}

					unsafe
					{
						// ReSharper disable once CommentTypo
						// This is a C3 Signature. Create a comparison array which says: "C3 textfile.<NULL><NULL><NULL><NULL>"
						byte* caesar3Signature = stackalloc byte[] {
							0x43, 0x33, 0x20, 0x74, 0x65, 0x78, 0x74, 0x66, 0x69, 0x6C, 0x65, 0x2E, 0x00, 0x00, 0x00, 0x00
						};
						//byte[] caesar3Signature = {
						//	0x43, 0x33, 0x20, 0x74, 0x65, 0x78, 0x74, 0x66, 0x69, 0x6C, 0x65, 0x2E, 0x00, 0x00, 0x00, 0x00
						//};

						// Check if the arrays are equal.
						for (int i = 0; i < FileSignature.Length; ++i)
						{
							if (FileSignature[i] != caesar3Signature[i])
							{
								fileSignatureErrorMessageCreation(2, ref Messages, out IsNewFileFormat, out WasSuccessful);
								return;
							}
						}
					}
					IsNewFileFormat = false;
					break;

				case 0x50:	// if first letter of Signature = "P" for Pharaoh
					// Is Pharaoh the game being worked on?
					if (GameName != Game.Pharaoh)
					{
						fileSignatureErrorMessageCreation(1, ref Messages, out IsNewFileFormat, out WasSuccessful);
						return;
					}

					unsafe
					{
						// ReSharper disable once CommentTypo
						// This is a Pharaoh Signature. Create a comparison array which says: "Pharaoh textfile"
						byte* pharaohSignature = stackalloc byte[] {
							0x50, 0x68, 0x61, 0x72, 0x61, 0x6F, 0x68, 0x20, 0x74, 0x65, 0x78, 0x74, 0x66, 0x69, 0x6C, 0x65
						};

						// Check if the arrays are equal.
						for (int i = 0; i < FileSignature.Length; ++i)
						{
							if (FileSignature[i] != pharaohSignature[i])
							{
								fileSignatureErrorMessageCreation(2, ref Messages, out IsNewFileFormat,
									out WasSuccessful);
								return;
							}
						}
					}

					IsNewFileFormat = false;
					break;

				case 0x5A:	// if first letter of Signature = "Z" for Zeus
					// Is Zeus the game being worked on?
					if (GameName != Game.Zeus)
					{
						fileSignatureErrorMessageCreation(1, ref Messages, out IsNewFileFormat, out WasSuccessful);
						return;
					}

					unsafe
					{
						// ReSharper disable once CommentTypo
						// This is a Zeus Signature. Create a comparison array which says: "Zeus textfile.<NULL><NULL>"
						byte* zeusSignature = stackalloc byte[] {
							0x5A, 0x65, 0x75, 0x73, 0x0, 0x74, 0x65, 0x78, 0x74, 0x66, 0x69, 0x6C, 0x65, 0x2E, 0x00, 0x00
						};

						// Check if the arrays are equal.
						for (int i = 0; i < FileSignature.Length; ++i)
						{
							if (FileSignature[i] != zeusSignature[i])
							{
								fileSignatureErrorMessageCreation(2, ref Messages, out IsNewFileFormat, out WasSuccessful);
								return;
							}
						}
					}
					IsNewFileFormat = true;
					break;

				case 0x45:	// if first letter of Signature = "E" for Emperor
					// Is Emperor the game being worked on?
					if (GameName != Game.Emperor)
					{
						fileSignatureErrorMessageCreation(1, ref Messages, out IsNewFileFormat, out WasSuccessful);
						return;
					}

					unsafe
					{
						// ReSharper disable once CommentTypo
						// This is a Emperor Signature. Create a comparison array which says: "Emperor textfile"
						byte* emperorSignature = stackalloc byte[] {
							0x45, 0x6D, 0x70, 0x65, 0x72, 0x6F, 0x72, 0x20, 0x74, 0x65, 0x78, 0x74, 0x66, 0x69, 0x6C, 0x65
						};

						// Check if the arrays are equal.
						for (int i = 0; i < FileSignature.Length; ++i)
						{
							if (FileSignature[i] != emperorSignature[i])
							{
								fileSignatureErrorMessageCreation(2, ref Messages, out IsNewFileFormat, out WasSuccessful);
								return;
							}
						}
					}
					IsNewFileFormat = true;
					break;

				default:
					fileSignatureErrorMessageCreation(2, ref Messages, out IsNewFileFormat, out WasSuccessful);
					return;
			}

			WasSuccessful = true;
		}

		/// <summary>
		/// Common code for creating an error message
		/// </summary>
		/// <param name="MessageNumber">
		///		Set to 1 to create a message about the File Signature not matching one supported by any of the games.
		///		Set to 2 to create a message about the File Signature not matching what the current game expects.
		/// </param>
		/// <param name="Messages">A StringBuilder that is used to hold all error and warning messages generated.</param>
		/// <param name="IsNewFileFormatParam">Set the IsNewFileFormat field to False.</param>
		/// <param name="WasSuccessful">Set the WasSuccessful field to False.</param>
		private static void fileSignatureErrorMessageCreation(byte MessageNumber, ref StringBuilder Messages, out bool IsNewFileFormatParam, out bool WasSuccessful)
		{
			switch (MessageNumber)
			{
				case 1:
					Messages.Append("The Eng file's File Signature does not match one used by C3, Pharaoh, Zeus or Emperor.\n");
					Messages.Append("This could indicate a corrupt file.\n\n");
					break;

				case 2:
					Messages.Append("The Eng file's File Signature does not match one used by the game being patched.\n");
					Messages.Append("Please supply the correct Text Eng for the game.\n\n");
					break;
			}
			IsNewFileFormatParam = false;
			WasSuccessful = false;
		}
	}
}
