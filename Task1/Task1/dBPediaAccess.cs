using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF.Query;

namespace Task1
{
    class dbPediaAccess
    {

        public string GenSPARQL_Query(string ontologyClass, string entity)
        {
            string query = 
               "Select ?name " + "?" + ontologyClass.ToString().ToLower() +
               " WHERE {" +
               "?" + ontologyClass.ToString().ToLower() + " a <http://dbpedia.org/ontology/" + ontologyClass + ">. " +
               "?" + ontologyClass.ToString().ToLower() + " foaf:name ?name. " +
               "FILTER (?name=\"" + entity + "\"@en).} " +
               "LIMIT 1";

            return query;
        }

        public void checkdBPedia(string ontologyClass, string entity)
        {
            SparqlRemoteEndpoint sparqlEndpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"));
            string newQuery = GenSPARQL_Query(ontologyClass, entity);
            SparqlResultSet resultSet = sparqlEndpoint.QueryWithResultSet(newQuery);
            if (resultSet.Results.Count != 0)
            {
                string oldString = resultSet.Results.First().ToString();
                string trimmed = oldString.Substring(oldString.LastIndexOf("http"));
                Console.WriteLine("dbpedia: " + trimmed);
            }
            else
            {
                Console.WriteLine("no results found at dbpedia.");
            }
        }

    }
}
