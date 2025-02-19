﻿using System;
using System.IO;
using System.Text;

namespace IGAE_GUI
{
	//Heavily edited and renamed version of https://github.com/AdventureT/TrbMultiTool/blob/opengl/TrbMultiTool/TrbMultiTool/EndiannessAwareBinaryReader.cs

	public class StreamHelper : BinaryReader
	{
		public enum Endianness
		{
			Little,
			Big,
		}

		public Endianness _endianness = Endianness.Little;

		public StreamHelper(Stream input) : base(input)
		{
		}

		public StreamHelper(Stream input, Encoding encoding) : base(input, encoding)
		{
		}

		public StreamHelper(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
		{
		}

		public StreamHelper(Stream input, Endianness endianness) : base(input)
		{
			_endianness = endianness;
		}

		public StreamHelper(Stream input, Encoding encoding, Endianness endianness) : base(input, encoding)
		{
			_endianness = endianness;
		}

		public StreamHelper(Stream input, Encoding encoding, bool leaveOpen, Endianness endianness) : base(input, encoding, leaveOpen)
		{
			_endianness = endianness;
		}

		public override string ReadString()
		{
			var sb = new StringBuilder();
			while (true)
			{
				var newByte = ReadByte();
				if (newByte == 0) break;
				sb.Append((char)newByte);
			}
			return sb.ToString();
		}

		public string ReadUnicodeString()
		{
			var sb = new StringBuilder();
			while (true)
			{
				byte newByte;
				byte newByte2;

				try
				{
					newByte = ReadByte();
					newByte2 = ReadByte();
				}
				catch (EndOfStreamException)
				{
					break;
				}
				if (newByte == 0 && newByte2 == 0) break;
				string convertedChar;
				if (_endianness == Endianness.Big) convertedChar = Encoding.Unicode.GetString(new byte[] { newByte2, newByte });
				else convertedChar = Encoding.Unicode.GetString(new byte[] { newByte, newByte2 });
				sb.Append(convertedChar);
			}
			return sb.ToString();
		}

		public string ReadUnicodeString(uint size)
		{
			var sb = new StringBuilder();
			for (int i = 0; i < size; i++)
			{
				byte newByte;
				byte newByte2;

				try
				{
					newByte = ReadByte();
					newByte2 = ReadByte();
				}
				catch (EndOfStreamException)
				{
					break;
				}
				if (newByte == 0 && newByte2 == 0) break;
				string convertedChar;
				if (_endianness == Endianness.Big) convertedChar = Encoding.Unicode.GetString(new byte[] { newByte2, newByte });
				else convertedChar = Encoding.Unicode.GetString(new byte[] { newByte, newByte2 });
				sb.Append(convertedChar);
			}
			return sb.ToString();
		}

		public byte ReadByte() => ReadByte((uint)BaseStream.Position);
		public byte ReadByte(uint offset)
		{
			BaseStream.Seek(offset, SeekOrigin.Begin);
			byte[] buffer = new byte[1];
			BaseStream.Read(buffer, 0x00, 0x01);
			return buffer[0];
		}

		public string ReadStringFromOffset(uint offset)
		{
			var pos = BaseStream.Position;
			BaseStream.Seek(offset, SeekOrigin.Begin);
			string str = ReadString();
			BaseStream.Seek(pos, SeekOrigin.Begin);
			return str;
		}

		public byte[] ReadFromOffset(uint bytesToRead, uint offset)
		{
			var pos = BaseStream.Position;
			BaseStream.Seek(offset, SeekOrigin.Begin);
			var buffer = new byte[bytesToRead];
			var bytesRead = Read(buffer, 0x00, (int)bytesToRead);
			BaseStream.Seek(pos, SeekOrigin.Begin);
			return buffer;
		}
		public override short ReadInt16() => ReadInt16(_endianness);
		public short ReadInt16(uint offset, Endianness endianness)
		{
			BaseStream.Seek(offset, SeekOrigin.Begin);
			return ReadInt16(endianness);
		}
		public override int ReadInt32() => ReadInt32(_endianness);
		public int ReadInt32WithOffset(uint offset)
		{
			BaseStream.Seek(offset, SeekOrigin.Begin);
			return ReadInt32(_endianness);
		}
		public override long ReadInt64() => ReadInt64(_endianness);

		public override ushort ReadUInt16() => ReadUInt16(_endianness);
		public uint ReadUInt16WithOffset(uint offset) => ReadUInt16WithOffset(offset, _endianness);
		public uint ReadUInt16WithOffset(uint offset, Endianness endianness)
		{
			BaseStream.Seek(offset, SeekOrigin.Begin);
			return ReadUInt16(endianness);
		}
		public override uint ReadUInt32() => ReadUInt32(_endianness);
		public uint ReadUInt32WithOffset(uint offset) => ReadUInt32WithOffset(offset, _endianness);
		public uint ReadUInt32WithOffset(uint offset, Endianness endianness)
		{
			BaseStream.Seek(offset, SeekOrigin.Begin);
			return ReadUInt32(endianness);
		}
		public override ulong ReadUInt64() => ReadUInt64(_endianness);

		public short ReadInt16(Endianness endianness) => BitConverter.ToInt16(ReadForEndianness(sizeof(short), endianness), 0);

		public int ReadInt32(Endianness endianness) => BitConverter.ToInt32(ReadForEndianness(sizeof(int), endianness), 0);

		public long ReadInt64(Endianness endianness) => BitConverter.ToInt64(ReadForEndianness(sizeof(long), endianness), 0);

		public ushort ReadUInt16(Endianness endianness) => BitConverter.ToUInt16(ReadForEndianness(sizeof(ushort), endianness), 0);

		public uint ReadUInt32(Endianness endianness) => BitConverter.ToUInt32(ReadForEndianness(sizeof(uint), endianness), 0);

		public ulong ReadUInt64(Endianness endianness) => BitConverter.ToUInt64(ReadForEndianness(sizeof(ulong), endianness), 0);

		public byte[] ReadForEndianness(int bytesToRead, Endianness endianness)
		{
			byte[] bytesRead = new byte[bytesToRead];
			int res = BaseStream.Read(bytesRead, 0, bytesToRead);
			if(res != bytesToRead)
			{
				throw new Exception($"Read {res} instead of {bytesToRead}");
			}
			switch (endianness)
			{
				case Endianness.Little:
					if (!BitConverter.IsLittleEndian)
					{
						Array.Reverse(bytesRead);
					}
					break;

				case Endianness.Big:
					if (BitConverter.IsLittleEndian)
					{
						Array.Reverse(bytesRead);
					}
					break;
			}

			return bytesRead;
		}
	}
}
