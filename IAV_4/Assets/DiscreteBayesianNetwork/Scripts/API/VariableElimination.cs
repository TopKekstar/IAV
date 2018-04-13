/*
 * Copyright (c) by Junjie Chen
 * Please refer to https://unity3d.com/legal/as_terms for the terms and conditions
 */

using System.Collections.Generic;
using System;
using System.Linq;

namespace Jackyjjc.Bayesianet
{
    /// <summary>
    /// Variable Elimination is one of the most commonly used algorithms for exact inferences on a Bayesian Network. 
    /// This class models variable elimination algorithm. It keep track of the BayesianNetwork object it is operating on.
    /// </summary>
    public class VariableElimination
    {
        private BayesianNetwork network;

        public VariableElimination(BayesianNetwork net)
        {
            this.network = net;
        }

        public BayesianNetwork GetNetwork()
        {
            return network;
        }
        
        /// <summary>
        /// Given the distribution of a variable, this function randomly pick a value based on the distribution.
        /// For example, if we have a node named 'fight' and it has two values: true (index 0), false (index 1).
        /// The distribution of the node is [0.8, 0.2]. In this case, the function will return either 0 or 1 
        /// (the index of the value) and it would have 80% chance returning 0 and 20% chance returning 1.
        /// </summary>
        /// <param name="distribution">a distribution table</param>
        /// <param name="random">a random number generator</param>
        /// <returns>the index of the value picked randomly</returns>
        public int PickOne(double[] distribution, Random random)
        {
            double result = random.NextDouble();

            int i;
            for (i = 0; i < distribution.Length; i++)
            {
                if (result < distribution[i])
                {
                    return i;
                }

                result -= distribution[i];
            }

            return i;
        }

        /// <summary>
        /// Given the distribution of a variable, this function randomly pick a value based on the distribution.
        /// For example, if we have a node named 'fight' and it has two values: true (index 0), false (index 1).
        /// The distribution of the node is [0.8, 0.2]. In this case, the function will return either 0 or 1 
        /// (the index of the value) and it would have 80% chance returning 0 and 20% chance returning 1. This
        /// function uses the random number generator in the Unity engine.
        /// </summary>
        /// <param name="distribution">a distribution table</param>
        /// <returns>the index of the value picked randomly</returns>
        public int PickOne(double[] distribution)
        {
            double result = UnityEngine.Random.Range(0f, 1f);

            int i;
            for (i = 0; i < distribution.Length; i++)
            {
                if (result < distribution[i])
                {
                    return i;
                }

                result -= distribution[i];
            }

            return i;
        }
        
        /// <summary>
        /// Given the query node name and an array of observation strings in the format of "name=value",
        /// this function perform inferences and return the inferred distribution of the node variable.
        /// </summary>
        /// <param name="queryNode">the name of the node</param>
        /// <param name="observationStrings">the observation strings</param>
        /// <returns>a distribution table for the values of the query node</returns>
        public double[] Infer(string queryNode, params string[] observationStrings)
        {
            IEnumerable<Proposition> observations;
            try {
                observations = observationStrings.Select(o =>
                {
                    string[] components = o.Split('=');
                    return network.FindNode(components[0].Trim()).Instantiate(components[1].Trim());
                });
            } catch (Exception)
            {
                throw new ArgumentException("The evidence/observation strings are not in the format of 'x = y'");
            }

            return Infer(network.FindNode(queryNode), observations);
        }

        /// <summary>
        /// Given the query node name and a list of observation strings in the format of "name=value",
        /// this function perform inferences and return the inferred distribution of the node variable.
        /// </summary>
        /// <param name="queryNode">the name of the node</param>
        /// <param name="observationStrings">the observation strings</param>
        /// <returns>a distribution table for the values of the query node</returns>
        public double[] Infer(string queryNode, IEnumerable<string> observationStrings)
        {
            return Infer(queryNode, observationStrings.ToArray());
        }

        /// <summary>
        /// Given a BayesianNode object and a list of Proposition objects as observations,
        /// this function perform inferences and return the inferred distribution of the node variable.
        /// </summary>
        /// <param name="query">the BayesianNode object being query</param>
        /// <param name="observations">the list of Proposition objects</param>
        /// <returns>a distribution table for the values of the query node</returns>
        public double[] Infer(BayesianNode query, IEnumerable<Proposition> observations) {
            return Infer(query, observations.ToArray());
        }

