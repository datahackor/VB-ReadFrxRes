using System;
using Scripting;
namespace ReadFrxRes1
{
    public class Modfile
    {

        public static string ExtractFilePath(string Value) 
        {
            string ExtractFilePath = String.Empty;
            int tmpCount = 0;
            int MainCount = 0;
            // 获取文件所在的路径
            // WARNING: On Error Resume Next is not supported


            tmpCount = Value.Length;

            for(MainCount = 0; MainCount <= Value.Length; MainCount += 1)
            {

                // ISSUE: Potential Substring problem; VB6 Original: Mid$(Value, tmpCount, 1)
                if (Value.Substring(tmpCount - 1, 1) != "\\")
                {

                    tmpCount = tmpCount - 1;

                }
                else
                {

                    // ISSUE: Potential Substring problem; VB6 Original: Left$(Value, tmpCount)
                    ExtractFilePath = Value.Substring(0, tmpCount);

                    return ExtractFilePath;

                }

            }

            return ExtractFilePath;
        }

        public static string ExtractFileName(string TarStrings) 
        {
            string ExtractFileName = String.Empty;
            string Tmp = String.Empty;
            int MainCount = 0;
            int tmpCount = 0;
            // 从一个包含文件名的路径中提取文件名
            // WARNING: On Error Resume Next is not supported


            Tmp = "";
            tmpCount = TarStrings.Length;

            for(MainCount = 0; MainCount <= TarStrings.Length; MainCount += 1)
            {

                // ISSUE: Potential Substring problem; VB6 Original: Mid$(TarStrings, tmpCount, 1)
                if (TarStrings.Substring(tmpCount - 1, 1) != "\\")
                {

                    // ISSUE: Potential Substring problem; VB6 Original: Mid$(TarStrings, tmpCount, 1)
                    Tmp = TarStrings.Substring(tmpCount - 1, 1) + Tmp;
                    tmpCount = tmpCount - 1;

                }
                else
                {

                    ExtractFileName = Tmp;
                    return ExtractFileName;

                }

            }

            return ExtractFileName;
        }

        public static string ExtractFileExt(string Value) 
        {
            string ExtractFileExt = String.Empty;
            string Tmp = String.Empty;
            int tmpCount = 0;
            int MainCount = 0;
            // 获取文件的后辍名
            // WARNING: On Error Resume Next is not supported


            tmpCount = Value.Length;

            for(MainCount = 0; MainCount <= Value.Length; MainCount += 1)
            {

                // ISSUE: Potential Substring problem; VB6 Original: Mid$(Value, tmpCount, 1)
                if (Value.Substring(tmpCount - 1, 1) != ".")
                {

                    // ISSUE: Potential Substring problem; VB6 Original: Mid$(Value, tmpCount, 1)
                    Tmp = Value.Substring(tmpCount - 1, 1) + Tmp;
                    tmpCount = tmpCount - 1;

                }
                else
                {

                    if (Tmp != "")
                    {
                        ExtractFileExt = Tmp;
                    }

                    return ExtractFileExt;

                }

            }

            return ExtractFileExt;
        }

        public static string ExtractMainFileName(string Value) 
        {
            string ExtractMainFileName = String.Empty;
            int i = 0;
            int intCount = 0;
            string Tmp = String.Empty;
            // 从文件名中获取主文件名

            intCount = Value.Length;

            for(i = 0; i <= Value.Length; i += 1)
            {
                // ISSUE: Potential Substring problem; VB6 Original: Mid$(Value, intCount, 1)
                if (Value.Substring(intCount - 1, 1) != ".")
                {
                    // Tmp = Mid$(Value, intCount, 1) & Tmp
                    intCount = intCount - 1;

                }
                else
                {
                    // ISSUE: Potential Substring problem; VB6 Original: Left$(Value, intCount - 1)
                    ExtractMainFileName = Value.Substring(0, intCount - 1);
                    return ExtractMainFileName;

                }

            }

            return ExtractMainFileName;
        }

        public static string[] FileList(string strPath, string FileExt) 
        {
            string[] strFileList = null;
            Scripting.FileSystemObject fso = new Scripting.FileSystemObject();
            Scripting.Folder Folder1 = null;
            Scripting.Files F = null;
            Scripting.File F1 = null;
            int intFileCount = 0;
            int i = 0;
            // 参数:  strPath - 列表文件的目录
            // FileExt - 文件扩展名,支持*代表任意扩展名,即目录下的全部文件
            // 返回值：文件名列表字符数组
            // 一个目录下的文件名列表数组



            i = 0;

            FileExt = FileExt.ToLower(); // 转成小写

            Folder1 = fso.GetFolder(strPath);
            F = Folder1.Files;

            intFileCount = F.Count;
            if (intFileCount > 0)
            {
                strFileList = new string[intFileCount - 1 + 1];
            }
            else
            {
                strFileList = new string[0 + 1];
            }

            foreach(Scripting.File __each1 in F)
            {
                F1 = __each1;
                // If FileExt = "*" Then
                // Debug.Print F1.Name
                // strFileList(i) = F1.Name
                // i = i + 1
                // Else
                // If LCase$(ExtractFileExt(F1.Name)) = FileExt Then
                // Debug.Print F1.Name
                // strFileList(i) = F1.Name
                // i = i + 1
                // End If
                // 
                // End If
                if (FileExt == "*" || (FileExt != "*" && (ExtractFileExt(F1.Name)).ToLower() == FileExt))
                {
                    // Debug.Print F1.Name
                    strFileList[i] = F1.Name;
                    i = i + 1;

                }

            }

            if (i > 0)
            {
                // ISSUE: Unsupported Statement: ReDim-Preserve
                strFileList = new string[i - 1 + 1];
            }
            else
            {
                strFileList = new string[0 + 1];
            }


            return strFileList;
        }


    }

}
