using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisGA {
    [Serializable]
    struct SaveData {
        public TimeSpan nextInterval;
        public TetrisAIManager tetrisAIManager;
        public int placeCount;
        public int[] previousBestGene;

        public SaveData(TetrisGAWindow window) {
            nextInterval = window.NextInterval;
            tetrisAIManager = window.TAManager;
            placeCount = window.PlaceCount;
            previousBestGene = window.PreviousBestGene;
        }
    }
}
