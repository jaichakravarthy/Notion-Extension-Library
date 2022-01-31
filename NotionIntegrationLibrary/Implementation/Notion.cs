using System;
using System.Collections.Generic;
using System.Configuration;
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
    public class Notion : INotion
    {
        private string Token { get; set; }
        public Notion()
        {
           Token =  ConfigurationManager.AppSettings.Get("Token");
        }
        public IEnumerable<NotionPageObject> QueryDatabase(String id)
        {
            PageResponse pageResponse = null;
            List<NotionPageObject> notionPageObjects = new List<NotionPageObject>();
            try
            {
                var internalIntegrationToken = Token;
                var client = new RestClient("https://api.notion.com/v1/databases/" + id + "/query");
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddParameter("Authorization", internalIntegrationToken, ParameterType.HttpHeader);
                request.AddParameter("Notion-Version", "2021-08-16", ParameterType.HttpHeader);

                request.RequestFormat = DataFormat.Json;

                request.AddJsonBody(new { page_size = 100 });

                IRestResponse response = client.Execute(request);
                if (response.StatusCode.ToString().ToLower() != "ok")
                {
                    Log("Create Page", response.Content);
                }

                pageResponse = JsonConvert.DeserializeObject<PageResponse>(response.Content);
                notionPageObjects.AddRange(pageResponse.Results);

                while (pageResponse.HasMore)
                {
                    request = new RestRequest(Method.POST);
                    request.AddHeader("cache-control", "no-cache");
                    request.AddHeader("content-type", "application/json");
                    request.AddParameter("Authorization", internalIntegrationToken, ParameterType.HttpHeader);
                    request.AddParameter("Notion-Version", "2021-08-16", ParameterType.HttpHeader);
                    request.RequestFormat = DataFormat.Json;
                    request.AddJsonBody(new { page_size = 100, start_cursor = pageResponse.Next_cursor });
                    response = client.Execute(request);
                    if (response.StatusCode.ToString().ToLower() != "ok")
                    {
                        Log("Create Page", response.Content);
                    }

                    pageResponse = JsonConvert.DeserializeObject<PageResponse>(response.Content);
                    notionPageObjects.AddRange(pageResponse.Results);
                }
            }
            catch (Exception ex)
            {
                Log("Sync Database", ex.Message + " " + ex.StackTrace);
            }

            return notionPageObjects;

        }

        public PageResponse RetrieveDatabase(String id)
        {
            PageResponse pageResponse = null;

            try
            {
                var internalIntegrationToken = Token;
                var client = new RestClient("https://api.notion.com/v1/databases/" + id);
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddParameter("Authorization", internalIntegrationToken, ParameterType.HttpHeader);
                request.AddParameter("Notion-Version", "2021-08-16", ParameterType.HttpHeader);

                request.RequestFormat = DataFormat.Json;

                IRestResponse response = client.Execute(request);
                if (response.StatusCode.ToString().ToLower() != "ok")
                {
                    Log("Create Page", response.Content);
                }

                pageResponse = JsonConvert.DeserializeObject<PageResponse>(response.Content);


            }
            catch (Exception ex)
            {
                Log("RetrieveDatabase", ex.Message + " " + ex.StackTrace);
            }

            return pageResponse;

        }
        public string CreatePage(string databaseId, NotionPageObject notionPageObject)
        {
            NotionPageObject newNotionPage = null;

            try
            {
                var internalIntegrationToken = Token;
                var client = new RestClient("https://api.notion.com/v1/pages");
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddParameter("Authorization", internalIntegrationToken, ParameterType.HttpHeader);
                request.AddParameter("Notion-Version", "2021-08-16", ParameterType.HttpHeader);

                request.RequestFormat = DataFormat.Json;

                var properties = new Dictionary<string, ColumnValue>();

                foreach (var item in notionPageObject.properties)
                {
                    var colValue = item.Value;
                    var allPropertiesNull = true;
                    foreach (PropertyInfo prop in colValue.GetType().GetProperties())
                    {

                        if (prop.GetValue(colValue) != null)//|| (prop.Name == "url" &&  prop.GetValue(colValue).ToString() != "\"\"") )
                        {
                            allPropertiesNull = false;
                            continue;
                        }

                    }
                    if (!allPropertiesNull)
                    {
                        properties.Add(item.Key, item.Value);
                    }
                }

                var json = JsonConvert.SerializeObject(new { parent = new { database_id = databaseId }, properties }, Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
                request.AddJsonBody(json, "application/json");

                IRestResponse response = client.Execute(request);
                if (response.StatusCode.ToString().ToLower() != "ok")
                {
                    Log("Error", "Create Page", response.Content, json);
                }

                newNotionPage = JsonConvert.DeserializeObject<NotionPageObject>(response.Content);


            }
            catch (Exception ex)
            {
                Log("Error", "Create Page", ex.Message + " " + ex.StackTrace);
            }

            return (newNotionPage == null) ? null : newNotionPage.Id;
        }
        public string UpdatePage(NotionPageObject notionPageObject)
        {
            NotionPageObject updatedNotionPage = null;

            try
            {
                var internalIntegrationToken = Token;
                var client = new RestClient("https://api.notion.com/v1/pages/" + notionPageObject.Id);
                var request = new RestRequest(Method.PATCH);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddParameter("Authorization", internalIntegrationToken, ParameterType.HttpHeader);
                request.AddParameter("Notion-Version", "2021-08-16", ParameterType.HttpHeader);

                request.RequestFormat = DataFormat.Json;

                //var json = JsonConvert.SerializeObject(new { parent = new { database_id = databaseId }, notionPageObject.properties });

                var json = JsonConvert.SerializeObject(new { notionPageObject.properties }, Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

                request.AddJsonBody(json, "application/json");

                IRestResponse response = client.Execute(request);
                if (response.StatusCode.ToString().ToLower() != "ok")
                {
                    Log("Error", "Update Page", response.Content, json);
                }

                updatedNotionPage = JsonConvert.DeserializeObject<NotionPageObject>(response.Content);
            }
            catch (Exception ex)
            {
                Log("Error", "Update Page", ex.Message + " " + ex.StackTrace);
            }
            return (updatedNotionPage == null) ? null : updatedNotionPage.Id;

        }

        public string GetObjectId(string name)
        {
            DatabaseResponse databaseResponse = null;
            try
            {
                var internalIntegrationToken = Token;
                var client = new RestClient("https://api.notion.com/v1/search");
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddParameter("Authorization", internalIntegrationToken, ParameterType.HttpHeader);
                request.AddParameter("Notion-Version", "2021-08-16", ParameterType.HttpHeader);


                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { query = name }); // Anonymous type object is converted to Json body


                IRestResponse response = client.Execute(request);
                if (response.StatusCode.ToString().ToLower() != "ok")
                {
                    Log("Create Page", response.Content);
                }

                databaseResponse = JsonConvert.DeserializeObject<DatabaseResponse>(response.Content);
            }
            catch (Exception ex)
            {
                Log("Sync Database", ex.Message + " " + ex.StackTrace);
            }
            return databaseResponse.Results[0].Id;

        }

        public void Log(string databaseName, string logMessage)
        {
            try
            {
                Notion notionConnect = new Notion();
                var databaseId = notionConnect.GetObjectId("Log");

                //Database - title
                //TimeStamp - Date
                //Description - Text

                var db_title_text_content = new Dictionary<string, string>();
                db_title_text_content.Add("content", databaseName);

                var text = new Text { TextContent = db_title_text_content };

                var db_title_text = new Dictionary<string, Text>();
                db_title_text.Add("text", text);

                var db_Desc_text_content = new Dictionary<string, string>();
                db_Desc_text_content.Add("content", logMessage);

                var desc = new Text { TextContent = db_Desc_text_content };

                //var titleObj = new TitleObj { Title = db_title_text };

                //var db_titleObj = new Dictionary<string, TitleObj>();
                //db_titleObj.Add("title", titleObj);

                var colDatabaseValue = new ColumnValue { Title = new List<Text>() { text } };
                var colDescValue = new ColumnValue { RichText = new List<Text> { desc } };
                var colTimeStampValue = new ColumnValue { DateObj = new DateObj { startDate = DateTime.Now } };

                var log_properties = new Dictionary<string, ColumnValue>();
                log_properties.Add("Operation", colDatabaseValue);
                log_properties.Add("Description", colDescValue);
                log_properties.Add("TimeStamp", colTimeStampValue);

                var newNotionPageObject = new NotionPageObject
                {
                    properties = log_properties

                };

                notionConnect.CreatePage(databaseId, newNotionPageObject);
            }
            catch (Exception ex)
            {
                Log("Sync Database", ex.Message + " " + ex.StackTrace);
            }

        }

        public void Log(string logType, string operation, string logMessage, string additionalInfo = null)
        {
            try
            {

                Notion notionConnect = new Notion();
                var databaseId = notionConnect.GetObjectId("Log");

                //Database - title
                //TimeStamp - Date
                //Description - Text

                var db_title_text_content = new Dictionary<string, string>();
                db_title_text_content.Add("content", operation);

                var text = new Text { TextContent = db_title_text_content };

                var db_title_text = new Dictionary<string, Text>();
                db_title_text.Add("text", text);

                var db_Desc_text_content = new Dictionary<string, string>();
                db_Desc_text_content.Add("content", logMessage);

                var db_Desc_type_content = new Dictionary<string, string>();
                db_Desc_type_content.Add("content", logType);

                var db_Desc_additionalInfo_content = new Dictionary<string, string>();
                db_Desc_additionalInfo_content.Add("content", additionalInfo);

                var desc = new Text { TextContent = db_Desc_text_content };
                var type = new Text { TextContent = db_Desc_type_content };
                var info = new Text { TextContent = db_Desc_additionalInfo_content };

                //var titleObj = new TitleObj { Title = db_title_text };

                //var db_titleObj = new Dictionary<string, TitleObj>();
                //db_titleObj.Add("title", titleObj);
                var colTypeValue = new ColumnValue { RichText = new List<Text> { type } };

                var colDatabaseValue = new ColumnValue { Title = new List<Text>() { text } };
                var colDescValue = new ColumnValue { RichText = new List<Text> { desc } };
                var colTimeStampValue = new ColumnValue { DateObj = new DateObj { startDate = DateTime.Now } };
                var colInfoValue = new ColumnValue { RichText = new List<Text> { info } };


                var log_properties = new Dictionary<string, ColumnValue>();
                log_properties.Add("Operation", colDatabaseValue);
                log_properties.Add("Description", colDescValue);
                log_properties.Add("TimeStamp", colTimeStampValue);
                log_properties.Add("Type", colTypeValue);
                log_properties.Add("Additional Information", colInfoValue);

                var newNotionPageObject = new NotionPageObject
                {
                    properties = log_properties

                };

                notionConnect.CreatePage(databaseId, newNotionPageObject);
            }
            catch (Exception ex)
            {
                Log("Sync Database", ex.Message + " " + ex.StackTrace);
            }

        }

        public List<DatabaseHeader> GetAllDatabases()
        {
            List<DatabaseHeader> databaseHeaders = new List<DatabaseHeader>();
            try
            {
                var internalIntegrationToken = Token;
                var client = new RestClient("https://api.notion.com/v1/search");
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddParameter("Authorization", internalIntegrationToken, ParameterType.HttpHeader);
                request.AddParameter("Notion-Version", "2021-08-16", ParameterType.HttpHeader);
                //request.AddParameter("start_cursor", "", ParameterType.HttpHeader);
                request.AddJsonBody(new { page_size = 100 });
                request.RequestFormat = DataFormat.Json;

                IRestResponse response = client.Execute(request);
                if (response.StatusCode.ToString().ToLower() != "ok")
                {
                    Log("Create Page", response.Content);
                }

                DatabaseResponse databaseResponse = JsonConvert.DeserializeObject<DatabaseResponse>(response.Content);

                foreach (var item in databaseResponse.Results)
                {
                    if (item.Objecttype == "database")
                    {
                        databaseHeaders.Add(new DatabaseHeader { Id = item.Id, Name = item.Title[0].TextContent.First().Value });
                    }
                }

                
                while (databaseResponse.HasMore)
                {
                    //request.AddOrUpdateParameter("start_cursor", databaseResponse.Next_cursor, ParameterType.HttpHeader);
                    request = new RestRequest(Method.POST);
                    request.AddHeader("cache-control", "no-cache");
                    request.AddHeader("content-type", "application/json");
                    request.AddParameter("Authorization", internalIntegrationToken, ParameterType.HttpHeader);
                    request.AddParameter("Notion-Version", "2021-08-16", ParameterType.HttpHeader);
                    request.RequestFormat = DataFormat.Json;
                    request.AddJsonBody(new { page_size = 100, start_cursor = databaseResponse.Next_cursor });
                    response = client.Execute(request);
                    if (response.StatusCode.ToString().ToLower() != "ok")
                    {
                        Log("Create Page", response.Content);
                    }

                    databaseResponse = JsonConvert.DeserializeObject<DatabaseResponse>(response.Content);
                    foreach (var item in databaseResponse.Results)
                    {
                        if (item.Objecttype == "database")
                        {
                            databaseHeaders.Add(new DatabaseHeader { Id = item.Id, Name = item.Title[0].TextContent.First().Value });
                        }
                    }
                }



            }
            catch (Exception ex)
            {
                Log("Sync Database", ex.Message + " " + ex.StackTrace);
            }

            return databaseHeaders;
        }
    }
}

