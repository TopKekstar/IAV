/*
 * Copyright (c) by Junjie Chen
 * Please refer to https://unity3d.com/legal/as_terms for the terms and conditions
 */

using System;
using System.Collections.Generic;
using System.Linq;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Tests")]
namespace Jackyjjc.Bayesianet
{
    internal class CPT
    {
        internal RandomVariable[] vars;
        internal Dictionary<RandomVariable, int> keyPosMap;
        internal CptEntry[] cpt;

        internal CPT(RandomVariable[] vars, Dictionary<RandomVariable, int> keyPosMap, CptEntry[] cpt)
        {
            this.vars = vars;
            this.keyPosMap = keyPosMap;
            this.cpt = cpt;
        }

        internal CPT(RandomVariable[] vars, double[] values)
        {
            this.vars = vars;
            this.keyPosMap = new Dictionary<RandomVariable, int>();
            this.cpt = new CptEntry[values.Length];

            for(int i = 0; i < vars.Length; i++) {
                keyPosMap.Add(vars[i], i);
            }

            CptKey[] keys = Enumerable.Range(1, values.Length).Select(_ => new CptKey(vars.Length)).ToArray();
            SetKeys(new ArraySegment<CptKey>(keys), new ArraySegment<RandomVariable>(vars));

            for (int i = 0; i < cpt.Length; i++)
            {
                cpt[i] = new CptEntry(keys[i], values[i]);
            }
        }

        internal CPT MakeFactor(params Proposition[] evidence)
        {
            evidence = RemoveIrrelevantEvidence(evidence);
            return new CPT(vars.Where(d => !evidence.Select(e => e.name).Contains(d.name)).ToArray(), Distribution(evidence));
        }

        internal double[] Distribution(params Proposition[] evidence)
        {
            evidence = RemoveIrrelevantEvidence(evidence);

            double[] filteredCpt = FilterCptByEvidence(evidence)
                .Select(entry => entry.value)
                .ToArray();
            return Normalise(filteredCpt);
        }

        internal bool ContainsVar(RandomVariable var)
        {
            return vars.Contains(var);
        }

        internal CPT SumOut(RandomVariable varToSumOut)
        {
            int varToSumOutIndex = keyPosMap[varToSumOut];

            RandomVariable[] remainingVars = vars.Where(v => !v.Equals(varToSumOut)).ToArray();

            Dictionary<RandomVariable, int> newStartPosMap = keyPosMap
                .Where(kvp => !kvp.Key.Equals(varToSumOut))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value > varToSumOutIndex ? kvp.Value - 1 : kvp.Value);

            Dictionary<CptKey, List<CptEntry>> mapping = new Dictionary<CptKey, List<CptEntry>>();
            foreach (CptEntry entry in cpt)
            {
                CptKey key = entry.key.Remove(varToSumOutIndex);
                if (!mapping.ContainsKey(key))
                {
                    mapping.Add(key, new List<CptEntry>());
                }
                mapping[key].Add(entry);
            }

            List<CptEntry> newEntries = new List<CptEntry>();
            foreach (KeyValuePair<CptKey, List<CptEntry>> entry in mapping)
            {
                double value = entry.Value.Aggregate<CptEntry, double>(0, (acc, e) => acc + e.value);
                newEntries.Add(new CptEntry(entry.Key, value));
            }

            return new CPT(remainingVars, newStartPosMap, newEntries.ToArray());
        }

        internal CPT PointWiseProduct(CPT other)
        {
            IEnumerable<RandomVariable> commonVars = this.vars.Intersect(other.vars);
            HashSet<RandomVariable> rightVars = new HashSet<RandomVariable>(other.vars);
            rightVars.ExceptWith(commonVars);

            int[] commVarsIndicesInLeft = commonVars.Select<RandomVariable, int>(v => keyPosMap[v]).ToArray();
            int[] commVarsIndicesInRight = commonVars.Select<RandomVariable, int>(v => other.keyPosMap[v]).ToArray();

            List<RandomVariable> varsInResultCPT = new List<RandomVariable>(this.vars);
            List<RandomVariable> rightVarsInResult = new List<RandomVariable>(other.vars);
            rightVarsInResult.RemoveAll(v => commonVars.Contains(v));
            varsInResultCPT.AddRange(rightVarsInResult);

            Dictionary<RandomVariable, int> newKeyPosMap = new Dictionary<RandomVariable, int>();
            for (int i = 0; i < varsInResultCPT.Count(); i++)
            {
                newKeyPosMap.Add(varsInResultCPT[i], i);
            }

            int newCptSize = varsInResultCPT.Aggregate<RandomVariable, int>(1, (acc, v) => acc * v.tokens.Length);
            CptEntry[] newCptEntries = new CptEntry[newCptSize];

            Dictionary<CptKey, List<CptEntry>> mapping = new Dictionary<CptKey, List<CptEntry>>();

            CategoriseEntries(other, commVarsIndicesInRight, mapping);

            int cptIndex = 0;
            foreach (CptEntry leftEntry in this.cpt)
            {
                CptKey key = leftEntry.key.Extract(commVarsIndicesInLeft);
                List<CptEntry> right = mapping[key];
                foreach (CptEntry rightEntry in right)
                {
                    CptKey newKey = leftEntry.key.Concat(rightEntry.key.Remove(commVarsIndicesInRight));
                    newCptEntries[cptIndex] = new CptEntry(newKey, leftEntry.value * rightEntry.value);
                    cptIndex++;
                }
            }

            return new CPT(varsInResultCPT.ToArray(), newKeyPosMap, newCptEntries);
        }

