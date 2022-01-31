using System.Collections.Generic;

namespace NotionIntegrationLibrary.Interface
{
    public interface INotion
    {
        string CreatePage(string databaseId, NotionPageObject notionPageObject);
        string GetObjectId(string name);
        void Log(string databaseName, string logMessage);
        void Log(string logType, string operation, string logMessage, string additionalInfo = null);
        IEnumerable<NotionPageObject> QueryDatabase(string id);
        PageResponse RetrieveDatabase(string id);
        string UpdatePage(NotionPageObject notionPageObject);

        List<DatabaseHeader> GetAllDatabases();
    }
}