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
using YamlDotNet.Serialization;

namespace CRandomizerStudio {
    public class RandoConfigFile {
        // null means the side does not exist. zero rooms means the side needs to be lazy-loaded
        public List<RandoConfigRoom> ASide { get; set; }
        public List<RandoConfigRoom> BSide { get; set; }
        public List<RandoConfigRoom> CSide { get; set; }

        public Dictionary<char, char> RedirectFGTiles = new Dictionary<char, char>();
    }
    public class RandoConfigRoom {
        [YamlIgnore]
        public List<RandoConfigHole> GetHoles {
            get {
                List<RandoConfigHole> holes = Holes == null ? new() : new(Holes);
                HoldsOfSide?.ForEach(hOs => hOs.Holes?.ForEach(h => {
                    h.Side = hOs.Side;
                    holes.Add(h);
                }));

                if (Subrooms is List<RandoConfigRoom> subrooms)
                    subrooms.ForEach(s => {
                        s.Holes?.ForEach(h2 => {
                            h2._InSubroom = s.Room;
                            holes.Add(h2);
                        });
                        s.HoldsOfSide?.ForEach(hOs2 => hOs2.Holes?.ForEach(h3 => {
                            h3._InSubroom = s.Room;
                            h3.Side = hOs2.Side;
                            holes.Add(h3);
                        }));
                    });
                return holes;
            }
        }
        [YamlIgnore]
        public List<RandoConfigHole> GetCollectables {
            get {
                List<RandoConfigHole> colls = new();
                Collectables?.ForEach(coll => {
                    RandoConfigHole coll2 = new();
                    coll2._InSubroom = "main";
                    coll2.Kind = HoleKind.inout;
                    coll2.Idx = coll.Idx ?? colls.Count;
                    colls.Add(coll2);
                });
                InternalEdges?.ForEach(coll => {
                    if (coll.Collectable is int idx) {
                        RandoConfigHole coll2 = new();
                        coll2._InSubroom = $"main_coll{idx}";
                        coll2.Kind = HoleKind.inout;
                        coll2.Idx = idx;
                        colls.Add(coll2);
                    }
                });

                if (Subrooms is List<RandoConfigRoom> subrooms)
                    subrooms.ForEach(s => {
                        s.Collectables?.ForEach(coll => {
                            RandoConfigHole coll2 = new();
                            coll2._InSubroom = s.Room;
                            coll2.Kind = HoleKind.inout;
                            coll2.Idx = coll.Idx ?? colls.Count;
                            colls.Add(coll2);
                        });
                        s.InternalEdges?.ForEach(coll => {
                            if (coll.Collectable is int idx) {
                                RandoConfigHole coll2 = new();
                                coll2._InSubroom = $"{s.Room}_coll{idx}";
                                coll2.Kind = HoleKind.inout;
                                coll2.Idx = idx;
                                colls.Add(coll2);
                            }
                        });
                    });
                return colls;
            }
        }

        public string Room;
        public bool? Hub { get; set; }

        public bool? SpinnersShatter;
        public bool? ForceMovingSpinnerToDust;
        public RandoConfigCoreMode Core { get; set; }

        public List<string> Flags;
        public RandoConfigReq ReqEnd { get; set; }

        public List<RandoConfigCollectable> Collectables {
            get => _Collectables;
            set {
                foreach (RandoConfigCollectable col in value) {
                    if (col.ReqBoth != null || col.ReqIn != null || col.ReqOut != null) {
                        InternalEdges.Add(new RandoConfigInternalEdge() {
                            Collectable = col.Idx,
                            MustFly = col.MustFly,
                            ReqBoth = col.ReqBoth,
                            ReqIn = col.ReqIn,
                            ReqOut = col.ReqOut,
                        });
                    } else {
                        _Collectables.Add(col);
                    }
                }
            }
        }
        [YamlIgnore]
        private List<RandoConfigCollectable> _Collectables = new List<RandoConfigCollectable>();
        public List<RandoConfigRectangle> ExtraSpace { get; set; }


        public List<RandoConfigHole> Holes { get; set; }
        public List<RandoConfigHolesOfSide> HoldsOfSide;


        public List<RandoConfigRoom> Subrooms { get; set; }
        public List<RandoConfigInternalEdge> InternalEdges { get; set; } = new List<RandoConfigInternalEdge>();

        public List<RandoConfigEdit> Tweaks { get; set; }
        public float? Worth;
    }

    public class RandoConfigRectangle {
        public int X, Y;
        public int Width, Height;
    }

    public class RandoConfigHole {
        [YamlMember(Alias = "Side")]
        public ScreenDirection? _Side { get => _side; set => _side = Side = value; }

