using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NotionIntegrationLibrary.Interface;
using RestSharp;

namespace NotionIntegrationLibrary.Implementation
{
    public class NotionExtension : INotionExtension
    {
        public INotion NotionClient { get; set; }
        public NotionExtension(INotion notion)
        {
            NotionClient = notion;
        }
        public void SyncDatabase(string source, string target)
        {
            try
            {
                var source_databaseId = NotionClient.GetObjectId(source);
                var source_pageResponse_Results_b4_filter = NotionClient.QueryDatabase(source_databaseId);
                var source_pageResponse_Results = new List<NotionPageObject>();

                var target_databaseId = NotionClient.GetObjectId(target);
                var target_pageResponse_Results = NotionClient.QueryDatabase(target_databaseId);

                //Identify and get new rows
                IEnumerable<NotionPageObject> newRecords = null;
                IEnumerable<NotionPageObjectToUpdate> updatedRecords = new List<NotionPageObjectToUpdate>();

                //Filter records does not have "Name" property
                foreach (var row in source_pageResponse_Results_b4_filter)
                {
                    if (row.properties.ContainsKey("Name") || row.properties["Name"].Title.First().TextContent.First().Value != null)
                    {
                        source_pageResponse_Results.Add(row);
                    }
                }


                if (target_pageResponse_Results.Count() > 0)
                {
                    newRecords = source_pageResponse_Results.Where(a => !target_pageResponse_Results.Select(b => b.properties["Name"].Title.First().TextContent.First().Value).Contains(a.properties["Name"].Title.First().TextContent.First().Value));

                    //Identify and get updated rows
                    updatedRecords = (from source_record in source_pageResponse_Results
                                      join
                                       target_record in target_pageResponse_Results
                                      on source_record.properties["Name"].Title.First().TextContent.First().Value equals target_record.properties["Name"].Title.First().TextContent.First().Value
                                      where source_record.Last_Edited_Time > target_record.Last_Edited_Time
                                      select new NotionPageObjectToUpdate { Id = target_record.Id, NotionPageObject = source_record }).ToList<NotionPageObjectToUpdate>();


                }
                else
                {
                    //newRecords = (from source_record in source_pageResponse_Results
                    //              where source_record.properties["Name"].Title.First().TextContent.First().Value != string.Empty
                    //              select source_record);
                    newRecords = source_pageResponse_Results;
                }


                //Create new records
                ////foreach (var row in newRecords)
                //Parallel.ForEach<NotionPageObject>(newRecords, newRecord =>
                //{
                //    NotionClient.CreatePage(target_databaseId, newRecord);
                //}
                //);

                foreach (var row in newRecords)
                {
                    NotionClient.CreatePage(target_databaseId, row);
                }

                //Identify the dirty records
                //Set the Id field from Target

                //Update dirty records
                foreach (var row in updatedRecords)
                {
                    var updated_record = row.NotionPageObject;
                    updated_record.Id = row.Id;
                    NotionClient.UpdatePage(updated_record);
                }

            }
            catch (Exception ex)
            {
                NotionClient.Log("Sync Database", ex.Message + " " + ex.StackTrace);
            }

        }
        public List<DatabaseHeader> GetAllDatabases()
        {
            List<DatabaseHeader> databaseHeaders = new List<DatabaseHeader>();
            try
            {
                databaseHeaders = NotionClient.GetAllDatabases();

            }
            catch (Exception ex)
            {
                NotionClient.Log("Sync Database", ex.Message + " " + ex.StackTrace);
            }

            return databaseHeaders;
        }
    }
}