        /// <summary>
        /// Given a BayesianNode object and an array of Proposition objects as observations,
        /// this function perform inferences and return the inferred distribution of the node variable.
        /// </summary>
        /// <param name="query">the BayesianNode object being query</param>
        /// <param name="observations">the array of Proposition objects</param>
        /// <returns>a distribution table for the values of the query node</returns>
        public double[] Infer(BayesianNode query, params Proposition[] observations)
        {
            try {
                string[] evidenceNames = observations.Select(o => o.name).ToArray();

                IEnumerable<BayesianNode> nodes = TopologicalSort(network.GetNodes()).Reverse();

                HashSet<BayesianNode> relevantNodes = new HashSet<BayesianNode>();
                relevantNodes.Add(query);
                foreach (Proposition p in observations)
                {
                    relevantNodes.Add(network.FindNode(p.name));
                }
                MarkRelevantVariables(relevantNodes, nodes);
                // Remove variables that are not ancestor of a query variable or evidence variable
                nodes = nodes.Intersect(relevantNodes);

                List<CPT> factors = new List<CPT>();
                foreach (BayesianNode node in nodes)
                {
                    factors.Add(node.MakeFactor(observations));
                    if (node.var.name.Equals(query.var.name) || Array.IndexOf(evidenceNames, node.var.name) >= 0)
                    {
                        continue;
                    }

                    CPT[] factorsToSumOut = factors.Where(f => f.ContainsVar(node.var)).ToArray();
                    factors.RemoveAll(f => Array.IndexOf(factorsToSumOut, f) != -1);

                    CPT factorToSumOut;
                    if (factorsToSumOut.Count() > 1) {
                        factorToSumOut = factorsToSumOut.Skip(1).Aggregate(factorsToSumOut.First(), (acc, f) => acc.PointWiseProduct(f));
                    } else {
                        factorToSumOut = factorsToSumOut.First();
                    }

                    CPT afterSumOut = factorToSumOut.SumOut(node.var);
                    factors.Add(afterSumOut);
                }

                CPT result;
                if (factors.Count() > 1) {
                    result = factors.Skip(1).Aggregate<CPT, CPT>(factors.First(), (acc, f) => acc.PointWiseProduct(f));
                } else {
                    result = factors.First();
                }
                return result.Distribution();
            } catch (Exception e)
            {
                throw new Exception("Unable to perform inference on the network. " + 
                    "Please make sure the network is valid and the propositions are valid. " + 
                    "Contact the author of this library if you suspect it is a problem in the library." + 
                    "Actual Error (Reason: " + e.Message + " " + e.StackTrace + ")"
				);
            }
        }

        private void MarkRelevantVariables(HashSet<BayesianNode> relevantNodes, IEnumerable<BayesianNode> nodes)
        {
            HashSet<BayesianNode> irrelevantVariables = new HashSet<BayesianNode>();
            foreach (BayesianNode node in nodes)
            {
                MarkVariablesRecursive(node, relevantNodes, irrelevantVariables);
            }
        }

        private bool MarkVariablesRecursive(BayesianNode node, HashSet<BayesianNode> relevantNodes, HashSet<BayesianNode> irrelevantNodes)
        {
            if (relevantNodes.Contains(node))
            {
                return true;
            }

            if (irrelevantNodes.Contains(node))
            {
                return false;
            }

            foreach (BayesianNode child in node.children)
            {
                if (MarkVariablesRecursive(child, relevantNodes, irrelevantNodes))
                {
                    relevantNodes.Add(node);
                }
            }
            return relevantNodes.Contains(node);
        }

        private IEnumerable<BayesianNode> TopologicalSort(IEnumerable<BayesianNode> nodes)
        {
            HashSet<BayesianNode> visited = new HashSet<BayesianNode>();

            Stack<BayesianNode> result = new Stack<BayesianNode>();
            foreach (BayesianNode node in nodes)
            {
                TopologicalSortRecursive(node, visited, result);
            }

            return result;
        }

        private void TopologicalSortRecursive(BayesianNode node, HashSet<BayesianNode> visited, Stack<BayesianNode> result)
        {
            if (visited.Contains(node))
            {
                return;
            }
            
            // Visit the current node
            visited.Add(node);

            // Visit the children
            foreach (BayesianNode child in node.children)
            {
                TopologicalSortRecursive(child, visited, result);
            }

            result.Push(node);
        }
    }
}