        // _side for yaml deserialization. Side for other.
        [YamlIgnore]
        public ScreenDirection? _side;
        [YamlIgnore]
        public ScreenDirection? Side;


        public int Idx { get; set; }

        public int? LowBound { get; set; }
        public int? HighBound { get; set; }
        public string Bound { get; set; }


        public bool? HighOpen { get; set; }

        public RandoConfigReq ReqIn { get; set; }
        public RandoConfigReq ReqOut { get; set; }
        public RandoConfigReq ReqBoth { get; set; }
        public HoleKind Kind { get; set; }
        public int? Launch;
        public bool? New;
        public RandoConfigHole Split;

        [YamlIgnore]
        public string _InSubroom = "main";
        [YamlIgnore]
        public bool _InOfSide = false;
    }

    public class RandoConfigCollectable {
        public int? Idx;
        public int? X;
        public int? Y;
        public bool? MustFly;

        public RandoConfigReq ReqIn { get; set; }
        public RandoConfigReq ReqOut { get; set; }
        public RandoConfigReq ReqBoth { get; set; }

        [YamlIgnore]
        public string _InSubroom = "main";
    }

    public class RandoConfigInternalEdge {
        public String To { get; set; }
        public String Warp { get; set; }
        public RandoConfigReq ReqIn { get; set; }
        public RandoConfigReq ReqOut { get; set; }
        public RandoConfigReq ReqBoth { get; set; }

        public enum SplitKind {
            TopToBottom,
            BottomToTop,
            LeftToRight,
            RightToLeft,
        }

        public SplitKind? Split;

        public int? Collectable;
        public bool? MustFly;
        public bool? CustomWarp;
    }

    public class RandoConfigReq {
        public NumDashes? Dashes;
        public Difficulty? Difficulty;
        public bool? Key;
        public int? KeyholeID;
        public string Flag;

        public List<RandoConfigReq> And;
        public List<RandoConfigReq> Or;

        [YamlIgnore]
        public (string, bool)? GetFlag {
            get {
                string[] split = Flag?.Split(':');
                if (split?.Length > 1) {
                    var v = split[1].ToLower();
                    bool state = v == "on" || v == "set" || v == "true" || v == "yes";
                    return (split[0], state);
                }
                return null;
            }
        }
    }

    public class RandoConfigEdit {
        public String Name { get; set; }
        public int? ID { get; set; }
        public float? X { get; set; }
        public float? Y { get; set; }
        public RandoConfigDecalType? Decal { get; set; }
        public RandoConfigUpdate Update { get; set; }

        [YamlIgnore]
        public bool Done;

        public override string ToString() {
            string o = $"{Name ?? "*"}:{(ID == null ? "*" : ID.ToString())}, X:{(X == null ? "*" : X.ToString())}, Y:{(Y == null ? "*" : Y.ToString())}";
            if (Decal != RandoConfigDecalType.None) {
                o += $" {Decal}";
            }

            return o;
        }
    }

    public class RandoConfigUpdate {
        public bool? Remove { get; set; }
        public bool? Add { get; set; }
        public bool? Default { get; set; }

        public string Name { get; set; }
        public float? X { get; set; }
        public float? Y { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public float? ScaleX { get; set; }
        public float? ScaleY { get; set; }
        public List<RandoConfigNode> Nodes { get; set; }
        public Dictionary<string, string> Values { get; set; }
        public string Tile;
    }

    public enum RandoConfigDecalType {
        None, FG, BG,
    }

    public class RandoConfigNode {
        public int Idx { get; set; }
        public float? X { get; set; }
        public float? Y { get; set; }
    }

    public class RandoConfigCoreMode {
        private CoreModes? left, right, up, down;
        public CoreModes? All = null;

        public CoreModes? Left {
            get => left ?? All;
            set => left = value;
        }

        public CoreModes? Right {
            get => right ?? All;
            set => right = value;
        }

        public CoreModes? Up {
            get => up ?? All;
            set => up = value;
        }

        public CoreModes? Down {
            get => down ?? All;
            set => down = value;
        }
    }


    public class RandoConfigHolesOfSide {
        public ScreenDirection Side { get; set; }
        public List<RandoConfigHole> Holes { get; set; }
    }
    public enum ScreenDirection {
        Up, Down, Left, Right
    }
    public enum Difficulty {
        easy,
        normal,
        hard,
        expert,
        master,
        perfect,
        Last
    }
    public enum NumDashes {
        zero,
        one,
        two,
        Last
    }
    public enum HoleKind {
        none, @in, @out, inout, unknown
    }
    public enum CoreModes {
        none,
        hot,
        cold
    }
    public enum AreaModes {
        ASide,
        BSide,
        CSide
    }
}