using System.Collections.Generic;
using Domain.Interfaces;

namespace Services.DataStructures.Structs
{
    public struct PaginationResult<T> where T:IUuidModel
    {
        public PaginationData Pagination { get; set; }
        public IEnumerable<T> Elements { get; set; }
    }
}