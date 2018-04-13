/*
 * Copyright (c) by Junjie Chen
 * Please refer to https://unity3d.com/legal/as_terms for the terms and conditions
 */

using System;

namespace Jackyjjc.Bayesianet
{
    internal class RandomVariable
    {
        internal string name;
        internal string[] tokens;

        internal RandomVariable(string name, string[] tokens)
        {
            this.name = name;
            this.tokens = tokens;
        }

        internal int GetTokenIndex(string token)
        {
            return Array.IndexOf(tokens, token);
        }

        public override bool Equals(object obj)
        {
            if (obj == this) return true;
            if (obj == null) return false;
            if (!(obj is RandomVariable))
            {
                return false;
            }
            RandomVariable otherVar = (RandomVariable)obj;
            return this.name.Equals(otherVar.name);
        }

        public override int GetHashCode()
        {
            return this.name.GetHashCode();
        }

        public override string ToString()
        {
            return name;
        }
    }
}
