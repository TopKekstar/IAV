/*
 * Copyright (c) by Junjie Chen
 * Please refer to https://unity3d.com/legal/as_terms for the terms and conditions
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Jackyjjc.Bayesianet
{
    /// <summary>
    /// This parser parses Bayesian Networks in GeNIe 2.0 format.
    /// GeNIe is a Bayesian Network editor developed by BayesFusion, LLC.
    ///
    /// Disclaimer: This package is not affiliated with BayesFusion, LLC in anyway.
    /// </summary>
    public class BayesianGenieParser : BayesianParser
    {
        /// <summary>
        /// Given an xml string in GeNIe 2.0 format, this function parses it and creates a Bayesian Network.
        /// </summary>
        /// <param name="s">an xml string in GeNIe 2.0 format</param>
        /// <returns>the bayesian network parsed from the xml string</returns>
        public override BayesianNetwork Parse(string s)
        {
            Dictionary<string, BayesianNode> nodeMap = new Dictionary<string, BayesianNode>();
            List<BayesianNode> nodes = new List<BayesianNode>();

            try {
                XElement rootElement = XDocument.Parse(s).Root;
                IEnumerable<XElement> cpts = rootElement.Elements()
                    .First(node => node.Name.LocalName.Equals("nodes"))
                    .Elements().Where(node => node.Name.LocalName.Equals("cpt"));

                foreach (XElement cpt in cpts)
                {
                    string name = cpt.Attribute("id").Value;
                    string[] domains = cpt.Elements().Where(e => e.Name.LocalName.Equals("state")).Select(e => e.Attribute("id").Value).ToArray();
                    double[] values = ((XText)cpt.Elements().First(e => e.Name.LocalName.Equals("probabilities")).FirstNode)
                        .Value.Split(' ').Select(v => double.Parse(v)).ToArray();

                    IEnumerable<XElement> parentsXmlNode = cpt.Elements().Where(e => e.Name.LocalName.Equals("parents"));
                    BayesianNode[] parents;
                    if (parentsXmlNode.Count() == 0)
                    {
                        parents = new BayesianNode[] { };
                    } else
                    {
                        parents = ((XText)parentsXmlNode.First().FirstNode).Value.Split(' ').Select(n => nodeMap[n]).ToArray();
                    }

                    BayesianNode node = new BayesianNode(name, domains, values, parents);
                    nodes.Add(node);
                    nodeMap.Add(name, node);
                }

                return new BayesianNetwork(nodes.ToArray());
            } catch (Exception e)
            {
				throw new FormatException("Encounter error when trying to read in the network from the GeNie file. " +
                    "If you believe it is not your fault, please report to the author of this package." +
                    "(Reason: " + e.Message + " " + e.StackTrace + ")"
				);
            }
        }
    }
}
