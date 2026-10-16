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
using static CRandomizerStudio.YamlHelper;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;

namespace CRandomizerStudio {
    public static class RichTextOperation {
        [DllImport("user32.dll")]
        private static extern int GetScrollPos(IntPtr hWnd, int nBar);
        [DllImport("user32.dll")]
        private static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
        private const int SB_VERT = 1;          // 垂直滚动条标记
        private const int WM_VSCROLL = 0x0115;    // 垂直滚动消息
        private const int SB_THUMBPOSITION = 4; // 滚动到指定位置
        private const int WM_SETREDRAW = 0x000B;

        public static void AddCustomOperation(RichTextBox box) {
            box.KeyDown += new KeyEventHandler(RichTextBox_KeyDown);
            box.GotFocus += new EventHandler(RichTextBox_GotFocus);
            box.TextChanged += new EventHandler(RichTextBox_TextChanged);


            box.KeyDown += new KeyEventHandler(RichTextBox_FillText_KeyDown);
            box.KeyUp += new KeyEventHandler(RichTextBox_FillText_KeyUp);
            box.Leave += new EventHandler(RichTextBox_FillText_Leave);
        }

        public static ListBox InitializeAutoSuggestBox() {
            autoSuggestBox = new ListBox();
            autoSuggestBox.Visible = false;
            autoSuggestBox.Width = 150;
            autoSuggestBox.Height = 100;
            autoSuggestBox.DoubleClick += AutoSuggestBox_DoubleClick;
            autoSuggestBox.TabStop = false;
            return autoSuggestBox;
        }
        public static ListBox autoSuggestBox;
        private static int currentWordStartIndex = 0;
        public static void Update() {
            if (readyAddUndoList > 0f) {
                readyAddUndoList -= Rate;
            }
            if (fileBox_WaitingUpdateFullWords) {
                AllWords = GetWords(Instance.fileBox.Text);
                fileBox_WaitingUpdateFullWords = false;
            }
        }
        public static bool fileBox_WaitingUpdateFullWords;

        private static float Rate = 1f / 60f;
        private static Dictionary<string, int> PrevTextLength = new();
        private static Dictionary<string, Stack<string>> undoList = new();
        private static float readyAddUndoList;

        private static int GetScrollPosition(RichTextBox box) {
            return GetScrollPos(box.Handle, SB_VERT);
        }
        private static void SetScrollPosition(RichTextBox box, int pos) {
            SetScrollPos(box.Handle, SB_VERT, pos, true);
            IntPtr wParam = (IntPtr)((pos << 16) | SB_THUMBPOSITION);
            SendMessage(box.Handle, WM_VSCROLL, wParam, IntPtr.Zero);
        }
        private static void StopDraw(RichTextBox box) {
            SendMessage(box.Handle, WM_SETREDRAW, (IntPtr)0, (IntPtr)0);
        }
        private static void StartDraw(RichTextBox box) {
            SendMessage(box.Handle, WM_SETREDRAW, (IntPtr)1, (IntPtr)0);
            box.Invalidate();
        }

        #region Undo
        private static void RichTextBox_KeyDown(object sender, KeyEventArgs e) {
            if (sender is not RichTextBox box)
                return;
            if (e.KeyCode == Keys.Z && (e.Control)) {
                readyAddUndoList = 0f;
                if (!undoList.ContainsKey(box.Name))
                    undoList.Add(box.Name, new Stack<string>());

                int textLength = box.TextLength;
                int selectionStart = box.SelectionStart;

                if (undoList[box.Name].Count > 0) {
                    StopDraw(box);
                    string prevText = undoList[box.Name].Pop();

                    int pos = GetScrollPosition(box);
                    box.Text = prevText;
                    SetScrollPosition(box, pos);
                    box.SelectionStart = selectionStart - textLength + prevText.Length;

                    if (undoList[box.Name].Count == 0)
                        undoList[box.Name].Push(prevText);

                    StartDraw(box);
                }
            }
        }
        private static void RichTextBox_GotFocus(object sender, EventArgs e) {
            if (sender is not RichTextBox box)
                return;
            PrevTextLength[box.Name] = box.Text.Length;

            if (!undoList.ContainsKey(box.Name))
                undoList.Add(box.Name, new Stack<string>());

            if (undoList[box.Name].Count < 1)
                undoList[box.Name].Push(box.Text);
        }
        private static void RichTextBox_TextChanged(object sender, EventArgs e) {
            if (sender is not RichTextBox box)
                return;
            if (!undoList.ContainsKey(box.Name))
                undoList.Add(box.Name, new Stack<string>());

            if (box.Focused) {
                int selectionStart = Math.Min(box.SelectionStart, box.TextLength);
                if (readyAddUndoList <= 0f && box.Text.Length > PrevTextLength[box.Name]) {
                    undoList[box.Name].Push(box.Text);
                    readyAddUndoList += 0.8f;
                }

                if (box.Text.Length > PrevTextLength[box.Name]) {
                    if (selectionStart > 1 && box.TextLength > 1 && box.Text[selectionStart - 1] == '\n') {
                        int i = box.Text.LastIndexOf('\n', selectionStart - 2) + 1;
                        int j = box.Text.IndexOf('\n', selectionStart);

                        if (i > 0 && j > i) {
                            StopDraw(box);

                            int level = Level(box.Text.Substring(i, j - i));
                            int pos = GetScrollPosition(box);
                            box.Text = box.Text.Insert(selectionStart, Level(level));
                            SetScrollPosition(box, pos);
                            box.SelectionStart = selectionStart + level;

                            StartDraw(box);
                        }
                    }
                }
                PrevTextLength[box.Name] = box.Text.Length;
            } else {
                undoList[box.Name].Clear();
            }
        }
        #endregion



