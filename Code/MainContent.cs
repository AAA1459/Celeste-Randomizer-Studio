using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using static System.Windows.Forms.LinkLabel;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.ObjectFactories;
using System.DirectoryServices.ActiveDirectory;
using static System.Net.Mime.MediaTypeNames;
using YamlDotNet.Core.Tokens;
using System.Data;
using System.Collections;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ScrollBar;
using System.Reflection.Emit;

namespace CRandomizerStudio {
    public static class MainContent {
        public static Form1 Instance => Form1.Instance;
        public static BaseSettings Settings => Form1.Instance.Settings;


        public static void Update() {
            if (panel_WaitingRefresh) {
                Instance.panel1.Invalidate(); // 触发 Form 的 Paint 事件进行重绘
                panel_WaitingRefresh = false;
            }
            if (fileBox_WaitingScroll) {
                fileBox_ScrollToCaret(fileBox_WaitingSelectRoom);
                fileBox_WaitingScroll = fileBox_WaitingSelectRoom = false;
            }
        }
        public static bool fileBox_WaitingScroll;
        public static bool fileBox_WaitingSelectRoom;

        public static bool panel_WaitingRefresh;






        public static void RefreshRoom(bool bySelectRoom = false, bool bySelectSide = false) {
            if (Instance.randoConfigFile is null) { Instance.NewRoomBox.Text = "Invalid file!"; return; }

            List<RandoConfigRoom> rooms;
            switch (Settings.AreaMode) {
                case AreaModes.CSide:
                    rooms = Instance.randoConfigFile.CSide;
                    break;
                case AreaModes.BSide:
                    rooms = Instance.randoConfigFile.BSide;
                    break;
                case AreaModes.ASide:
                default:
                    rooms = Instance.randoConfigFile.ASide;
                    break;
            }
            if (bySelectSide && rooms?.FirstOrDefault().Room is string name) {
                Instance.SelectRoom.Text = name;
            }
            string select = Instance.SelectRoom.Text;
                int repeat = 0;
            if (select.LastIndexOf(" #") > 0) {
                if (int.TryParse(select.Substring(select.Remove(select.LastIndexOf(" #")).Length + 2), out int i)) {
                    repeat = i;
                }
                select = select.Remove(select.LastIndexOf(" #"));
            }

            if (!bySelectRoom) {
                Instance.SelectRoom.Items.Clear();
                int i = 0;
                string perName = "";
                rooms?.ForEach(side => {
                    i = perName == side.Room ? i + 1 : 0;
                    Instance.SelectRoom.Items.Add((perName = side.Room) + (i > 0 ? $" #{i}" : null));
                });
            }

            rooms = rooms?.FindAll(side => side.Room == select) ?? new();
            if (repeat < rooms.Count) {
                currentRoom = rooms[repeat];
                Instance.NewRoomBox.Text = YamlHelper.ParseAsYamlData(new List<RandoConfigRoom>() { currentRoom });

                Instance.Message = $"Refresh room: '{rooms[repeat].Room}'";
            } else { Instance.NewRoomBox.Text = "Can't find room!"; }
            ReloadHolesRange();
        }

        public static RandoConfigRoom currentRoom;
        public static Dictionary<string, RandoConfigHole> allHolesCache;

        public static int NormalHolesCount;

