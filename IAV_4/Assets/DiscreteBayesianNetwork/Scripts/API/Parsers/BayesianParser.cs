/*
 * Copyright (c) by Junjie Chen
 * Please refer to https://unity3d.com/legal/as_terms for the terms and conditions
 */

namespace Jackyjjc.Bayesianet
{
    /// <summary>
    /// This is the abstract class existing parsers extend from. It provides an implementation of the 
    /// ParseFile function. When you write you own parser, you can choose whether to extend from this 
    /// class or not depends on your requirement.
    /// </summary>
    public abstract class BayesianParser
    {
        /// <summary>
        /// Given a string, the function parses it and creates a Bayesian Network object
        /// </summary>
        /// <param name="s">the string to parse</param>
        /// <returns>the bayesian network parsed from the string</returns>
        public abstract BayesianNetwork Parse(string s);

        /// <summary>
        /// Given the absolute path of a file, the function reads and parses the file.
        /// </summary>
        /// <param name="path">the absolute path to a file</param>
        /// <returns>the bayesian network parsed from file</returns>
        public BayesianNetwork ParseFile(string path)
        {
            return Parse(System.IO.File.ReadAllText(path));
        }
    }
}
