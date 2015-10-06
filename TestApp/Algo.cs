using AppUpdater.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestApp
{
    public class Algo : IService
    {
        public void Start()
        {
            System.Windows.Forms.MessageBox.Show("Start");
        }

        public void Stop()
        {
            System.Windows.Forms.MessageBox.Show("Stop");
        }
    }
}
