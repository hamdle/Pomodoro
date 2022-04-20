using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using System.IO;
using System.Diagnostics;


namespace Pomo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Boolean on = false;
        private int count = 0;
        private int savedCount = 0;
        private readonly DispatcherTimer timer;
        private readonly DispatcherTimer messageTimer;
        private int messageFadeInSeconds = 6;
        private int elapsedCount = 0;

        public MainWindow()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;

            messageTimer = new DispatcherTimer();
            messageTimer.Interval = TimeSpan.FromSeconds(messageFadeInSeconds);
            messageTimer.Tick += messageTimer_Tick;

            btnSave.IsEnabled = false;
            btnSave.Visibility = Visibility.Hidden;
        }

        private void timer_Tick(object? sender, EventArgs e)
        {
            count++;
            TimeSpan timeSpan = TimeSpan.FromSeconds(count);
            lblTimer.Content = timeSpan.ToString();

            if (btnSave.IsEnabled == false)
            {
                btnSave.IsEnabled = true;
                btnSave.Visibility = Visibility.Visible;
            }
        }
        private void messageTimer_Tick(object? sender, EventArgs e)
        {
            ShowMessage("");
            messageTimer.Stop();
        }

        private void btnTimer_Click(object sender, RoutedEventArgs e)
        {
            if (btnSave.IsEnabled == false)
            {
                btnSave.IsEnabled = true;
                btnSave.Visibility = Visibility.Visible;
            }

            if (on)
            {
                // Turn off
                timer.Stop();
                on = false;
                btnTimer.Content = "Start";
            }
            else
            {
                // Turn on
                timer.Start();
                on = true;
                btnTimer.Content = "Pause";
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string filename = @"C:\Users\ermarty\timesheet.txt";
            List<string> entries = ReadTimesheet(filename);

            if (WriteTimesheet(filename, entries))
            {
                btnSave.IsEnabled = false;
                btnSave.Visibility = Visibility.Hidden;
                ShowMessage("+" + TimeSpan.FromSeconds(elapsedCount));
            }
            else
            {
                ShowMessage("Save failed!");
            }
        }

        private List<string> ReadTimesheet(string filename)
        {
            List<string> entries = new List<string>();

            if (!File.Exists(filename))
            {
                throw new FileNotFoundException(filename);
            }

            try
            {
                using (StreamReader reader = File.OpenText(filename))
                {
                    String entry;

                    while ((entry = reader.ReadLine()) != null)
                    {
                        entries.Add(entry);
                    }

                    if (entries.Count > 0)
                    {
                        DateOnly currentDate = DateOnly.Parse(DateTime.Today.ToString("MM-dd-yyyy"));
                        string[] line = ((string)entries[entries.Count - 1]).Split(", ");

                        if (line.Length == 2)
                        {
                            DateOnly entryDate = DateOnly.Parse(line[0]);

                            if (currentDate.Equals(entryDate))
                            {
                                // Modify todays entry.
                                elapsedCount = count - savedCount;
                                savedCount = count;
                                TimeSpan timeSpan = TimeSpan.FromSeconds(elapsedCount);
                                TimeSpan existingSpan = TimeSpan.Parse(line[1]);
                                string newEntry = currentDate + ", " + (timeSpan + existingSpan);
                                entries[entries.Count - 1] = newEntry;
                            }
                            else
                            {
                                // This is the first save for today so add a new line to the timesheet.
                                TimeSpan timeSpan = TimeSpan.FromSeconds(count);
                                savedCount = count;
                                string newEntry = currentDate + ", " + timeSpan.ToString();
                                entries.Add(newEntry);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An exception occurred while reading the timesheet.");
                Debug.WriteLine(ex.Message);
            }

            return entries;
        }

        private bool WriteTimesheet(string filename, List<String> entries)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException(filename);
            }

            try
            {
                File.WriteAllLines(filename, entries);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An exception occurred while writing the timesheet.");
                Debug.WriteLine(ex.Message);
            }
            
            return true;
        }

        private void ShowMessage(string message)
        {
            lblMessage.Content = message;

            messageTimer.Start();
        }
    }
}
