using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// addded this namespace to make use of Task class to achieve multithreading needs of this task
using System.Threading.Tasks;
//added this namespace to make use of DirectoryInfo class to access and collect information about the files and folders in the directories
using System.IO;
// added this namespace to format the results
using System.Globalization;

namespace FileProcessorUsingWPF
{
    class FileProcessor
    {
        // delegate that is used as a pointer for the method to update the result and close the progressbar window
        public delegate void OnWorkerMethodCompleteDelegate(string result);
        // event that is used to indicate the tasks running on the threads are complete so that the results could be updated on the main window
        public event OnWorkerMethodCompleteDelegate OnWorkerComplete;


        public void WorkerMethod(string[] filePath, bool includeSubDir)
        {
            try
            {
                long result;

                
                // create 3 different threads to calculate the directory size (one thread for each one the 3 user input directories) 
                var task1 = Task.Factory.StartNew(() => result = GetAggregateDirectorySize(filePath[0], true));
                var task2 = Task.Factory.StartNew(() => result = GetAggregateDirectorySize(filePath[1], true));
                var task3 = Task.Factory.StartNew(() => result = GetAggregateDirectorySize(filePath[2], true));

                // this will ensure the threads wait until all three tasks are completed
                Task.WaitAll();

                // add the results from all the 3 threads to get the aggregate of file sizes from all 3 user input directories
                result = task1.Result + task2.Result + task3.Result;

                // raise the event to indicate that the tasks on all 3 threads are complete so that the UI will be updated accordingly
                OnWorkerComplete("The aggregate of all the file sizes is "
                                    + String.Format(CultureInfo.InvariantCulture, "{0:#,#}", result) + " bytes ("
                                    + String.Format(CultureInfo.InvariantCulture, "{0:#,#}", (result / 1024)) + " KB / "
                                    + String.Format(CultureInfo.InvariantCulture, "{0:#,#}", (result / (1024 * 1024))) + " MB" + ")");
            }
            catch (Exception ex)
            {
                OnWorkerComplete("Error has occurred. Please try again. Additional error information: " + ex.Message);
            }
        }



        // method will calculate the directory size and it calls the same method recursively to include all the subdirectories
        public long GetAggregateDirectorySize(string filePath, bool includeSubDir)
        {
            try
            {
                DirectoryInfo dInfo = new DirectoryInfo(filePath);

                // Enumerate all the files
                long aggregateFileSize = dInfo.EnumerateFiles().Sum(file => file.Length);

                // If Subdirectories are to be included
                if (includeSubDir)
                {
                    // Enumerate all sub-directories and call the same method recursively
                    aggregateFileSize += dInfo.EnumerateDirectories().Sum(dir => GetAggregateDirectorySize(dir.FullName, true));
                }
                return aggregateFileSize;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

      
    }
}

