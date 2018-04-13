/*
 * Copyright (c) by Junjie Chen
 * Please refer to https://unity3d.com/legal/as_terms for the terms and conditions
 */

namespace Jackyjjc.Bayesianet
{
    /// <summary>
    /// This class represents a proposition. In other words, an instantiation of a node.
    /// For instance, fight=true would be a proposition. Proposition is used in inferences.
    /// </summary>
    public struct Proposition
    {
        public string name;
        public string value;
        public int valueIndex;

        /// <summary>
        /// Constructor for a Proposition object. 
        /// It is recommended to use the Instantiate method on the BayesianNode object instead.
        /// </summary>
        /// <param name="name">the name of the variable</param>
        /// <param name="value">the value to be assigned to the variable</param>
        /// <param name="index">the index of the value in the BayesianNode domain</param>
        public Proposition(string name, string value, int index)
        {
            this.name = name;
            this.value = value;
            this.valueIndex = index;
        }

        public override string ToString()
        {
            return name + ": " + value;
        }
    }
}
