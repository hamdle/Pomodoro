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
using System.Windows.Threading;
using System.IO;
using System.Diagnostics;
using System.Collections;


namespace Pomo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Boolean on = false;
        private int count = 0;
        private readonly DispatcherTimer timer;
        private Boolean flicker = false;

        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
        }

        private void timer_Tick(object? sender, EventArgs e)
        {
            count++;
            TimeSpan timeSpan = TimeSpan.FromSeconds(count);
            lblTimer.Content = timeSpan.ToString();
        }

        private void btnTimer_Click(object sender, RoutedEventArgs e)
        {
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
            string filename = @"C:\Users\EricMarty\timesheet.txt";
            if (File.Exists(filename))
            {
                ArrayList entries = ReadTimesheet(filename);
                if (WriteTimesheet(filename, entries))
                {
                    lblSave.Content = "Saved " + DateTime.Now.ToString("hh:mm:ss tt");
                }
                else
                {
                    lblSave.Content = "Save failed";
                }
            }
        }

        private ArrayList ReadTimesheet(string filename)
        {
            ArrayList entries = new ArrayList();

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
                                // Modify this entry
                                Debug.WriteLine("Modify last line");
                            }
                            else
                            {
                                // Add a new line
                                Debug.WriteLine("Add new line");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An exception occurred while reading/writing the file.");
                Debug.WriteLine(ex.Message);
            }

            return entries;
        }

        private bool WriteTimesheet(string filename, ArrayList entries)
        {
            return true;
        }
    }
}
