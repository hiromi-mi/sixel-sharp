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
using Microsoft.Win32;
using System.IO;




namespace Example
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        // Example from wikipedia
        // https://github.com/ushitora-anqou/tinysixel/
        // ftp://ftp.fu-berlin.de/unix/www/lynx/pub/shuford/terminal/all_about_sixels.txt
        private readonly string str = $@"
\ePq
#0;2;0;0;0#1;2;100;100;0#2;2;0;100;0
#1~~@@vv@@~~@@~~$
#2??}}GG}}??}}??-
#1!14@
\e\\;";
        public MainWindow()
        {
            InitializeComponent();

            // https://docs.microsoft.com/ja-jp/dotnet/api/system.windows.media.imaging.bitmapsource
            /*
            PixelFormat pf = PixelFormats.Bgr32;
            var (width, height) = (200, 200);

            // 切り上げる
            int rawStride = (width * pf.BitsPerPixel + 7) / 8;
            var rawImage = new byte[rawStride * height];

            // randomize
            var value = new Random();
            value.NextBytes(rawImage);

            var bitmap = BitmapSource.Create(width, height, 96, 96, pf, null, rawImage, rawStride);
            sixelimage.Source = bitmap;

            // http://funct.hatenablog.com/entry/20150929/1443538237
            (width, height) = (bitmap.PixelWidth, bitmap.PixelHeight);
            var stride = (width * bitmap.Format.BitsPerPixel + 7) / 8;

            var data = new byte[stride * height];
            bitmap.CopyPixels(data, stride, 0);
            */

            // Pixel Data に取り出し

            var uri = new Uri("file:///Users/MizunoMidori/Pictures/キャプチャ.PNG");
            var rawimage = JpegBitmapDecoder.Create(uri, BitmapCreateOptions.None, BitmapCacheOption.Default);
            var raw = new SixelRawImage(rawimage.Frames[0]);
            raw.ToString();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "テキストファイル (*.txt)|*.txt|すべてのファイル (*.*)|*.*";
            ofd.Title = "Open Sixel Image";
            ofd.ShowDialog();
            byte[] output = new byte[100000];
            int size = 20;
            int height = 1;
            using (FileStream fs = File.OpenRead(ofd.FileName))
            {
                byte[] b = new byte[fs.Length + 1];
                

                
                UTF8Encoding encoding = new UTF8Encoding();
                var (x, y) = (0, 0);

                Func<byte, int> writedown = abyte =>
                {
                    for (int k = 0; k < 6; k++)
                    {
                        if ((abyte & (1 << k)) > 0)
                        {
                            output[size * (y + k) + x] = 255;
                        }
                    }
                    x += 1;
                    return x;
                };
                for (int i = 3; i < fs.Read(b, 0, b.Length); i++)
                {
                    if (b[i] == '\u001B')
                    {
                        // Escape
                        break;
                    }
                    if (b[i] == '-')
                    {
                        y += 6;
                        x += 0;
                        height++;

                    }
                    else if (b[i] == '!')
                    {
                        var cnt = b[i + 1] - '0';
                        for (int l = 0; l < cnt; l++)
                        {
                            writedown(b[i + 2]);
                        }
                        i += 2;
                    }
                    else if (b[i] >= '?' && b[i] <= 63 + 64)
                    {
                        writedown(b[i]);
                    }
                    // else: just ignore
                }
            }

            PixelFormat pf = PixelFormats.Gray8;

            // 切り上げる
            int rawStride = (size * pf.BitsPerPixel + 7) / 8;

            var bitmap = BitmapSource.Create(size, height, 96, 96, pf, null, output, rawStride);
            sixelimage.Source = bitmap;

            // http://funct.hatenablog.com/entry/20150929/1443538237
            // (size, height) = (bitmap.PixelWidth, bitmap.PixelHeight);
            var stride = (size * bitmap.Format.BitsPerPixel + 7) / 8;

            var data = new byte[stride * height];
            bitmap.CopyPixels(data, stride, 0);
        }
    }


    public class SixelRawImage
    {
        private string instr;
        public SixelRawImage(BitmapSource bitmap)
        {

            // https://docs.microsoft.com/en-us/dotnet/framework/wpf/graphics-multimedia/how-to-convert-a-bitmapsource-to-a-different-pixelformat
            FormatConvertedBitmap formatConvertedBitmap = new FormatConvertedBitmap();
            formatConvertedBitmap.BeginInit();
            formatConvertedBitmap.Source = bitmap;
            formatConvertedBitmap.DestinationFormat = PixelFormats.Bgr24;
            formatConvertedBitmap.EndInit();

            var (width, height) = (formatConvertedBitmap.PixelWidth, formatConvertedBitmap.PixelHeight);
            var stride = (width * formatConvertedBitmap.Format.BitsPerPixel + 7) / 8;

            // 8バイトづつ
            var rawpixeldata = new byte[stride * height];
            formatConvertedBitmap.CopyPixels(rawpixeldata, stride, 0);

            StringBuilder builder = new StringBuilder();

            for (int i = 0;i<(height+5)/6;i++)
            {
                for (int k = 0; k < width; k++)
                {
                    int item = 63;
                    for (int j = 0; j < 6; j++)
                    {
                        int index = (6 * i + j) * width * 3 + k * 3;
                        if (index < stride * height && rawpixeldata[index] == 0xFF)
                        {
                            item += 1 << j;
                        }
                    }
                    builder.Append(char.ConvertFromUtf32(item));
                }

                builder.Append("-"); // create new line
            }

            instr = builder.ToString();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("\u001BPq"); // Getting into Sixel Mode
            builder.Append(instr);
            builder.Append("\u001B\\"); // Gettting out of sixel mode
            return builder.ToString();

            // 6行づつ0, 1 それぞれバイトごとで格納されて、+63 オフセットついてる
            /* <-1
             * 
             * 
             * 
             * 
             * <-32 */

            // - は次の行にいく、
            // $ は重複を表す

            // 色番号: #n;p1;p2;p3;p4".
        }
    }
}
