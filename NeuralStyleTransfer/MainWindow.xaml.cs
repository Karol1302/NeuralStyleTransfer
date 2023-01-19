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
using Tensor = Tensorflow.Tensor;
using Tensorflow.Gradients;
using Tensorflow.Framework;

namespace NeuralStyleTransfer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Net _net;
        private Mat _content;
        private Mat _style;
        private double _progress;
        private bool _contentImageLoaded;
        private bool _styleImageLoaded;
        private Mat inputBlob;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                //_net = Net.ReadNetFromONNX("model.onnx");
                //_net = Net.ReadNetFromTensorflow("model.pb");
                //_net = Net.ReadNet("model.onnx");

                string filePath = "C:\\Users\\karol\\Documents\\GitHub\\NeuralStyleTransfer\\NeuralStyleTransfer\\keras_model.onnx";
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("Model file does not exist at: " + filePath);
                    return;
                }

                var modelLoader = new MyModelLoader(filePath);
                _net = modelLoader.GetModel();
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to load the model: " + e.Message);
            }

            _contentImageLoaded = false;
            _styleImageLoaded = false;

        }

        private Mat TransferStyle(Mat content, Mat style)
        {

            /*
            Cv2.Resize(content, content, new OpenCvSharp.Size(174, 175));
            Cv2.CvtColor(content, content, ColorConversionCodes.BGR2RGB);

            var blob1 = CvDnn.BlobFromImage(content, 1.0, new OpenCvSharp.Size(176, 3));

            Cv2.Resize(style, style, new OpenCvSharp.Size(174, 175));
            Cv2.CvtColor(style, style, ColorConversionCodes.BGR2RGB);

            var blob2 = CvDnn.BlobFromImage(style, 1.0, new OpenCvSharp.Size(176, 3));

            /*
            var contentBlob = CvDnn.BlobFromImage(content, 1.0, new OpenCvSharp.Size(256, 256));
            var styleBlob = CvDnn.BlobFromImage(style, 1.0, new OpenCvSharp.Size(256, 256));
            */
            /*
            Cv2.Resize(content, content, new OpenCvSharp.Size(256, 256));
            Cv2.CvtColor(content, content, ColorConversionCodes.BGR2RGB);
            Cv2.Resize(style, style, new OpenCvSharp.Size(256, 256));
            Cv2.CvtColor(style, style, ColorConversionCodes.BGR2RGB);
            Cv2.HConcat(content, style, inputBlob);
            */

            Cv2.Resize(content, content, new OpenCvSharp.Size(174, 175));
            Cv2.CvtColor(content, content, ColorConversionCodes.BGR2RGB);

            var contentTensor = Graph.ImportTensor(content.ToBytes(), "content");
            contentTensor = contentTensor.Reshape(new TensorShape(174, 175, 176, 3));

            Cv2.Resize(style, style, new OpenCvSharp.Size(174, 175));
            Cv2.CvtColor(style, style, ColorConversionCodes.BGR2RGB);

            var styleTensor = Tensorflow.Graph.ImportTensor(style.ToBytes(), "style");
            styleTensor = styleTensor.Reshape(new tensor_shape(174, 175, 176, 3));
            var inputTensor = Tensor.Concat(new Tensor[] { contentTensor, styleTensor }, 3);

            return inputTensor;


        _net.SetInput(inputTensor, "input_1");
 
            var output = _net.Forward("output");

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
                _content = new Mat(openFileDialog.FileName);

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
                _style = new Mat(openFileDialog.FileName);

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
            if (_content == null || _style == null)
            {
                MessageBox.Show("An error occurred while loading the image. Please make sure both content and style images are loaded.");
                return;
            }
            var result = TransferStyle(_content, _style);
            Cv2.ImWrite("result.jpg", result);
            Image_result.Source = new BitmapImage(new Uri("result.jpg", UriKind.Relative));

        }
    }
}
