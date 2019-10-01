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

namespace Example
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }

    public sealed class SixelBitmapDecoder : BitmapDecoder
    {
        private SixelBitmapDecoder()
        {
            // Prohibit construction without Stream
        }

        public SixelBitmapDecoder(System.IO.Stream stream)
        {
            throw new NotImplementedException();
        }

        internal override void SealObject()
        {
            throw new NotImplementedException();
        }
    }
}
