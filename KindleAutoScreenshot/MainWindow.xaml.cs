﻿using System;
using System.Windows;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;

namespace KindleAutoScreenshot
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //設定ファイルを読み込む
            if (Setting.HasSettingFile)
            {
                FolderTextBox.Text = Setting.Load();
            }
        }
        CaptureAreaWindow Area;
        private void SetAreaButton_Click(object sender, RoutedEventArgs e)
        {
            //foreach (Process p in Process.GetProcesses())
            //{
            //    //Kindleをアクテイブにする
            //    if (p.MainWindowTitle.Length != 0)
            //    {
            //        if (p.ProcessName == "Kindle")
            //        {
            //            SetForegroundWindow(p.MainWindowHandle);
            //        }
            //    }
            //}
            this.Area = new CaptureAreaWindow();
            this.Area.ShowDialog();
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            //ここにスクショする処理を書く
            this.RunScreenShot();
        }

        private void RunScreenShot()
        {
            this.WindowState = WindowState.Minimized;
            foreach (Process p in Process.GetProcesses())
            {
                //メインウィンドウのタイトルがある時だけ列挙する
                if (p.MainWindowTitle.Length != 0)
                {
                    if (p.ProcessName == "Kindle")
                    {
                        //フォルダを作成する
                        string title = p.MainWindowTitle;
                        string createFileName = title.Substring(title.IndexOf("Kindle for PC -") + "Kindle for PC -".Length).Trim();
                        Directory.CreateDirectory($"{FolderTextBox.Text}\\{createFileName}");
                        //Kindleをアクティブにする
                        SetForegroundWindow(p.MainWindowHandle);
                        Thread.Sleep(2000);
                        Bitmap bm = new Bitmap(Area.XSize, Area.YSize);
                        Graphics gr = Graphics.FromImage(bm);
                        IntPtr hwnd = p.MainWindowHandle;
                        for (int i = 1; i <= int.Parse(PageTextBox.Text); i++)
                        {
                            gr.CopyFromScreen(Area.StartPosX, Area.StartPosY, 0, 0, bm.Size);
                            bm.Save($"{FolderTextBox.Text}\\{createFileName}\\{ i.ToString().PadLeft(4, '0')}.png", System.Drawing.Imaging.ImageFormat.Png);
                            SendMessage(hwnd, 0x0100, 0x27, 0x00);
                            Thread.Sleep(2000);
                        }
                    }
                }
            }
            this.WindowState = WindowState.Normal;
            this.Activate();
            System.Windows.MessageBox.Show("処理が完了しました");
        }

        private void FolderSelect_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "フォルダを指定してください。";
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            fbd.ShowNewFolderButton = true;
            fbd.ShowDialog();
            FolderTextBox.Text = fbd.SelectedPath;
            if (Setting.HasSettingFile)
            {
                Setting.Edit(FolderTextBox.Text);
            }
            else
            {
                Setting.Create(FolderTextBox.Text);
            }
        }
        //外部アプリケーションにキーを送る
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);
        //アクテイブウィンドウにする
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
