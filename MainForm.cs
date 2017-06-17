using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;

namespace PortableLauncher
{
    public partial class MainForm : Form
    {
        LauncherSetting setting = new LauncherSetting();
        private const string SETTING_FILE = "setting.ini";
        private bool exitFlg = false;
        public MainForm()
        {
            InitializeComponent();
            if (!System.IO.File.Exists(SETTING_FILE))
            {
                try
                {
                    // 設定ファイルが存在しない場合はようこそ画面を表示し、編集画面へ誘導します。
                    if (MessageBox.Show("Portable Launcherへようこそ" + Environment.NewLine + "編集画面を表示しますか？", "初期化", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        Show();
                    }
                }
                catch (Exception excep)
                {
                    MessageBox.Show(excep.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    throw;
                }
            }
            else
            {
                try
                {
                    // 末に存在している場合は設定ファイルを読み込みます。
                    System.IO.StreamReader settingReader = new System.IO.StreamReader(SETTING_FILE, new UTF8Encoding(false), false);
                    for (int i = 0; settingReader.EndOfStream == false; i++)
                    {
                        String readStr = settingReader.ReadLine();
                        int separatePos = readStr.IndexOf("<>");
                        SettingItem addItem = new SettingItem(1);
                        addItem.appPath = readStr.Substring(0, separatePos);
                        addItem.appName = readStr.Substring(separatePos + "<>".Length);
                        setting.items.Add(addItem);
                        appListBox.Items.Add(addItem.appName);
                    }
                    settingReader.Close();
                }
                catch (Exception excep)
                {
                    MessageBox.Show(excep.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    throw;
                }
                menuInit();
            }
        }

        private void menuInit()
        {
            mainContextMenuStrip.Items.Clear();
            try
            {
                foreach (SettingItem item in setting.items)
                {
                    ToolStripMenuItem addMenu = new ToolStripMenuItem();
                    addMenu.Text = item.appName;
                    addMenu.Tag = item.appPath;
                    addMenu.Click += new EventHandler(menuItem_Click);
                    mainContextMenuStrip.Items.Add(addMenu);
                }
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
            mainContextMenuStrip.Items.Add(toolStripSeparator);
            mainContextMenuStrip.Items.Add(editToolStripMenuItem);
            mainContextMenuStrip.Items.Add(exitToolStripMenuItem);
        }

        private void EditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (exitFlg)
            {
                Application.Exit();
            }
            else
            {
                Visible = false;
                e.Cancel = true;
            }
        }

        private void mainNotifyIcon_Click(object sender, EventArgs e)
        {
            mainContextMenuStrip.Show(MousePosition);
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
        }

        private void mainNotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            mainContextMenuStrip.Hide();
            Show();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            if (browseOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                pathTextBox.Text = browseOpenFileDialog.FileName;
                nameTextBox.Text = System.IO.Path.GetFileNameWithoutExtension(browseOpenFileDialog.FileName);
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (!System.IO.File.Exists(pathTextBox.Text))
            {
                MessageBox.Show("ファイルが見つかりません！", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            SettingItem addItem = new SettingItem(1);
            addItem.setApp(pathTextBox.Text, nameTextBox.Text);
            setting.items.Add(addItem);
            appListBox.Items.Add(nameTextBox.Text);
        }

        private void appListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 未選択状態の場合は選択解除で終わる
            if (appListBox.SelectedIndex == -1)
            {
                appListBox.ClearSelected();
                return;
            }
            // 項目を選択した場合
            SettingItem selectedItem = setting.items[appListBox.SelectedIndex];
            nameTextBox.Text = selectedItem.appName;
            pathTextBox.Text = selectedItem.appPath;
            editButton.Enabled = true;
            deleteButton.Enabled = true;
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            if (appListBox.SelectedIndex == -1)
            {
                MessageBox.Show("項目が選択されていません！", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SettingItem selectedItem = setting.items[appListBox.SelectedIndex];
            selectedItem.appPath = pathTextBox.Text;
            selectedItem.appName = nameTextBox.Text;
            appListBox.Items[appListBox.SelectedIndex] = nameTextBox.Text; // リストボックスの変更も忘れずに
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (appListBox.SelectedIndex == -1)
            {
                MessageBox.Show("項目が選択されていません！", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            setting.items.Remove(setting.items[appListBox.SelectedIndex]);
            appListBox.Items.Remove(appListBox.SelectedItem); // SelectedIndexの値が変更されるためリストの削除は最後にする
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Visible = false;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            try
            {
                // 保存処理
                System.IO.StreamWriter settingWriter = new System.IO.StreamWriter(SETTING_FILE, false, new UTF8Encoding(false));
                foreach (SettingItem item in setting.items)
                {
                    settingWriter.WriteLine(item.appPath + "<>" + item.appName);
                }
                settingWriter.Close();
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
            Visible = false;
            menuInit();
        }

        private void menuItem_Click(object sender, EventArgs e)
        {
            // 項目の不具合確認
            if (!sender.GetType().Equals(typeof(ToolStripMenuItem)))
            {
                MessageBox.Show("クリックした項目に不具合が発生しています！", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // 問題なければ項目を導入
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;
            if (!System.IO.File.Exists((string)clickedItem.Tag))
            {
                MessageBox.Show("指定したファイルが見つかりません！" + Environment.NewLine + "ファイルのパス：" + (string)Tag, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            System.Diagnostics.Process.Start((string)clickedItem.Tag);
        }

        private void separateButton_Click(object sender, EventArgs e)
        {
            // →区切り機能の実装は届かぬ...
            SettingItem addItem = new SettingItem(0);
            setting.items.Add(addItem);
            appListBox.Items.Add("-----");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exitFlg = true;
            Close();
        }

        private void mainContextMenuStrip_MouseLeave(object sender, EventArgs e)
        {
            mainContextMenuStrip.Close();
        }

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            // →まだ作っていない
        }

        /*
         * ここからさきは各項目のヘルプを表示する箇所。
         * →たまにヘルプのポップアップが飛ぶ原因を探す。
         */

        private void pathTextBox_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            Help.ShowPopup(pathTextBox, "登録するアプリケーションのパスを指定します。絶対パスの他に相対パスでも指定できます。", pathTextBox.Location);
        }

        private void nameTextBox_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            Help.ShowPopup(nameTextBox, "登録するアプリケーションの名前を指定します。", nameTextBox.Location);
        }

        private void addButton_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            Help.ShowPopup(addButton, "アプリケーションをランチャーのリストに登録します。", addButton.Location);
        }

        private void editButton_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            Help.ShowPopup(editButton, "ランチャーのリストで選択したアプリケーションの項目の情報を変更します。", editButton.Location);
        }

        private void deleteButton_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            Help.ShowPopup(deleteButton, "ランチャーのリストで選択したアプリケーションの項目を削除します。", deleteButton.Location);
        }
    }
}