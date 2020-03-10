using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tex2Dds
{
    class Program
    {
        const int MagicNumberTex = 0x00584554;
        const int MagicNumberDds = 0x20534444;
        const string WMagicNumberDds = "444453207C00000007100A00";
        const string WMagicNumberTex = "5445580010000000000000000000000002000000";
        const string CompressOption = "08104000";
        const string Empty2 = "0000000000000000";
        const string Empty4 = "00000000000000000000000000000000";
        const string Empty5 = "0000000000000000000000000000000000000000";
        const string Empty6 = "000000000000000000000000000000000000000000000000";
        const string Empty11 = "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
        const string bc7Num = "6300000003000000000000000100000000000000";
        const string bc6hNum = "5f00000003000000000000000100000000000000";
        const string rgbaNum = "5700000003000000000000000100000000000000";
        const string FF4 = "FFFFFFFF";
        const string FF40 = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
        const string TexSolid = "01000000000000000000000000000000FFFFFFFF0000000000000000";

        static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("Usage: <Program_path> <tex_source_path>");
                Console.ReadLine();
                return -1;
            }
            foreach (String arg in args)
            {
                FileInfo fi = new FileInfo(arg);
                if (!fi.Exists)
                {
                    Console.Error.WriteLine("ERROR: {0} not found", arg);
                    continue;
                }
                if (fi.Length < 0xC0)
                {
                    Console.Error.WriteLine("ERROR: {0} is too small", arg);
                    continue;
                }
                if (Path.GetExtension(arg).Equals(".tex"))
                {

                    using (BinaryReader reader = new BinaryReader(File.OpenRead(arg)))
                    {
                        int magicNumber = reader.ReadInt32();
                        if (magicNumber != MagicNumberTex)
                        {
                            Console.Error.WriteLine("ERROR: {0} is not a valid tex file.", arg);
                            continue;
                        }
                        reader.BaseStream.Position = 0x14;
                        int mipMapCount = reader.ReadInt32();
                        int width = reader.ReadInt32();
                        int height = reader.ReadInt32();
                        reader.BaseStream.Position = 0x24;

                        int type = reader.ReadInt32();

                        reader.BaseStream.Position = 0xB8;

                        long offset = reader.ReadInt64();
                        int size;

                        if (mipMapCount > 1)
                            size = (int)(reader.ReadInt64() - offset);
                        else
                            size = (int)(fi.Length - offset);

                        reader.BaseStream.Position = offset;

                        uint internalFormat;
                        string typeMagic = "";
                        string compresstype = "";
                        switch (type)
                        {
                            case 0x16:
                                internalFormat = 0x83F1; // COMPRESSED_RGBA_S3TC_DXT1_EXT
                                typeMagic = "DXT1";
                                compresstype = "DXT1_";
                                break;
                            case 0x17:
                                internalFormat = 0x83F1; // COMPRESSED_RGBA_S3TC_DXT1_EXT
                                typeMagic = "DXT1";
                                compresstype = "DXT1_";
                                break;
                            case 0x18:
                                internalFormat = 0x8DBB; //BC4U
                                typeMagic = "BC4U";
                                compresstype = "BC4_";
                                break;
                            case 0x1A:
                                internalFormat = 0x8DBD; // COMPRESSED_RG_RGTC2
                                typeMagic = "BC5U";
                                compresstype = "BC5_";
                                break;
                            case 0x1c:
                                internalFormat = 0x8E8F;
                                typeMagic = "DX10";
                                compresstype = "BC6H_";
                                break;
                            case 0x1d:
                            case 0x1e:
                            case 0x1f:
                                internalFormat = 0x8E8C; // COMPRESSED_RGBA_BPTC_UNORM_ARB
                                typeMagic = "DX10";
                                compresstype = "BC7_";
                                break;
                            case 0x7:
                                internalFormat = 0x57; // DXGI_FORMAT_B8G8R8A8_UNORM,
                                typeMagic = "DX10";
                                compresstype = "R8G8B8A8_";
                                break;
                            default:
                                internalFormat = 0;
                                break;
                        }

                        if (internalFormat == 0)
                        {
                            Console.Error.WriteLine("ERROR: Unknown TEX format {0}. " + arg, type);
                            Console.ReadLine();
                            continue;
                        }
                        string outfile_name = compresstype + Path.GetFileName(arg);
                        //string outfile_name = Path.GetFileName(arg);

                        string destPath = Path.GetFullPath(Path.ChangeExtension(Path.GetDirectoryName(arg) + "\\" + outfile_name, ".dds"));
                        Directory.CreateDirectory(Path.GetDirectoryName(destPath));
                        byte[] data;
                        if (type != 0x1c) data = reader.ReadBytes(size * 2);
                        else data = reader.ReadBytes(size * 12);

                        using (FileStream fsWrite = new FileStream(destPath, FileMode.Create))
                        {
                            byte[] WMagicNumberHead = Program.StringToByteArray(WMagicNumberDds);
                            fsWrite.Write(WMagicNumberHead, 0, WMagicNumberHead.Length);
                            fsWrite.Write(Program.intToBytesLittle(height), 0, 4);
                            fsWrite.Write(Program.intToBytesLittle(width), 0, 4);
                            if (!typeMagic.Equals("DXT"))
                            {
                                fsWrite.Write(Program.intToBytesLittle(width * height), 0, 4);
                            }
                            else
                            {
                                fsWrite.Write(Program.intToBytesLittle(width * height / 2), 0, 4);
                            }
                            fsWrite.Write(Program.intToBytesLittle(1), 0, 4);
                            fsWrite.Write(Program.intToBytesLittle(mipMapCount), 0, 4);
                            byte[] EmptyByte11 = Program.StringToByteArray(Empty11);
                            fsWrite.Write(EmptyByte11, 0, EmptyByte11.Length);
                            fsWrite.Write(Program.intToBytesLittle(32), 0, 4);
                            fsWrite.Write(Program.intToBytesLittle(4), 0, 4);
                            byte[] typeMagicBytes = Program.AsciiStringToByteArray(typeMagic);
                            fsWrite.Write(typeMagicBytes, 0, typeMagicBytes.Length);
                            byte[] EmptyByte5 = Program.StringToByteArray(Empty5);
                            fsWrite.Write(EmptyByte5, 0, EmptyByte5.Length);
                            byte[] CompressOptionByte = Program.StringToByteArray(CompressOption);
                            fsWrite.Write(CompressOptionByte, 0, CompressOptionByte.Length);
                            byte[] EmptyByte4 = Program.StringToByteArray(Empty4);
                            fsWrite.Write(EmptyByte4, 0, EmptyByte4.Length);
                            if (typeMagic.Equals("DX10"))
                            {
                                string ArbNum = bc7Num;
                                if (internalFormat == 0x8e8f)
                                {
                                    ArbNum = bc6hNum;
                                }
                                else if (internalFormat == 0x57) {
                                    ArbNum = rgbaNum;
                                }
                                byte[] ArbNumByte = Program.StringToByteArray(ArbNum);
                                fsWrite.Write(ArbNumByte, 0, ArbNumByte.Length);
                            }
                            fsWrite.Write(data, 0, data.Length);

                        }

                    }
                }
                else if (Path.GetExtension(arg).Equals(".dds"))
                {
                    string destPath = Path.GetFullPath(Path.ChangeExtension(arg, ".tex"));
                    Directory.CreateDirectory(Path.GetDirectoryName(destPath));
                    FileInfo destFile = new FileInfo(destPath);
                    if (destFile.Exists)
                    {
                        if (File.Exists(destPath + ".old")) File.Delete(destPath + ".old");
                        Directory.Move(destPath, destPath + ".old");
                    }
                    using (BinaryReader reader = new BinaryReader(File.OpenRead(arg)))
                    {
                        int magicNumber = reader.ReadInt32();
                        if (magicNumber != MagicNumberDds)
                        {
                            Console.Error.WriteLine("ERROR: {0} is not a valid dds file.", magicNumber);
                            continue;
                        }
                        reader.BaseStream.Position = 0x0C;
                        int height = reader.ReadInt32();
                        int width = reader.ReadInt32();
                        reader.BaseStream.Position = 0x1C;
                        int mipMapCount = reader.ReadInt32();
                        reader.BaseStream.Position = 0x54;
                        int filetypecode = reader.ReadInt32();
                        reader.BaseStream.Position = 0x80;
                        int innerfiletype = reader.ReadInt32();
                        int filetype = 0;
                        switch (filetypecode)
                        {
                            case 0x30315844:
                                if (innerfiletype <= 99 && innerfiletype >= 97) { filetype = 0x1f; }
                                else if (innerfiletype <= 96 && innerfiletype >= 94) { filetype = 0x1c; }
                                break;
                            case 0x31545844:
                                filetype = 0x17;
                                break;
                            case 0x55344342:
                                filetype = 0x18;
                                break;
                            case 0x55354342:
                            case 0x32495441:
                                filetype = 0x1a;
                                break;
                        }

                        reader.BaseStream.Position = (filetype == 0x1f || filetype == 0x1c) ? 0x94 : 0x80;
                        byte[] data = reader.ReadBytes(width * height * 2);

                        if (filetype == 0)
                        {
                            Console.Error.WriteLine("ERROR: Unsupported DDS format {0}. " + arg, filetypecode);
                            Console.ReadLine();
                            continue;
                        }
                        using (FileStream fsWrite = new FileStream(destPath, FileMode.Create))
                        {
                            byte[] WMagicNumberHead = Program.StringToByteArray(WMagicNumberTex);
                            fsWrite.Write(WMagicNumberHead, 0, WMagicNumberHead.Length);
                            fsWrite.Write(Program.intToBytesLittle(mipMapCount), 0, 4);
                            fsWrite.Write(Program.intToBytesLittle(width), 0, 4);
                            fsWrite.Write(Program.intToBytesLittle(height), 0, 4);
                            fsWrite.Write(Program.intToBytesLittle(1), 0, 4);
                            fsWrite.Write(Program.intToBytesLittle(filetype), 0, 4);
                            byte[] WTexSolid = Program.StringToByteArray(TexSolid);
                            fsWrite.Write(WTexSolid, 0, WTexSolid.Length);
                            if (filetype == 0x1f)
                            {
                                fsWrite.Write(Program.intToBytesLittle(1), 0, 4);
                            }
                            else
                            {
                                fsWrite.Write(Program.intToBytesLittle(0), 0, 4);
                            }
                            fsWrite.Write(Program.intToBytesLittle(0), 0, 4);
                            fsWrite.Write(Program.intToBytesLittle(0), 0, 4);
                            byte[] WFF40 = Program.StringToByteArray(FF40);
                            fsWrite.Write(WFF40, 0, WFF40.Length);
                            fsWrite.Write(Program.intToBytesLittle(width), 0, 4);
                            fsWrite.Write(Program.intToBytesLittle(width / 2), 0, 2);
                            fsWrite.Write(Program.intToBytesLittle(width), 0, 2);
                            fsWrite.Write(Program.intToBytesLittle(0), 0, 4);
                            fsWrite.Write(Program.intToBytesLittle(0), 0, 4);
                            fsWrite.Write(Program.intToBytesLittle(width / 2), 0, 2);
                            fsWrite.Write(Program.intToBytesLittle(width), 0, 2);
                            fsWrite.Write(Program.intToBytesLittle(0), 0, 4);
                            fsWrite.Write(Program.intToBytesLittle(0), 0, 4);
                            fsWrite.Write(Program.intToBytesLittle(width / 2), 0, 2);
                            fsWrite.Write(Program.intToBytesLittle(width), 0, 2);
                            fsWrite.Write(Program.intToBytesLittle(0), 0, 4);
                            fsWrite.Write(Program.intToBytesLittle(0), 0, 4);
                            fsWrite.Write(Program.StringToByteArray(Empty6), 0, 6 * 4);
                            int cur_width = width;
                            int cur_height = height;

                            int base_loc = 0xb8 + mipMapCount * 8;
                            for (int i = 0; i < mipMapCount; i++)
                            {
                                fsWrite.Write(Program.intToBytesLittle(base_loc), 0, 4);
                                fsWrite.Write(Program.intToBytesLittle(0), 0, 4);
                                if (filetype == 0x17)
                                {
                                    int cur_size = cur_width * cur_height / 2;
                                    if (cur_size < 0x10) cur_size = 0x10;
                                    base_loc = base_loc + cur_size;
                                }
                                else
                                {
                                    base_loc = base_loc + cur_width * cur_height;
                                }
                                cur_width = cur_width / 2;
                                cur_height = cur_height / 2;
                                cur_width = cur_width > 4 ? cur_width : 4;
                                cur_height = cur_height > 4 ? cur_height : 4;
                            }
                            fsWrite.Write(data, 0, data.Length);
                        }
                    }
                }
                Console.WriteLine(arg + "finished!"); 
            }
            return 0;
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
        /*
        public static byte[] intToBytesBig(int value)
        {
            byte[] src = new byte[4];
            src[0] = (byte)((value >> 24) & 0xFF);
            src[1] = (byte)((value >> 16) & 0xFF);
            src[2] = (byte)((value >> 8) & 0xFF);
            src[3] = (byte)(value & 0xFF);
            return src;
        }*/

        public static byte[] AsciiStringToByteArray(string origin) {
            byte[] ret = new byte[origin.Length];
            for (int i = 0; i < origin.Length; i++) {
                ret[i] = Convert.ToByte(origin[i]);
            }
            return ret;
        }

        public static byte[] intToBytesLittle(int value)
        {
            byte[] src = new byte[4];
            src[3] = (byte)((value >> 24) & 0xFF);
            src[2] = (byte)((value >> 16) & 0xFF);
            src[1] = (byte)((value >> 8) & 0xFF);
            src[0] = (byte)(value & 0xFF);
            return src;
        }
    }
}
