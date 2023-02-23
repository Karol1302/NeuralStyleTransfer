using System;
using System.Windows;
using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace StyleTransfer
{
    internal class MyModelLoader
    {
        private readonly Net _net;
        public MyModelLoader(string modelPath, Target target = default, Backend backend = default)
        {
                _net = Net.ReadNetFromONNX(modelPath);
                _net.SetPreferableTarget(Target.CPU);
                _net.SetPreferableBackend(Backend.DEFAULT);
        }
        public Net GetModel() => _net;
    }
}
