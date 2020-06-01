using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using edu.stanford.nlp.ie.crf;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using System.Text.RegularExpressions;
using System.Xml;

namespace Task1
{
    class ProcessInput
    {
        enum ontologyClass
        {
          LOCATION, PERSON, ORGANIZATION
        }

        public ProcessInput()
        {

        }
        private CRFClassifier classifier { get; set; }
        private string inputPath = @"C:\Users\micha\source\repos\OKE_Tests.ttl";

        private List<string> locationEntityL = new List<string>();
        private List<string> organizationEntityL = new List<string>();
        private List<string> personEntityL = new List<string>();


        public void EntityExtractor(string classifierOutputXML, string ontologyClass)
        {

            var openingTag = "<" + ontologyClass + ">";

            bool isInTag = false;
            string name = "";
            string opn = "";

            for (int i = 0; i < classifierOutputXML.Length; i++)
            {
                opn += classifierOutputXML[i];

                if (opn.Contains(openingTag) || isInTag)
                {
                    if (classifierOutputXML[i].ToString() != "<")
                    {
                        if (classifierOutputXML[i].ToString() != ">")
                        {
                            name += classifierOutputXML[i];
                            isInTag = true;
                        }
                    }
                    else
                    {
                        if (ontologyClass == "LOCATION")
                        {
                            locationEntityL.Add(name);
                        }
                        else if (ontologyClass == "ORGANIZATION")
                        {
                            organizationEntityL.Add(name);
                        }
                        else if (ontologyClass == "PERSON")
                        {
                            personEntityL.Add(name);
                        }
                        isInTag = false;
                        opn = "";
                        name = "";
                    }
                }
            }

        }

        public void ProcessInputData()
        {
            Console.WriteLine("Processing input file...");

            dbPediaAccess dbPediaChecker = new dbPediaAccess();

            var jarRoot = @"C:\Users\micha\source\repos\ConsoleApp2\ConsoleApp2\data\paket-files\nlp.stanford.edu\stanford-ner-2016-10-31";
            var classifiersDirecrory = jarRoot + @"\classifiers";
            var classifier = CRFClassifier.getClassifierNoExceptions(classifiersDirecrory + @"\english.all.3class.distsim.crf.ser.gz");  
            IGraph g = new Graph();
            new TurtleParser().Load(g, inputPath);
            var triples = g.Triples.Where(q => q.Predicate.ToString().ToLower().Contains("isString".ToLower())).GroupBy(q => q.Object).Select(q => q.First());


            //https://csharp.hotexamples.com/examples/VDS.RDF.Parsing/TurtleParser/-/php-turtleparser-class-examples.html\
            //linq microsoft, regex
            var dataSet = new List<data.Input>();
            foreach (var t in triples)
            {
                var match = new Regex("char=([0-9]+),([0-9]+)").Match(t.Subject.ToString());
                if (!match.Success) continue;
                int startIndex = 0;
                int stopIndex = int.Parse(match.Groups[2].Value);

                var newData = new data.Input(t.Object.ToString().Substring(startIndex, stopIndex));
                dataSet.Add(newData);
            }

            //http://sergey-tihon.github.io/Stanford.NLP.NET/StanfordNER.html

            Console.WriteLine();

            string classifierOutput = "";

            for (int cnt = 0; cnt < dataSet.Count(); cnt++)
            {
                classifierOutput = classifier.classifyWithInlineXML(dataSet[cnt].sentence);
                // Console.WriteLine("{0}\n", classifierOutput);
                Console.WriteLine(dataSet[cnt].sentence + "\n");
                EntityExtractor(classifierOutput, "LOCATION");
                EntityExtractor(classifierOutput, "ORGANIZATION");
                EntityExtractor(classifierOutput, "PERSON");

                if (locationEntityL.Count > 0)
                {
                    locationEntityL.Distinct();
                    Console.WriteLine("Locations:");
                    for (int i = 0; i < locationEntityL.Count; i++)
                    {
                        Console.WriteLine(locationEntityL[i]);
                        dbPediaChecker.checkdBPedia("Place", locationEntityL[i]);
                    }
                    Console.WriteLine();
                    locationEntityL.Clear();
                }

                if (organizationEntityL.Count > 0)
                {
                    organizationEntityL.Distinct();
                    Console.WriteLine("Organisations:");
                    for (int i = 0; i < organizationEntityL.Count; i++)
                    {
                        Console.WriteLine(organizationEntityL[i]);
                        dbPediaChecker.checkdBPedia("Organisation", organizationEntityL[i]);
                    }
                    Console.WriteLine();
                    organizationEntityL.Clear();
                }

                if (personEntityL.Count > 0)
                {
                    personEntityL.Distinct();
                    Console.WriteLine("Persons:");
                    for (int i = 0; i < personEntityL.Count; i++)
                    {
                        Console.WriteLine(personEntityL[i]);
                        dbPediaChecker.checkdBPedia("Person", personEntityL[i]);
                    }
                    Console.WriteLine();
                    personEntityL.Clear();
                }



            }


            //    Console.WriteLine("{0}\n", classifier.classifyToString(dataSet.First().sentence));
            // (

            //var s1 = "Good afternoon Rajat Raina, how are you today?";
            //Console.WriteLine("{0}\n", classifier.classifyToString(s1));

            //var s2 = "I go to school at Stanford University, which is located in California.";
            //Console.WriteLine("{0}\n", classifier.classifyWithInlineXML(s2));

            //var s3 = "Michael Jackson and Donald Trump never met in New York.";
            //Console.WriteLine("{0}\n", classifier.classifyWithInlineXML(s3));

            //Console.WriteLine("{0}\n", classifier.classifyToString(s2, "xml", true));

            Console.ReadKey();

        }

       

    }
        
}
