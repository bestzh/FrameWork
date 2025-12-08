using UnityEngine;
using System.Collections;

using System;
using System.IO;
using UnityEditor;

public class TableImport
{
    static string m_sourcePath = @"\..\..\..\Design\2.配置文件\client";
    //static string m_targetPath = @"\StreamingAssets\Table";
    static string m_targetPath = @"\Resources\Table";

    //[MenuItem("Tools/Table/表导入")]
    public static void ImportTable()
    {
        string dataPath = Application.dataPath;
        string pathPan = dataPath.Substring(0, 1);
        string path = string.Format(@"..\Tools\ui", dataPath);
        //string cmd = @"cd&E:&&cd E:\Develop\Code\TankClient\Tools\ui&import.bat&cd&cd&pause";
        string cmd = string.Format(@"cd&cd&cd Tools&cd&cd table&cd & cd {0}&import.bat&cd&cd&pause", @"..\Tools\table");
        //Util.Log("cmd : " + cmd);
        string output = "";
        CmdHelper.ExeCmd(cmd, out output);

        ////Util.Log(Encoding.Unicode.GetString(Encoding.GetEncoding("GBK").GetBytes(output)));
    }

    public static void Import()
    {
        //AssetDatabase.StartAssetEditing();        

        Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");

        if (Directory.Exists(Application.dataPath + m_targetPath))
        {
            //RemoveFolder(Application.dataPath + m_targetPath);
        }

        CopyFolder(Application.dataPath + m_sourcePath, Application.dataPath + m_targetPath);

        //AssetDatabase.Refresh();
        //AssetDatabase.SaveAssets();

        //AssetDatabase.StopAssetEditing();

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    public static void CopyFolder(string source, string target)
    {
        DirectoryInfo folder = new DirectoryInfo(source);

        string folderName = folder.FullName;

        if (!Directory.Exists(target))
        {
            Directory.CreateDirectory(target);
        }

        foreach (FileInfo file in folder.GetFiles())
        {
            CopyFile(file.FullName, file.FullName.Replace(folderName, target));
        }

        foreach (DirectoryInfo subFolder in folder.GetDirectories())
        {
            CopyFolder(subFolder.FullName, subFolder.FullName.Replace(folderName, target));
        }
    }

    public static void CopyFile(string source, string target)
    {
        //File.Copy(source, target,true);
        int length;
        byte[] data;
        using (FileStream sFile = new FileStream(source, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
        {

            length = (int)sFile.Length;
            data = new byte[length];
            sFile.Read(data, 0, (int)length);
            sFile.Flush();
            sFile.Close();
        }

        //using (FileStream tFile = new FileStream(target, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
        //{
        //    //    tFile.Write(data, 0, length);

        //    //    tFile.Flush();
        //    //    tFile.Close();
        //}


        StreamWriter sw = System.IO.File.CreateText(target);
        sw.Close();
        System.IO.File.WriteAllBytes(target, data);
    }

    public static void CopyTextAsset(string source, string target)
    {
        FileStream sFile = new FileStream(source, FileMode.Open);
        byte[] data = new byte[sFile.Length];
        sFile.Read(data, 0, (int)sFile.Length);

        TextAsset text = new TextAsset();
        //Util.Log(target.Replace(Application.dataPath, "Assets"));
        AssetDatabase.CreateAsset(text, target.Replace(Application.dataPath, "Assets"));

    }

    public static void RemoveFolder(string path)
    {
        if (Directory.Exists(path))
        {
            DirectoryInfo folder = new DirectoryInfo(path);
            folder.Delete(true);
        }
    }
}
