# FontImageHx

This software can convert installed fonts to binary bitmaps or hexadecimal byte arrays. It was created to simplify the creation of text for monochrome LCDs.

## Quick Guide

1. Select File -> New in Menu
   
   ![](https://github.com/karakirimu/FontImageHx/blob/Image/screenshot/MainWindow_1.png?raw=true)

2. Set the font size, etc., then press the Paste ascii button, followed by the OK button.
   
   ![](https://github.com/karakirimu/FontImageHx/blob/Image/screenshot/TextWizard_1.png?raw=true)

3. The text set (ascii) in the table is displayed. it can edit by row selection. The left area is updated by pressing Update.
   
   ![](https://github.com/karakirimu/FontImageHx/blob/Image/screenshot/MainWindow_2.png?raw=true)

   4. It can export to C-header or Bitmap image from File -> Export.

## Hexadecimal Output

White in the width direction of a binary image is represented as 1 and black as 0. 8px worth of information is packed into a single byte. If the width is not divisible by 8, it is stored left-justified.

Example

```
5x4 px (WxH)
□■□■□ -> 0x50,
□■□■□ -> 0x50,
□■■■□ -> 0x70,
□□□□□ -> 0x00

12x12 px
□□□□□□□□□□□□ -> 0x00,0x00,
□□□□□□■□□□□□ -> 0x02,0x00,
□□□□■■□■□□□□ -> 0x0D,0x00,
□□□□■□□□■□□□ -> 0x08,0x80,
□□□□■□□□■□□□ -> 0x08,0x80,
□□□□■□□■■□□□ -> 0x09,0x80,
□□□□□■■□■□□□ -> 0x06,0x80,
□□□□□□□□■□□□ -> 0x00,0x80,
□□□□□□□■□□□□ -> 0x01,0x00,
□□□□■■■□□□□□ -> 0x0E,0x00,
□□□□□□□□□□□□ -> 0x00,0x00,
□□□□□□□□□□□□ -> 0x00,0x00
```

## Installation

Download the zip file from Release, extract and run it. .NET 6.0 runtime is required.

## Environment

- NET 6.0
- Only x64 is available
- Windows 11 pro 22H2

## License

- MIT License