        private void CategoriseEntries(CPT factor, int[] commVarsIndices, Dictionary<CptKey, List<CptEntry>> mapping)
        {
            foreach (CptEntry entry in factor.cpt)
            {
                CptKey key = entry.key.Extract(commVarsIndices);
                if (!mapping.ContainsKey(key))
                {
                    mapping.Add(key, new List<CptEntry>());
                }
                mapping[key].Add(entry);
            }
        }

        private Proposition[] RemoveIrrelevantEvidence(Proposition[] evidence)
        {
            HashSet<string> variables = new HashSet<string>(vars.Select(v => v.name));
            return evidence.Where(e => variables.Contains(e.name)).ToArray();
        }

        private void SetKeys(ArraySegment<CptKey> keys, ArraySegment<RandomVariable> remainingVars)
        {
            if (remainingVars.Count == 0)
            {
                return;
            }

            RandomVariable var = remainingVars.Array[remainingVars.Offset];
            remainingVars = new ArraySegment<RandomVariable>(remainingVars.Array, remainingVars.Offset + 1, remainingVars.Count - 1);

            ArraySegment<CptKey>[] tokenSegments = DivideIntoNSegments(keys, var.tokens.Length);
            for (int i = 0; i < tokenSegments.Length; i++)
            {
                ArraySegment<CptKey> segment = tokenSegments[i];
                for (int index = segment.Offset; index < segment.Offset + segment.Count; index++)
                {
                    CptKey key = segment.Array[index];
                    key.Set(keyPosMap[var], (char)i);
                }
                SetKeys(tokenSegments[i], remainingVars);
            }
        }

        private ArraySegment<CptKey>[] DivideIntoNSegments(ArraySegment<CptKey> keys, int n)
        {
            int segmentSize = keys.Count / n;
            ArraySegment<CptKey>[] segments = new ArraySegment<CptKey>[n];
            for (int i = 0; i < n; i++)
            {
                segments[i] = new ArraySegment<CptKey>(keys.Array, keys.Offset + segmentSize * i, segmentSize);
            }
            return segments;
        }

        private CptKey EvidenceToKey(Proposition[] evidence)
        {
            CptKey searchKey = new CptKey(GetKeySize(), true);
            foreach (Proposition e in evidence)
            {
                RandomVariable var = this.vars.Single(d => d.name.Equals(e.name));
                searchKey.Set(keyPosMap[var], (char)e.valueIndex);
            }

            return searchKey;
        }

        private CptEntry[] FilterCptByEvidence(Proposition[] evidence)
        {
            CptKey searchKey = EvidenceToKey(evidence);

            return cpt
                .Where(entry => entry.key.Match(searchKey))
                .ToArray();
        }

        private double[] Normalise(double[] distribution)
        {
            double sum = distribution.Aggregate<double, double>(0, (acc, v) => acc + v);
            if (Math.Abs(1 - sum) < 0.001)
            {
                return distribution;
            }
            else
            {
                return distribution.Select(v => v / sum).ToArray();
            }
        }

        protected int GetKeySize()
        {
            return cpt[0].key.size;
        }
        
        
        public override string ToString() {
            string result = "RandomVariables: ";
            
            result += string.Join(" ", vars.Select(v => v.name).ToArray());
            result += "\n";
            
            foreach(CptEntry entry in cpt) {
                result += entry.key + " " + entry.value + "\n";
            }
            
            return result;
        }
    }

    internal struct CptEntry
    {
        internal CptKey key;
        internal double value;
        internal CptEntry(CptKey key, double value)
        {
            this.key = key;
            this.value = value;
        }
    }

    internal struct Range
    {
        internal int start;
        internal int end;
        internal Range(int start, int end) {
            this.start = start;
            this.end = end;
        }
    }
}
