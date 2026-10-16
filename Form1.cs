using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using WindowsFormsApp1.Properties;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Runtime.CompilerServices;
using System.Collections;
using static CRandomizerStudio.YamlHelper;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ScrollBar;
using System.Configuration;
using static CRandomizerStudio.RandoConfigInternalEdge;
using System.Drawing.Drawing2D;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace CRandomizerStudio {

    public partial class Form1 : Form {
        private string _Message;
        public string Message {
            get { return _Message; }
            set {
                _Message = $"[{DateTime.Now}] " + value;
                messageBox.Text = _Message;
            }
        }
        public Form1() {
            InitializeComponent();
            this.Controls.Add(RichTextOperation.InitializeAutoSuggestBox());
            this.Text = Title;
            frameTimer = new Timer();


            // 限制帧率，减轻进程的连锁反应。
            frameTimer.Interval = 16; // 16毫秒大约对应 60 FPS (1000ms / 60)
            frameTimer.Tick += FrameTimer_Tick;
            frameTimer.Start();
        }
        private void ForcedSelectingCancel(object sender, TabControlCancelEventArgs e) {
            e.Cancel = true; // 强制取消切换
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public Timer frameTimer;
        public static Form1 Instance;

        public RandoConfigFile randoConfigFile = new();

        private void Form1_Load(object sender, EventArgs e) {
            Instance = this;
            SettingFilePath = _OwnPath.Replace(".exe", "_setting.yaml");
            YamlHelper.CachePath = _OwnPath.Replace(".exe", "_cache.yaml");

            LoadSettings();

            RichTextOperation.AddCustomOperation(this.fileBox);
            RichTextOperation.AddCustomOperation(this.PresetWrite);
            RichTextOperation.AddCustomOperation(this.AbilitiesBox);
            RichTextOperation.AddCustomOperation(this._FreeTextBox);
            RichTextOperation.AddCustomOperation(this.NewRoomBox);


            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(formClosing);
            void formClosing(object sender, System.Windows.Forms.FormClosingEventArgs e) {
                _FreeTextBox_Save();
                SaveSettings();
            }
            this.fileBox.AutoWordSelection = true;
            this.NewRoomBox.AutoWordSelection = true;
        }
        private void FrameTimer_Tick(object sender, EventArgs e) {
            MainContent.Update();
            RichTextOperation.Update();
        }

        private void OpenFile_Click(object sender, EventArgs e) {
            AskOpenFile();
            Message = $"Opened '{selectedFile.Text}'";
        }

        private void SelectFile_TextChanged(object sender, EventArgs e) {
        }
        private void SelectFile_Leave(object sender, EventArgs e) {
            if (selectedFile.Modified && !OpenFile(selectedFile.Text)) {
                SelectFile_Updated();
            }
        }
        private void SelectFile_Updated() {
            if (FilesTabs.SelectedTab == RandoYamlFileTab) {
                selectedFile.Text = selectedFilePath;
            } else if (FilesTabs.SelectedTab == FreeTextBox) {
                selectedFile.Text = Settings.FreeTextBoxPath;
            }
        }

        private void AskOpenFile() {
            using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                openFileDialog.InitialDirectory = selectedFilePath ?? "c:\\";
                openFileDialog.Filter = "配置文件 (*.rando.yaml)|*.rando.yaml|所有文件 (*.*)|*.*";
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    // 获取用户选择的文件完整路径
                    OpenFile(openFileDialog.FileName);
                }
            }
        }
        private bool OpenFile(string path) {
            if (!File.Exists(path))
                return false;
            if (!(path.EndsWith(".txt") || path.EndsWith(".yaml")))
                return false;

            if (FilesTabs.SelectedTab == RandoYamlFileTab) {
                selectedFilePath = path;
                selectedFile.Text = selectedFilePath;
                fileBox.Text = System.IO.File.ReadAllText(selectedFilePath);
                randoConfigFile = YamlHelper.LoadConfig<RandoConfigFile>(selectedFilePath);
                MainContent.RefreshRoom();
                Settings.selectedFilePath = selectedFilePath;
                SaveSettings();
            } else if (FilesTabs.SelectedTab == FreeTextBox) {
                selectedFile.Text = Settings.FreeTextBoxPath = path;
                _FreeTextBox.Text = System.IO.File.ReadAllText(Settings.FreeTextBoxPath);
            }
            return true;
        }
        private void messageBox_TextChanged(object sender, EventArgs e) {
            if (!messageBox.Focused) {
                return;
            }
        }
        private bool filehasChange;
        private void fileBox_TextChanged(object sender, EventArgs e) {
            filehasChange = true;
        }
        private void fileBox_Leave(object sender, EventArgs e) {
            if (filehasChange) {
                Message = $"Change and saved file";
                File.WriteAllText(selectedFilePath, fileBox.Text);
                randoConfigFile = YamlHelper.LoadConfig<RandoConfigFile>(selectedFilePath);
                MainContent.RefreshRoom();
                RichTextOperation.fileBox_WaitingUpdateFullWords = true;
                filehasChange = false;
            }
        }
        // Save file
        private void Savefile_Click(object sender, EventArgs e) {
            System.IO.File.WriteAllText(selectedFilePath, fileBox.Text);
            System.IO.File.WriteAllText(Settings.FreeTextBoxPath, FreeTextBox.Text);
            Message = $"Saved file";
        }

        private void SelectRoom_TextChanged(object sender, EventArgs e) {
            if (!SelectRoom.Focused) {
                return;
            }
            MainContent.fileBox_WaitingScroll = true;
            MainContent.RefreshRoom(bySelectRoom: true);
        }
        private void SelectRoom_IndexChanged(object sender, EventArgs e) {
            MainContent.fileBox_WaitingScroll = true;
            MainContent.RefreshRoom(bySelectRoom: true);
        }
        private void SelectRoom_GotFocus(object sender, EventArgs e) {
            MainContent.fileBox_WaitingScroll = true;
        }
        private void NewRoomBox_TextChanged(object sender, EventArgs e) {
            if (!NewRoomBox.Focused) {
                return;
            }
            if (!string.IsNullOrEmpty(NewRoomBox.Text) && YamlHelper.LoadYamlData<List<RandoConfigRoom>>(NewRoomBox.Text) is List<RandoConfigRoom> rooms && rooms.Count > 0) {
                MainContent.currentRoom = rooms[0];
                try {
                    MainContent.ReloadHolesRange();
                } catch { }
            }
        }
        private void fileBox_Enter(object sender, EventArgs e) {
            if (NewRoomBox.Text.Length == NewRoomBox.SelectionLength) {
                MainContent.fileBox_WaitingScroll = true;
                MainContent.fileBox_WaitingSelectRoom = true;
            }
        }

        private void SelectSide_IndexChanged(object sender, EventArgs e) {
            selectSide.Text = selectSide.SelectedItem as string;
            if (Enum.TryParse(selectSide.Text, true, out AreaModes result)) {
                Settings.AreaMode = result;
            }
            MainContent.fileBox_WaitingScroll = true;
            MainContent.RefreshRoom(false, true);
            SaveSettings();
        }
        private void SelectSide_Leave(object sender, EventArgs e) {
            var oldAreaMode = Settings.areaMode;
            if (Enum.TryParse(selectSide.Text, true, out AreaModes result)) {
                Settings.AreaMode = result;
            } else if (int.TryParse(selectSide.Text, out int i) && i >= 0 && i < 3) {
                Settings.AreaMode = result;
            }
            MainContent.fileBox_WaitingScroll = true;
            MainContent.RefreshRoom(false, oldAreaMode != Settings.areaMode);
            SaveSettings();
        }

        private void SelectHoldA_SelectedIndexChanged(object sender, EventArgs e) {
            MainContent.panel_WaitingRefresh = true;
        }
        private void SelectHoldB_SelectedIndexChanged(object sender, EventArgs e) {
            MainContent.panel_WaitingRefresh = true;
        }

        private void panel1_Paint(object sender, PaintEventArgs e) {
            Graphics g = e.Graphics;
            var vcb = g.VisibleClipBounds;

            #region Basis
            bool[][] PathIsOkay = new bool[2][] { (new bool[2] { true, true }), (new bool[2] { true, true }) };

            List<List<(string, Color?)>> textpos = new List<List<(string, Color?)>>() { new List<(string, Color?)>(), new List<(string, Color?)>(), new List<(string, Color?)>(), };
            textpos[0].Add(("", null));
            textpos[0].Add(("", null));
            textpos[2].Add(("", null));
            textpos[2].Add(("", null));

            // 图形
            using (Pen pen = new Pen(Color.Gray, 2)) {
                float lineX = SelectHoldA.Left + SelectHoldA.Width + 1.5f;
                g.DrawLine(pen, lineX, vcb.Y + 1, lineX, vcb.Height);

                lineX = vcb.Width - SelectHoldB.Width - 5f;
                g.DrawLine(pen, lineX, vcb.Y + 1, lineX, vcb.Height);
            }
            RandoConfigHole holeA = MainContent.GetHoleA();
            RandoConfigHole holeB = MainContent.GetHoleB();

            // 文字

            RandoConfigReq reqP = Settings.PlayerAbilities;
            if (reqP != null) {
                ReqType(reqP, 1, -1);
            }


            bool LockHoleB = false;

            if (MainContent.currentRoom?.ReqEnd is RandoConfigReq reqE) {
                LockHoleB = true;
                holeB = new RandoConfigHole();
                holeB._InSubroom = "main";
                holeB.Kind = HoleKind.@out;
                ReqType(reqE, 2, 3);
            } else if (MainContent.currentRoom?.InternalEdges.Find(s => (s.Warp != null && s.To != null) || s.CustomWarp != null) is RandoConfigInternalEdge warp) {
                LockHoleB = true;
                holeB = new RandoConfigHole();
                holeB._InSubroom = "main";
                textpos[1].Add((warp.CustomWarp == true ? "CustomWarp: true" : $"Warp: {warp.Warp}::{warp.To}", null));
                textpos[1].Add(($"", null));

                holeB.Kind = HoleKind.@out;
                if (warp.ReqBoth is RandoConfigReq v)
                    ReqType(v, 2, 4);
                if (warp.ReqOut is RandoConfigReq v2)
                    ReqType(v2, 2, 6);
            }
            SelectHoldB.Enabled = !LockHoleB;


            if (holeA != null) {
                textpos[0].Add(($"HoleKind is '{holeA.Kind}'", null));
                textpos[0].Add(($"", null));
                if (holeA.ReqBoth is RandoConfigReq req)
                    ReqType(req, 0, 0);
                else {
                    if (holeA.ReqIn is RandoConfigReq req1)
                        ReqType(req1, 0, 1);
                    if (holeA.ReqOut is RandoConfigReq req2)
                        ReqType(req2, 0, 2);
                }
            }
            if (!LockHoleB && holeB != null) {
                textpos[2].Add(($"HoleKind is '{holeB.Kind}'", null));
                textpos[2].Add(($"", null));
                if (holeB.ReqBoth is RandoConfigReq req_)
                    ReqType(req_, 2, 0);
                else {
                    if (holeB.ReqIn is RandoConfigReq req1)
                        ReqType(req1, 2, 1);
                    if (holeB.ReqOut is RandoConfigReq req2)
                        ReqType(req2, 2, 2);
                }
            }
            if (holeA != null && holeA == holeB) {
                textpos[1].Add(("Same hole? ...", Color.Gray));
                textpos[1].Add(("", null));
            }
            #endregion

            #region ReqType
            void ReqType(RandoConfigReq req, int i, int j) {
                string reqType = "";
                reqType += j switch {
                    0 => "(ReqBoth)",
                    1 => "(ReqIn)",
                    2 => "(ReqOut)",
                    3 => "(ReqEnd)",
                    4 => "(InternalEdges)(ReqBoth)",
                    5 => "(InternalEdges)(ReqIn)",
                    6 => "(InternalEdges)(ReqOut)",
                    _ => "(PlayerAbilities)",
                };
                textpos[i].Add((reqType, null));
                Color color = j < 0 ? Color.DarkBlue : Color.Black;

                int IsB = i == 0 ? 0 : 1;
                bool pathIsOkay = true;
                if (j == 0 || j == 2 || j == 3)
                    pathIsOkay = PathIsOkay[IsB][0];
                if (pathIsOkay && j == 0 || j == 1)
                    pathIsOkay = PathIsOkay[IsB][1];

                int level = 0;
                if (req == reqP) {
                    level = 0;
                    RequireOutput(level, req);
                } else {
                    if (!AbilityCheck(req, out color)) {
                        pathIsOkay = false;
                    }
                    RequireOutput(level, req);
                    if (req.Or != null)
                        OrCheck(req.Or, level);
                    void OrCheck(List<RandoConfigReq> reqs, int level2) {
                        bool OrFine = false;
                        bool listStart = level > 1;

                        textpos[i].Add(($"{Level(level2)}Or:", Color.Black));
                        foreach (var req2 in reqs) {
                            if (AbilityCheck(req2, out color)) {
                                OrFine = true;
                            }
                            RequireOutput(level2 + 2, req2);
                            if (req2.Or != null)
                                OrCheck(req2.Or, level2 + 2);
                        }
                        if (pathIsOkay) {
                            pathIsOkay = OrFine;
                        }
                    }
                }

                if (j == 0 || j == 2 || j == 3 || j == 4 || j == 6)
                    PathIsOkay[IsB][0] = pathIsOkay;
                if (j == 0 || j == 1 || j == 5)
                    PathIsOkay[IsB][1] = pathIsOkay;

                bool AbilityCheck(RandoConfigReq req, out Color color) {
                    if (reqP == null) {
                        color = Color.Black;
                        return true;
                    }
                    color = Color.Green;
                    var val = req.GetFlag;

                    Dictionary<string, bool> flags = new();
                    if (req.GetFlag is (string, bool) f)
                        flags[f.Item1] = f.Item2;
                    foreach (var reqC in req.And ?? new())
                        if (reqC != null && reqC.GetFlag is (string, bool) f2)
                            flags[f2.Item1] = f2.Item2;

                    List<string> uncheck = new List<string>(flags.Keys);
                    if (reqP.GetFlag is (string, bool) fB && flags.TryGetValue(fB.Item1, out bool b)) {
                        uncheck.Remove(fB.Item1);
                        if (b == fB.Item2)
                            flags.Remove(fB.Item1);
                    }
                    foreach (var reqPC in reqP.And ?? new()) {
                        if (reqPC != null && reqPC.GetFlag is (string, bool) fB2 && flags.TryGetValue(fB2.Item1, out bool b2)) {
                            uncheck.Remove(fB2.Item1);
                            if (b2 == fB2.Item2)
                                flags.Remove(fB2.Item1);
                        }
                    }
                    foreach (string uc in uncheck)
                        if (flags.TryGetValue(uc, out bool f2) && f2 == false)
                            flags.Remove(uc);

                    if ((reqP.Dashes ?? NumDashes.two) < (req.Dashes ?? NumDashes.zero)
                        || (reqP.Difficulty ?? Difficulty.normal) < (req.Difficulty ?? Difficulty.normal)
                        || flags.Count > 0) {
                        //PresetWrite.Text = YamlHelper.ParseAsYamlData<Dictionary<string, bool>>(flags);
                        color = Color.DarkRed;
                        return false;
                    }
                    return true;
                }
                void RequireOutput(int level, RandoConfigReq req) {
                    bool listStart = level > 1;
                    if (req.Dashes != null)
                        textpos[i].Add(($"{Level(level, ref listStart)}Dashes: {req.Dashes}", color));
                    if (req.Difficulty != null)
                        textpos[i].Add(($"{Level(level, ref listStart)}Difficulty: {req.Difficulty}", color));
                    if (req.GetFlag is (string, bool) flag)
                        textpos[i].Add(($"{Level(level, ref listStart)}Flag: {flag.Item1}:{(flag.Item2 ? "set" : "unset")}", color));
                    if (req.And is List<RandoConfigReq> reqs) {
                        textpos[i].Add(($"{Level(level, ref listStart)}And:", color));
                        foreach (var req2 in reqs)
                            if (req2 != null)
                                RequireOutput(level + 2, req2);
                    }
                }
                textpos[i].Add(("", null));
            }
            #endregion

            #region Subrooms
            if (holeA != null && holeB != null) {
                if (MainContent.currentRoom.InternalEdges is null) {
                    goto TagSubroomAfter;
                }

                bool FB = false;
                RandoConfigInternalEdge edge = (holeA._InSubroom is "main" ?
                    MainContent.currentRoom :
                    MainContent.currentRoom?.Subrooms?.FirstOrDefault(s => s.Room == holeA._InSubroom))
                        ?.InternalEdges.Find(t => t.Collectable is null ? t.To == holeB._InSubroom : $"{holeA._InSubroom}_coll{t.Collectable}" == holeB._InSubroom);

                if (edge == null && (holeB._InSubroom is "main" ?
                    MainContent.currentRoom :
                    MainContent.currentRoom?.Subrooms?.FirstOrDefault(s => s.Room == holeB._InSubroom))
                        ?.InternalEdges.Find(t => t.Collectable is null ? t.To == holeA._InSubroom : $"{holeB._InSubroom}_coll{t.Collectable}" == holeA._InSubroom) is RandoConfigInternalEdge edge2) {
                    FB = true;
                    edge = edge2;
                }

                string holeB_InSubroom = holeB._InSubroom;
                if (!LockHoleB && holeA != holeB && holeA.Side != null && holeB.Side != null && edge == null && MainContent.NormalHolesCount == 2 && MainContent.currentRoom.Subrooms == null
                    && MainContent.currentRoom?.InternalEdges?.FirstOrDefault() is RandoConfigInternalEdge edge3 && edge3.Split is SplitKind split) {
                    #region Split
                    switch (split) {
                        case SplitKind.BottomToTop:
                            if ((holeA.Side == ScreenDirection.Left || holeA.Side == ScreenDirection.Right) && holeA.Side == holeB.Side) {
                                FB = holeA.Idx < holeB.Idx;
                                holeB_InSubroom = "Split_Top";
                            } else if ((holeA.Side != ScreenDirection.Up && holeB.Side == ScreenDirection.Up)
                                  || (FB = (holeB.Side != ScreenDirection.Up && holeA.Side == ScreenDirection.Up))) {
                                holeB_InSubroom = "Split_Top";
                            }
                            break;
                        case SplitKind.TopToBottom:
                            if ((holeA.Side == ScreenDirection.Left || holeA.Side == ScreenDirection.Right) && holeA.Side == holeB.Side) {
                                FB = holeA.Idx > holeB.Idx;
                                holeB_InSubroom = "Split_Bottom";
                            } else if ((holeA.Side != ScreenDirection.Down && holeB.Side == ScreenDirection.Down)
                                  || (FB = (holeB.Side != ScreenDirection.Down && holeA.Side == ScreenDirection.Down))) {
                                holeB_InSubroom = "Split_Bottom";
                            }
                            break;
                        case SplitKind.LeftToRight:
                            if ((holeA.Side == ScreenDirection.Up || holeA.Side == ScreenDirection.Down) && holeA.Side == holeB.Side) {
                                FB = holeA.Idx > holeB.Idx;
                                holeB_InSubroom = "Split_Right";
                            } else if ((holeA.Side != ScreenDirection.Right && holeB.Side == ScreenDirection.Right)
                                  || (FB = (holeB.Side != ScreenDirection.Right && holeA.Side == ScreenDirection.Right))) {
                                holeB_InSubroom = "Split_Right";
                            }
                            break;
                        case SplitKind.RightToLeft:
                            if ((holeA.Side == ScreenDirection.Up || holeA.Side == ScreenDirection.Down) && holeA.Side == holeB.Side) {
                                FB = holeA.Idx < holeB.Idx;
                                holeB_InSubroom = "Split_Left";
                            } else if ((holeA.Side != ScreenDirection.Left && holeB.Side == ScreenDirection.Left)
                                  || (FB = (holeB.Side != ScreenDirection.Left && holeA.Side == ScreenDirection.Left))) {
                                holeB_InSubroom = "Split_Left";
                            }
                            break;
                    }
                    #endregion
                    edge = edge3;
                }
                if (holeA._InSubroom == holeB_InSubroom)
                    goto TagSubroomAfter;
                if (edge == null)
                    goto TagSubroomIndependent;

                textpos[1].Add(($"'{(!FB ? holeA._InSubroom : holeB_InSubroom)}' vs '{(FB ? holeA._InSubroom : holeB_InSubroom)}'", null));
                textpos[1].Add(("", null));

                if (edge.ReqBoth is RandoConfigReq req)
                    ReqType(req, 1, 4);
                else {
                    if (edge.ReqIn is RandoConfigReq req1) {
                        ReqType(req1, !FB ? 0 : 2, 5);
                    } else {
                        PathIsOkay[!FB ? 1 : 0][1] = false;
                    }
                    if (edge.ReqOut is RandoConfigReq req2) {
                        ReqType(req2, FB ? 0 : 2, 6);
                    } else {
                        PathIsOkay[FB ? 1 : 0][1] = false;
                    }
                }
            }
            #endregion
            goto TagSubroomAfter;

        TagSubroomIndependent:

            textpos[1].Add(($"('{holeA._InSubroom}' vs '{holeB._InSubroom}')", null));
            textpos[1].Add(("Both rooms are independent ...", Color.DarkRed));
            textpos[1].Add(("... of each other.", Color.DarkRed));
            textpos[1].Add(("", null));

            PathIsOkay[0][1] = false;
            PathIsOkay[1][1] = false;
            goto TagSubroomAfter;

        TagSubroomAfter:
        TagEnd:

            if (holeA != null) {
                switch (holeA.Kind) {
                    case HoleKind.@out:
                    case HoleKind.unknown:
                        PathIsOkay[0][1] = false;
                        break;
                    case HoleKind.@in:
                        PathIsOkay[0][0] = false;
                        break;
                    case HoleKind.none:
                        PathIsOkay[0][0] = false;
                        PathIsOkay[0][1] = false;
                        break;
                }
            }
            if (holeB != null) {
                switch (holeB.Kind) {
                    case HoleKind.@out:
                    case HoleKind.unknown:
                        PathIsOkay[1][1] = false;
                        break;
                    case HoleKind.@in:
                        PathIsOkay[1][0] = false;
                        break;
                    case HoleKind.none:
                        PathIsOkay[1][0] = false;
                        PathIsOkay[1][1] = false;
                        break;
                }
            }

            float LeftX = 2.5f;
            float CenterX = 5 + SelectHoldA.Left + SelectHoldA.Width;
            float RightX = vcb.Width - SelectHoldB.Width - 5;

            textpos[1].Insert(0, ("", null));
            if (!LockHoleB) {
                if (PathIsOkay[0][0] && PathIsOkay[1][1])
                    textpos[1].Insert(0, ("The path B to A is feasible.", Color.Green));
                else
                    textpos[1].Insert(0, ("The path B to A is invalid.", Color.DarkRed));
            }
            if (PathIsOkay[1][0] && PathIsOkay[0][1])
                textpos[1].Insert(0, ("The path A to B is feasible.", Color.Green));
            else
                textpos[1].Insert(0, ("The path A to B is invalid.", Color.DarkRed));


            for (int i = 0; i < textpos.Count; i++) {
                var text = textpos[i];
                float X = i == 0 ? LeftX : (i == 1 ? CenterX : RightX);


                int fontSize = 8;
                float lineSpaceX = 5.5f;
                if (text.Count > 32)
                    fontSize--;
                if (text.Count > 30 && i != 1)
                    text.Insert(0, ("", null));
                if (text.Count > 42 && i != 1)
                    text.Insert(0, ("", null));
                if (text.Count > 25)
                    lineSpaceX -= 0.5f;

                float lineSpace = fontSize * lineSpaceX / 3f;
                float windowSize = vcb.Height;
                if (lineSpace * text.Count > windowSize) {
                    lineSpace = windowSize / text.Count;
                }

                Font font = new Font("Microsoft Sans Serif", fontSize);
                for (int j = 0; j < text.Count; j++) {
                    using (Brush textBrush = new SolidBrush(text[j].Item2 ?? Color.Black))
                        g.DrawString(text[j].Item1, font, textBrush, X, j * lineSpace);
                }
            }
        }


        private void checkedList_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void FilterNoneHole_CheckedChanged(object sender, EventArgs e) {
            MainContent.ReloadHolesRange();
        }

        private void tabPage2_Click(object sender, EventArgs e) {

        }
        private void RandoYamlFileTab_Click(object sender, EventArgs e) {
        }
        private void FilesTabs_SelectedIndexChanged(object sender, EventArgs e) {
            SelectFile_Updated();
        }

        private void AbilitiesBox_TextChanged(object sender, EventArgs e) {
            Settings.playerAbilities = AbilitiesBox.Text;
            MainContent.panel_WaitingRefresh = true;
            panel1.Refresh();
        }

        private void FreeTextBox_Click(object sender, EventArgs e) {

        }
        private void _FreeTextBox_Leave(object sender, EventArgs e) {
            if (_FreeTextBox.Modified) {
                _FreeTextBox_Save();
            }
        }
        private void _FreeTextBox_Save() {
            if (!string.IsNullOrEmpty(Settings.FreeTextBoxPath) && File.Exists(Settings.FreeTextBoxPath)) {
                File.Delete(Settings.FreeTextBoxPath);
                File.WriteAllText(Settings.FreeTextBoxPath, _FreeTextBox.Text);
            }
        }

        private void _FreeTextBox_TextChanged(object sender, EventArgs e) {

        }

    }
}