        public static void ReloadHolesRange() {
            if (allHolesCache == null)
                return;
            allHolesCache.Clear();
            Instance.SelectHoldA.Items.Clear();
            Instance.SelectHoldB.Items.Clear();
            List<string> range = new();
            NormalHolesCount = 0;


            currentRoom?.GetHoles?.ForEach(h => {
                if (!Instance.FilterNoneHole.Checked || h.Kind != HoleKind.none) {
                    string hkey = $"{h._InSubroom}::{h.Side}:{h.Idx}";
                    allHolesCache[hkey] = h;
                    range.Add(hkey);
                }
                if (h.Kind != HoleKind.none) {
                    NormalHolesCount++;
                }
            });
            RandoConfigHole spawn = new();
            string skey = $"main::Intro";
            spawn.Kind = HoleKind.@in;
            allHolesCache[skey] = spawn;
            Instance.SelectHoldA.Items.Add(skey);

            Instance.SelectHoldA.Items.AddRange(range.ToArray());
            Instance.SelectHoldB.Items.AddRange(range.ToArray());

            currentRoom?.GetCollectables?.ForEach(c => {
                string ckey = $"{c._InSubroom}::Coll:{c.Idx}";
                allHolesCache[ckey] = c;
                Instance.SelectHoldB.Items.Add(ckey);
            });

            if (string.IsNullOrEmpty(Instance.SelectHoldA.Text + Instance.SelectHoldB.Text)) {
                Instance.SelectHoldA.Text = "the room have no holes.";
            }

            int i = -1;
            if (!allHolesCache.ContainsKey(Instance.SelectHoldA.Text)) {
                if (range.Count > 0 && range[0] is string text && text != Instance.SelectHoldB.Text) {
                    Instance.SelectHoldA.Text = text;
                }
            } else if ((i = Instance.SelectHoldA.Items.IndexOf(Instance.SelectHoldA.Text)) > 0) {
                Instance.SelectHoldA.SelectedIndex = i;
            }

            if (!allHolesCache.ContainsKey(Instance.SelectHoldB.Text)) {
                if (range.Count > 1 && range[1] is string text && text != Instance.SelectHoldA.Text)
                    Instance.SelectHoldB.Text = text;
            } else if ((i = Instance.SelectHoldB.Items.IndexOf(Instance.SelectHoldB.Text)) > 0) {
                Instance.SelectHoldB.SelectedIndex = i;
            }
            panel_WaitingRefresh = true;
        }
        public static RandoConfigHole GetHoleA() {
            if (allHolesCache.TryGetValue(Instance.SelectHoldA.Text, out var hole)) {
                return hole;
            }
            return null;
        }
        public static RandoConfigHole GetHoleB() {
            if (allHolesCache.TryGetValue(Instance.SelectHoldB.Text, out var hole)) {
                return hole;
            }
            return null;
        }
        public static void fileBox_ScrollToCaret(bool selectAllAtRoom = false) {
            if (Instance.fileBox == null) {
                return;
            }
            AreaModes area = Settings.AreaMode;
            string select = Instance.SelectRoom.Text;
            int repeat = 0;
            if (select.LastIndexOf(" #") > 0) {
                if (int.TryParse(select.Substring(select.Remove(select.LastIndexOf(" #")).Length + 2), out int i)) {
                    repeat = i;
                }
                select = select.Remove(select.LastIndexOf(" #"));
            }
            int j = Instance.fileBox.Text.IndexOf($"{area}:");
            if (j < 0) {
                j = 0;
            }
            int j_range = area == AreaModes.CSide ? -1 : Instance.fileBox.Text.IndexOf($"{(area == AreaModes.BSide ? "CSide:" : "BSide:")}:");

            while (j >= 0 && repeat >= 0) {
                int k = Instance.fileBox.Text.IndexOf($"\n  - Room: \"{select}\"", j);
                if (k < 0 || (j_range > 0 && j_range < k))
                    k = Instance.fileBox.Text.IndexOf($"\n  - Room: {select}", j);

                if (k >= 0) {
                    j = k + 1;
                    repeat--;
                    continue;
                }
                break;
            }
            if (j > 0) {
                int f = 0;
                if (selectAllAtRoom) {
                    f = Instance.fileBox.Text.IndexOf($"\n  - Room:", j);
                    // AAA
                    if (j > 0 && j_range > 0 && j_range == f - 6) {
                        f -= 6;
                    }
                }
                Instance.fileBox.Select(j, f > j ? f - j : 0);
                if (!Instance.fileBox.Focused) {
                    Instance.fileBox.ScrollToCaret();
                }
            }
        }
    }
    public static class StringAdd {
        private static string Current;

        public static string Get() {
            return Current;
        }
        public static void Clear() {
            Current = "";
        }
        public static string Line(string line, int level = 1) {
            string s = null;
            for (int i = 0; i < level; i++) {
                s += "  ";
            }
            return Current += $"{s}{line}\n";
        }
    }
    public static class YamlHelper {
        public static Form1 Instance => Form1.Instance;

        public static string CachePath;
        public static IDeserializer Deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
        public static ISerializer Serializer = new SerializerBuilder().ConfigureDefaultValuesHandling(DefaultValuesHandling.Preserve).Build();

