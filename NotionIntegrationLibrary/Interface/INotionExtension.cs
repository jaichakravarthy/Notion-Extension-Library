using System.Collections.Generic;

namespace NotionIntegrationLibrary.Interface
{
    public interface INotionExtension
    {
        List<DatabaseHeader> GetAllDatabases();
        void SyncDatabase(string source, string target);
    }
}