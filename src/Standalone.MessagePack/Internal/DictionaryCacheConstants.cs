﻿using System;

namespace MessagePack.Internal
{
    /// <summary>Recommended cache sizes, based on expansion policy of ConcurrentDictionary.
    /// Internal implementation of ConcurrentDictionary resizes to prime numbers (not divisible by 3 or 5 or 7)
    /// 31
    /// 67
    /// 137
    /// 277
    /// 557
    /// 1,117
    /// 2,237
    /// 4,477
    /// 8,957
    /// 17,917
    /// 35,837
    /// 71,677
    /// 143,357
    /// 286,717
    /// 573,437
    /// 1,146,877
    /// 2,293,757
    /// 4,587,517
    /// 9,175,037
    /// 18,350,077
    /// 36,700,157
    /// </summary>
    internal static class DictionaryCacheConstants
    {
        /// <summary>SIZE_SMALL: 67</summary>
        public const int SIZE_SMALL = 67;
        /// <summary>SIZE_MEDIUM: 1117</summary>
        public const int SIZE_MEDIUM = 1117;
        /// <summary>SIZE_LARGE: 143357</summary>
        public const int SIZE_LARGE = 143357;
        /// <summary>SIZE_X_LARGE: 2293757</summary>
        public const int SIZE_X_LARGE = 2293757;

        /// <summary>DefaultCacheCleanupFreq: 10 minutes</summary>
        public static readonly TimeSpan DefaultCacheCleanupFreq = TimeSpan.FromMinutes(10);
    }
}
