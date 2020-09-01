using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Media;
using System.Windows.Threading;

namespace Timer
{
    public partial class MainWindow : Window
    {
        DispatcherTimer Timer = new DispatcherTimer();
        string[][] Config;
        SoundPlayer AlarmSound;
        bool AlarmNotPlayed = true;

        public MainWindow()
        {
            InitializeComponent();    

            //Timer init
            Timer.Tick += new EventHandler(Timer_tick);
            Timer.Interval = new TimeSpan(0, 0, 0,0,100);            

            //Loads config file
            Config = LoadConfig();

            //Alarm sound init
            AlarmSound = new SoundPlayer("beep.wav");
            try
            {
                AlarmSound.Load();
            }
            catch
            {
                MessageBox.Show("File beep.wav was not found. Make sure its in same directory as this application.");
                this.Close();
                Application.Current.Shutdown();
            }

            //App start
            Update();
            Timer.Start();
            
        }

        /// <summary>
        /// Event called every timer tick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_tick(object sender,EventArgs e)
        {
            Update();
        }

        /// <summary>
        /// Should be called every timer tick. Updates UI and plays alarm sound
        /// </summary>
        void Update()
        {
            //current position in config file
            string[] current_position = FindCurrentPositionInConfig(Config);

            //current time
            TimeSpan now = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            nowTXT.Content = now.ToString(); //current time in ui


            if (current_position == null) //end of config file
            {
                Background = Brushes.White;
                leftTXT.Content = "END";
                mesTXT.Content = "";
                timeTXT.Content = "";
           
            }
            else
            {
                //ui update
                mesTXT.Content = current_position[1];
                timeTXT.Content = current_position[0];
                    
                //calculates remaining time
                string[] timea = current_position[0].Split(':');      
                TimeSpan target = new TimeSpan(int.Parse(timea[0]), int.Parse(timea[1]), 0);
                TimeSpan remaining = target.Subtract(now);
                leftTXT.Content = remaining.ToString();

                if (remaining.TotalSeconds <= 1) //end of one step
                {
                    if (AlarmNotPlayed) AlarmSound.Play();
                    AlarmNotPlayed = false;
                    leftTXT.Content = "0:00";
                    Background = Brushes.Red;
                }
                else
                {
                    //so next update where remaining.TotalSeconds <= 1 will play alarm
                    AlarmNotPlayed = true;

                    //progress bar update
                    progres.Maximum = target.TotalSeconds;
                    progres.Minimum = inSeconds(FindCurrentPositionInConfig(Config, true)[0]);
                    progres.Value = progres.Maximum - (remaining.TotalSeconds) ;
                    if (remaining.TotalMinutes <= 3) progres.Background = Brushes.Red;
                    else progres.Background = Brushes.Green;

                    Background = Brushes.White;
                }
            }
        }

        /// <summary>
        /// Checks if time is in hh:mm format. 
        /// </summary>
        /// <param name="input">Input time in hh:mm format</param>
        /// <returns>true if its correct, false if not</returns>
        bool CheckTimeFormat(string input)
        {
            string[] time = input.Split(':');
            if (time.Length != 2) return false;

            int hours = 0;
            if (!int.TryParse(time[0], out hours)) return false;
            if (hours > 24) return false;

            int mins = 0;
            if (!int.TryParse(time[1], out mins)) return false;
            if (mins > 60 || mins < 0) return false;

            return true;
        }

        /// <summary>
        /// Compares two strings in format hh:mm. Will return 1 if a is later then b, -1 if not and 0 if they are equal.
        /// </summary>
        /// <param name="a">First string in hh:mm format</param>
        /// <param name="b">Second string in hh:mm format</param>
        /// <returns></returns>
        int IsLaterThen (string a, string b)
        {          
            string[] timea = a.Split(':');
            int aHour = int.Parse(timea[0]);
            int aMinute = int.Parse(timea[1]);

            string[] timeb = b.Split(':');
            int bHour = int.Parse(timeb[0]);
            int bMinute = int.Parse(timeb[1]);

            if (aHour > bHour) return 1;
            if (aHour < bHour) return -1;
            else if (aMinute > bMinute) return 1;
            else if (aMinute == bMinute) return 0;
            else return -1;   
           
        }

        /// <summary>
        /// Loads config file 
        /// </summary>
        /// <param name="filename">name of the file, that is in the same directory</param>
        /// <returns>Loaded config file</returns>
        string[][] LoadConfig (string filename = "config.txt")
        {
            try
            {
                string[] lines = System.IO.File.ReadAllLines(filename, System.Text.Encoding.UTF8);
                string[][] result = new string[lines.Length / 2][];

                int counter = 0;
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = new string[2];
                    //time
                    result[i][0] = lines[counter];
                    counter++;
                    if (!CheckTimeFormat(result[i][0]))
                        throw new Exception();

                    //text
                    result[i][1] = lines[counter];                  
                    counter++;
                }

                return result;
            }
            catch
            {
                MessageBox.Show("Error - loading config file failed. Check if it is in same directory as this application and if " +
                    "it has correct format: \n\n" +
                    "Time in hh:mm format\n" +
                    "Text you want to display");
                this.Close();
                Application.Current.Shutdown();
            }
            return null;
        }

        /// <summary>
        /// Finds current step in config based on current time. 
        /// </summary>
        /// <param name="config">Config</param>
        /// <param name="prev">If true, will return previous step instead</param>
        /// <returns></returns>
        string[] FindCurrentPositionInConfig(string[][] config, bool prev = false)
        {
            string now = DateTime.Now.Hour + ":" + DateTime.Now.Minute;
            for (int i = 0; i < config.Length; i++)
            {
                int r = IsLaterThen(config[i][0], now);
               
                if (r == 1) return prev ? (i ==0?new string[2] { "0:00", "start" }: config[i - 1]) : config[i];                
            }
            return null;
        }

        /// <summary>
        /// Converts time in hh:mm format to seconds
        /// </summary>
        /// <param name="a">input string</param>
        /// <returns>Number of seconds</returns>
        int inSeconds(string a)
        {
            string[] timea = a.Split(':');
            int aHour = int.Parse(timea[0]);
            int aMinute = int.Parse(timea[1]);

            return (aMinute + (60 * aHour )) * 60;
        }
        /// <summary>
        /// Converts time in hh:mm format to minutes
        /// </summary>
        /// <param name="a">input string</param>
        int inMinutes(string a)
        {
            string[] timea = a.Split(':');
            int aHour = int.Parse(timea[0]);
            int aMinute = int.Parse(timea[1]);

            return (aMinute + (60 * aHour));
        }

        /// <summary>
        /// Event handling window resize
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double w = e.NewSize.Width;
            double h = e.NewSize.Height;

            leftTXT.FontSize = w / 6;
          

            progres.Margin = new Thickness(0, leftTXT.FontSize * 1.5,0,0);
            progres.Width = w / 2;
            progres.Height = h / 15;


        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
        }

        //Sound test
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.T)
            {
                AlarmSound.Play();
            }
        }
    }
}

