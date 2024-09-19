using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace HttpClientCreate.Controllers
{
    public class ClientService
    {
        private readonly HttpClient _httpClient;
        public ClientService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        public async Task<T> SendRequestAsync<T>(HttpRequestMessage request)
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.SendAsync(request);
            var Content = await httpResponseMessage.Content.ReadAsStringAsync();
            T result = JsonConvert.DeserializeObject<T>(Content);
            return result;
        }
        public async Task<string> SendRequestAsync(HttpRequestMessage request, string log, string password, string LoginApi)
        {
            HttpRequestMessage loginReq = new HttpRequestMessage(HttpMethod.Post, LoginApi);
            Credential credential = new Credential()
            {
                Username = log,
                Password = password
            };
            string Jsonbody = JsonConvert.SerializeObject(credential);
            loginReq.Content = new StringContent(Jsonbody, Encoding.UTF8, "application/json");
            var LoginResponse = await _httpClient.SendAsync(loginReq);
            string jsontoken = await LoginResponse.Content.ReadAsStringAsync();
            ResponseModelForall<Token>? tokens = JsonConvert.DeserializeObject<ResponseModelForall<Token>>(jsontoken);
            return tokens.Result.AccessToken;
        }

    }
}
public class ResponseModelForall<T>
{
    public ResponseModelForall(string error, int statusCode = 400)
    {
        Error = error;
        StatusCode = statusCode;

    }
    public ResponseModelForall(T result)
    {
        Error = null;
        Result = result;
    }
    public ResponseModelForall()
    {

    }
    public int StatusCode { get; set; }
    public string Error { get; set; }
    public T Result { get; set; }
}
public class Token
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}

