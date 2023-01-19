using System;
using System.Windows;
using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace NeuralStyleTransfer
{
    internal class MyModelLoader
    {
        private readonly Net _net;
        public MyModelLoader(string modelPath, Target target = default, Backend backend = default)
        {

            _net = Net.ReadNetFromONNX(modelPath);

            _net.SetPreferableTarget(Target.CPU);

            //_net.SetPreferableBackend(Backend.INFERENCE_ENGINE);

        }

        //public MyModelLoader(string modelPath) => _net = Net.ReadNetFromONNX(modelPath);

        public Net GetModel() => _net;
    }
}