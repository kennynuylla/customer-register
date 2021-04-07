namespace Services.DataStructures.Structs
{
    public struct PaginationData
    {
        public int CurrentPage { get; set; }
        public int PerPage { get; set; }
        public int Total { get; set; }
    }
}