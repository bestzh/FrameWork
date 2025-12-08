using UnityEngine;
using UnityEditor;
using System.Threading;
using System.Diagnostics;
public class CmdHelper
{
    private static string CmdPath = @"C:\Windows\System32\cmd.exe";

    /// <summary>
    /// 执行cmd命令
    /// 多命令请使用批处理命令连接符：
    /// <![CDATA[
    /// &:同时执行两个命令
    /// |:将上一个命令的输出,作为下一个命令的输入
    /// &&：当&&前的命令成功时,才执行&&后的命令
    /// ||：当||前的命令失败时,才执行||后的命令]]>
    /// 其他请百度
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="output"></param>
    public static void RunCmd(string cmd, out string output)
    {
        cmd = cmd.Trim().TrimEnd('&') + "&exit";//说明：不管命令是否成功均执行exit命令，否则当调用ReadToEnd()方法时，会处于假死状态
        using (Process p = new Process())
        {
            p.StartInfo.FileName = CmdPath;
            p.StartInfo.UseShellExecute = false;        //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;   //接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;  //由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;   //重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;          //不显示程序窗口
            p.Start();//启动程序

            //向cmd窗口写入命令
            p.StandardInput.WriteLine(cmd);
            p.StandardInput.AutoFlush = true;

            //获取cmd窗口的输出信息
            //output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();//等待程序执行完退出进程
            p.Close();
            //p.CloseMainWindow();

            output = "";
        }
    }

    public static string m_cmd;
    public static string m_output;
    public static bool m_success = false;
    public static void ExeCmd(string cmd, out string output)
    {
        m_cmd = cmd;
        m_success = false;
        Thread thread = new Thread(TheadStart);
        thread.Start();
        output = "";
    }

    public static void TheadStart()
    {
        string output = "";
        RunCmd(m_cmd,out output);
        m_output = output;

        m_success = true;

        //UnityEngine.Debug.Log("UIImport: " + m_output);
    }

    public static void CMD(string cmd)
    {
        float progress = 0f;
        EditorUtility.DisplayProgressBar("CMD", "CMD", progress);
        string output = "";
        CmdHelper.ExeCmd(cmd, out output);
        while (!CmdHelper.m_success)
        {
            Thread.Sleep(100);
            progress += 0.1f;
            EditorUtility.DisplayProgressBar("CMD", "CMD", progress);
        }

        progress = 1f;
        EditorUtility.DisplayProgressBar("CMD", "CMD", progress);
    }
}