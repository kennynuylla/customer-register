using System;
using System.IO;

namespace UnitTests.Base
{
    public abstract class DatabaseTestsBase : IDisposable
    {
        protected string DatabasePath;

        protected DatabaseTestsBase()
        {
            DatabasePath = Path.Combine(Environment.CurrentDirectory, $"{Guid.NewGuid()}.sqlite");
        }

        public virtual void Dispose()
        {
            if(File.Exists(DatabasePath)) File.Delete(DatabasePath);
        }
    }
}