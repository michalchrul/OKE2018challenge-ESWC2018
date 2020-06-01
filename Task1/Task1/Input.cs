using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using edu.stanford.nlp.ie.crf;

namespace Task1.data
{
   public class Input
    {
        public string sentence { get; set; }


        public Input(string text)
        {
            sentence = text;
        }

        public Input()
        {

        }

        public void ReadInputData()
        {
            var inputPath = @"C:\Users\micha\source\repos\OKE_Tests.ttl";
            Console.WriteLine("Input file:" + "\n");
            sentence = File.ReadAllText(inputPath) + "\n";
            Console.Write(sentence);

        }


    }
}
