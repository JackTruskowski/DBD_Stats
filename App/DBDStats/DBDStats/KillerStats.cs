using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDStats
{
    public class KillerStats : Stats
    {
        public int sacrificeCount;
        public int breakActionCount;
        public int hookCount;
        public int firstHookCount;
        public int bloodpointsEarned;

        public KillerStats()
        {
            this.sacrificeCount = 0;
            this.breakActionCount = 0;
            this.hookCount = 0;
            this.firstHookCount = 0;
            this.bloodpointsEarned = 0;
        }

        public string [] formatData()
        {
            var bp = "";
            var bpavg = "";

            int denom = this.matchesPlayed == 0 ? 1 : this.matchesPlayed;

            if (this.bloodpointsEarned >= 10000)
                bp = "" + this.bloodpointsEarned / 1000 + "k";
            else
                bp = "" + this.bloodpointsEarned;
            if (Math.Round(this.bloodpointsEarned / (1.0 * denom), 2) >= 10000)
                bpavg = "" + Math.Round(this.bloodpointsEarned / denom / 1000.0, 1) + "k";
            else
                bpavg += "" + this.bloodpointsEarned / denom;

            return new string[] {"" + this.matchesPlayed, "" + this.sacrificeCount, "" + Math.Round(this.sacrificeCount / (1.0 * denom), 2), "" + this.breakActionCount, bp, bpavg  };
        }

        public string formatDataForDebug()
        {
            string outData = "";
            outData += "Games Played:\t\t" + this.matchesPlayed + "\n";
            outData += "Sacrifices:\t\t" + this.sacrificeCount + "\n";

            int denom = this.matchesPlayed == 0 ? 1 : this.matchesPlayed;
            outData += "Sacr/Game:\t\t" + Math.Round(this.sacrificeCount / (1.0 * denom), 2) + "\n";

            outData += "Pallets Broken:\t\t" + this.breakActionCount + "\n";
            if (bloodpointsEarned >= 100000)
                outData += "Bloodpoints Earned:\t" + this.bloodpointsEarned / 1000 + "k\n";
            else
                outData += "Bloodpoints Earned:\t" + this.bloodpointsEarned + "\n";

            if (Math.Round(this.bloodpointsEarned / (1.0 * denom), 2) >= 100000)
                outData += "BP/Game:\t\t" + this.bloodpointsEarned / denom / 1000 + "k\n";
            else
                outData += "BP/Game:\t\t" + this.bloodpointsEarned / denom + "\n";
            return outData;
        }

    }
}
