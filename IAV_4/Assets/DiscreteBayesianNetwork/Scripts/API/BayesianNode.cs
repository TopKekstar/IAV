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
    /// This class represents a node in the Bayesian Network.
    /// </summary>
    public class BayesianNode
    {
        internal RandomVariable var;
        internal List<BayesianNode> children;
        private CPT cpt;

        /// <summary>
        /// Construct a node
        /// </summary>
        /// <param name="name">name of the node</param>
        /// <param name="domain">the domain of the node (list of states the node can be in)</param>
        /// <param name="values">the probabilities of those states given all possible parent states</param>
        /// <param name="parents">the parent nodes</param>
        public BayesianNode(string name, string[] domain, double[] values, params BayesianNode[] parents)
        {
            this.var = new RandomVariable(name, domain);
            this.children = new List<BayesianNode>();
            foreach (BayesianNode p in parents)
            {
                p.children.Add(this);
            }

            RandomVariable[] vars = parents
                .Select<BayesianNode, RandomVariable>(p => p.var)
                .Concat(new RandomVariable[] { this.var })
                .ToArray();

			// Validating the node configuration
            int numberOfValues = vars.Aggregate<RandomVariable, int>(1, (acc, next) => acc * next.tokens.Length);
            if (numberOfValues != values.Length) {
                throw new ArgumentException ("The expect number of values for node " + name + " is " + numberOfValues + 
                    ". The actual number of value given is " + values.Length);
            }

            this.cpt = new CPT(vars, values);
        }

        /// <summary>
        /// Get the distribution table of this node
        /// </summary>
        /// <returns>the raw probabilities table with all the values</returns>
        public double[] Distribution()
        {
            return this.cpt.Distribution();
        }

        // Generate a proposition from this node. In other words, assign a value to this node variable.
        // For example, if the node is 'fight', you can instantiate it to fight=true or fight=false
        /// <summary>
        /// Creates a proposition from this node. In other words, assign a value to this node variable.
        /// For example, if the node represents the boolean variable 'fight', you can create a Proposition
        /// figh=true and use it for inference
        /// </summary>
        /// <param name="value">the value to be assigned to the node</param>
        /// <returns>a proposition object represents the instantiation</returns>
        public Proposition Instantiate(string value)
        {
            return Instantiate(var.GetTokenIndex(value));
        }
        
        /// <summary>
        /// Generate a proposition from this node. In other words, assign the value at 'index' to this node variable.
        /// For example, if the value at index 0 is true and the value at index 1 is false, then calling the function
        /// with index 0 is the same as calling the overloaded function with the value "true". The index is the same
        /// as the order of the domain values whe constructing the node.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Proposition Instantiate(int index)
        {
            if (index < 0) {
				throw new ArgumentException("Index cannot be negative.");   
            }
				
            if (index > var.tokens.Length - 1)
            {
				throw new ArgumentException("The index is invalid because it is greater than the number of possible values.");
            }

            return new Proposition(var.name, var.tokens[index], index);
        }

        internal double[] Distribution(params Proposition[] evidence)
        {
            return this.cpt.Distribution(evidence);
        }

        internal CPT MakeFactor(params Proposition[] evidence)
        {
            return this.cpt.MakeFactor(evidence);
        }

        public override string ToString()
        {
            string result = "";

            result += var.name + "\n";
            result += cpt.ToString();

            return result;
        }
    }
}