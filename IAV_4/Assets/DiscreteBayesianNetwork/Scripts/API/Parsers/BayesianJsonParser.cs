/*
 * Copyright (c) by Junjie Chen
 * Please refer to https://unity3d.com/legal/as_terms for the terms and conditions
 */

using System.Collections.Generic;
using System.Linq;
using System;
using MiniJSON;

namespace Jackyjjc.Bayesianet
{
    /// <summary>
    /// This parser parses the Bayesian Network stored in Json format.
    /// It uses the popular MiniJSON library: https://gist.github.com/darktable/1411710
    /// You can modify this file if you want to use another Json library.
    /// </summary>
    public class BayesianJsonParser : BayesianParser
    {
        /// <summary>
        /// Given a JSON string, this function parses it and creates a Bayesian Network.
        /// </summary>
        /// <param name="s">JSON string</param>
        /// <returns>a bayesian network parsed from the json string</returns>
        public override BayesianNetwork Parse(string json)
        {
            Dictionary<string, BayesianNode> nodeMap = new Dictionary<string, BayesianNode>();
            List<BayesianNode> nodes = new List<BayesianNode>();

            Dictionary<string, object> jsonObj;
            try
            {
                jsonObj = Json.Deserialize(json) as Dictionary<string, object>;
            }
            catch (Exception e)
            {
                throw new FormatException("The string is not valid JSON. Error detail: " + e.Message + " " + e.StackTrace);
            }

            try
            {
                List<object> jsonNodes = (List<object>)jsonObj["nodes"];
                foreach (Dictionary<string, object> jsonNode in jsonNodes)
                {
                    string name = (string)jsonNode["name"];
                    string[] domains = ((List<object>)jsonNode["domain"]).Select(d => (string)d).ToArray();
                    double[] values = ((List<object>)jsonNode["values"]).Select(v => (double)v).ToArray();
                    BayesianNode[] parents = ((List<object>)jsonNode["parents"]).Select(n => nodeMap[(string)n]).ToArray();

                    BayesianNode node = new BayesianNode(name, domains, values, parents);
                    nodes.Add(node);
                    nodeMap.Add(name, node);
                }

                return new BayesianNetwork(nodes.ToArray());
            }
            catch (Exception e)
            {
				throw new FormatException("Encounter error when trying to read in the network from the file." +
                    "Most likely the JSON file is not valid or the nodes are not valid. " +
                    "If you believe it is not your fault, please report to the author of this package." +
                    "Actual Error (Reason: " + e.Message + " " + e.StackTrace + ")"
				);
            }
        }
    }
}
