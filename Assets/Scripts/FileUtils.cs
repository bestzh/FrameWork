 /*
 * FileUtil.cs
 * 开启 ;RUNTIME_ENCRYPT;CUSTOM_CHANNEL;
 *     
 *      会加密，运行时检查文件，
 */

using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class FileUtils
{
    
    public static bool isEncrypt = false;

    public static Encoding encoding = Encoding.GetEncoding("UTF-8");

    public static string StreamingPath()
    {
        string path = Application.streamingAssetsPath;
        return path;
    }

    private static void CreateFolder(string dir)
    {
        dir = dir.Replace(@"\", "/");
        if (!Directory.Exists(dir))
        {
            if (dir.IndexOf(@"/") != -1)
            {
                CreateFolder(dir.Substring(0, dir.LastIndexOf(@"/")));
            }

            Directory.CreateDirectory(dir);
        }
    }

    private static void CreateFolderForFile(string fileName)
    {
        fileName = fileName.Replace(@"\", "/");
        Debug.Log("fileName+++++++:" + fileName);
        if (fileName.LastIndexOf("/") != -1)
        {
            string dir = fileName.Substring(0, fileName.LastIndexOf("/"));
            Debug.Log("++dir::" + dir);
            CreateFolder(dir);
        }
    }
  
    static byte key = 0xab;
    public static void Decrypt(ref byte[] pToDecrypt)
    {
        int interval = (int)(pToDecrypt.Length * 0.001f) + 1;
        for (int i = 0; i < pToDecrypt.Length; )
        {
            pToDecrypt[i] ^= key;
            i += interval;
        }
        return;
    }

    public static string ReadString(string fullpath, bool bDescrypt)
    {
        byte[] bytes = ReadBytes(fullpath, bDescrypt);


        if (bytes == null)
            return null;
        return encoding.GetString(bytes);
    }

    public static byte[] ReadBytes(string fullpath, bool bDescrypt)
    {
        string filename = fullpath;
        FileStream file = null;
        if (!File.Exists(filename))
        {
            Debug.LogWarning("path not exit: " + fullpath);
            return null;
        }
        try
        {
            file = new FileStream(filename, FileMode.Open, FileAccess.Read);

            byte[] bytes = new byte[file.Length];
            //  byte[] bytes = new byte[filename.Length];
            file.Read(bytes, 0, bytes.Length);
            file.Close();

            if (bDescrypt && isEncrypt)
            {
                Decrypt(ref bytes);
            }

            return bytes;
        }
        catch (Exception e)
        {
            Debug.LogError("Load" + filename + " error" + e.ToString());
        }
        finally
        {
            file.Close();
        }
        return null;
    }

    public static string ReadStringFromStreaming(string fileName)
    {
        return ReadStringFromStreaming(fileName, true);
    }

    public static string ReadStringFromStreaming(string fileName, bool bDescrypt)
    {
        string filePath = FileUtils.StreamingPath() + "/" + fileName;
        //return ReadString(filePath, bDescrypt);
        //Debug.Log(fileName);
        return ReadString(filePath, bDescrypt);

    }
}
