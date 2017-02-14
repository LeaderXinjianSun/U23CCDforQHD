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
using MahApps.Metro.Controls;
using System.Windows.Xps.Packaging;

namespace U23CCD
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        //public MainWindow()
        //{
        //    InitializeComponent();
        //}
        XpsDocument doc;
        string path = Environment.CurrentDirectory;
        FixedDocumentSequence fds;
        public MainWindow()
        {
            InitializeComponent();
            doc = new System.Windows.Xps.Packaging.XpsDocument((path + "\\help.xps"), System.IO.FileAccess.Read);
            fds = doc.GetFixedDocumentSequence();
            Helps.Document = fds;
        }
    }
}
