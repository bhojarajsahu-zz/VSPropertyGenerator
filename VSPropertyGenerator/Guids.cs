// Guids.cs
// MUST match guids.h
using System;

namespace bhojarajsahu88.VSPropertyGenerator
{
    static class GuidList
    {
        public const string guidVSPropertyGeneratorPkgString = "1e14f965-0a8c-4870-b4e9-35bf3683a3a8";
        public const string guidVSPropertyGeneratorCmdSetString = "68e951cf-4693-478f-ba2e-4e6cf3601a1a";

        public static readonly Guid guidVSPropertyGeneratorCmdSet = new Guid(guidVSPropertyGeneratorCmdSetString);
    };
}