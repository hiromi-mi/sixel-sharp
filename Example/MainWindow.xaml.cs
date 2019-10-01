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

            // https://docs.microsoft.com/ja-jp/dotnet/api/system.windows.media.imaging.bitmapsource
            PixelFormat pf = PixelFormats.Bgr32;
            var (width, height) = (200, 200);

            // 切り上げる
            int rawStride = (width * pf.BitsPerPixel + 7) / 8;
            var rawImage = new byte[rawStride * height];

            // randomize
            var value = new Random();
            value.NextBytes(rawImage);

            var bitmap = BitmapSource.Create(width, height, 96, 96, pf, null, rawImage, rawStride);
            this.sixelimage.Source = bitmap;
        }
    }
}
