﻿using Services.DataStructures.Interfaces;

namespace Services.DataStructures
{
    public record SuccessResult<T> : IServiceResult
    {
        public bool IsSuccessful => true;
        public T Result { get; init; }
    }
    
    public record SuccessResult: IServiceResult
    {
        public bool IsSuccessful => true;
    }
}