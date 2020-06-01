using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using edu.stanford.nlp.ie.crf;
using Console = System.Console;
using System.IO;

namespace Task1
{
    class Program
    {
        private CRFClassifier Classifier { get; set; }


        static void Main(string[] args)
        {
            ProcessInput processInput = new ProcessInput();
            data.Input inputData = new data.Input();

            inputData.ReadInputData();
            processInput.ProcessInputData();
      

        }
    }
}