        #region Word completion
        private static string[] AllWords;
        private static void CompleteWord(RichTextBox box) {

            if (autoSuggestBox.SelectedItem == null)
                return;
            StopDraw(box);
            string selectedWord = autoSuggestBox.SelectedItem.ToString();
            int cursorPosition = box.SelectionStart;

            // 选择残缺部分并替换
            box.SelectionStart = currentWordStartIndex;
            box.SelectionLength = Math.Max(0, cursorPosition - currentWordStartIndex);
            box.SelectedText = selectedWord;

            // 光标移到新单词末尾并加个空格
            box.SelectionStart = currentWordStartIndex + selectedWord.Length;
            box.SelectionLength = 0;

            autoSuggestBox.Visible = false;

            Instance.Message = $"{box.TextLength}";

            StartDraw(box);
            box.Focus();
        }
        static string[] GetWords(string input) {
            MatchCollection matches = Regex.Matches(input, @"\b[\w']*\b");

            var words = from m in matches.Cast<Match>()
                        where !string.IsNullOrEmpty(m.Value) && !int.TryParse(m.Value, out _)
                        select TrimSuffix(m.Value);

            return words.ToArray();
        }

        static string TrimSuffix(string word) {
            int apostropheLocation = word.IndexOf('\'');
            if (apostropheLocation != -1) {
                word = word.Substring(0, apostropheLocation);
            }
            return word;
        }

        private static void RichTextBox_FillText_KeyUp(object sender, KeyEventArgs e) {

            if (sender is not RichTextBox box)
                return;

            // 忽略控制键
            if ((!autoSuggestBox.Visible && e.KeyCode == Keys.ControlKey) ||
                e.KeyCode == Keys.CapsLock || e.KeyCode == Keys.Tab || e.KeyCode == Keys.Escape || e.KeyData == Keys.Back || e.KeyCode == Keys.Enter ||
                e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right) {
                return;
            }

            int cursorPosition = box.SelectionStart;
            if (cursorPosition == 0) { autoSuggestBox.Visible = false; return; }

            // 1. 获取当前正在输入的单词
            string text = box.Text;
            int wordStart = cursorPosition - 1;
            while (wordStart >= 0 && !char.IsWhiteSpace(text[wordStart]) && !char.IsPunctuation(text[wordStart])) {
                wordStart--;
            }
            wordStart++; // 修正到单词起始索引
            currentWordStartIndex = wordStart;

            string currentWord = text.Substring(wordStart, cursorPosition - wordStart);

            // 2. 匹配词库
            if (AllWords != null && !string.IsNullOrEmpty(currentWord)) {
                HashSet<string> matches = new HashSet<string>();
                Array.ForEach(AllWords, w => {
                    if (w != currentWord && w.StartsWith(currentWord, StringComparison.OrdinalIgnoreCase)) {
                        matches.Add(w);
                    }
                });
                if (matches.Count > 0) {
                    autoSuggestBox.Items.Clear();

                    foreach (var match in matches)
                        autoSuggestBox.Items.Add(match);

                    // 3. 定位提示框位置（光标下方）
                    Point point = box.GetPositionFromCharIndex(cursorPosition);
                    Point point2 = Form1.Instance.GetPoint(box);
                    point.Y += (int)box.Font.GetHeight() + 5; // 光标下一行

                    autoSuggestBox.Location = new Point(point.X + point2.X, point.Y + point2.Y);

                    autoSuggestBox.SelectedIndex = 0;
                    autoSuggestBox.Visible = true;
                    return;
                }
            }
            autoSuggestBox.Visible = false;
        }

        private static void RichTextBox_FillText_KeyDown(object sender, KeyEventArgs e) {
            if (sender is not RichTextBox box)
                return;
            autoSuggestBox.BringToFront();
            if (autoSuggestBox.Visible) {

                if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.Escape || e.KeyData == Keys.Back || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right) {
                    autoSuggestBox.Visible = false;
                }
                // 允许用户用上下方向键选择词条
                else if (e.KeyCode == Keys.Down) {
                    if (autoSuggestBox.SelectedIndex < autoSuggestBox.Items.Count - 1)
                        autoSuggestBox.SelectedIndex++;
                    e.Handled = true;
                } else if (e.KeyCode == Keys.Up) {
                    if (autoSuggestBox.SelectedIndex > 0)
                        autoSuggestBox.SelectedIndex--;
                    e.Handled = true;
                }
                  // 回车或Tab键自动补全
                  else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab) {
                    e.SuppressKeyPress = true;
                    CompleteWord(box);
                    e.Handled = true;
                }
            }
        }
        private static void RichTextBox_FillText_Leave(object sender, EventArgs e) {
            autoSuggestBox.Visible = false;
        }
        private static void AutoSuggestBox_DoubleClick(object sender, EventArgs e) {
            if (sender is not RichTextBox box)
                return;
            CompleteWord(box);
        }
        #endregion
    }
}
