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

namespace Impressions.Formats.Text {

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
		/// <param name="_Input_"></param>
		/// <returns></returns>
		public static EngText LoadENG(Stream _Input_) {

			EngText _eng_ = new EngText();

			using (BinaryReader _br_ = new BinaryReader(_Input_))
			{
				// Header
				_eng_.Header.Magic = Encoding.ASCII.GetString(_br_.ReadBytes(16)).Trim('\0');
				_eng_.Header.GroupCount = _br_.ReadInt32();
				_eng_.Header.StringCount = _br_.ReadInt32();
				_eng_.Header.WordCount = _br_.ReadInt32();

				// Groups
				for (int _i_ = 0; _i_ < 1000; _i_++)
				{
					int _offset_ = _br_.ReadInt32();
					int _stringCount_ = _br_.ReadInt32();

					if (_stringCount_ > 0)
					{
						_eng_.Groups.Add(new EngTextGroup { ID = _i_, Offset = _offset_, StringCount = _stringCount_ });
						if (_stringCount_ > 1)
						{
							_eng_.IndexWithCounts = true;
						}
					}
				}

				long _baseOffset_ = _br_.BaseStream.Position;
				long _textSize_ = _br_.BaseStream.Length - _baseOffset_;

				// Strings
				for (int _i_ = 0; _i_ < _eng_.Groups.Count; _i_++)
				{
					EngTextGroup _group_ = _eng_.Groups[_i_];
					long _nextGroupOffset_ = _i_ < _eng_.Groups.Count - 1 ? _eng_.Groups[_i_ + 1].Offset : _textSize_;

					// TODO: If the reading code is working 100% properly this line should have no effect.
					// Testing is required before it is removed.
					_br_.BaseStream.Position = _baseOffset_ + _group_.Offset;

					// Engage Caesar 3 string reading loop.
					if (!_eng_.IndexWithCounts)
					{
						while (_br_.BaseStream.Position - _baseOffset_ < _nextGroupOffset_)
						{
							List<byte> _bytes_ = new List<byte>();
							byte _byte_ = _br_.ReadByte();

							while (_byte_ != 0)
							{
								_bytes_.Add(_byte_);
								_byte_ = _br_.ReadByte();
							}

							if (_bytes_.Count > 0)
							{
								// TODO: Encoding support for the various game language versions will need to be implemented as this is currently only ASCII compatible.
								_group_.Strings.Add(Encoding.ASCII.GetString(_bytes_.ToArray()));
							}
						}
					}
					else // Handle all of the others much more simply.
					{
						for (int _j_ = 0; _j_ < _group_.StringCount; _j_++)
						{
							List<byte> _bytes_ = new List<byte>();
							byte _byte_ = _br_.ReadByte();
							while (_byte_ != 0)
							{
								_bytes_.Add(_byte_);
								_byte_ = _br_.ReadByte();
							}

							if (_bytes_.Count > 0)
							{
								// TODO: Encoding support for the various game language versions will need to be implemented as this is currently only ASCII compatible.
								_group_.Strings.Add(Encoding.ASCII.GetString(_bytes_.ToArray()));
							}
						}
					}
				}
			}

			return _eng_;
		}

		/// <summary>
		/// Saves an in-memory <see cref="EngText"/> to the output <see cref="Stream"/>.
		/// </summary>
		/// <param name="_Output_"></param>
		/// <returns></returns>
		public bool Save(Stream _Output_) {

			// Here you go XJDHDR, this is the fun part. ^_^;;

			return true;
		}
	}
}
