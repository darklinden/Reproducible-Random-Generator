using System.Collections.Generic;

// convert from https://github.com/lordofduct/spacepuppy-unity-framework-3.0/blob/18ce7778875037ea72c1a5df1e5ee79f02688411/SpacepuppyUnityFramework/Utils/RandomUtil.cs

/**
https://zh.wikipedia.org/zh-cn/%E7%BD%AE%E6%8D%A2%E5%90%8C%E4%BD%99%E7%94%9F%E6%88%90%E5%99%A8
置换同余生成器，简称PCG（英语：Permuted congruential generator）是一个用于产生伪随机数的算法，开发于2014年。
该算法在线性同余生成器（LCG）的基础上增加了输出置换函数（output permutation function），以此优化LCG算法的统计性能。
因此，PCG算法在拥有出色的统计性能的同时[1][2]，也拥有LCG算法代码小、速度快、状态小的特性。[3]

置换同余生成器（PCG）和线性同余生成器（LCG）的差别有三点，在于：
    * LCG的模数以及状态大小比较大，状态大小一般为输出大小的二倍。
    * PCG的核心使用2的N次幂作为模数，以此实现一个非常高效的全周期、无偏差的伪随机数生成器，
    * PCG的状态不会被直接输出，而是经过输出置换函数计算后才输出。
    * 使用2的N次幂作为模数的LCG算法，普遍出现输出低位周期短小的问题，而PCG通过输出置换函数解决了这个问题。

https://www.pcg-random.org/

 */

namespace ReproducibleRandomGenerator
{
    public class SimplePCG : IRandomProvider
    {
        #region Fields
        private ulong _start_seed;
        private ulong _inc = 1;

        private List<RandomCache> _cache = new List<RandomCache>(32);

        #endregion

        #region CONSTRUCTOR
        public SimplePCG(ulong inc = 1) { _inc = inc; }
        #endregion

        #region Methods

        public void SetSeed(ulong seed)
        {
            _start_seed = seed;
            _cache.Clear();
        }

        public RandomCache Random(int index)
        {
            if (index < _cache.Count)
            {
                return _cache[index];
            }

            if (_cache.Count == 0)
            {
                ulong old = _start_seed;
                float rand = this.Next(old, out var new_seed);
                _cache.Add(new RandomCache()
                {
                    index = 0,
                    value = rand,
                    seed = new_seed
                });
            }

            for (int i = _cache.Count - 1; i <= index; i++)
            {
                var last = _cache[i];
                float rand = this.Next(last.seed, out var new_seed);
                _cache.Add(new RandomCache()
                {
                    index = i,
                    value = rand,
                    seed = new_seed
                });
            }

            return _cache[index];
        }

        public float Random()
        {
            return this.Random(_cache.Count).value;
        }

        private uint GetNext(ulong old_seed, out ulong new_seed)
        {
            new_seed = old_seed * 6364136223846793005 + _inc;

            uint xor = (uint)(((old_seed >> 18) ^ old_seed) >> 27);
            int rot = (int)(old_seed >> 59);
            uint ret = (xor >> rot) | (xor << (64 - rot));

            return ret;
        }

        #endregion

        #region IRandom Interface

        public double NextDouble(ulong old_seed, out ulong new_seed)
        {
            return (double)this.GetNext(old_seed, out new_seed) / (double)(0x100000000u);
        }

        public float Next(ulong old_seed, out ulong new_seed)
        {
            return UnityEngine.Mathf.Clamp01((float)(NextDouble(old_seed, out new_seed)));
        }

        #endregion
    }
}