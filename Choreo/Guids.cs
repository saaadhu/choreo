// Guids.cs
// MUST match guids.h
using System;

namespace Choreo
{
    static class GuidList
    {
        public const string guidChoreoPkgString = "ff1d0d69-7fa3-43fe-a41e-65c15297a351";
        public const string guidChoreoCmdSetString = "77ab6c12-3af4-43db-98df-e3969a5e9511";
        public const string guidToolWindowPersistanceString = "361e8700-d96a-482d-8666-02f4ada76689";

        public static readonly Guid guidChoreoCmdSet = new Guid(guidChoreoCmdSetString);
    };
}