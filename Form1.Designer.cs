using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using static System.Windows.Forms.LinkLabel;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.ObjectFactories;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ScrollBar;
using WindowsFormsApp1.Properties;

namespace CRandomizerStudio {
    partial class Form1 {

        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            this.button1 = new System.Windows.Forms.Button();
            this.selectedFile = new System.Windows.Forms.TextBox();
            this.fileBox = new System.Windows.Forms.RichTextBox();
            this.messageBox = new System.Windows.Forms.TextBox();
            this.Savefile = new System.Windows.Forms.Button();
            this.NewRoomBox = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SelectHoldB = new System.Windows.Forms.ComboBox();
            this.SelectHoldA = new System.Windows.Forms.ComboBox();
            this.SelectRoom = new System.Windows.Forms.ComboBox();
            this.selectSide = new System.Windows.Forms.ComboBox();
            this.FilterNoneHole = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.AbilitiesBox = new System.Windows.Forms.RichTextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.PresetRequire = new System.Windows.Forms.TabPage();
            this.PresetWrite = new System.Windows.Forms.RichTextBox();
            this.FilesTabs = new System.Windows.Forms.TabControl();
            this.RandoYamlFileTab = new System.Windows.Forms.TabPage();
            this.FreeTextBox = new System.Windows.Forms.TabPage();
            this._FreeTextBox = new System.Windows.Forms.RichTextBox();
            this.panel1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.PresetRequire.SuspendLayout();
            this.FilesTabs.SuspendLayout();
            this.RandoYamlFileTab.SuspendLayout();
            this.FreeTextBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(106, 23);
            this.button1.TabIndex = 10;
            this.button1.TabStop = false;
            this.button1.Text = "Open file\r\n";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OpenFile_Click);
            // 
            // selectedFile
            // 
            this.selectedFile.Location = new System.Drawing.Point(12, 33);
            this.selectedFile.Multiline = true;
            this.selectedFile.Name = "selectedFile";
            this.selectedFile.Size = new System.Drawing.Size(735, 21);
            this.selectedFile.TabIndex = 9;
            this.selectedFile.TabStop = false;
            this.selectedFile.TextChanged += new System.EventHandler(this.SelectFile_TextChanged);
            this.selectedFile.Leave += new System.EventHandler(this.SelectFile_Leave);
            // 
            // fileBox
            // 
            this.fileBox.AutoWordSelection = true;
            this.fileBox.Location = new System.Drawing.Point(-2, -2);
            this.fileBox.Name = "fileBox";
            this.fileBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.fileBox.Size = new System.Drawing.Size(278, 545);
            this.fileBox.TabIndex = 0;
            this.fileBox.TabStop = false;
            this.fileBox.Text = "";
            this.fileBox.WordWrap = false;
            this.fileBox.TextChanged += new System.EventHandler(this.fileBox_TextChanged);
            this.fileBox.Enter += new System.EventHandler(this.fileBox_Enter);
            this.fileBox.Leave += new System.EventHandler(this.fileBox_Leave);
            // 
            // messageBox
            // 
            this.messageBox.Location = new System.Drawing.Point(205, 14);
            this.messageBox.Multiline = true;
            this.messageBox.Name = "messageBox";
            this.messageBox.Size = new System.Drawing.Size(542, 21);
            this.messageBox.TabIndex = 8;
            this.messageBox.TabStop = false;
            this.messageBox.Text = "hello world!";
            this.messageBox.TextChanged += new System.EventHandler(this.messageBox_TextChanged);
            // 
            // Savefile
            // 
            this.Savefile.Location = new System.Drawing.Point(124, 12);
            this.Savefile.Name = "Savefile";
            this.Savefile.Size = new System.Drawing.Size(75, 23);
            this.Savefile.TabIndex = 7;
            this.Savefile.TabStop = false;
            this.Savefile.Text = "Save file";
            this.Savefile.UseVisualStyleBackColor = true;
            this.Savefile.Click += new System.EventHandler(this.Savefile_Click);
            // 
            // NewRoomBox
            // 
            this.NewRoomBox.AutoWordSelection = true;
            this.NewRoomBox.EnableAutoDragDrop = true;
            this.NewRoomBox.Location = new System.Drawing.Point(304, 84);
            this.NewRoomBox.Name = "NewRoomBox";
            this.NewRoomBox.Size = new System.Drawing.Size(299, 204);
            this.NewRoomBox.TabIndex = 6;
            this.NewRoomBox.TabStop = false;
            this.NewRoomBox.Text = "";
            this.NewRoomBox.WordWrap = false;
            this.NewRoomBox.TextChanged += new System.EventHandler(this.NewRoomBox_TextChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.SelectHoldB);
            this.panel1.Controls.Add(this.SelectHoldA);
            this.panel1.Location = new System.Drawing.Point(304, 314);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(443, 317);
            this.panel1.TabIndex = 1;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // SelectHoldB
            // 
            this.SelectHoldB.FormattingEnabled = true;
            this.SelectHoldB.Location = new System.Drawing.Point(305, 3);
            this.SelectHoldB.Name = "SelectHoldB";
            this.SelectHoldB.Size = new System.Drawing.Size(135, 20);
            this.SelectHoldB.TabIndex = 0;
            this.SelectHoldB.TabStop = false;
            this.SelectHoldB.SelectedIndexChanged += new System.EventHandler(this.SelectHoldB_SelectedIndexChanged);
            // 
            // SelectHoldA
            // 
            this.SelectHoldA.FormattingEnabled = true;
            this.SelectHoldA.Location = new System.Drawing.Point(3, 3);
            this.SelectHoldA.Name = "SelectHoldA";
            this.SelectHoldA.Size = new System.Drawing.Size(135, 20);
            this.SelectHoldA.TabIndex = 1;
            this.SelectHoldA.TabStop = false;
            this.SelectHoldA.SelectedIndexChanged += new System.EventHandler(this.SelectHoldA_SelectedIndexChanged);
            // 
            // SelectRoom
            // 
            this.SelectRoom.Location = new System.Drawing.Point(364, 60);
            this.SelectRoom.Name = "SelectRoom";
            this.SelectRoom.Size = new System.Drawing.Size(239, 20);
            this.SelectRoom.TabIndex = 5;
            this.SelectRoom.TabStop = false;
            this.SelectRoom.Text = "Select room";
            this.SelectRoom.SelectedIndexChanged += new System.EventHandler(this.SelectRoom_IndexChanged);
            this.SelectRoom.TextChanged += new System.EventHandler(this.SelectRoom_TextChanged);
            this.SelectRoom.GotFocus += new System.EventHandler(this.SelectRoom_GotFocus);
            // 
            // selectSide
            // 
            this.selectSide.FormattingEnabled = true;
            this.selectSide.Items.AddRange(new object[] {
            "ASide",
            "BSide",
            "CSide"});
            this.selectSide.Location = new System.Drawing.Point(304, 60);
            this.selectSide.Name = "selectSide";
            this.selectSide.Size = new System.Drawing.Size(54, 20);
            this.selectSide.TabIndex = 4;
            this.selectSide.TabStop = false;
            this.selectSide.Text = "ASide";
            this.selectSide.SelectedIndexChanged += new System.EventHandler(this.SelectSide_IndexChanged);
            this.selectSide.Leave += new System.EventHandler(this.SelectSide_Leave);
            // 
            // FilterNoneHole
            // 
            this.FilterNoneHole.AutoSize = true;
            this.FilterNoneHole.Checked = true;
            this.FilterNoneHole.CheckState = System.Windows.Forms.CheckState.Checked;
            this.FilterNoneHole.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.FilterNoneHole.Location = new System.Drawing.Point(304, 292);
            this.FilterNoneHole.Name = "FilterNoneHole";
            this.FilterNoneHole.Size = new System.Drawing.Size(108, 16);
            this.FilterNoneHole.TabIndex = 3;
            this.FilterNoneHole.TabStop = false;
            this.FilterNoneHole.Text = "FilterNoneHole";
            this.FilterNoneHole.UseVisualStyleBackColor = true;
            this.FilterNoneHole.CheckedChanged += new System.EventHandler(this.FilterNoneHole_CheckedChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.White;
            this.tabPage2.Controls.Add(this.AbilitiesBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 40);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(130, 186);
            this.tabPage2.TabIndex = 0;
            this.tabPage2.Text = "Applicability";
            this.tabPage2.Click += new System.EventHandler(this.tabPage2_Click);
            // 
            // AbilitiesBox
            // 
            this.AbilitiesBox.EnableAutoDragDrop = true;
            this.AbilitiesBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.AbilitiesBox.Location = new System.Drawing.Point(0, 0);
            this.AbilitiesBox.Name = "AbilitiesBox";
            this.AbilitiesBox.Size = new System.Drawing.Size(130, 190);
            this.AbilitiesBox.TabIndex = 0;
            this.AbilitiesBox.TabStop = false;
            this.AbilitiesBox.Text = "";
            this.AbilitiesBox.WordWrap = false;
            this.AbilitiesBox.TextChanged += new System.EventHandler(this.AbilitiesBox_TextChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.PresetRequire);
            this.tabControl1.Location = new System.Drawing.Point(609, 58);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(138, 230);
            this.tabControl1.TabIndex = 2;
            this.tabControl1.TabStop = false;
            // 
            // PresetRequire
            // 
            this.PresetRequire.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.PresetRequire.Controls.Add(this.PresetWrite);
            this.PresetRequire.Location = new System.Drawing.Point(4, 40);
            this.PresetRequire.Name = "PresetRequire";
            this.PresetRequire.Padding = new System.Windows.Forms.Padding(3);
            this.PresetRequire.Size = new System.Drawing.Size(130, 186);
            this.PresetRequire.TabIndex = 1;
            this.PresetRequire.Text = "PresetPage";
            this.PresetRequire.UseVisualStyleBackColor = true;
            // 
            // PresetWrite
            // 
            this.PresetWrite.EnableAutoDragDrop = true;
            this.PresetWrite.ForeColor = System.Drawing.SystemColors.WindowText;
            this.PresetWrite.Location = new System.Drawing.Point(-2, -2);
            this.PresetWrite.Name = "PresetWrite";
            this.PresetWrite.Size = new System.Drawing.Size(131, 190);
            this.PresetWrite.TabIndex = 0;
            this.PresetWrite.TabStop = false;
            this.PresetWrite.Text = "";
            this.PresetWrite.WordWrap = false;
            // 
            // FilesTabs
            // 
            this.FilesTabs.Controls.Add(this.RandoYamlFileTab);
            this.FilesTabs.Controls.Add(this.FreeTextBox);
            this.FilesTabs.Location = new System.Drawing.Point(12, 60);
            this.FilesTabs.Name = "FilesTabs";
            this.FilesTabs.SelectedIndex = 0;
            this.FilesTabs.Size = new System.Drawing.Size(286, 571);
            this.FilesTabs.TabIndex = 0;
            this.FilesTabs.TabStop = false;
            this.FilesTabs.SelectedIndexChanged += new System.EventHandler(this.FilesTabs_SelectedIndexChanged);
            // 
            // RandoYamlFileTab
            // 
            this.RandoYamlFileTab.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.RandoYamlFileTab.Controls.Add(this.fileBox);
            this.RandoYamlFileTab.Location = new System.Drawing.Point(4, 22);
            this.RandoYamlFileTab.Name = "RandoYamlFileTab";
            this.RandoYamlFileTab.Padding = new System.Windows.Forms.Padding(3);
            this.RandoYamlFileTab.Size = new System.Drawing.Size(278, 545);
            this.RandoYamlFileTab.TabIndex = 0;
            this.RandoYamlFileTab.Text = "RandoYamlFile";
            this.RandoYamlFileTab.UseVisualStyleBackColor = true;
            this.RandoYamlFileTab.Click += new System.EventHandler(this.RandoYamlFileTab_Click);
            // 
            // FreeTextBox
            // 
            this.FreeTextBox.Controls.Add(this._FreeTextBox);
            this.FreeTextBox.Location = new System.Drawing.Point(4, 22);
            this.FreeTextBox.Name = "FreeTextBox";
            this.FreeTextBox.Padding = new System.Windows.Forms.Padding(3);
            this.FreeTextBox.Size = new System.Drawing.Size(278, 545);
            this.FreeTextBox.TabIndex = 1;
            this.FreeTextBox.Text = "FreeTextBox";
            this.FreeTextBox.UseVisualStyleBackColor = true;
            this.FreeTextBox.Click += new System.EventHandler(this.FreeTextBox_Click);
            // 
            // _FreeTextBox
            // 
            this._FreeTextBox.AutoWordSelection = true;
            this._FreeTextBox.Location = new System.Drawing.Point(0, 0);
            this._FreeTextBox.Name = "_FreeTextBox";
            this._FreeTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._FreeTextBox.Size = new System.Drawing.Size(278, 545);
            this._FreeTextBox.TabIndex = 0;
            this._FreeTextBox.TabStop = false;
            this._FreeTextBox.Text = "";
            this._FreeTextBox.WordWrap = false;
            this._FreeTextBox.TextChanged += new System.EventHandler(this._FreeTextBox_TextChanged);
            this._FreeTextBox.Leave += new System.EventHandler(this._FreeTextBox_Leave);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(753, 643);
            this.Controls.Add(this.FilesTabs);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.FilterNoneHole);
            this.Controls.Add(this.selectSide);
            this.Controls.Add(this.SelectRoom);
            this.Controls.Add(this.NewRoomBox);
            this.Controls.Add(this.Savefile);
            this.Controls.Add(this.messageBox);
            this.Controls.Add(this.selectedFile);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.PresetRequire.ResumeLayout(false);
            this.FilesTabs.ResumeLayout(false);
            this.RandoYamlFileTab.ResumeLayout(false);
            this.FreeTextBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }



        #endregion
        public readonly static string Title = "hello world";

        public string _OwnPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
        public string SettingFilePath;

        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.TextBox selectedFile;
        public System.Windows.Forms.RichTextBox fileBox;
        public System.Windows.Forms.TextBox messageBox;
        private System.Windows.Forms.Button Savefile;

        public ComboBox SelectRoom;
        public ComboBox selectSide;

        public static string selectedFilePath;

        public System.Windows.Forms.Panel panel1;

        public BaseSettings Settings;

        private readonly System.Drawing.Text.PrivateFontCollection _privateFontCollection = new();
        public Font courFont, courFontSmall;
        public void LoadSettings() {
            _privateFontCollection.AddFontFile(@"C:\Windows\Fonts\cour.ttf");
            courFont = new Font(_privateFontCollection.Families[0], 9f, FontStyle.Regular);
            courFontSmall = new Font(_privateFontCollection.Families[0], 7.5f, FontStyle.Regular);

            Settings = YamlHelper.LoadConfig<BaseSettings>(SettingFilePath) ?? new();
            SelectRoom.Text = Settings.prevRoom;
            int idx = Instance.SelectRoom.Items.IndexOf(SelectRoom.Text ?? "");
            if (idx > 0) {
                Instance.SelectRoom.SelectedIndex = idx;
            }

            selectedFilePath = Settings.selectedFilePath;
            SelectFile_Updated();

            // TODO

            fileBox.Font = courFont;
            NewRoomBox.Font = courFont;
            _FreeTextBox.Font = courFont;
            AbilitiesBox.Font = courFontSmall;
            PresetWrite.Font = courFont;


            selectSide.Text = Settings.areaMode;
            if (!string.IsNullOrEmpty(selectedFilePath) && File.Exists(selectedFilePath)) {
                fileBox.Text = System.IO.File.ReadAllText(selectedFilePath);
                randoConfigFile = YamlHelper.LoadConfig<RandoConfigFile>(selectedFilePath);
            }
            if (!string.IsNullOrEmpty(Settings.FreeTextBoxPath) && File.Exists(Settings.FreeTextBoxPath)) {
                _FreeTextBox.Text = System.IO.File.ReadAllText(Settings.FreeTextBoxPath);
            }

            AbilitiesBox.Text = Settings.playerAbilities;
            MainContent.allHolesCache = new Dictionary<string, RandoConfigHole>();
            MainContent.RefreshRoom();
            panel1.Refresh();

            RichTextOperation.fileBox_WaitingUpdateFullWords = true;
            filehasChange = false;
        }
        public void SaveSettings() {
            Settings.prevRoom = SelectRoom.Text;
            Settings.selectedFilePath = selectedFilePath;
            YamlHelper.SaveConfig(SettingFilePath, Settings);
        }

        public Point GetPoint(Control sender) {
            return this.PointToClient(sender.PointToScreen(Point.Empty));
        }


        public string _selectHoldA, _selectHoldB;
        public ComboBox SelectHoldA;
        public ComboBox SelectHoldB;
        public CheckBox FilterNoneHole;
        private TabPage tabPage2;
        public RichTextBox AbilitiesBox;
        private TabControl tabControl1;
        private TabPage PresetRequire;
        public RichTextBox PresetWrite;
        public TabControl FilesTabs;
        private TabPage RandoYamlFileTab;
        private TabPage FreeTextBox;
        public RichTextBox _FreeTextBox;
        public RichTextBox NewRoomBox;
    }
}