        /// <summary>
        /// Builds a deserializer that will provide YamlDotNet with the given object instead of creating a new one.
        /// This will make YamlDotNet update this object when deserializing.
        /// </summary>
        /// <param name="objectToBind">The object to set fields on</param>
        /// <returns>The newly-created deserializer</returns>
        public static IDeserializer DeserializerUsing(object objectToBind) {
            IObjectFactory defaultObjectFactory = new DefaultObjectFactory();
            Type objectType = objectToBind.GetType();

            return new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                // provide the given object if type matches, fall back to default behavior otherwise.
                .WithObjectFactory(type => type == objectType ? objectToBind : defaultObjectFactory.Create(type))
                .Build();
        }

        public static T LoadConfig<T>(string path) {
            T result = default(T);
            if (!File.Exists(path))
                return result;
            try {
                using (Stream stream = File.OpenRead(path)) {
                    using (StreamReader reader = new StreamReader(stream))
                        result = YamlHelper.Deserializer.Deserialize<T>(reader);
                }
            } catch (Exception e) {
                Instance.Message = "ERROR occurred! " + e;
            }
            return result;
        }
        public static void SaveConfig<T>(string path, T values) {
            if (values.GetType() == null || values == null)
                return;

            if (File.Exists(path))
                File.Delete(path);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            try {
                using (FileStream fileStream = File.OpenWrite(path)) {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream)) {
                        YamlHelper.Serializer.Serialize(streamWriter, values, values.GetType());
                        streamWriter.Flush();
                        fileStream.Flush(true);
                    }
                }
            } catch (Exception e) {
                throw (e);
            }
        }
        public static T LoadYamlData<T>(string values) {
            T result = default(T);
            File.WriteAllText(CachePath, values);
            try {
                using (Stream stream = File.OpenRead(CachePath)) {
                    using (StreamReader reader = new StreamReader(stream))
                        result = YamlHelper.Deserializer.Deserialize<T>(reader);
                }
            } catch (Exception e) {
                Instance.Message = "ERROR occurred! " + e;
            }
            return result;
        }
        public static string ParseAsYamlData<T>(T values) {
            if (values.GetType() == null || values == null)
                return null;
            if (File.Exists(CachePath))
                File.Delete(CachePath);
            try {
                using (FileStream fileStream = File.OpenWrite(CachePath)) {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream)) {
                        YamlHelper.Serializer.Serialize(streamWriter, values, values.GetType());
                        streamWriter.Flush();
                        fileStream.Flush(true);

                    }
                }
            } catch (Exception e) {
                throw (e);
            }
            // Filter out redundant null values
            string[] lines = File.ReadAllText(CachePath).Split('\n');
            List<string> lines2 = new();
            bool preIsList = false;
            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i];
                int j = line.IndexOf(':');

                bool isList = line.IndexOf('-') > 0;
                preIsList = preIsList || isList;

                if (j < 0 || line.Length == j + 3 || line.Length == 0) { continue; }
                if (line.Substring(j + 2).StartsWith("[]")) { continue; }

                if (line.Length > 2 && preIsList && !isList) {
                    line = line.Substring(2).Insert(Level(line) - 2, "- ");
                }
                preIsList = false;
                if (Level(line) > 0 || line[0] == '-') {
                    line = Level(2) + line;
                }
                lines2.Add(line);
            }
            string text = null;
            foreach (string line3 in lines2) {
                text += $"{line3}\n";
            }
            return text;
        }

        public static int Level(string text) {
            if (string.IsNullOrEmpty(text)) { return -1; }
            int level = 0;
            while (level < text.Length && text[level] == ' ') { level++; }
            return level;
        }
        public static string Level(int level) {
            string s = "";
            while (level > 0) {
                s += " ";
                level--;
            }
            return s;
        }
        public static string Level(int level, ref bool listStart) {
            if (listStart) {
                listStart = false;
                return Level(level - 2) + "- ";
            }
            return Level(level);
        }
        public static int MultiIndexOf(this string str, char c, int startIndex, int times) {
            int j = startIndex;
            for (int i = times; i > 0; i--) {
                int k = str.IndexOf('\n', j);
                if (k < 0)
                    return j;
                j = k + 1;
            }
            return j;
        }
        public static int MultiLastIndexOf(this string str, char c, int startIndex, int times) {
            int j = startIndex;
            for (int i = times; i > 0; i--) {
                int k = str.LastIndexOf('\n', j);
                if (k < 0)
                    return j;
                j = k + 1;
            }
            return j;
        }
    }
}

