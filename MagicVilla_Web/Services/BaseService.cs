using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace MagicVilla_Web.Services
{
    public class BaseService : IBaseService
    {
        public APIRespone ResponeModel { get; set; }
        public IHttpClientFactory httpClient { get; set; }
        public BaseService(IHttpClientFactory httpClient)
        {
            this.ResponeModel = new();
            this.httpClient = httpClient;
        }

        public async Task<T> SendAsync<T>(APIRequest apiRequest)
        {
            try
            {
                var client = httpClient.CreateClient("MagicAPI");
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(apiRequest.Url);
                if (apiRequest.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data),
                        Encoding.UTF8, "application/json");
                }
                switch (apiRequest.ApiType)
                {
                    case SD.ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case SD.ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case SD.ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                HttpResponseMessage apiRespone = null;

                if (!string.IsNullOrEmpty(apiRequest.Token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiRequest.Token);
                }
                apiRespone = await client.SendAsync(message);

                var apiContent = await apiRespone.Content.ReadAsStringAsync();
                try
                {
                    APIRespone ApiRespone = JsonConvert.DeserializeObject<APIRespone>(apiContent);
                    if(ApiRespone != null && (apiRespone.StatusCode==HttpStatusCode.BadRequest || apiRespone.StatusCode == HttpStatusCode.NotFound))
                    {
                        ApiRespone.StatusCode = HttpStatusCode.BadRequest;
                        ApiRespone.IsSuccess = false;
                        var res = JsonConvert.SerializeObject(ApiRespone);
                        var returnObj = JsonConvert.DeserializeObject<T>(res);
                        return returnObj;
                    }
                }
                catch(Exception e)
                {
                    var exceptionRespone = JsonConvert.DeserializeObject<T>(apiContent);
                    return exceptionRespone;
                }
                var APIRespone = JsonConvert.DeserializeObject<T>(apiContent);
                return APIRespone;

            }
            catch (Exception ex)
            {
                var dto = new APIRespone
                {
                    ErrorMessages = new List<string> { Convert.ToString(ex.Message) },
                    IsSuccess = false
                };
                var res = JsonConvert.SerializeObject(dto);
                var APIRespone = JsonConvert.DeserializeObject<T>(res);
                return APIRespone;
            }
        }
    }
}
