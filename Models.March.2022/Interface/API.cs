using Newtonsoft.Json;

using RestSharp;

using System.Diagnostics;
using System.Net;
using System.Xml;

namespace ShareInvest.Interface
{
    public class API
    {
        public async Task<object?> PostContextAsync<T>(T args) where T : class
        {
            try
            {
                var response = await client
                    .ExecuteAsync(new RestRequest($"api/{args.GetType().Name}", RestSharp.Method.POST)
                    .AddJsonBody(args), source.Token);

                if (HttpStatusCode.OK.Equals(response.StatusCode))
                    return args switch
                    {
                        Models.OpenAPI.Message => JsonConvert.DeserializeObject<Models.OpenAPI.Message>(response.Content),
                        _ => JsonConvert.DeserializeObject<Initialization>(response.Content)
                    };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }
        public async Task<object?> PostStructAsync<T>(T args) where T : struct
        {
            try
            {
                var response = await client
                    .ExecuteAsync(new RestRequest($"api/{args.GetType().Name}", RestSharp.Method.POST)
                    .AddJsonBody(JsonConvert.SerializeObject(args)), source.Token);

                if (HttpStatusCode.OK.Equals(response.StatusCode))
                    return args switch
                    {
                        File or _ => JsonConvert.DeserializeObject<File>(response.Content)
                    };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }
        public async Task<object?> GetContextAsync(string query)
        {
            try
            {
                var response = await client.ExecuteAsync(new RestRequest($"api/{query}", RestSharp.Method.GET), source.Token);

                if (HttpStatusCode.OK.Equals(response.StatusCode))
                    return query.Split('?')[0] switch
                    {
                        "securities" => JsonConvert.DeserializeObject<Admin>(response.Content),
                        "stock" => JsonConvert.DeserializeObject<string[]>(response.Content),
                        _ => JsonConvert.DeserializeObject<File>(response.Content)
                    };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }
        public async Task<object?> GetContextAsync(Models.Dart api, string? corp_code)
        {
            try
            {
                var crtfc_key = "";
                var url = string.Concat(nameof(api), '/', Enum.GetName(api)!.Replace('_', '.'), '?', nameof(crtfc_key), '=', crtfc_key, '&', nameof(corp_code), '=', corp_code);
                var response = await client.ExecuteAsync(new RestRequest(url, RestSharp.Method.GET), source.Token);

                if (HttpStatusCode.OK.Equals(response.StatusCode))
                    switch (api)
                    {
                        case Models.Dart.company_json:
                            return JsonConvert.DeserializeObject<Models.CompanyOverview>(response.Content);

                        case Models.Dart.corpCode_xml:
                            using (var stream = new MemoryStream(response.RawBytes))
                            {
                                var stack = new Stack<Models.CompanyOverview>();

                                using (var compress = new System.IO.Compression.ZipArchive(stream, System.IO.Compression.ZipArchiveMode.Read))
                                    foreach (var entry in compress.Entries)
                                        using (var sr = new StreamReader(entry.Open()))
                                        {
                                            var xml = new XmlDocument();
                                            xml.LoadXml(sr.ReadToEnd());

                                            foreach (XmlNode node in xml.GetElementsByTagName("list"))
                                                if (string.IsNullOrEmpty(node["stock_code"]?.InnerText))
                                                    continue;

                                                else
                                                    stack.Push(new Models.CompanyOverview
                                                    {
                                                        Code = node["stock_code"]?.InnerText,
                                                        CorpCode = node["corp_code"]?.InnerText,
                                                        CorpName = node["corp_name"]?.InnerText,
                                                        ModifyDate = node["modify_date"]?.InnerText,
                                                        Date = DateTime.Now
                                                    });
                                        }
                                if (stack.Count > 0)
                                    return stack;
                            }
                            break;
                    }
                else
                {
                    if (Condition.IsDebug)
                        Debug.WriteLine(response.Content);

                    else
                        Console.WriteLine(response.Content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }
        public async Task<Kakao.LocalAddress?> GetLocalAddressAsync(string query)
        {
            try
            {
                var res = await client
                    .ExecuteAsync(new RestRequest($"v2/local/search/address?query={query}", RestSharp.Method.GET)
                    .AddHeader("Authorization", Properties.Resources.kakao_api_key), source.Token);

                if (HttpStatusCode.OK.Equals(res.StatusCode))
                    return JsonConvert.DeserializeObject<Kakao.LocalAddress>(res.Content);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        public async Task PutContextAsync<T>(T arg) where T : class
        {
            try
            {
                if (HttpStatusCode.OK.Equals((await client
                    .ExecuteAsync(new RestRequest("api/stock", RestSharp.Method.PUT)
                    .AddJsonBody(arg), source.Token)).StatusCode))
                {

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        public API(string url)
        {
            client = new RestClient(url)
            {
                Timeout = -1
            };
            source = new CancellationTokenSource();
        }
        readonly CancellationTokenSource source;
        readonly IRestClient client;
    }
}