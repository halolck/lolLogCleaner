using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cleaner
{
    public partial class Form1 : Form
    {
        string savepath = "Savepath.txt";
        public Form1()
        {
            InitializeComponent();
            
            string ApplicationFileName = Path.GetFileName(Application.ExecutablePath);
            File.Move(ApplicationFileName, GenerateRandom(16)+".exe");
            Thread.Sleep(5);
            this.Text = GenerateRandom(12);
        }
        List<string> list = new List<string>();
        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            #region SavePath
            if (checkBox2.Checked == true)
            {
                Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
                using (StreamWriter writer = new StreamWriter(savepath, false, sjisEnc))
                {
                    writer.WriteLine(textBox1.Text.Replace("\r","").Replace("\n",""));
                }
                //保存パス名:Savepath.txt 上書きor作成
            }
            #endregion
            button1.Text = "処理中";
            list.Clear();
            InputLog("タスクを終了しています...");
            TaskKillAll();
            Thread.Sleep(10);
            InputLog("Logファイルを削除します。");
            DeleteLog(textBox1.Text + "\\Logs");
            DeleteLog(textBox1.Text + "\\Config");
            DeleteLog("C:\\ProgramData\\Riot Games");
            DeleteLog(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)+ "\\Riot Games");
            InputLog("ファイルが正しく削除できたか確認します。");
            FolderCheck(list);
            InputLog("全ての処理が完了しました!!");
            button1.Enabled = true;
            button1.Text = "CleanLog";
        }

        
        private void TaskKillAll()
        {
            TaskKill("RiotClientServices.exe");
            TaskKill("RiotClientServices.exe");
            TaskKill("RiotClientCrashHandler.exe");
            TaskKill("RiotClientUx.exe");
            TaskKill("RiotClientUxRender.exe");
            TaskKill("LeagueClient.exe");
            TaskKill("LeagueCrashHandler.exe");
            TaskKill("LeagueClientUx.exe");
            TaskKill("LeagueClientUxRender.exe");
        }

        private void DeleteLog(string path)
        {
            
            try
            {
                list.Add(path);
                DirectoryInfo fi = new DirectoryInfo(path);
                if(fi.Exists)
                {

                    if ((fi.Attributes & System.IO.FileAttributes.Hidden) ==System.IO.FileAttributes.Hidden)
                    {
                        //隠し属性あり
                        fi.Attributes &= ~System.IO.FileAttributes.Hidden;
                    }
                    if ((fi.Attributes & System.IO.FileAttributes.ReadOnly) == System.IO.FileAttributes.ReadOnly)
                    {
                        //読み取り専用属性あり
                        fi.Attributes &= ~System.IO.FileAttributes.ReadOnly;
                    }
                    fi.Delete();
                    InputLog($"フォルダ({path})の削除に成功しました!!");
                }
                else
                {
                    InputLog($"フォルダ({path})は存在しませんでした。スキップします。");
                    return;
                }
                
            }
            catch
            {
                InputLog($"[!]ログフォルダ({path})の削除に失敗しました。");
            }
        }
        
        private void FolderCheck(List<string> strlist)
        {
            InputLog("-----------------------------");
            foreach (var a in strlist)
            {
                
                DirectoryInfo fi = new DirectoryInfo(a);
                InputLog($"{a}:{(fi.Exists ? "存在します。":"存在しません")}");
            }
            InputLog("-----------------------------");
        }

        private void InputLog(string text)
        {
            textBox2.AppendText(textBox2.Text == "" ? text : "\r\n" + text);
            textBox2.Update();
        }

        private void TaskKill(string task)
        {
            System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcessesByName(task);

            foreach (System.Diagnostics.Process p in ps)
            {
                p.Kill();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                this.Size = new Size(404, 240);
                textBox2.Visible = true;
            }
            else
            {
                this.Size = new Size(404, 133);
                textBox2.Visible = false;
            }
                
        }

        private void button2_Click(object sender, EventArgs e)
        {

            var dlg = new CommonOpenFileDialog();
            dlg.IsFolderPicker = true;
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                textBox1.Text = dlg.FileName;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //pathチェック
            if (System.IO.File.Exists(savepath))
            {
                using (StreamReader sr = new StreamReader(savepath, Encoding.GetEncoding("Shift_JIS")))
                {
                    textBox1.Text = sr.ReadToEnd().Replace("\r", "").Replace("\n", "");
                }
            }
        }
        private static readonly string randomstr = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public string GenerateRandom(int length)
        {
            StringBuilder sb = new StringBuilder(length);
            Random r = new Random();

            for (int i = 0; i < length; i++)
            {
                int pos = r.Next(randomstr.Length);
                char c = randomstr[pos];
                sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
