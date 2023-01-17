using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace NeuralStyleTransfer
{
    public class MyModelLoader
    {
        public MyModelLoader(Net net)
        {
            Net = net;
        }

        public Net Net { get; }

        public MyModelLoader Build(string modelPath)
        {
            var _net = Net.ReadNet(modelPath);
            return new MyModelLoader(_net);
        }
    }
}
