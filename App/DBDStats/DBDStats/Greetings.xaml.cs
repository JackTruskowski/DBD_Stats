using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Threading;
using System.Text.RegularExpressions;

namespace DBDStats
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        String[] goodLines = { "Logging Player Score Event", "DBDPlayerScore_AddBloodpoints" };
        
        public MainWindow()
        {
           
            InitializeComponent();

            //handle visibility
            loadingicon.Visibility = Visibility.Hidden;
            scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;

            KillerStats kStats = loadKillerStats();
            dataList.Width = Constants.DataItemWidth;
            initUIElements(kStats.formatData());
            doStartupPrompts();

            //generate a random background
            Random newRand = new Random();
            background.Source = new BitmapImage(new Uri("Resources/backgrounds/banner_" + newRand.Next(9) + ".png", UriKind.Relative));
        }

        private void Window_SourceInitialized(object sender, EventArgs ea)
        {
            WindowAspectRatio.Register((Window)sender);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //run the batch script to pull new log files
            KillerStats temp = new KillerStats();
            ExecuteBatFileAndUpdateUI(temp);
            
        }

        public void doStartupPrompts()
        {
        }

        public void initUIElements(String[] stats)
        {
            dataList.Children.Clear();
            //create and populate a wrap panel for each stat
            for(int i = 0; i < stats.Length; i++)
            {
                WrapPanel statElement = new WrapPanel();
                statElement.Width = Constants.DataItemWidth;
                Image icon = new Image();
                switch (i)
                {
                    case 0:
                        icon.Source = new BitmapImage(new Uri(@"Resources/categoryIcon_weapons.png", UriKind.RelativeOrAbsolute)); break;
                    case 1:
                        icon.Source = new BitmapImage(new Uri(@"Resources/help_levelIcon_killer.png", UriKind.RelativeOrAbsolute)); break;
                    case 2:
                        icon.Source = new BitmapImage(new Uri(@"Resources/iconPerks_noOneEscapesDeath.png", UriKind.RelativeOrAbsolute)); break;
                    case 3:
                        icon.Source = new BitmapImage(new Uri(@"Resources/iconPerks_brutalStrength.png", UriKind.RelativeOrAbsolute)); break;
                    case 4:
                        icon.Source = new BitmapImage(new Uri(@"Resources/auricCellPack_04.png", UriKind.RelativeOrAbsolute)); break;
                    case 5:
                        icon.Source = new BitmapImage(new Uri(@"Resources/auricCellPack_03.png", UriKind.RelativeOrAbsolute)); break;
                    default:
                        icon.Source = new BitmapImage(new Uri(@"Resources/help_levelIcon_killer.png", UriKind.RelativeOrAbsolute)); break;
                }
                icon.Height = Constants.ScaleFactor * 65;
                icon.Width = Constants.ScaleFactor * 65;
                statElement.Children.Add(icon);

                TextBlock text = new TextBlock();
                text.Height = Constants.ScaleFactor * 65;
                text.Width = Constants.ScaleFactor * 100;
                text.FontFamily = new FontFamily(Constants.DefaultFontFamily);
                text.FontSize = 24;
                text.Text = stats[i];
                statElement.Children.Add(text);

                TextBlock statName = new TextBlock();
                statName.Height = Constants.ScaleFactor * 65;
                statName.Width = 200;
                statName.FontFamily = new FontFamily(Constants.DefaultFontFamily);
                statName.FontSize = Constants.DefaultFontSize;
                switch (i)
                {
                    case 0:
                        statName.Text = "Matches Played";  break;
                    case 1:
                        statName.Text = "Total Sacrifices"; break;
                    case 2:
                        statName.Text = "Kills/Match"; break;
                    case 3:
                        statName.Text = "Pallets Broken"; break;
                    case 4:
                        statName.Text = "Total BP Earned"; break;
                    case 5:
                        statName.Text = "Avg BP/Match"; break;
                    default:
                        statName.Text = "Unknown"; break;

                }
                statElement.Children.Add(statName); 
                dataList.Children.Add(statElement);
            }
        }

        /// <summary>
        /// Track when was the last log file has been read
        /// </summary>
        public DateTime newestFileRead = DateTime.MinValue;
        public void ExecuteBatFileAndUpdateUI(KillerStats statsObj)
        {
            /*Process proc = new Process();
            //oof
            proc.StartInfo.FileName = System.AppDomain.CurrentDomain.BaseDirectory + Constants.PathToRoot + "cpy.bat";
            proc.StartInfo.Arguments = Constants.PathToRoot + "settings.ini";
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.UseShellExecute = false;
            proc.EnableRaisingEvents = true;
            loadingicon.Visibility = Visibility.Visible;
            proc.Start();

            //listen for the process to finish running and alert the main thread to update the visibility
            proc.Exited += (sender, e) => { this.Dispatcher.Invoke(() => {

            }); };
            */

            //Why not reading log file directly?
            //Like this?

            //Get DBD log folder
            string log_path = System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            log_path += "\\AppData\\Local\\DeadByDaylight\\Saved\\Logs";

            DateTime latestLog = System.IO.File.GetCreationTime(log_path + "\\DeadByDaylight.log");
            if (newestFileRead < latestLog)
            {
                //Read the file
                loadingicon.Visibility = Visibility.Hidden;
                updateKillerStatsFromLogfiles(ref statsObj, log_path);
                initUIElements(statsObj.formatData());
                //write the new stats to file, overwriting whatever was there
                writeKillerDataToJSON(statsObj);
            }            
        }

        //Tries to load existing killer data from a JSON object and creates a new one if one doesn't already exist
        public KillerStats loadKillerStats()
        {
            //check if there is existing data
            if (System.IO.File.Exists(Constants.PathToRoot + "data\\killer.json"))
            {
                try
                {
                    string jsonInput = System.IO.File.ReadAllText(Constants.PathToRoot + "data\\killer.json");
                    KillerStats existingData = Newtonsoft.Json.JsonConvert.DeserializeObject<KillerStats>(jsonInput);
                    return existingData;
                }
                catch
                {
                    MessageBox.Show("Something went wrong loading existing data. Exiting.");
                    System.Environment.Exit(0);
                }

            }

            //else display a message and create new data
            MessageBox.Show("No existing data found.");
            KillerStats newStats = new KillerStats();
            writeKillerDataToJSON(newStats);
            return newStats;
        }

        public void writeKillerDataToJSON(KillerStats kStats)
        {
            try
            {
                //write the data to a json object
                string jsonObj = Newtonsoft.Json.JsonConvert.SerializeObject(kStats);
                System.IO.File.WriteAllText(Constants.PathToRoot + "data\\killer.json", jsonObj);
            }
            catch
            {
                MessageBox.Show("Error writing out new data. Exiting.");
                System.Environment.Exit(0);
            }
        }

        //Takes a KillerStats object and displays the data on the window
        /*
        public void displayKillerStats(KillerStats killStat)
        {
            dataBox.Text = killStat.formatData();
        
        }
        */

        public void updateKillerStatsFromLogfiles(ref KillerStats statsObj, string log_file)
        {
            //get the filenames for the log files
            //string path = Constants.PathToRoot + "data\\";
            string searchPattern = "*.log";
            string[] MyFiles = System.IO.Directory.GetFiles(log_file, searchPattern);

            foreach(string file in MyFiles)
            {
                string prevLine = "";
                string[] lines = System.IO.File.ReadAllLines(file);
                foreach (string line in lines)
                {
                    if (goodLines.Any(line.Contains))
                    {
                        processLogLine(line, prevLine, ref statsObj);
                    }
                    prevLine = line;
                }
            }
        }

        public void processLogLine(string line, string prevLine, ref KillerStats statsObj)
        {
            Boolean isBPtotal = false;
            if (line.IndexOf("Logging Player Score Event") != -1)
            {
                line = line.Substring(line.IndexOf("Logging Player Score Event") + 1);
                if (line.Contains("DBDPlayerScore_EnterParadise")) statsObj.matchesPlayed++;
                else if (line.Contains("DBDSlasherScore_Destroy")) statsObj.breakActionCount++;
                else if (line.Contains("DBDSlasherScore_SacrificeSuccess")) statsObj.sacrificeCount++;
                else if (line.Contains("DBDSlasherScore_CamperHookedFirstTime")) statsObj.firstHookCount++;
                else if (line.Contains("DBDSlasherScore_Hook")) statsObj.hookCount++;


            } else if (line.IndexOf("DBDPlayerScore_AddBloodpoints") != -1 && !line.Equals(prevLine))
            {
                line = line.Substring(line.IndexOf("DBDPlayerScore_AddBloodpoints") + 1);
                isBPtotal = true;
            } else
            {
                return;
            }
            
            try
            {
                int bpValue = (int) float.Parse(Regex.Match(line, @"[+-]?([0-9]*[.])?[0-9]+").Value);
                if (isBPtotal)
                {
                    statsObj.bloodpointsEarned += bpValue;
                }
            }
            catch
            {
                return;
            }
        }
       
        
    }
}
