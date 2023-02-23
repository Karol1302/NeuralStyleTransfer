using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using Rect = OpenCvSharp.Rect;

namespace StyleTransfer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private readonly string[] _modelNames;
        private Mat? _content;
        private bool _contentImageLoaded;
        private string _styleImageName;
        private const string OutputLayer = "output1";

        public MainWindow()
        {
            InitializeComponent();

            _modelNames = Directory
                .GetFiles("models\\")
                .Select(Path.GetFileNameWithoutExtension)
                .ToArray()!;

            foreach (var modelName in _modelNames)
            {
                ModelSelection.Items.Add(modelName);
            }
        }

        private void Content_Btn_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JPG files (*.jpg)|*.jpg|PNG files (*.png)|*.png|BMP files (*.bmp)|*.bmp|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                if (_content is not null)
                {
                    _content.Dispose();
                    _content = null;
                }

                _content = new Mat(openFileDialog.FileName);

                if (_content.Empty())
                {
                    MessageBox.Show("Podczas wczytywania obrazu wystąpił błąd.");
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

            if (_contentImageLoaded)
                Transform_Btn.IsEnabled = true;
        }

        private void ModelSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var modelName = _modelNames[ModelSelection.SelectedIndex];
            _styleImageName = $"{modelName}.jpg";
            Image_style.Source = new BitmapImage(new Uri($"C:\\Users\\karol\\Downloads\\StyleTransfer\\pictures_style\\{_styleImageName}"));
        }

        private void Transform_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (ModelSelection.SelectedIndex < 0)
            {
                MessageBox.Show("Nie wybrano modelu");
                return;
            }

            var modelName = _modelNames[ModelSelection.SelectedIndex];
            var modelPath = Path.Combine("models", modelName);
            modelPath = Path.ChangeExtension(modelPath, ".onnx");

            if (!File.Exists(modelPath))
            {
                MessageBox.Show("Plik modelu nie istnieje w: " + modelPath);
                return;
            }

            var modelLoader = new MyModelLoader(modelPath);
            using var net = modelLoader.GetModel();

            var imageProcessor = new ImageProcessor();
            using var blob = imageProcessor.Process(_content);
            net.SetInput(blob, "input1");

            using var output = net.Forward(OutputLayer);

            var sizes = Enumerable
                .Range(0, output.Dims)
                .Select(output.Size)
                .ToArray();
            
            var outputReshaped = output.Reshape(
                1,
                sizes[2] * sizes[1],
                sizes[3]
            );
            outputReshaped.ConvertTo(outputReshaped, MatType.CV_8UC1);

            using var channel0 = new Mat(sizes[2], sizes[3], MatType.CV_8UC1);
            using var channel1 = new Mat(sizes[2], sizes[3], MatType.CV_8UC1);
            using var channel2 = new Mat(sizes[2], sizes[3], MatType.CV_8UC1);

            outputReshaped[new Rect(0, 0, sizes[2], sizes[3])].CopyTo(channel2);
            outputReshaped[new Rect(0, 1 * sizes[3], sizes[2], sizes[3])].CopyTo(channel1);
            outputReshaped[new Rect(0, 2 * sizes[3], sizes[2], sizes[3])].CopyTo(channel0);
            
            using var merged = new Mat(sizes[2], sizes[3], MatType.CV_8UC3);
            Cv2.Merge(
                new[] { channel0, channel1, channel2 },
                merged
            );
            Cv2.Resize(merged, merged, _content.Size(), 0, 0, InterpolationFlags.Cubic);
            Image_result.Source = merged.ToBitmapSource();
            Cv2.ImWrite("output.jpg", merged);

        }

    }
}
