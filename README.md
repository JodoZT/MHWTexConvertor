# MHWTexConverter
## A two-way converter of .tex and .dds files in MHW resources.

## Usage:
- The tool is developed based on .net framework 4.0.
- Drag and drop the .tex or .dds file onto the icon of the .exe file. Then it will auto generate the output file to same directory as the input file.
- If the suffix is .tex, the output file will add a prefix of it's type of compression.

You have to install a photoshop plugin like Intel Texture workshop or Nvidia texture tool to open and save the .dds file edited.

Links to the plugins:
* Intel: http://gametechdev.github.io/Intel-Texture-Works-Plugin/
* Nvidia: https://developer.nvidia.com/nvidia-texture-tools-adobe-photoshop

## To save .dds file after edited:
If the prefix of the output .dds is:
* DXT1: better use Nvidia tool and compression type:  DXT1 ARGB 4bpp | 1bit alpha, other configs stay default
* DTX3: better use Nvidia tool and compression type: DXT3 ARGB 8bpp | explicit alpha, other configs stay default
* BC5: use Intel tool and save configuration: Normal Map,  BC5 8bpp(Linear, 2 Channel tangent map), other configs stay default
* BC6H: you may never edit this type of texture, ignore it.
* BC7: use Intel tool and save configuration: Color + Alpha, BC7 8bpp Fine(sRGB, DX11+),other configs stay default
* Notice that if the texture you are editing does not contain mipmap(like icons of monsters), you should not generate the mip maps. Instead, select None but not Auto Generate as Mip Maps generation option in Intel tool.
