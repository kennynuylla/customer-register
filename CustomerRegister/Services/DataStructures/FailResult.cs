using System.Collections;
using System.Collections.Generic;
using Services.DataStructures.Interfaces;

namespace Services.DataStructures
{
    public record FailResult : IServiceResult
    {
        public bool IsSuccessful => false;
        public IEnumerable<string> Errors { get;  }

        public FailResult()
        {
            Errors = new[] {"Unable to perform operation;"};
        }

        public FailResult(IEnumerable<string> errors)
        {
            Errors = errors;
        }
    }
}