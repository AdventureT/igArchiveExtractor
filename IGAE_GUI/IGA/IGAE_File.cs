﻿using System;
using System.IO;
using System.IO.Compression;
using SevenZip.Compression.LZMA;

namespace IGAE_GUI
{
	class IGAE_File
	{
		StreamHelper stream;
		readonly string name;
		public IGA_Descriptor[] localFileHeaders;

		public IGA_Version _version;
		public uint numberOfFiles;
		public uint chunkAlignment;
		public uint nametableLocation;
		public uint nametableLength;
		public string[] names;
		uint chunkPropertiesStart = 0;

		public IGAE_File(string filepath, IGA_Version version)
		{
			FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read);

			name = filepath;

			byte[] magicNumber = new byte[4];

			fs.Read(magicNumber, 0x00, 0x04);

			StreamHelper.Endianness endianness;

			if (BitConverter.ToUInt32(magicNumber, 0) == 0x4947411A) endianness = StreamHelper.Endianness.Big;
			else if (BitConverter.ToUInt32(magicNumber, 0) == 0x1A414749) endianness = StreamHelper.Endianness.Little;
			else throw new InvalidOperationException("File is corrupt.");

			stream = new StreamHelper(fs, endianness);

			_version = version;

			numberOfFiles = stream.ReadUInt32WithOffset(IGAE_Globals.headerData[_version][(uint)IGAE_HeaderData.NumberOfFiles]);
			chunkAlignment = stream.ReadUInt32WithOffset(0x00000010);
			if(_version == IGA_Version.SkylandersSpyrosAdventureWii)
			{
				chunkAlignment = 0x0800;	//It doesn't store it, i found that funny
			}
			nametableLocation = stream.ReadUInt32WithOffset(IGAE_Globals.headerData[_version][(uint)IGAE_HeaderData.NametableLocation]);
			nametableLength = stream.ReadUInt32WithOffset(IGAE_Globals.headerData[_version][(uint)IGAE_HeaderData.NametableLength]);

			localFileHeaders = new IGA_Descriptor[numberOfFiles];
			names = new string[numberOfFiles];

			//Console.WriteLine(IGAE_Globals.headerData[_version][(uint)IGAE_HeaderData.ChecksumLocation] + IGAE_Globals.headerData[_version][(uint)IGAE_HeaderData.ChecksumLength] * numberOfFiles);

			for (uint i = 0; i < numberOfFiles; i++)
			{
				names[i] = stream.ReadStringFromOffset(nametableLocation + stream.ReadUInt32WithOffset(nametableLocation + 0x04 * i));

				localFileHeaders[i].startingAddress = stream.ReadUInt32WithOffset(IGAE_Globals.headerData[_version][(uint)IGAE_HeaderData.ChecksumLocation] + IGAE_Globals.headerData[_version][(uint)IGAE_HeaderData.ChecksumLength] * numberOfFiles + IGAE_Globals.headerData[_version][(uint)IGAE_HeaderData.LocalHeaderLength] * i + IGAE_Globals.headerData[_version][(uint)IGAE_HeaderData.FileStartInLocal]);
				localFileHeaders[i].size = stream.ReadUInt32WithOffset(IGAE_Globals.headerData[_version][(uint)IGAE_HeaderData.ChecksumLocation] + IGAE_Globals.headerData[_version][(uint)IGAE_HeaderData.ChecksumLength] * numberOfFiles + IGAE_Globals.headerData[_version][(uint)IGAE_HeaderData.LocalHeaderLength] * i + IGAE_Globals.headerData[_version][(uint)IGAE_HeaderData.FileLengthInLocal]);
				localFileHeaders[i].mode = stream.ReadUInt32WithOffset(IGAE_Globals.headerData[_version][(uint)IGAE_HeaderData.ChecksumLocation] + IGAE_Globals.headerData[_version][(uint)IGAE_HeaderData.ChecksumLength] * numberOfFiles + IGAE_Globals.headerData[_version][(uint)IGAE_HeaderData.LocalHeaderLength] * i + IGAE_Globals.headerData[_version][(uint)IGAE_HeaderData.ModeInLocal]);
				localFileHeaders[i].path = names[i];
				localFileHeaders[i].chunkPropertiesOffset = localFileHeaders[i].mode & 0xFFFFFF;

				//For some reason the 3ds games have mode 0x10000000 as lzma whereas ssf has it as 0x20000000, so yeah this exists to make that work
				if (_version == IGA_Version.SkylandersSpyrosAdventureWii && this.localFileHeaders[i].mode == 0x10000000)
				{
					localFileHeaders[i].mode = 0x20000000;
				}

				localFileHeaders[i].index = i;
			}
			chunkPropertiesStart = (uint)stream.BaseStream.Position;
		}

