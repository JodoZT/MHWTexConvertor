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
        const string dx10FixedFlags = "03000000000000000100000000000000";
        const string TexFixedUnkn = "01000000000000000000000000000000FFFFFFFF0000000000000000";

        enum MHW_TEX_FORMAT{
            DXGI_FORMAT_UNKNOWN = 0,
            DXGI_FORMAT_R8G8B8A8_UNORM = 7,
            DXGI_FORMAT_R8G8B8A8_UNORM_SRGB = 9,//Not sure. The textures are weird.
            DXGI_FORMAT_R8_UNORM = 19,
            DXGI_FORMAT_BC1_UNORM = 22,
            DXGI_FORMAT_BC1_UNORM_SRGB = 23,
            DXGI_FORMAT_BC4_UNORM = 24,
            DXGI_FORMAT_BC5_UNORM = 26,
            DXGI_FORMAT_BC6H_UF16 = 28,
            DXGI_FORMAT_BC7_UNORM = 30,
            DXGI_FORMAT_BC7_UNORM_SRGB = 31,
            }

        enum DXGI_FORMAT
        {
            DXGI_FORMAT_UNKNOWN,
            DXGI_FORMAT_R32G32B32A32_TYPELESS,
            DXGI_FORMAT_R32G32B32A32_FLOAT,
            DXGI_FORMAT_R32G32B32A32_UINT,
            DXGI_FORMAT_R32G32B32A32_SINT,
            DXGI_FORMAT_R32G32B32_TYPELESS,
            DXGI_FORMAT_R32G32B32_FLOAT,
            DXGI_FORMAT_R32G32B32_UINT,
            DXGI_FORMAT_R32G32B32_SINT,
            DXGI_FORMAT_R16G16B16A16_TYPELESS,
            DXGI_FORMAT_R16G16B16A16_FLOAT,
            DXGI_FORMAT_R16G16B16A16_UNORM,
            DXGI_FORMAT_R16G16B16A16_UINT,
            DXGI_FORMAT_R16G16B16A16_SNORM,
            DXGI_FORMAT_R16G16B16A16_SINT,
            DXGI_FORMAT_R32G32_TYPELESS,
            DXGI_FORMAT_R32G32_FLOAT,
            DXGI_FORMAT_R32G32_UINT,
            DXGI_FORMAT_R32G32_SINT,
            DXGI_FORMAT_R32G8X24_TYPELESS,
            DXGI_FORMAT_D32_FLOAT_S8X24_UINT,
            DXGI_FORMAT_R32_FLOAT_X8X24_TYPELESS,
            DXGI_FORMAT_X32_TYPELESS_G8X24_UINT,
            DXGI_FORMAT_R10G10B10A2_TYPELESS,
            DXGI_FORMAT_R10G10B10A2_UNORM,
            DXGI_FORMAT_R10G10B10A2_UINT,
            DXGI_FORMAT_R11G11B10_FLOAT,
            DXGI_FORMAT_R8G8B8A8_TYPELESS,
            DXGI_FORMAT_R8G8B8A8_UNORM,
            DXGI_FORMAT_R8G8B8A8_UNORM_SRGB,
            DXGI_FORMAT_R8G8B8A8_UINT,
            DXGI_FORMAT_R8G8B8A8_SNORM,
            DXGI_FORMAT_R8G8B8A8_SINT,
            DXGI_FORMAT_R16G16_TYPELESS,
            DXGI_FORMAT_R16G16_FLOAT,
            DXGI_FORMAT_R16G16_UNORM,
            DXGI_FORMAT_R16G16_UINT,
            DXGI_FORMAT_R16G16_SNORM,
            DXGI_FORMAT_R16G16_SINT,
            DXGI_FORMAT_R32_TYPELESS,
            DXGI_FORMAT_D32_FLOAT,
            DXGI_FORMAT_R32_FLOAT,
            DXGI_FORMAT_R32_UINT,
            DXGI_FORMAT_R32_SINT,
            DXGI_FORMAT_R24G8_TYPELESS,
            DXGI_FORMAT_D24_UNORM_S8_UINT,
            DXGI_FORMAT_R24_UNORM_X8_TYPELESS,
            DXGI_FORMAT_X24_TYPELESS_G8_UINT,
            DXGI_FORMAT_R8G8_TYPELESS,
            DXGI_FORMAT_R8G8_UNORM,
            DXGI_FORMAT_R8G8_UINT,
            DXGI_FORMAT_R8G8_SNORM,
            DXGI_FORMAT_R8G8_SINT,
            DXGI_FORMAT_R16_TYPELESS,
            DXGI_FORMAT_R16_FLOAT,
            DXGI_FORMAT_D16_UNORM,
            DXGI_FORMAT_R16_UNORM,
            DXGI_FORMAT_R16_UINT,
            DXGI_FORMAT_R16_SNORM,
            DXGI_FORMAT_R16_SINT,
            DXGI_FORMAT_R8_TYPELESS,
            DXGI_FORMAT_R8_UNORM,
            DXGI_FORMAT_R8_UINT,
            DXGI_FORMAT_R8_SNORM,
            DXGI_FORMAT_R8_SINT,
            DXGI_FORMAT_A8_UNORM,
            DXGI_FORMAT_R1_UNORM,
            DXGI_FORMAT_R9G9B9E5_SHAREDEXP,
            DXGI_FORMAT_R8G8_B8G8_UNORM,
            DXGI_FORMAT_G8R8_G8B8_UNORM,
            DXGI_FORMAT_BC1_TYPELESS,
            DXGI_FORMAT_BC1_UNORM,
            DXGI_FORMAT_BC1_UNORM_SRGB,
            DXGI_FORMAT_BC2_TYPELESS,
            DXGI_FORMAT_BC2_UNORM,
            DXGI_FORMAT_BC2_UNORM_SRGB,
            DXGI_FORMAT_BC3_TYPELESS,
            DXGI_FORMAT_BC3_UNORM,
            DXGI_FORMAT_BC3_UNORM_SRGB,
            DXGI_FORMAT_BC4_TYPELESS,
            DXGI_FORMAT_BC4_UNORM,
            DXGI_FORMAT_BC4_SNORM,
            DXGI_FORMAT_BC5_TYPELESS,
            DXGI_FORMAT_BC5_UNORM,
            DXGI_FORMAT_BC5_SNORM,
            DXGI_FORMAT_B5G6R5_UNORM,
            DXGI_FORMAT_B5G5R5A1_UNORM,
            DXGI_FORMAT_B8G8R8A8_UNORM,
            DXGI_FORMAT_B8G8R8X8_UNORM,
            DXGI_FORMAT_R10G10B10_XR_BIAS_A2_UNORM,
            DXGI_FORMAT_B8G8R8A8_TYPELESS,
            DXGI_FORMAT_B8G8R8A8_UNORM_SRGB,
            DXGI_FORMAT_B8G8R8X8_TYPELESS,
            DXGI_FORMAT_B8G8R8X8_UNORM_SRGB,
            DXGI_FORMAT_BC6H_TYPELESS,
            DXGI_FORMAT_BC6H_UF16,
            DXGI_FORMAT_BC6H_SF16,
            DXGI_FORMAT_BC7_TYPELESS,
            DXGI_FORMAT_BC7_UNORM,
            DXGI_FORMAT_BC7_UNORM_SRGB,
            DXGI_FORMAT_AYUV,
            DXGI_FORMAT_Y410,
            DXGI_FORMAT_Y416,
            DXGI_FORMAT_NV12,
            DXGI_FORMAT_P010,
            DXGI_FORMAT_P016,
            DXGI_FORMAT_420_OPAQUE,
            DXGI_FORMAT_YUY2,
            DXGI_FORMAT_Y210,
            DXGI_FORMAT_Y216,
            DXGI_FORMAT_NV11,
            DXGI_FORMAT_AI44,
            DXGI_FORMAT_IA44,
            DXGI_FORMAT_P8,
            DXGI_FORMAT_A8P8,
            DXGI_FORMAT_B4G4R4A4_UNORM,
            DXGI_FORMAT_P208,
            DXGI_FORMAT_V208,
            DXGI_FORMAT_V408,
            DXGI_FORMAT_SAMPLER_FEEDBACK_MIN_MIP_OPAQUE,
            DXGI_FORMAT_SAMPLER_FEEDBACK_MIP_REGION_USED_OPAQUE,
            DXGI_FORMAT_FORCE_UINT
        }

        static Dictionary<MHW_TEX_FORMAT, string> FormatTagMap = new Dictionary<MHW_TEX_FORMAT, string>() {
            {MHW_TEX_FORMAT.DXGI_FORMAT_UNKNOWN, "UNKN_"},
            {MHW_TEX_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM, "R8G8B8_"},
            {MHW_TEX_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM_SRGB, "SR8G8B8_"},
            {MHW_TEX_FORMAT.DXGI_FORMAT_R8_UNORM, "R8_"},
            {MHW_TEX_FORMAT.DXGI_FORMAT_BC1_UNORM,"DXT1L_"},
            {MHW_TEX_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB,"BC1S_"},
            {MHW_TEX_FORMAT.DXGI_FORMAT_BC4_UNORM, "BC4_"},
            {MHW_TEX_FORMAT.DXGI_FORMAT_BC5_UNORM, "BC5_"},
            {MHW_TEX_FORMAT.DXGI_FORMAT_BC6H_UF16, "BC6_"},
            {MHW_TEX_FORMAT.DXGI_FORMAT_BC7_UNORM, "BC7L_"},
            {MHW_TEX_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB, "BC7S_"}
        };

        static Dictionary<MHW_TEX_FORMAT, string> FormatMagicMap = new Dictionary<MHW_TEX_FORMAT, string>() {
            {MHW_TEX_FORMAT.DXGI_FORMAT_UNKNOWN, "UNKN"},
            {MHW_TEX_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM, "DX10"},
            {MHW_TEX_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM_SRGB, "DX10"},
            {MHW_TEX_FORMAT.DXGI_FORMAT_R8_UNORM, "DX10"},
            {MHW_TEX_FORMAT.DXGI_FORMAT_BC1_UNORM,"DXT1"},
            {MHW_TEX_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB,"DX10"},
            {MHW_TEX_FORMAT.DXGI_FORMAT_BC4_UNORM, "BC4U"},
            {MHW_TEX_FORMAT.DXGI_FORMAT_BC5_UNORM, "BC5U"},
            {MHW_TEX_FORMAT.DXGI_FORMAT_BC6H_UF16, "DX10"},
            {MHW_TEX_FORMAT.DXGI_FORMAT_BC7_UNORM, "DX10"},
            {MHW_TEX_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB, "DX10"}
        };

        static List<MHW_TEX_FORMAT> TexWith4Bpp = new List<MHW_TEX_FORMAT> { MHW_TEX_FORMAT.DXGI_FORMAT_BC1_UNORM, MHW_TEX_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB, MHW_TEX_FORMAT.DXGI_FORMAT_BC4_UNORM };
        static List<MHW_TEX_FORMAT> TexOfNewDDS = new List<MHW_TEX_FORMAT> { MHW_TEX_FORMAT.DXGI_FORMAT_BC7_UNORM, MHW_TEX_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB, MHW_TEX_FORMAT.DXGI_FORMAT_BC6H_UF16 };

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

                        string typeMagic = "";
                        string compresstype = "";
                        DXGI_FORMAT ddsformat = DXGI_FORMAT.DXGI_FORMAT_UNKNOWN;
                        MHW_TEX_FORMAT texformat = MHW_TEX_FORMAT.DXGI_FORMAT_UNKNOWN;
                        Array knownFormats = Enum.GetValues(typeof(MHW_TEX_FORMAT));
                        foreach (int known in knownFormats) {
                            if (known == type) {
                                ddsformat = (DXGI_FORMAT)Enum.Parse(typeof(DXGI_FORMAT), Enum.GetName(typeof(MHW_TEX_FORMAT), type));
                                texformat = (MHW_TEX_FORMAT)type;
                                typeMagic = FormatMagicMap[texformat];
                                compresstype = FormatTagMap[texformat];
                                break;
                            }
                        }

                        if (texformat == MHW_TEX_FORMAT.DXGI_FORMAT_UNKNOWN)
                        {
                            Console.Error.WriteLine("ERROR: Unknown TEX format {0}. " + arg, type);
                            Console.ReadLine();
                            continue;
                        }
                        string outfile_name = Program.FormatTagMap[texformat] + Path.GetFileName(arg);
                        
                        string destPath = Path.GetFullPath(Path.ChangeExtension(Path.GetDirectoryName(arg) + "\\" + outfile_name, ".dds"));
                        Directory.CreateDirectory(Path.GetDirectoryName(destPath));
                        byte[] data;
                        data = reader.ReadBytes((int)new FileInfo(arg).Length);

                        using (FileStream fsWrite = new FileStream(destPath, FileMode.Create))
                        {
                            byte[] WMagicNumberHead = Program.StringToByteArray(WMagicNumberDds);
                            fsWrite.Write(WMagicNumberHead, 0, WMagicNumberHead.Length);
                            fsWrite.Write(Program.intToBytesLittle(height), 0, 4);
                            fsWrite.Write(Program.intToBytesLittle(width), 0, 4);
                            if (TexWith4Bpp.Contains(texformat))
                            {
                                //4bpp texture size
                                fsWrite.Write(Program.intToBytesLittle(width * height / 2), 0, 4);
                            }
                            else
                            {
                                //8bpp texture size
                                fsWrite.Write(Program.intToBytesLittle(width * height), 0, 4);
                            }
                            fsWrite.Write(Program.intToBytesLittle(1), 0, 4);
                            fsWrite.Write(Program.intToBytesLittle(mipMapCount), 0, 4);
                            byte[] EmptyByte11 = Program.intToBytesLittle(0, 11);
                            fsWrite.Write(EmptyByte11, 0, EmptyByte11.Length);
                            fsWrite.Write(Program.intToBytesLittle(32), 0, 4);
                            fsWrite.Write(Program.intToBytesLittle(4), 0, 4);
                            byte[] typeMagicBytes = Program.AsciiStringToByteArray(typeMagic);
                            fsWrite.Write(typeMagicBytes, 0, typeMagicBytes.Length);
                            byte[] EmptyByte5 = Program.intToBytesLittle(0, 5);
                            fsWrite.Write(EmptyByte5, 0, EmptyByte5.Length);
                            byte[] CompressOptionByte = Program.StringToByteArray(CompressOption);
                            fsWrite.Write(CompressOptionByte, 0, CompressOptionByte.Length);
                            byte[] EmptyByte4 = Program.intToBytesLittle(0, 4);
                            fsWrite.Write(EmptyByte4, 0, EmptyByte4.Length);
                            if (typeMagic.Equals("DX10"))
                            {
                                fsWrite.Write(Program.intToBytesLittle((int)ddsformat), 0, 4);
                                byte[] ArbNumByte = Program.StringToByteArray(dx10FixedFlags);
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
                        reader.BaseStream.Position = 0x8;
                        int ddsflag = reader.ReadInt32();
                        bool isRaw = false;
                        if ((ddsflag & 0x8) == 0x8) isRaw = true;
                        int height = reader.ReadInt32();
                        int width = reader.ReadInt32();
                        reader.BaseStream.Position = 0x1C;
                        int mipMapCount = reader.ReadInt32();
                        reader.BaseStream.Position = 0x54;
                        int filetypecode = reader.ReadInt32();
                        reader.BaseStream.Position = 0x80;
                        int ddsformatint = reader.ReadInt32();
                        DXGI_FORMAT ddsformat = (DXGI_FORMAT)ddsformatint;
                        MHW_TEX_FORMAT texformat = MHW_TEX_FORMAT.DXGI_FORMAT_UNKNOWN;
                        switch (filetypecode)
                        {
                            //DX10
                            case 0x30315844:
                                string formatname = Enum.GetName(typeof(DXGI_FORMAT), ddsformatint);
                                foreach (string name in Enum.GetNames(typeof(MHW_TEX_FORMAT))) {
                                    if (formatname.Equals(name)) {
                                        texformat = (MHW_TEX_FORMAT)Enum.Parse(typeof(MHW_TEX_FORMAT), formatname);
                                    }
                                }
                                texformat = (MHW_TEX_FORMAT)Enum.Parse(typeof(MHW_TEX_FORMAT), Enum.GetName(typeof(DXGI_FORMAT), ddsformatint));
                                break;
                            //DXT1
                            case 0x31545844:
                                texformat = MHW_TEX_FORMAT.DXGI_FORMAT_BC1_UNORM;
                                break;
                            //BC4U
                            case 0x55344342:
                                texformat = MHW_TEX_FORMAT.DXGI_FORMAT_BC4_UNORM;
                                break;
                            //ATI2 BC5U
                            case 0x55354342:
                            case 0x32495441:
                                texformat = MHW_TEX_FORMAT.DXGI_FORMAT_BC5_UNORM;
                                break;
                            //Raw
                            case 0x0:
                                if(isRaw)texformat = MHW_TEX_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
                                break;
                        }

                        reader.BaseStream.Position = (FormatMagicMap[texformat].Equals("DX10") && !isRaw) ? 0x94 : 0x80;
                        byte[] data = reader.ReadBytes((int)new FileInfo(arg).Length);

                        if (texformat == MHW_TEX_FORMAT.DXGI_FORMAT_UNKNOWN)
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
                            fsWrite.Write(Program.intToBytesLittle((int)texformat), 0, 4);
                            byte[] WTexSolid = Program.StringToByteArray(TexFixedUnkn);
                            fsWrite.Write(WTexSolid, 0, WTexSolid.Length);
                            if (TexOfNewDDS.Contains(texformat))
                            {
                                fsWrite.Write(Program.intToBytesLittle(1), 0, 4);
                            }
                            else
                            {
                                fsWrite.Write(Program.intToBytesLittle(0), 0, 4);
                            }
                            fsWrite.Write(Program.intToBytesLittle(0, 4), 0, 4 * 4);
                            fsWrite.Write(Program.intToBytesLittle(-1, 8), 0, 4 * 8);
                            fsWrite.Write(Program.intToBytesLittle(width), 0, 4);
                            bool isFullWidth = isRaw || texformat == MHW_TEX_FORMAT.DXGI_FORMAT_R8_UNORM;
                            if(isFullWidth) fsWrite.Write(Program.intToBytesLittle(width), 0, 2);
                            else fsWrite.Write(Program.intToBytesLittle(width / 2), 0, 2);
                            fsWrite.Write(Program.intToBytesLittle(width), 0, 2);
                            fsWrite.Write(Program.intToBytesLittle(0, 2), 0, 4 * 2);
                            if (isFullWidth) fsWrite.Write(Program.intToBytesLittle(width), 0, 2);
                            else fsWrite.Write(Program.intToBytesLittle(width / 2), 0, 2);
                            fsWrite.Write(Program.intToBytesLittle(width), 0, 2);
                            fsWrite.Write(Program.intToBytesLittle(0, 2), 0, 4 * 2);
                            if (isFullWidth) fsWrite.Write(Program.intToBytesLittle(width), 0, 2);
                            else fsWrite.Write(Program.intToBytesLittle(width / 2), 0, 2);
                            fsWrite.Write(Program.intToBytesLittle(width), 0, 2);
                            fsWrite.Write(Program.intToBytesLittle(0, 8), 0, 4 * 8);
                            int cur_width = width;
                            int cur_height = height;

                            int base_loc = 0xb8 + mipMapCount * 8;
                            for (int i = 0; i < mipMapCount; i++)
                            {
                                fsWrite.Write(Program.intToBytesLittle(base_loc), 0, 4);
                                fsWrite.Write(Program.intToBytesLittle(0), 0, 4);
                                int maxWidth = isRaw ? 2 : 4;
                                if (TexWith4Bpp.Contains(texformat))
                                {
                                    base_loc = base_loc + cur_width * cur_height / 2;
                                }
                                else if (isRaw) {
                                    base_loc = base_loc + cur_width * cur_height * 4;
                                }
                                else
                                {
                                    base_loc = base_loc + cur_width * cur_height;
                                }
                                cur_width = cur_width / 2;
                                cur_height = cur_height / 2;
                                
                                cur_width = cur_width > maxWidth ? cur_width : maxWidth;
                                cur_height = cur_height > maxWidth ? cur_height : maxWidth;
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

        public static byte[] intToBytesLittle(int value, int repeat = 1)
        {
            byte[] src = new byte[4];
            src[3] = (byte)((value >> 24) & 0xFF);
            src[2] = (byte)((value >> 16) & 0xFF);
            src[1] = (byte)((value >> 8) & 0xFF);
            src[0] = (byte)(value & 0xFF);
            if (repeat > 1) {
                byte[] repeatsrc = new byte[4 * repeat];
                for (int i = 0; i < repeat; i++) {
                    repeatsrc[i * 4] = src[0];
                    repeatsrc[i * 4 + 1] = src[1];
                    repeatsrc[i * 4 + 2] = src[2];
                    repeatsrc[i * 4 + 3] = src[3];
                }
                return repeatsrc;
            }
            return src;
        }
    }
}
