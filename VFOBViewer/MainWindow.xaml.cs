using System;
using System.Collections.Generic;
using System.IO;
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
using WYQDoNet.DoNet.IO.DirectoryTools;

namespace VFOBViewer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public string FOBPath = "";
        public bool ShowDialog = false;
        public FileObj fob;

        private string currentPath;

        public string CurrentPath
        {
            get
            {
                return currentPath;
            }

            set
            {
                currentPath = value;
                TextBox_CurrentPath.Text = value;
                AddFileModels(fob.GetDirFileModels(value));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            if (Environment.GetCommandLineArgs().Length > 1)
                FOBPath = Environment.GetCommandLineArgs()[1].Trim();
        }

        private bool ViewBox = false;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FileDirDialog_FOB.TextBoxPath.IsReadOnly = true;
            FileDirDialog_FOB.TextBoxPath.TextChanged += TextBoxPath_TextChanged;
            FileDirDialog_NeedFOB.TextBoxPath.TextChanged -= FileDirDialog_NeedFOB.TextBoxPath_TextChanged;
            FileDirDialog_NeedFOB.ButtonBrowser.Click -= FileDirDialog_NeedFOB.ButtonBrowser_Click;
            FileDirDialog_NeedFOB.ButtonBrowser.Click += NFOB_ButtonBrowser_Click;
            if (!string.IsNullOrWhiteSpace(FOBPath))
            {
                ShowDialog = false;
                FileDirDialog_FOB.TextBoxPath.Text = FOBPath;
                fob = WYQDoNet.DoNet.IO.ObjectUtils.ObjectSerializationHelper.BinaryDeserialize<FileObj>(FOBPath);
                CurrentPath = fob.Root;
            }
            if (ShowDialog)
                FileDirDialog_FOB.ButtonBrowser.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, FileDirDialog_FOB.ButtonBrowser));
        }

        private void NFOB_ButtonBrowser_Click(object sender, RoutedEventArgs e)
        {
            string dir = FileDirDialog_NeedFOB.ShowFolderBrowserDialog("请选择需要序列化文件夹...");
            if (string.IsNullOrWhiteSpace(dir)) return;
            string tbPath = FileDirDialog_NeedFOB.TextBoxPath.Text;

            if (string.IsNullOrWhiteSpace(tbPath)) FileDirDialog_NeedFOB.TextBoxPath.Text = "";

            FileDirDialog_NeedFOB.TextBoxPath.Text += dir + ";";
        }



        private void TextBoxPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            fob = WYQDoNet.DoNet.IO.ObjectUtils.ObjectSerializationHelper.BinaryDeserialize<FileObj>(tb.Text);

            CurrentPath = fob.Root;
        }

        public void AddFileModels(FileModel[] fms)
        {
            WrapPanel_FileInfos.Children.Clear();

            if (ViewBox)
                foreach (var fm in fms)
                {
                    FileInfoBox fib = new FileInfoBox(fm, fob);

                    WrapPanel_FileInfos.Children.Add(fib);

                    fib.MouseDoubleClick += Fib_MouseDoubleClick;
                }
            else
                foreach (var fm in fms)
                {
                    FileInfoBar fib = new FileInfoBar(fm, fob);

                    WrapPanel_FileInfos.Children.Add(fib);

                    fib.MouseDoubleClick += Fib_MouseDoubleClick;
                }
        }

        public void AppendFileModels(FileModel[] fms)
        {
            foreach (var fm in fms)
            {
                if (ViewBox)
                {
                    FileInfoBox fib = new FileInfoBox(fm, fob);

                    WrapPanel_FileInfos.Children.Add(fib);

                    fib.MouseDoubleClick += Fib_MouseDoubleClick;
                }
                else
                {
                    FileInfoBar fib = new FileInfoBar(fm, fob);

                    WrapPanel_FileInfos.Children.Add(fib);

                    fib.MouseDoubleClick += Fib_MouseDoubleClick;
                }
            }
        }
        private void Fib_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ViewBox)
            {
                FileInfoBox fib = (FileInfoBox)sender;

                if (fib.FileInfo.IsFile)
                {

                }
                else
                {
                    CurrentPath = fib.FileInfo.FullPath;
                }
            }
            else
            {
                FileInfoBar fib = (FileInfoBar)sender;

                if (fib.FileInfo.IsFile)
                {

                }
                else
                {
                    CurrentPath = fib.FileInfo.FullPath;
                }
            }

        }

        private void Button_Parent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentPath = fob.GetDirectory(CurrentPath).ParentDirectory;
            }
            catch
            {
                CurrentPath = fob.Root;
            }
        }

        private void Button_Root_Click(object sender, RoutedEventArgs e)
        {
            CurrentPath = fob.Root;
        }

        private void Button_ViewChange_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CurrentPath)) return;
            if (ViewBox) ViewBox = false;
            else ViewBox = true;
            CurrentPath = CurrentPath;
        }

        private void Button_LocatePath_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TextBox_CurrentPath.Text))
                CurrentPath = TextBox_CurrentPath.Text;
        }

        private void Button_StartFOB_Click(object sender, RoutedEventArgs e)
        {
            string path = FileDirDialog_NeedFOB.TextBoxPath.Text;

            if (string.IsNullOrWhiteSpace(path)) return;
            string[] dirs = path.Trim().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            string savePath = FileDirDialog_NeedFOB.SaveFileDialog("选择保存文件位置...", "VFOB目录|*.vfob");
            if (string.IsNullOrWhiteSpace(savePath)) return;
            
            WYQDoNet.DoNet.IO.DirectoryTools.DirectoryUtils.SaveDirFilesBinFile(@"Root:\",dirs, savePath);

            MessageBox.Show("序列化完成!!!");
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FileDirDialog_FOB.TextBoxPath.Text)) return;
            if (TextBox_Search.Text.TrimStart().StartsWith("R:"))
                AddFileModels(fob.SearchDirFiles(TextBox_Search.Text.TrimStart().Remove(0, 2), true));
            else
                AddFileModels(fob.SearchDirFiles(TextBox_Search.Text, false));
        }
    }
}