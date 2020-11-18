using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System;
using Microsoft.VisualBasic;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ReadFrxRes1
{
    public partial class Form1 : System.Windows.Forms.Form
    {

        public Form1()
        {
            vb6Globals.Form1 = this;
            InitializeComponent();
        }

        [STAThread]
        static void Main()
        {
            Application.Run(new Form1());
        }

        string strFilePath = String.Empty; 
        string strFileFullPath = String.Empty; // frx,ctx
        string strTextFilePath = String.Empty; // frm,ctl
        string strFileName = String.Empty; // 二进制资源文件名
        static List<string> m_strKey = new List<string>();
        static List<string> m_Desc = new List<string>();
        static List<long> m_lngOffset = new List<long>();
        // 记录一个文件的全部偏移数据
        private void cmdOpenFile_Click(System.Object _sender, System.EventArgs _e1) 
        {
            string strFileExt = String.Empty;

            openFileDialog1.Filter = "VB二进制资源文件(*.frx;*.ctx)|*.frx;*.ctx|所有文件(*.*)|*.*";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName == "")
            {
                return ;
            }
            strFileFullPath = openFileDialog1.FileName;
            strFilePath = Modfile.ExtractFilePath(openFileDialog1.FileName);
            strFileName = Modfile.ExtractFileName(openFileDialog1.FileName);

            strFileExt = (Modfile.ExtractFileExt(strFileName)).ToLower();
            if (strFileExt == "frx")
            {
                strTextFilePath = strFilePath + Modfile.ExtractMainFileName(strFileName) + ".frm";
            }
            else if( strFileExt == "ctx")
            {
                strTextFilePath = strFilePath + Modfile.ExtractMainFileName(strFileName) + ".ctl";
            }
            else
            {
                MessageBox.Show("不支持该种格式文件！", "ReadFrxRes1");
            }
            m_Desc.Clear();
            m_strKey.Clear();
            m_lngOffset.Clear();
            this.Text = "VB Frx Resource Parse -- " + strFileName;

            ReadFrm(strTextFilePath);

            
        }

        public void ReadFrm(string FileName) 
        {
            string strLine = String.Empty;
            string[] strArray = null;
            string[] strArrayOffset = null;
            string strKey = String.Empty;
            string strKeyDesc = String.Empty;
            long lngOffset = 0;
            int i = 0;
            bool beg=false, end=false;
            string Pattern = @"([a-fA-F0-9]+)";

            List1.Items.Clear();
            using (StreamReader sReader = new StreamReader(FileName, Encoding.UTF8))
            {
                bool condition = true;
                while (true)
                {
                    strLine = sReader.ReadLine();

                    // aline=null -> 文本读完了，那么控制量condition结合if语句 跳出循环

                    // 如果文本没有读完，那么condition结合if语句的作用就是输出读到的文本
                    if (strLine == null)
                    {
                        condition = false;
                    }

                    if (condition)
                    {
                        if (strLine.Length > 5)
                        {

                            beg = strLine.TrimStart().StartsWith("Begin");
                            if (beg == true)
                                strKeyDesc = strLine.TrimStart().Substring(6);//.Remove(6);

                            //if (end == false)
                            //    end = strLine.TrimStart().StartsWith("End");

                            strArray = strLine.Split('=');// Strings.Split(strLine, "=", -1, CompareMethod.Binary);
                            strKey = (strArray[0]).Trim(); // 关键字
                            if (strArray.Length >= 2) { 
                                strArrayOffset = strArray[1].Split(':');
                                if (strArrayOffset.Length >= 2) {
                                    string strm = strArrayOffset[1].Trim();
                                    MatchCollection Matches = Regex.Matches(strArrayOffset[1].Trim(), Pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                                    if (Matches.Count != 1 || Matches[0].Value != strm)
                                        return;
                                    string hexStr = "0x" + strArrayOffset[1].Trim();

                                    lngOffset = Convert.ToInt32(hexStr, 16);
                                    //lngOffset = Convert.ToInt32(off);
                                    m_Desc.Add(strKeyDesc);

                                    m_strKey.Add(strKey);
                                    m_lngOffset.Add(lngOffset);
                                    richTextBox1.Text += strKey +" -- " + lngOffset + "\r\n";
                                    i++;

                                    List1.Items.Add(strKey + (strKey.Length >= 8 ? "\t" : "\t" + "\t") + (lngOffset).ToString("X"));
                                }
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }


        }

        private void cmdSavePicture_Click(System.Object _sender, System.EventArgs _e1) 
        {
            long lngSize = 0;
            Array bytData = null;
            long lngTmp = 0; // 读取比较标志
            const int FlagPicture = 0x746C;
            const int FlagImageList = 0xBE35204;
            // 29804      '8
            // 16
            saveFileDialog1.Filter = "全部文件(*.*)|*.*";
            saveFileDialog1.FileName = "";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName == "")
            {
                return ;
            }
            FileStream fs = File.OpenRead(strFileFullPath);
            try
            {
                fs.Seek(m_lngOffset[List1.SelectedIndex], SeekOrigin.Begin);
                byte[] src = new byte[4];
                fs.Read(src, 0, 4);
                lngSize = bytes2Int(src);
                fs.Read(src, 0, 4);
                lngTmp = bytes2Int(src);
                if (lngTmp == FlagPicture)
                {
                    fs.Read(src, 0, 4);
                    // 再跳过4位
                    byte[] bData = new byte[lngSize];
                    fs.Read(bData, 0, (int)lngSize);
                    //FileStream fw = File.OpenWrite(saveFileDialog1.FileName);
                    //fw.Write(bData, 0, (int)lngSize);
                    //fw.Close();
                    ModstdPicture.CreateImageFromBytes(saveFileDialog1.FileName, bData);
                    Picture1.Image = ModstdPicture.BytesToImage((byte[])bData);//ModstdPicture.BytesToPicture(bytData);

                }
                else if (lngTmp == FlagImageList)
                {
                    fs.Seek(16 + 4, SeekOrigin.Current);
                    byte[] bData = new byte[lngSize];
                    fs.Read(bData, 0, (int)lngSize);
                    //FileStream fw = File.OpenWrite(saveFileDialog1.FileName);
                    //fw.Write(bData,0, (int)lngSize);
                    //fw.Close();
                    ModstdPicture.CreateImageFromBytes(saveFileDialog1.FileName, bData);
                }
            }
            finally
            {
                fs.Close();
            }

            //FileSystem.FileOpen(1, strFileFullPath, Microsoft.VisualBasic.OpenMode.Binary, Microsoft.VisualBasic.OpenAccess.Default, Microsoft.VisualBasic.OpenShare.Default, 0);
            // FileSystem.Seek(1, (int)(m_lngOffset[List1.SelectedIndex] + 1));
            //// 注意，这里要加1，第1个字节是从1开始的
            //FileSystem.FileGet(1, ref lngSize,  -1);

            //// Picture控件的Picture属性，后面带着8个字节的数据
            //// picture    6C 74 00 00 7E 01 00 00
            //// imagelist  04 52 E3 0B 91 8F CE 11 9D E3 00 AA 00 4B  B8 51
            //// image      6C 74 00 00 48 42 00 00     '可能后面4位字节的数据是不同的
            //// Picture可能后面随着控件的不同会附着一些数据，再后面才是图片数据
            //FileSystem.FileGet(1, ref lngTmp,  -1);
            //if (lngTmp == FlagPicture)
            //{
            //    FileSystem.FileGet(1, ref lngTmp,  -1);
            //    // 再跳过4位
            //    bytData = new byte[(int)(lngSize - 1 - 8) + 1];
            //    FileSystem.FileGet(1, ref bytData,  -1);

            //}
            //else if( lngTmp == FlagImageList)
            //{
            //    FileSystem.Seek(1, (int)(FileSystem.Seek(1) + 16 + 4));
            //    bytData = new byte[(int)(lngSize - 1 - 16 - 8) + 1];
            //    FileSystem.FileGet(1, ref bytData,  -1);
            //}
            //else
            //{
            //}
            //FileSystem.FileClose(new int[] { 1 });

            //FileSystem.FileOpen(1, saveFileDialog1.FileName, Microsoft.VisualBasic.OpenMode.Binary, Microsoft.VisualBasic.OpenAccess.Default, Microsoft.VisualBasic.OpenShare.Default, 0);
            //FileSystem.FilePut(1,bytData, -1);
            //FileSystem.FileClose(new int[] { 1 });

            MessageBox.Show("Over", "ReadFrxRes");

        }

        private void Form_Resize(System.Object _sender, System.EventArgs _e1) 
        {
            Picture1.Left = List1.Left + List1.Width;
            Picture1.Width = this.Width - List1.Left - List1.Width - 100;

        }
        public static string Utf16ToUtf8(string utf16String)
        {

            string utf8String = String.Empty;

            //UTF-16 bytes are obtained as anarray.

            byte[] utf16Bytes = Encoding.Unicode.GetBytes(utf16String);

            //Converts UTF-16 bytes to UTF-8.

            byte[] utf8Bytes = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, utf16Bytes);

            // Add UTF8 characters inside the UTF8 bytes array.

            for (int i = 0; i > utf8Bytes.Length; i++)
            {
                //Because char always saves 2 bytes, fill char with 0

                byte[] utf8Container = new byte[2] { utf8Bytes[i], 0 };
                utf8String += BitConverter.ToChar(utf8Container, 0);

            }

            return utf8String;

        }

        public static short bytes2short(byte[] bytes)
        {
            int num = bytes[0] & 0xFF;
            num |= (bytes[1] << 8) & 0xFF00;

            return (short)num;
        }
        public static UInt32 bytes2Int(byte[] bytes)
        {
            UInt32 c = 0,c1,c2,c3;
            c = bytes[0]; c1 = bytes[1]; c2 = bytes[2]; c3 = bytes[3];
            UInt32 num = c & (UInt32)0xFF;
            num |= ((c1 << 8) & 0xFF00);
            num |= ((c2 << 16) & 0xFF0000);
            num |= (c3 << 24) & 0xFF000000;
            return num;
        }
    private void List1_Click(System.Object _sender, System.EventArgs _e1) 
        {
            long lngSize = 0;
            Array bytData = null;
            long lngTmp = 0; // 读取比较标志
            const int FlagPicture = 0x746C;
            const int FlagImageList = 0xBE35204;
            const int FlagImageList1 = 0x0452E30B;
            string fExtName = "";

            byte[] bytes;
            if (List1.Items.Count <= 0)
                return;
            // WARNING: On Error GOTO Err1 is not supported
            try 
            {

                // 29804      '8
                // 16
                // ISSUE: Object reference not set to an instance of an object. at LoadPicture
                // ISSUE: Object reference not set to an instance of an object.
                string sw = (m_strKey[List1.SelectedIndex]).ToLower();
                if (sw.StartsWith("tabpicture"))
                    sw = "tabpicture";
                textBox1.Text = m_Desc[List1.SelectedIndex];
                switch (sw)
                {
                    case "base64":
                        {
                            FileStream fs = File.OpenRead(strFileFullPath);
                            string decodedString = "", encodedString= "";
                            string[] b64s;
                            try
                            {
                                fs.Seek(m_lngOffset[List1.SelectedIndex], SeekOrigin.Begin);
                                byte[] src = new byte[4];
                                fs.Read(src, 0, 4);
                                int lSize = (int)bytes2Int(src);
                                byte[] bData = new byte[lSize+1];
                                fs.Read(bData, 0, lSize);

                                encodedString = Encoding.UTF8.GetString(bData);


                                richTextBox1.Text = "--Base64---\r\n" + encodedString;
                                encodedString = encodedString.Replace("\r", "");
                                b64s = encodedString.Split('\n');
                                //foreach ( string s in b64s)
                                //{
                                //    var bytesB64 = Convert.FromBase64String(s);
                                //    decodedString = Encoding.UTF8.GetString(bytesB64);
                                //    richTextBox1.Text += decodedString + "\r\n";
                                //}

                            }
                            catch (Exception ex)
                            {
                                richTextBox1.Text += ex.Message + "\r\n";

                            }
                            finally
                            {
                                fs.Close();
                            }


                        }
                        break;
                    case "itemdata":
                        break;
                    case "text":
                        {
                            FileStream fs = File.OpenRead(strFileFullPath);
                            try { 
                                fs.Seek(m_lngOffset[List1.SelectedIndex], SeekOrigin.Begin);
                                byte[] src = new byte[4];
                                fs.Read(src, 0, 1);
                                int lSize = Convert.ToInt32(src[0]);
                                if (lSize > 0)
                                {
                                    byte[] bData = new byte[lSize + 1];
                                    fs.Read(bData, 0, lSize);
                                    string itemdata = Encoding.Default.GetString(bData);
                                    //itemdata = Utf16ToUtf8(itemdata);
                                    richTextBox1.Text += "\r\n" + itemdata + "\r\n";
                                }
                            }
                            catch (Exception ex)
                            {
                                richTextBox1.Text += ex.Message + "\r\n";

                            }
                            finally
                            {
                                fs.Close();
                            }
                        }
                        break;
                    case "list":
                        {
                            FileStream fs = File.OpenRead(strFileFullPath);
                            string itemdata = "";
                            try
                            {
                                fs.Seek(m_lngOffset[List1.SelectedIndex], SeekOrigin.Begin);
                                byte[] src = new byte[1000];
                                fs.Read(src, 0, 2);
                                int ItemCount = bytes2short(src);
                                fs.Read(src, 0, 2);
                                int MaxItemLength = bytes2short(src);
                                richTextBox1.Text += "\r\n";
                                //richTextBox1.Text += m_Desc[List1.SelectedIndex]+ "\r\n";
                                for (int i = 0;i< ItemCount;i++)
                                {
                                    fs.Read(src, 0, 2);
                                    int l = bytes2short(src);
                                    fs.Read(src, 0, l);
                                    itemdata = Encoding.Default.GetString(src);
                                    //itemdata = Utf16ToUtf8(itemdata);
                                    richTextBox1.Text += "\r\n" + itemdata + "\r\n";
                                }
       

                                //richTextBox1.Text = "--Base64---\r\n" + encodedString;


                            }
                            catch (Exception ex)
                            {
                                richTextBox1.Text += ex.Message + "\r\n";

                            }
                            finally
                            {
                                fs.Close();
                            }
                        }
                        break;
                    case "picture":
                    case "icon":
                    case "mouseicon":
                    case "tabpicture":
                    case "toolboxbitmap":
                        { 
                            // Picture
                            FileStream fs = File.OpenRead(strFileFullPath);
                            try
                            {
                                fs.Seek(m_lngOffset[List1.SelectedIndex], SeekOrigin.Begin);
                                byte[] src = new byte[4];
                                fs.Read(src, 0, 4);
                                lngSize = bytes2Int(src);
                                fs.Read(src, 0, 4);
                                lngTmp = bytes2Int(src);
                                if (lngTmp == FlagPicture)
                                {
                                    fs.Read(src, 0, 4);
                                    lngSize = bytes2Int(src);
                                    if (lngSize > 0)
                                    {
                                        byte[] bData = new byte[lngSize];
                                        fs.Read(bData, 0, (int)lngSize);
                                        Picture1.Image = ModstdPicture.BytesToImage((byte[])bData);//ModstdPicture.BytesToPicture(bytData);
                                    }
                                    else
                                        richTextBox1.Text += "No data\r\n";
                                }
                                else if (lngTmp == FlagImageList)
                                {
                                    fs.Seek(16, SeekOrigin.Current);
                                    fs.Read(src, 0, 4);
                                    lngSize = bytes2Int(src);
                                    if (lngSize > 0)
                                    {
                                        byte[] bData = new byte[lngSize];
                                        fs.Read(bData, 0, (int)lngSize);
                                        Picture1.Image = ModstdPicture.BytesToImage((byte[])bData);// ModstdPicture.BytesToPicture(bytData);
                                    }else
                                        richTextBox1.Text += "No data\r\n";

                                }
                            }
                            finally
                            {
                                fs.Close();
                            }

                            //FileSystem.FileOpen(1, strFileFullPath, Microsoft.VisualBasic.OpenMode.Binary, Microsoft.VisualBasic.OpenAccess.Default, Microsoft.VisualBasic.OpenShare.Default, 0);
                            //FileSystem.Seek(1, (int)(m_lngOffset[List1.SelectedIndex] + 1));
                            //// 注意，这里要加1，第1个字节是从1开始的
                            //FileSystem.FileGet(1, ref lngSize,  -1);
                            //// Picture控件的Picture属性，后面带着8个字节的数据
                            //// picture    6C 74 00 00 7E 01 00 00
                            //// imagelist  04 52 E3 0B 91 8F CE 11 9D E3 00 AA 00 4B  B8 51
                            //// image      6C 74 00 00 48 42 00 00     '可能后面4位字节的数据是不同的
                            //// Picture可能后面随着控件的不同会附着一些数据，再后面才是图片数据
                            //FileSystem.FileGet(1, ref lngTmp,  -1);
                            //if (lngTmp == FlagPicture)
                            //{
                            //    FileSystem.FileGet(1, ref lngTmp,  -1);
                            //    // 再跳过4位
                            //    bytData = new byte[(int)(lngSize - 1 - 8) + 1];
                            //    FileSystem.FileGet(1, ref bytData,  -1);
                            //    Picture1.Image = ModstdPicture.BytesToImage((byte[])bytData);//ModstdPicture.BytesToPicture(bytData);

                            //}
                            //else if( lngTmp == FlagImageList)
                            //{
                            //    FileSystem.Seek(1, (int)(FileSystem.Seek(1) + 16 + 4));
                            //    bytData = new byte[(int)(lngSize - 1 - 16 - 8) + 1];
                            //    FileSystem.FileGet(1, ref bytData,  -1);
                            //    Picture1.Image = ModstdPicture.BytesToImage((byte[])bytData);// ModstdPicture.BytesToPicture(bytData);
                            //}
                            //else
                            //{
                            //}
                            //FileSystem.FileClose(new int[] { 1 });
                        }
                        break;

                    // 说明,使用BytesToPicture不能显示ICO文件和Cur文件,其它一般的图片格式可以正常显示
                    default:
                        richTextBox1.Text += "Fixme -> " + sw + "  \r\n";
                        break; 
                }

                return ;

                // WARNING: Err1: is not supported 
            }
            catch(Exception ex)
            {
                richTextBox1.Text += ex.Message + "\r\n";

            }
        }


    }

}
