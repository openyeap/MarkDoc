using System;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Configuration;
using DotLiquid;

namespace Bzway.Writer.App
{
    public static class Extensions
    {
        public static T Get<T>(this Hash hash, string key, T @default)
        {
            var obj = hash.Get<T>(key);
            if (obj == null)
            {
                return @default;
            }
            return obj;
        }
    }
}