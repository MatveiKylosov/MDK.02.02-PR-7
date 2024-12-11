using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;

namespace Kylosov
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private UsersResponse _usersResponse;
        private string _token;
        private string _url;

        public ApiClient()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:133.0) Gecko/20100101 Firefox/133.0");
            _httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");
            _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            _httpClient.DefaultRequestHeaders.Add("DNT", "1");
            System.Net.ServicePointManager.Expect100Continue = false;
        }

        public async Task<string> AuthenticateAsync(string username, string password, string url)
        {
            _url = url;
            var loginUrl = $"{_url}/api/admin/token";
            var content = new MultipartFormDataContent
            {
                { new StringContent(username), "username" },
                { new StringContent(password), "password" },
                { new StringContent("password"), "grant_type" }
            };

            try
            {
                var response = await _httpClient.PostAsync(loginUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    return $"Ошибка: {response.StatusCode} - {response.ReasonPhrase}";
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(jsonResponse);
                _token = tokenResponse.AccessToken;

                await GetUserDataAsync();
                return "Авторизация успешна";
            }
            catch (HttpRequestException ex)
            {
                return $"Ошибка в запросе: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"Неизвестная ошибка: {ex.Message}";
            }
        }

        public async Task<string> GetUserDataAsync()
        {
            if (string.IsNullOrEmpty(_token))
            {
                return "Ошибка: Необходима авторизация.";
            }

            var requestUrl = $"{_url}/api/users?limit=10&sort=-created_at";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            try
            {
                var response = await _httpClient.GetAsync(requestUrl);
                if (!response.IsSuccessStatusCode)
                {
                    return $"Ошибка: {response.StatusCode} - {response.ReasonPhrase}";
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                _usersResponse = JsonConvert.DeserializeObject<UsersResponse>(jsonResponse);

                foreach (var user in _usersResponse.Users)
                {
                    Console.WriteLine($"Username: {user.Username}");
                    if (user.Links != null && user.Links.Count > 0)
                    {
                        Console.WriteLine($"Link: {user.Links[0]}");
                    }
                }

                return "Данные о пользователях успешно получены";
            }
            catch (HttpRequestException ex)
            {
                return $"Ошибка в запросе: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"Неизвестная ошибка: {ex.Message}";
            }
        }

        public async Task<string> AddUserAsync(string username)
        {
            if (string.IsNullOrEmpty(_token))
            {
                return "Ошибка: Необходима авторизация.";
            }

            var requestUrl = $"{_url}/api/user";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var userData = new
            {
                status = "active",
                username = username,
                note = "",
                proxies = new
                {
                    vless = new { flow = "" }
                },
                data_limit = 0,
                expire = (string)null,
                data_limit_reset_strategy = "no_reset",
                inbounds = new
                {
                    vless = new[] { "VLESS TCP REALITY" },
                    shadowsocks = new[] { "Shadowsocks TCP" }
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(userData), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(requestUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    return $"Ошибка: {response.StatusCode} - {response.ReasonPhrase}";
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                return $"Новый пользователь {username} успешно добавлен: {jsonResponse}";
            }
            catch (HttpRequestException ex)
            {
                return $"Ошибка в запросе: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"Неизвестная ошибка: {ex.Message}";
            }
        }
    }
}