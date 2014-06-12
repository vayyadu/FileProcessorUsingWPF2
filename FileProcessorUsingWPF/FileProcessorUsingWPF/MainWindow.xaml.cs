using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
// added this namespace to make use of class FolderBrowserDialog for browse funtionality
using System.Windows.Forms;
// added this namespace to make use of ThreadStart and Thread classes to spin a thread to do the task
using System.Threading;

namespace FileProcessorUsingWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProgressBarWindow pbw = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Process_Click(object sender, RoutedEventArgs e)
        {
            OnWorkerMethodStart();
        }

        private void OnWorkerMethodStart()
        {
            try
            {
                // check if the user has selected all the 3 directories. If not display the error message and do nothing
                if (textBox1.Text.Trim() == string.Empty || textBox2.Text.Trim() == string.Empty || textBox3.Text.Trim() == string.Empty)
                {
                    textBlock1.Text = "Please choose valid directories.";
                    return;
                }


                
                // disable the "Process" button as soon as the processing starts 
                Process.IsEnabled = false;
                textBlock1.Text = "Processing.. Please wait..";

                //Create a string array to pass the user input directories to the worker method of the FileProcessor
                string[] filePath = new string[3];
                filePath[0] = textBox1.Text;
                filePath[1] = textBox2.Text;
                filePath[2] = textBox3.Text;

                FileProcessor fileProcessor = new FileProcessor();

                // wire the event to the corresponding handler
                // OnWorkerComplete event will be raise after the 3 tasks are completed in 3 different tasks
                // on raising this event, OnWorkerMethodComplete on the main window will be called to close the progressbar window and update the results to the user.
                fileProcessor.OnWorkerComplete += new FileProcessor.OnWorkerMethodCompleteDelegate(OnWorkerMethodComplete);

                // a separate thread is created and started to execute the WorkerMethod
                ThreadStart tStart = new ThreadStart(() => fileProcessor.WorkerMethod(filePath, true));
                Thread t = new Thread(tStart);
                t.Start();

                // display the progress bar to show the user that the program is still processing the task 
                pbw = new ProgressBarWindow();
                pbw.Owner = this;
                pbw.ShowDialog();

                // 
                Process.IsEnabled = true;
            }
            catch (Exception ex)
            {
                textBlock1.Text = "Error has occurred. Please try again. Additional error information: " + ex.Message;
            }
        }

        private void OnWorkerMethodComplete(string result)
        {
            // close the progressbar window after the task is completed
            // use dispatcher invoke to close the UI element because the UI elements owned by UI(Main thread) cannot be access by the other thread
            pbw.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                                    new Action(
                                                delegate()
                                                {
                                                    pbw.Close();
                                                }
                                    ));

            // use dispatcher invoke to update the results to the user
            textBlock1.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                                       new Action(
                                                   delegate()
                                                   {
                                                       textBlock1.Text = result;
                                                   }
                                       ));

            
        }


        private void BrowseButton1_Click(object sender, RoutedEventArgs e)
        {
            //use FolderBrowseDialog class to set the user input directories to the textbox
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.SelectedPath;

                textBox1.Text = path;
            }
        }

        private void BrowseButton2_Click(object sender, RoutedEventArgs e)
        {
            //use FolderBrowseDialog class to set the user input directories to the textbox
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.SelectedPath;

                textBox2.Text = path;
            }
        }

        private void BrowseButton3_Click(object sender, RoutedEventArgs e)
        {
            //use FolderBrowseDialog class to set the user input directories to the textbox
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.SelectedPath;

                textBox3.Text = path;
            }
        }
    }
}
