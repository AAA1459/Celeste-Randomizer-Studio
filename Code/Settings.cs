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
    public class BaseSettings {
        public string selectedFilePath { get; set; } = "";
        public string prevRoom { get; set; } = "a_00";
        public string areaMode { get; set; } = "ASide";
        [YamlIgnore]
        public AreaModes AreaMode {
            get => (Enum.TryParse(areaMode, true, out AreaModes result) ? result : AreaModes.ASide);
            set {
                areaMode = value.ToString();
            }
        }
        public string playerAbilities { get; set; } =
            "Dashes: one\n" +
            "Difficulty: hard";
        [YamlIgnore]
        public RandoConfigReq PlayerAbilities {
            get {
                return YamlHelper.LoadYamlData<RandoConfigReq>(playerAbilities);
            }
        }

        public string FreeTextBoxPath { get; set; } = "";
    }
}
