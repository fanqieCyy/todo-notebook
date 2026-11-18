using System;using System.Drawing;using System.Drawing.Drawing2D;using System.Drawing.Imaging;using System.IO;using System.Collections.Generic;
class IconPack {
 static int Main(string[] args){using(var source=new Bitmap(args[0])){
  Console.WriteLine("Source: {0}x{1}, corner alpha={2}",source.Width,source.Height,source.GetPixel(source.Width-1,0).A);

  var sizes=new[]{16,24,32,48,64,128,256};var images=new List<byte[]>();
  foreach(int size in sizes){using(var bitmap=new Bitmap(size,size,PixelFormat.Format32bppArgb)){using(var g=Graphics.FromImage(bitmap)){g.Clear(Color.Transparent);g.InterpolationMode=InterpolationMode.HighQualityBicubic;g.PixelOffsetMode=PixelOffsetMode.HighQuality;float scale=Math.Min((float)size/source.Width,(float)size/source.Height);float w=source.Width*scale,h=source.Height*scale;g.DrawImage(source,(size-w)/2,(size-h)/2,w,h);}using(var ms=new MemoryStream()){bitmap.Save(ms,ImageFormat.Png);images.Add(ms.ToArray());}}}
  using(var writer=new BinaryWriter(File.Create(args[1]))){writer.Write((ushort)0);writer.Write((ushort)1);writer.Write((ushort)sizes.Length);int offset=6+16*sizes.Length;for(int i=0;i<sizes.Length;i++){writer.Write((byte)(sizes[i]==256?0:sizes[i]));writer.Write((byte)(sizes[i]==256?0:sizes[i]));writer.Write((byte)0);writer.Write((byte)0);writer.Write((ushort)1);writer.Write((ushort)32);writer.Write(images[i].Length);writer.Write(offset);offset+=images[i].Length;}foreach(var bytes in images)writer.Write(bytes);}
 }return 0;}
}

