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
using WYQDoNet.DoNet.BaseUtils.StringUtils;
using WYQDoNet.DoNet.IOUtils.DirectoryUtils;

namespace VFOBViewer
{
    /// <summary>
    /// FileInfoBox.xaml 的交互逻辑
    /// </summary>
    public partial class FileInfoBox : UserControl
    {
        Dictionary<string, string> ExtImg = new Dictionary<string, string>();

        public static FileInfoBox fib;
        public FileInfoBox()
        {
            InitializeComponent();

        }
        public FileModel FileInfo;
        public FileObj fileObj;
        public FileInfoBox(FileModel fm, FileObj fileObj)
        {
            AddExtImg();
            InitializeComponent();
            FileInfo = fm;
            this.fileObj = fileObj;
            TextBox_Name.Text = FileInfo.Name;
            if (fm.IsFile)
                Label_Length.Content = StringHelper.ConvertBytes(FileInfo.Length, 2);
            else
            {
                Label_Length.Content = fileObj.GetDirFileModels(fm.FullPath).Length + "文件";
            }
            SetImage();
            this.ToolTip = "创建日期：" + FileInfo.CreationTime + Environment.NewLine + 
                "访问日期：" + FileInfo.LastAccessTime + Environment.NewLine + 
                "修改日期：" + FileInfo.LastWriteTime+Environment.NewLine+
                "路径：" + FileInfo.ParentDirectory; 
        }

        public void SetImage()
        {
            if (FileInfo.IsFile)
            {
                BitmapImage bi = new BitmapImage(new Uri("pack://application:,,,/Images/File/format_unkown.png"));
                string ext = FileInfo.Extension;
                if (!string.IsNullOrWhiteSpace(ext))
                {
                    foreach (var key in ExtImg.Keys)
                    {
                        if (ext.Trim().ToLower().Contains(key))
                        {
                            bi = new BitmapImage(new Uri("pack://application:,,,/Images/File/" + ExtImg[key]));
                        }
                    }
                }

                Image_Format.Source = bi;
            }
        }

        public void AddExtImg()
        {
            ExtImg.Add("txt", "format_text.png");
            ExtImg.Add("doc", "format_word.png");
            ExtImg.Add("xls", "format_excel.png");
            ExtImg.Add("pdf", "format_pdf.png");
            ExtImg.Add("zip", "format_zip.png");
            ExtImg.Add("rar", "format_zip.png");
            ExtImg.Add("ppt", "format_ppt.png");
            ExtImg.Add("chm", "format_chm.png");
            ExtImg.Add("unkown", "format_unkown.png");

            ExtImg.Add("mp4", "format_media.png");
            ExtImg.Add("avi", "format_media.png");
            ExtImg.Add("flv", "format_media.png");
            ExtImg.Add("rm", "format_media.png");
            ExtImg.Add("rmvb", "format_media.png");
            ExtImg.Add("wmv", "format_media.png");
            ExtImg.Add("mkv", "format_media.png");


            ExtImg.Add("mp3", "format_music.png");
            ExtImg.Add("wav", "format_music.png");
            ExtImg.Add("ogg", "format_music.png");


            ExtImg.Add("png", "format_picture.png");
            ExtImg.Add("bmp", "format_picture.png");
            ExtImg.Add("jpg", "format_picture.png");
            ExtImg.Add("gif", "format_picture.png");
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (fib != null) fib.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            this.Background = new SolidColorBrush(Color.FromArgb(100, 0, 255, 0));
            fib = this;
        }
    }
}