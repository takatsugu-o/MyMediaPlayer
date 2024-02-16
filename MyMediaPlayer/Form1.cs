using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using WMPLib;

namespace MyMediaPlayer
{
    public partial class Form1 : Form
    {
        private bool isFullScreen;

        private Size normalPlayerSize;
        private Point normalPlayerLocation;
        

        public Form1()
        {
            InitializeComponent();
            // ドラッグ＆ドロップを受け入れるように設定
            this.AllowDrop = true;

            Player.fullScreen = false;

            normalPlayerSize = Player.Size;
            normalPlayerLocation = Player.Location;
        }

        private void PlayMedia(string url)
        {
            Player.URL = url;
        }

        private void select_video_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            if (opf.ShowDialog() == DialogResult.OK)
            {
                PlayMedia(opf.FileName);
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // ファイルがドラッグされてきた場合
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                // ファイル以外がドラッグされてきた場合
                e.Effect = DragDropEffects.None;
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            // ドロップされたデータがファイルの場合
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // ドロップされたファイルのパスを取得
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // ドロップされたファイルがある場合、最初のファイルのパスを取得
                if (files != null && files.Length > 0)
                {
                    string fileUrl = files[0]; // ドロップされた最初のファイルのパスを取得

                    PlayMedia(fileUrl);
                }
            }
        }

        private Screen GetExtendedScreen()
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen != Screen.PrimaryScreen && screen.Bounds != Screen.PrimaryScreen.Bounds)
                {
                    return screen;
                }
            }
            return null;
        }

        private void ToggleScreen()
        {
            if (isFullScreen)
            {
                // 通常スクリーンモードに切り替え
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.WindowState = FormWindowState.Normal;
                // メディアプレイヤーのサイズを元のサイズに戻す
                Player.Dock = DockStyle.None;
                Player.Size = normalPlayerSize;
                Player.Location = normalPlayerLocation;

                Player.uiMode = "Full";

                //Select ボタンを有効にする
                select_video.Enabled = true;
                select_video.Visible = true;

                //Toggle state
                isFullScreen = false;
            }
            else
            {
                // フルスクリーンモードに切り替え
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;

                // 拡張ディスプレイの情報を取得
                Screen extendedScreen = GetExtendedScreen();
                if (extendedScreen != null)
                {
                    // フォームを拡張ディスプレイに配置
                    this.Location = extendedScreen.WorkingArea.Location;
                    this.Size = extendedScreen.WorkingArea.Size;
                }


                // メディアプレイヤーのサイズをウィンドウに合わせる
                Player.Dock = DockStyle.Fill;

                Player.uiMode = "None";
                
                //Select ボタンを無効にする
                select_video.Enabled = false;
                select_video.Visible = false;

                //Toggle State
                isFullScreen = true;
            }
        }

        private void PausePlayer()
        {
            if(Player.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                Player.Ctlcontrols.pause();
            }
            else
            {
                Player.Ctlcontrols.play();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            this.Focus();
            switch (e.KeyCode)
            {
                case Keys.M:
                    ToggleScreen();
                    break;
                case Keys.Space:
                    PausePlayer();
                    break;
                case Keys.Q:
                    Application.Exit();
                    break;
            }
            
        }

        
    }
}
