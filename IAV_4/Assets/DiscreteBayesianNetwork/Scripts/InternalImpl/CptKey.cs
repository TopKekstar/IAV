/*
 * Copyright (c) by Junjie Chen
 * Please refer to https://unity3d.com/legal/as_terms for the terms and conditions
 */

using System;
using System.Linq;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Tests")]
namespace Jackyjjc.Bayesianet
{
    internal class CptKey
    {
        internal static readonly char WILDCARD = char.MaxValue;

        internal char[] key;
        internal int size
        {
            get { return key.Length; }
        }

        internal CptKey(string input)
        {
            if (input.Length == 0)
            {
                this.key = new char[0];
            }
            else
            {
                this.key = input.Split(',').Select(k => k.Equals("?") ? WILDCARD : (char)int.Parse(k)).ToArray();
            }
        }

        internal CptKey(int keySize) {
			this.key = new char[keySize];
		}

        internal CptKey(int keySize, bool wildCard) : this(keySize) {
			if (wildCard) {
				for(int i = 0; i < keySize; i++) {
					key[i] = WILDCARD;
				}
			}
		}

        internal bool Match(CptKey searchKey) {
			if (searchKey.size != this.size) {
				throw new ArgumentException(string.Format("This key has length {0} but the key to match has length {1}", this.size, searchKey.size));
			}
			
			bool match = true;
			for(int i = 0; i < size; i++) {
				char c1 = key[i];
				char c2 = searchKey.key[i];
				if (c1 != WILDCARD && c2 != WILDCARD && c1 != c2) {
					match = false;
					break;
				}
			}
			
			return match;
		}

        internal CptKey Set(int i, char c) {
			if (i >= size) {
				throw new ArgumentOutOfRangeException();
            }
			
			this.key[i] = c;
            return this;
		}

        internal CptKey Remove(int index)
        {
            return Remove(new int[] { index });
        }

        internal CptKey Remove(params int[] indices) {
            if(indices.Any(i => i >= this.size)) {
                throw new ArgumentOutOfRangeException();
            }
            
            CptKey newKey = new CptKey(size - indices.Length);
			
			int newIndex = 0;
			for(int i = 0; i < size; i++) {
				if (Array.IndexOf(indices, i) != -1) {
					continue;
				}
				newKey.key[newIndex] = this.key[i];
				newIndex++;
			}
			
			return newKey;
		}

        internal CptKey Extract(params int[] indices) {
            if(indices.Any(i => i >= this.size)) {
                throw new ArgumentOutOfRangeException();
            }

			CptKey newKey = new CptKey(indices.Length);
			
			for(int i = 0; i < indices.Length; i++) {
				newKey.key[i] = this.key[indices[i]];
			}

			return newKey;
		}

        internal CptKey Concat(CptKey other) {
			CptKey newKey = new CptKey(this.size + other.size);
			
			Array.Copy(key, 0, newKey.key, 0, this.size);
			Array.Copy(other.key, 0, newKey.key, this.size, other.size);

			return newKey;
		}

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            CptKey otherKey = (CptKey)obj;
            return this.ToString().Equals(otherKey.ToString());
        }

        public override int GetHashCode()
        {
            return this.key.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return string.Join(",", key.Select(k => k == WILDCARD ? "?" : ((int)k).ToString()).ToArray());
        }
    }
}