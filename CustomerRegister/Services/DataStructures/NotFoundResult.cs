using Services.DataStructures.Interfaces;

namespace Services.DataStructures
{
    public class NotFoundResult : IServiceResult
    {
        public bool IsSuccessful => false;
    }
}