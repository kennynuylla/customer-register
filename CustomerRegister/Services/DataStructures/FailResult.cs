using System.Collections.Generic;
using Services.DataStructures.Interfaces;

namespace Services.DataStructures
{
    public record FailResult : IServiceResult
    {
        public bool IsSuccessful => false;
        public IEnumerable<string> Errors { get; init; }
    }
}