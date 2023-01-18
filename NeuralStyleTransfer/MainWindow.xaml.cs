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
using Tensorflow;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.OnnxRuntime;

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
        private double _progress;
        private bool _contentImageLoaded;
        private bool _styleImageLoaded;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                _net = Net.ReadNetFromONNX("model.onnx");

                //_net = Net.ReadNet("model.onnx");
                /*
                var modelLoader = new MyModelLoader("model.onnx");
                _net = modelLoader.GetModel();
                */
            }
            catch (Exception e)
            {
                MessageBox.Show("Nie mozna załadowac modelu: " + e.Message);
            }
            _contentImageLoaded = false;
            _styleImageLoaded = false;

        }


        private Mat TransferStyle(Mat content, Mat style)
        {
            var contentBlob = CvDnn.BlobFromImage(content, 1.0, new OpenCvSharp.Size(Width, Height));
            var styleBlob = CvDnn.BlobFromImage(style, 1.0, new OpenCvSharp.Size(Width, Height));

            _net.SetInput(contentBlob, "content");
            _net.SetInput(styleBlob, "style");

            var output = _net.Forward();

            var result = CvDnn.BlobFromImage(output);
            result.SaveImage("output.jpg");

            return result;
        }
        
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
            if (string.IsNullOrEmpty(openFileDialog.FileName))
            {
                MessageBox.Show("Nie wybrano pliku.");
                return;
            }
            Image_content.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            _contentImageLoaded = true;

            if (_contentImageLoaded && _styleImageLoaded)
                Transform_Btn.IsEnabled = true;
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
            if (string.IsNullOrEmpty(openFileDialog.FileName))
            {
                MessageBox.Show("Nie wybrano pliku.");
                return;
            }
            Image_style.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            _styleImageLoaded = true;

            if (_contentImageLoaded && _styleImageLoaded)
                Transform_Btn.IsEnabled = true;
        }

        private void Transform_Btn_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (_content == null || _style == null)
            {
                MessageBox.Show("An error occurred while loading the image. Please make sure both content and style images are loaded.");
                return;
            }
            */
            var result = TransferStyle(_content, _style);
            Cv2.ImWrite("result.jpg", result);
            Image_result.Source = new BitmapImage(new Uri("result.jpg", UriKind.Relative));

        }
    }
}