		public void ExtractFile(uint index, string outputDir, out int res, bool trueName = true)
		{
			string outputFilePath;			

			if(trueName && (Path.GetExtension(name) == ".bld" || Path.GetExtension(name) == ".pak"))
			{
				outputFilePath = Path.ChangeExtension(name, null) + "/" +  names[index].Substring(names[index][1] == ':' ? 2 : 0);
			}
			else if(!trueName && (Path.GetExtension(name) == ".arc" || Path.GetExtension(name) == ".pak"))
			{
				outputFilePath = outputDir + Path.GetFileName(names[index]);
			}
			else
			{
				outputFilePath = outputDir + names[index].Substring(names[index][1] == ':' ? 2 : 0);
			}

			Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));

			FileStream outputfs = null;
			outputfs = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write);

			Console.WriteLine(outputfs.Name);	
			switch (localFileHeaders[index].mode >> 24)
			{
				case 0x00:
				case 0x10:
					{
						uint chunkCount = 0;
						uint nextChunk = localFileHeaders[index].startingAddress;
						uint decompressedBytes = 0;
						uint chunkSize = 0x8000;

						//Very bad code but there's no other way to do thiss

						while(decompressedBytes < localFileHeaders[index].size)
						{
							//stream.BaseStream.Seek(chunkPropertiesStart + localFileHeaders[index].chunkPropertiesOffset * 2 + chunkCount * 2, SeekOrigin.Begin);
							//Console.WriteLine($"Properties: {stream.BaseStream.Position.ToString("X08")}");
							//uint chunkProperties = stream.ReadUInt16();
							stream.BaseStream.Seek(nextChunk, SeekOrigin.Begin);

							try
							{
								uint compressedSize;

								if (((uint)_version & 0x000000FF) <= 0x0B)
								{
									compressedSize = stream.ReadUInt16(StreamHelper.Endianness.Little);
								}
								else
								{
									compressedSize = stream.ReadUInt32(StreamHelper.Endianness.Little);
								}

								Console.WriteLine($"Chunk: {stream.BaseStream.Position.ToString("X08")} of size {compressedSize.ToString("X08")}");

								MemoryStream ms = new MemoryStream((int)compressedSize);
								byte[] compressedBytes = stream.ReadBytes((int)compressedSize);
								ms.Write(compressedBytes, 0x00, (int)compressedSize);
								ms.Seek(0x00, SeekOrigin.Begin);
								DeflateStream decompressionStream = new DeflateStream(ms, CompressionMode.Decompress, true);
								decompressionStream.CopyTo(outputfs);
								decompressionStream.Close();
							}
							catch(InvalidDataException)
							{
								stream.BaseStream.Seek(nextChunk, SeekOrigin.Begin);

								Console.WriteLine($"Chunk: {stream.BaseStream.Position.ToString("X08")} of size {chunkSize.ToString("X08")}");
								byte[] uncompressedData = new byte[chunkSize];
								stream.BaseStream.Read(uncompressedData, 0x00, (int)chunkSize);
								outputfs.Write(uncompressedData, 0x00, (int)chunkSize);
							}

							chunkCount++;
							decompressedBytes += 0x8000;		//Bad code that's definitely fail, you just don't know why yet


							nextChunk = (uint)(((int)stream.BaseStream.Position + (chunkAlignment - 1)) / chunkAlignment) * chunkAlignment;

							if(stream.BaseStream.Position % chunkAlignment == 0)
							{
								nextChunk = (uint)stream.BaseStream.Position;
							}

							if(nextChunk > stream.BaseStream.Length)
							{
								throw new Exception($"File truncated, {nextChunk.ToString("X08")} does not exist");
							}
						}
						outputfs.Close();
						res = 0;
					}
					break;
				case 0x20:
					{
						//The following was adapted from https://github.com/KillzXGaming/Switch-Toolbox/blob/master/File_Format_Library/FileFormats/CrashBandicoot/IGA_PAK.cs

						Decoder decoder = new Decoder();

						stream.BaseStream.Seek(localFileHeaders[index].startingAddress, SeekOrigin.Begin);

						//The following is lies and deceit
						//uint chunkSize = (((int)_version & 0x000000FF) < 0x09) ? 0x00008000u : 0x00800000u;

						uint chunkSize = 0x00008000u;

						uint attempts = 0;

						uint bytesDecompressed = 0;

						while (bytesDecompressed < localFileHeaders[index].size)
						{
							uint compressedSize;

							if (((uint)_version & 0x000000FF) <= 0x0B)
							{
								compressedSize = stream.ReadUInt16(StreamHelper.Endianness.Little);
							}
							else
							{
								compressedSize = stream.ReadUInt32(StreamHelper.Endianness.Little);
							}

							byte[] properties = stream.ReadBytes(5);

							if(properties[0] == 0x5D && BitConverter.ToUInt32(properties, 0x01) <= chunkSize)
							{
								decoder.SetDecoderProperties(properties);

								byte[] compressedBytes = new byte[compressedSize];
								stream.Read(compressedBytes, 0x00, (int)compressedSize);

								uint def_block = (uint)Math.Min(chunkSize, localFileHeaders[index].size - bytesDecompressed);

								if(((int)localFileHeaders[index].size - (int)bytesDecompressed) <= 10)
								{
									break;
								}

								MemoryStream ms = new MemoryStream(compressedBytes);

								try
								{
									decoder.Code(ms, outputfs, compressedSize, def_block, null);
								}
								catch(Exception e)
								{
									Console.WriteLine($"{e.Message} but let's ignore that");
									break;
								}

								bytesDecompressed += def_block;

							}
							else
							{
								if (((uint)_version & 0x000000FF) <= 0x0B)
								{
									stream.BaseStream.Seek(-7, SeekOrigin.Current);
								}
								else
								{
									stream.BaseStream.Seek(-9, SeekOrigin.Current);
								}
								byte[] uncompressedData = new byte[chunkSize];
								stream.BaseStream.Read(uncompressedData, 0x00, (int)chunkSize);
								outputfs.Write(uncompressedData, 0x00, (int)chunkSize);

								bytesDecompressed += chunkSize;
							}

							uint nextChunk = (uint)(((int)stream.BaseStream.Position + (chunkAlignment - 1)) / chunkAlignment) * chunkAlignment;

							if(stream.BaseStream.Position % chunkAlignment == 0)
							{
								nextChunk = (uint)stream.BaseStream.Position;
							}

							if(nextChunk > stream.BaseStream.Length)
							{
								throw new Exception("File truncated");
							}
							else
							{
								stream.BaseStream.Seek(nextChunk, SeekOrigin.Begin);
							}
							attempts++;
						}

						outputfs.Close();
						res = 0;
					}
					break;
				case 0x30:
					{
						//This functions like 0xFF except the size is stored at the start.
						//I don't know why either.
						//We're just gonna subtract 2 from the size and start at localFileHeaders[index].startingAddress + 4 so as not to break compatibility with any programs

						uint size = stream.ReadUInt16(StreamHelper.Endianness.Little) - 2u;

						stream.BaseStream.Seek(0x02, SeekOrigin.Current);

						byte[] buffer = stream.ReadBytes((int)size);

						outputfs.Write(buffer, 0x00, (int)size);

						outputfs.Close();
						res = 0;
					}
					break;
				case 0xFF:
					{
						stream.BaseStream.Seek(localFileHeaders[index].startingAddress, SeekOrigin.Begin);

						byte[] buffer = stream.ReadBytes((int)localFileHeaders[index].size);

						outputfs.Write(buffer, 0x00, (int)localFileHeaders[index].size);

						outputfs.Close();
						res = 0;
					}
					break;
				default:
					res = -1;
					break;
			}
		}
		~IGAE_File()
		{
			stream.Close();
		}
	}
}
