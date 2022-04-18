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
	/// Storage class for <see cref="EngTextReaderWriter"/> header data.
	/// </summary>
	public struct EngTextHeader
	{
		/// <summary>
		/// 16 byte string that constitutes the File Signature.
		/// </summary>
		public readonly byte[] FileSignature;

		/// <summary>
		/// Number of in-use <see cref="EngTextStringGroupAndData"/>s in the file.
		/// </summary>
		public int GroupCount;

		/// <summary>
		/// Number of individual strings in the file.
		/// </summary>
		public int StringCount;

		/// <summary>
		/// Number of individual words in the file. May be inaccurate according to bvschaik.
		/// </summary>
		public int WordCount;

		/// <summary>
		/// False if the file is in the old format used by Caesar 3 and Pharaoh.
		/// True if it is in the new format used by Zeus and Emperor.
		/// </summary>
		public readonly bool IsNewFileFormat;

		/// <summary>
		/// Populate the struct's fields by reading them from a BinaryReader.
		/// </summary>
		/// <param name="BinaryReader">The BinaryReader that points to the start of the Eng file's data.</param>
		/// <param name="GameName">Used to indicate the name of the game that is being patched.</param>
		/// <param name="ErrorMessages">If an error occurred, contains messages indicating the error(s) that occurred.</param>
		/// <param name="WasSuccessful">True if the constructor completed without errors. False otherwise.</param>
		public EngTextHeader(BinaryReader BinaryReader, GameName GameName, ref string ErrorMessages, out bool WasSuccessful)
		{
			FileSignature = BinaryReader.ReadBytes(16);
			GroupCount = BinaryReader.ReadInt32();
			StringCount = BinaryReader.ReadInt32();
			WordCount = BinaryReader.ReadInt32();

			if (GroupCount > 1001)
			{
				IsNewFileFormat = false;
				ErrorMessages += $"The Eng file's header indicates that it has {GroupCount.ToString()} String Groups present, " +
				                 "which is more than the permitted count of 1001.";
				WasSuccessful = false;
				return;
			}

			switch (FileSignature[0])
			{
				case 0x43:	// if first letter of Signature = "C" for C3
					// Is Caesar 3 the game being worked on?
					if (GameName != GameName.Caesar3)
					{
						fileSignatureErrorMessageCreation(1, ref ErrorMessages, out IsNewFileFormat, out WasSuccessful);
						return;
					}

					// This is a C3 Signature. Create a comparison array which says: "C3 textfile.<NULL><NULL><NULL><NULL>"
					byte[] caesar3Signature = {
						0x43, 0x33, 0x20, 0x74, 0x65, 0x78, 0x74, 0x66, 0x69, 0x6C, 0x65, 0x2E, 0x00, 0x00, 0x00, 0x00
					};

					// Check if the arrays are equal.
					for (int i = 0; i < FileSignature.Length; ++i)
					{
						if (FileSignature[i] != caesar3Signature[i])
						{
							fileSignatureErrorMessageCreation(2, ref ErrorMessages, out IsNewFileFormat, out WasSuccessful);
							return;
						}
					}
					IsNewFileFormat = false;
					break;

				case 0x50:	// if first letter of Signature = "P" for Pharaoh
					// Is Pharaoh the game being worked on?
					if (GameName != GameName.Pharaoh)
					{
						fileSignatureErrorMessageCreation(1, ref ErrorMessages, out IsNewFileFormat, out WasSuccessful);
						return;
					}

					// This is a Pharaoh Signature. Create a comparison array which says: "Pharaoh textfile"
					byte[] pharaohSignature = {
						0x50, 0x68, 0x61, 0x72, 0x61, 0x6F, 0x68, 0x20, 0x74, 0x65, 0x78, 0x74, 0x66, 0x69, 0x6C, 0x65
					};

					// Check if the arrays are equal.
					for (int i = 0; i < FileSignature.Length; ++i)
					{
						if (FileSignature[i] != pharaohSignature[i])
						{
							fileSignatureErrorMessageCreation(2, ref ErrorMessages, out IsNewFileFormat, out WasSuccessful);
							return;
						}
					}
					IsNewFileFormat = false;
					break;

				case 0x5A:	// if first letter of Signature = "Z" for Zeus
					// Is Zeus the game being worked on?
					if (GameName != GameName.Zeus)
					{
						fileSignatureErrorMessageCreation(1, ref ErrorMessages, out IsNewFileFormat, out WasSuccessful);
						return;
					}

					// This is a Zeus Signature. Create a comparison array which says: "Zeus textfile.<NULL><NULL>"
					byte[] zeusSignature = {
						0x5A, 0x65, 0x75, 0x73, 0x0, 0x74, 0x65, 0x78, 0x74, 0x66, 0x69, 0x6C, 0x65, 0x2E, 0x00, 0x00
					};

					// Check if the arrays are equal.
					for (int i = 0; i < FileSignature.Length; ++i)
					{
						if (FileSignature[i] != zeusSignature[i])
						{
							fileSignatureErrorMessageCreation(2, ref ErrorMessages, out IsNewFileFormat, out WasSuccessful);
							return;
						}
					}
					IsNewFileFormat = true;
					break;

				case 0x45:	// if first letter of Signature = "E" for Emperor
					// Is Emperor the game being worked on?
					if (GameName != GameName.Emperor)
					{
						fileSignatureErrorMessageCreation(1, ref ErrorMessages, out IsNewFileFormat, out WasSuccessful);
						return;
					}

					// This is a Emperor Signature. Create a comparison array which says: "Emperor textfile"
					byte[] emperorSignature = {
						0x45, 0x6D, 0x70, 0x65, 0x72, 0x6F, 0x72, 0x20, 0x74, 0x65, 0x78, 0x74, 0x66, 0x69, 0x6C, 0x65
					};

					// Check if the arrays are equal.
					for (int i = 0; i < FileSignature.Length; ++i)
					{
						if (FileSignature[i] != emperorSignature[i])
						{
							fileSignatureErrorMessageCreation(2, ref ErrorMessages, out IsNewFileFormat, out WasSuccessful);
							return;
						}
					}
					IsNewFileFormat = true;
					break;

				default:
					fileSignatureErrorMessageCreation(2, ref ErrorMessages, out IsNewFileFormat, out WasSuccessful);
					return;
			}

			WasSuccessful = true;
		}

		/// <summary>
		/// Common code for creating an error message
		/// </summary>
		/// <param name="MessageNumber"></param>
		/// <param name="ErrorMessages"></param>
		/// <param name="IsNewFileFormatParam"></param>
		/// <param name="WasSuccessful"></param>
		private static void fileSignatureErrorMessageCreation(byte MessageNumber, ref string ErrorMessages, out bool IsNewFileFormatParam, out bool WasSuccessful)
		{
			switch (MessageNumber)
			{
				case 1:
					ErrorMessages += "The Eng file's File Signature does not match one used by C3, Pharaoh, Zeus or Emperor. This could indicate a corrupt file.";
					break;

				case 2:
					ErrorMessages += "The Eng file's File Signature does not match one used by the game being patched. Please supply the correct Text Eng for the game.";
					break;
			}
			IsNewFileFormatParam = false;
			WasSuccessful = false;
		}
	}
}
