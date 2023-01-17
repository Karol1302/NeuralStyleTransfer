using System;
using System.IO;
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
using OpenCvSharp;
using OpenCvSharp.Dnn;
using Window = System.Windows.Window;

namespace NeuralStyleTransfer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Net _net;
        private const int Width = 512;
        private const int Height = 512;
        private Mat _content;
        private Mat _style;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                _net = CvDnn.ReadNetFromTensorflow("C:\\Users\\karol\\Documents\\GitHub\\NeuralStyleTransfer\\NeuralStyleTransfer\\saved_model.pb");
            }
            catch (Exception e)
            {
                MessageBox.Show("Nie mozna załadowac modelu: " + e.Message);
            }
        }
        /*
        private Mat TransferStyle(Mat content, Mat style)
        {
            var contentBlob = CvDnn.BlobFromImage(content, 1.0, new OpenCvSharp.Size(Width, Height));
            var styleBlob = CvDnn.BlobFromImage(style, 1.0, new OpenCvSharp.Size(Width, Height));

            _net.SetInput(contentBlob, "content");
            _net.SetInput(styleBlob, "style");

            var output = _net.Forward();

            var result = output.GetBlob("generated").MatRef();

            return result;
        }
        */
        private void Content_Btn_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JPG files (*.jpg)|*.jpg|PNG files (*.png)|*.png|BMP files (*.bmp)|*.bmp|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                _content = Cv2.ImRead(openFileDialog.FileName);

                if (_content.Empty())
                {
                    MessageBox.Show("An error occurred while loading the content image.");
                    return;
                }
            }
            Image_content.Source = new BitmapImage(new Uri(openFileDialog.FileName));
        }

        private void Style_Btn_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JPG files (*.jpg)|*.jpg|PNG files (*.png)|*.png|BMP files (*.bmp)|*.bmp|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                _style = Cv2.ImRead(openFileDialog.FileName);

                if (_style.Empty())
                {
                    MessageBox.Show("An error occurred while loading the content image.");
                    return;
                }
            }
            Image_style.Source = new BitmapImage(new Uri(openFileDialog.FileName));

        }

        private void Transform_Btn_Click(object sender, RoutedEventArgs e)
        {
            /*var result = TransferStyle(_content, _style);
            Cv2.ImWrite("result.jpg", result);
            */
        }
    }
}
