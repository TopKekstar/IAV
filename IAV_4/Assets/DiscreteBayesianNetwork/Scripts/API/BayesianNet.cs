/*
 * Copyright (c) by Junjie Chen
 * Please refer to https://unity3d.com/legal/as_terms for the terms and conditions
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace Jackyjjc.Bayesianet
{
    /// <summary>
    /// This class represents the Bayesian Network model
    /// </summary>
    public class BayesianNetwork
    {
        private BayesianNode[] nodes;
        private Dictionary<string, BayesianNode> nodeMap;

        /// <summary>
        /// Given an array of BayesianNode objects, this function constructs a Bayesian Network 
        /// that references those nodes. It does not perform a clone on those nodes objects.
        /// </summary>
        /// <param name="nodes"></param>
        public BayesianNetwork(params BayesianNode[] nodes)
        {
            this.nodes = nodes;
            this.nodeMap = this.nodes.ToDictionary(n => n.var.name, n => n);
        }
        
        /// <summary>
        /// Given the name of a node, this function tries to find the nodes in the Bayesian Network.
        /// If the node is not found, it throws a KeyNotFoundException.
        /// </summary>
        /// <param name="name">the name of the node to find in the bayesian network</param>
        /// <returns>the BayesianNode object that has the name</returns>
        public BayesianNode FindNode(string name) {
            if (! nodeMap.ContainsKey(name))
            {
                throw new KeyNotFoundException(string.Format("The node '{0}' is not in the network. Please make sure the name is correct.", name));
            }
            return nodeMap[name];
        }

        /// <summary>
        /// Get the array of all the nodes in the network. The array is a copy of the internal 
        /// structure but the BayesianNode objects are not clones. It means a modification on the node
        /// results in a modification of the same node in the network.
        /// </summary>
        /// <returns>an array of all the nodes in the network</returns>
        public BayesianNode[] GetNodes()
        {
            BayesianNode[] result = new BayesianNode[nodes.Length];
            Array.Copy(nodes, result, nodes.Length);
            return result;
        }
    }
}