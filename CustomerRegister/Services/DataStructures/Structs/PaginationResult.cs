using System.Collections.Generic;
using Domain.Models.Interfaces;

namespace Services.DataStructures.Structs
{
    public struct PaginationResult<T> where T:IUuidModel
    {
        public PaginationData Pagination { get; set; }
        public IEnumerable<T> Elements { get; set; }
        public int Total { get; set; }
    }
}