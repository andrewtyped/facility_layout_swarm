using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;

namespace FacilityLayoutWPF.Services
{
    class FacilityLayoutIOService : IIOService
    {
        private static string _lastDirectory = null;

        public string OpenFileDialog()
        {
            string result = null;

            var dialog = new OpenFileDialog();
            dialog.InitialDirectory =  _lastDirectory ?? @"E:\GitRepos\facility_layout_swarm\Sample Config Data";
            dialog.Title = "Load Facility Data";
            dialog.Filter = "Text Files (*.txt)|*.txt";
            dialog.FilterIndex = 1;

            var dResult = dialog.ShowDialog();

            if((bool)dResult)
            {
                result = dialog.FileName;
                _lastDirectory = result;
            }

            return result;
        }
    }
}
