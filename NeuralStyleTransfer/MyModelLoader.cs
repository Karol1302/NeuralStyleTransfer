using System;
using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace NeuralStyleTransfer
{
    internal class MyModelLoader
    {
        private readonly Net _net;
        public MyModelLoader(string modelPath) => _net = Net.ReadNet(modelPath);

        public Net GetModel() => _net;
    }
}