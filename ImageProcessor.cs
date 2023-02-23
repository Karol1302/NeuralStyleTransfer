using OpenCvSharp.Dnn;
using OpenCvSharp;

namespace StyleTransfer
{
    internal class ImageProcessor
    {

        private const int NetWidth = 224;
        private const int NetHeight = 224;
        private const double ScaleFactor = 1.0/255;

        public Mat Process(Mat image)
        {
            var channels = Cv2.Split(image);
            var mergedImage = new Mat();
            Cv2.Merge(
                new Mat[] { channels[2], channels[0], channels[1] },
                mergedImage
            );

            return CvDnn.BlobFromImage(
                mergedImage,
                1,
                new Size(NetWidth, NetHeight),
                Scalar.All(0),
                false,
                false
            );
        }
    }

}
