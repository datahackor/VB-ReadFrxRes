using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
namespace ReadFrxRes1
{
    public class ModstdPicture
    {
        /// <summary>
        /// Convert Byte[] to Image
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Image BytesToImage(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            Image image = System.Drawing.Image.FromStream(ms);
            return image;
        }
        /// <summary>
        /// Convert Byte[] to a picture and Store it in file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string CreateImageFromBytes(string fileName, byte[] buffer)
        {
            string file = fileName;
            Image image = BytesToImage(buffer);
            ImageFormat format = image.RawFormat;
            if (format.Equals(ImageFormat.Jpeg))
            {
                file += ".jpeg";
            }
            else if (format.Equals(ImageFormat.Png))
            {
                file += ".png";
            }
            else if (format.Equals(ImageFormat.Bmp))
            {
                file += ".bmp";
            }
            else if (format.Equals(ImageFormat.Gif))
            {
                file += ".gif";
            }
            else if (format.Equals(ImageFormat.Icon))
            {
                file += ".icon";
            }
            System.IO.FileInfo info = new System.IO.FileInfo(file);
            System.IO.Directory.CreateDirectory(info.Directory.FullName);
            File.WriteAllBytes(file, buffer);
            return file;
        }
        //public static System.Drawing.Image BytesToPicture(Array PictureData) 
        //{
        //    System.Drawing.Image BytesToPicture = null;
        //    long[] IID_IPicture = new long[3 + 1];
        //    object oPicture = null;
        //    long nResult = 0;
        //    System.Collections.IEnumerator oStream = null;
        //    long hGlobal = 0;


        //    // Array füllen um den KlassenID (CLSID) IID_IPICTURE
        //    IID_IPicture[0] = 0x7BF80980;
        //    IID_IPicture[1] = 0x101ABF32;
        //    IID_IPicture[2] = 0xAA00BB8B;
        //    IID_IPicture[3] = 0xAB0C3000;

        //    // Stream erstellen
        //    // ISSUE: Sub, Function, or Property not defined: VarPtr
        //    CreateStreamOnHGlobal(VarPtr(PictureData[0]), 0, oStream);

        //    // OLE IPicture-Objekt erstellen
        //    nResult = OleLoadPicture(oStream, 0, 0, IID_IPicture[0], oPicture);
        //    if (nResult == 0)
        //    {
        //        BytesToPicture = oPicture;
        //    }

        //    return BytesToPicture;
        //}

        [ DllImport( "ole32.dll.dll", CharSet = CharSet.Unicode, PreserveSig = false )]
        public static extern long CreateStreamOnHGlobal(long hGlobal, long fDeleteOnRelease, System.Collections.IEnumerator lpIStream);

        [ DllImport( "oleaut32.dll.dll", CharSet = CharSet.Unicode, PreserveSig = false )]
        public static extern long OleLoadPicture(System.Collections.IEnumerator lpStream, long lSize, long fRunmode, object riid, object lpIPicture);


    }

}
