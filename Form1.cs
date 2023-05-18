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

namespace Test2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    string driveName = drive.Name;
                    long driveSize = drive.TotalSize;
                    string sizeText = GetFileSizeText(driveSize);

                    Drivelist.Items.Add($"{driveName} ({sizeText})");
                }

            }
        }
        
        private void Drivelist_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedDrive = Drivelist.SelectedItem.ToString();
            DriveInfo drive = new DriveInfo(selectedDrive);

            if (drive.IsReady)
            {
                Folderlist.Items.Clear();
                Subfolderlist.Items.Clear();
                Filesize.Items.Clear();

                DirectoryInfo rootDirectory = drive.RootDirectory;

                try
                {
                    DirectoryInfo[] folders = rootDirectory.GetDirectories();

                    foreach (DirectoryInfo folder in folders)
                    {
                        try
                        {
                            Folderlist.Items.Add(folder.Name);

                            long folderSize = GetDirectorySize(folder);
                            string sizeText = GetFileSizeText(folderSize);
                            Filesize.Items.Add($"{folder.Name} ({sizeText})");
                        }
                        catch (UnauthorizedAccessException)
                        {
                            continue;
                        }
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show($"Access to the path '{rootDirectory.FullName}' is denied.");
                }
            }

        }

        private void Folderlist_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedFolder = Folderlist.SelectedItem.ToString();
            DriveInfo drive = new DriveInfo(Drivelist.SelectedItem.ToString());

            if (drive.IsReady)
            {
                Subfolderlist.Items.Clear();
                Filesize.Items.Clear();

                string selectedPath = Path.Combine(drive.RootDirectory.FullName,selectedFolder);
                DirectoryInfo folder = new DirectoryInfo(selectedPath);
                DirectoryInfo[] subfolders = folder.GetDirectories();

                

                foreach (DirectoryInfo subfolder in subfolders)
                {
                    Subfolderlist.Items.Add(subfolder.Name);
                }

                foreach (DirectoryInfo subfolder in subfolders)
                {
                    Subfolderlist.Items.Add(subfolder.Name);

                    long subfolderSize = GetDirectorySize(subfolder);
                    string subfolderSizeText = GetFileSizeText(subfolderSize);
                    Filesize.Items.Add($"- {subfolder.Name} ({subfolderSizeText})");
                }
                

                long folderSize = GetDirectorySize(folder);
                string sizeText = GetFileSizeText(folderSize);

                
                Filesize.Items.Add($"Selected folder size: {sizeText}");

                FileInfo[] files = folder.GetFiles();
                foreach (FileInfo file in files)
                {
                    long fileSize = file.Length;
                    string fileSizeText = GetFileSizeText(fileSize);
                    Filesize.Items.Add($"{file.Name} ({fileSizeText})");
                }
                

            }
        }

        private void Subfolderlist_SelectedIndexChanged(object sender, EventArgs e)
        {

            string selectedSubfolder = Subfolderlist.SelectedItem.ToString();
            DriveInfo drive = new DriveInfo(Drivelist.SelectedItem.ToString());

            if (drive.IsReady)
            {
                Filesize.Items.Clear();

                string selectedFolderPath = Path.Combine(drive.RootDirectory.FullName, Folderlist.SelectedItem.ToString());
                string selectedSubfolderPath = Path.Combine(selectedFolderPath, selectedSubfolder);

                DirectoryInfo subfolder = new DirectoryInfo(selectedSubfolderPath);

                long subfolderSize = GetDirectorySize(subfolder);
                string sizeText = GetFileSizeText(subfolderSize);

                Filesize.Items.Add($"Selected subfolder size: {sizeText}");

                DirectoryInfo[] subfolders = subfolder.GetDirectories();
                foreach (DirectoryInfo folder in subfolders)
                {
                    long folderSize = GetDirectorySize(folder);
                    string folderSizeText = GetFileSizeText(folderSize);
                    Filesize.Items.Add($"{folder.Name} ({folderSizeText})");
                }

                FileInfo[] files = subfolder.GetFiles();
                foreach (FileInfo file in files)
                {
                    long fileSize = file.Length;
                    string fileSizeText = GetFileSizeText(fileSize);
                    Filesize.Items.Add($"{file.Name} ({fileSizeText})");
                }
            }
        }

        private void Filesize_SelectedIndexChanged(object sender, EventArgs e)
        {
            //If Needed add something
        }
        private long GetDirectorySize(DirectoryInfo directory)
        {
            long size = 0;

            FileInfo[] files = directory.GetFiles();
            foreach (FileInfo file in files)
            {
                size += file.Length;
            }

            DirectoryInfo[] subDirectories = directory.GetDirectories();
            foreach (DirectoryInfo subDirectory in subDirectories)
            {
                size += GetDirectorySize(subDirectory);
            }

            return size;
        }

        private string GetFileSizeText(long fileSizeInBytes)
        {
            string[] sizeSuffixes = { "B", "KB", "MB", "GB", "TB" };
            int i = 0;
            double size = fileSizeInBytes;

            while (size >= 1024 && i < sizeSuffixes.Length - 1)
            {
                size /= 1024;
                i++;
            }

            return $"{size:0.##} {sizeSuffixes[i]}";
        }
    }
}
