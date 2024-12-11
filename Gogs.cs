using System;
using System.Net.Http;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Kylosov
{
    public class Gogs
    {
        private readonly string _url = @"http://git.permaviat.ru/";
        private readonly string _urlLogin = @"http://git.permaviat.ru/user/login?redirect_to=";
        private readonly string _username;
        private readonly string _password;

        private string _lang;
        private string _iLikeGogs;
        private string _csrfToken;

        public Gogs(string username, string password)
        {
            _username = username;
            _password = password;
        }

        private async Task<bool> GetCookies()
        {
            using (var client = new HttpClient())
            {
                
                var request = new HttpRequestMessage(HttpMethod.Get, _urlLogin);

                
                request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:133.0) Gecko/20100101 Firefox/133.0");
                request.Headers.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                request.Headers.AcceptLanguage.ParseAdd("ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");
                request.Headers.AcceptEncoding.ParseAdd("gzip, deflate, br");
                request.Headers.Add("DNT", "1");
                request.Headers.Connection.ParseAdd("keep-alive");
                request.Headers.Add("Upgrade-Insecure-Requests", "1");
                request.Headers.Add("Priority", "u=0, i");

                try
                {
                    
                    var response = await client.SendAsync(request);

                    
                    response.EnsureSuccessStatusCode();

                    
                    var content = await response.Content.ReadAsStringAsync();

                    
                    Debug.WriteLine("Response received successfully");

                    
                    if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
                    {
                        foreach (var cookie in cookies)
                        {
                            
                            if (cookie.StartsWith("lang="))
                            {
                                _lang = GetCookieValue(cookie); 
                                Debug.WriteLine($"!lang cookie: {_lang}");
                            }
                            else if (cookie.StartsWith("i_like_gogs="))
                            {
                                _iLikeGogs = GetCookieValue(cookie); 
                                Debug.WriteLine($"!i_like_gogs cookie: {_iLikeGogs}");
                            }
                            else if (cookie.StartsWith("_csrf="))
                            {
                                _csrfToken = GetCookieValue(cookie); 
                                Debug.WriteLine($"!_csrf cookie: {_csrfToken}");
                            }
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("An error occurred: " + ex.Message);
                    return false;
                }
            }
        }

        public async Task<bool> AuthorizationAsync()
        {
            if(!await GetCookies())
            {
                return false;
            }
            var baseAddress = new Uri("http://localhost");
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:133.0) Gecko/20100101 Firefox/133.0");
                client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                client.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                client.DefaultRequestHeaders.Add("DNT", "1");
                client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                client.DefaultRequestHeaders.Add("Priority", "u=0, i");

                
                string formData = $"_csrf={_csrfToken}&user_name={_username}&password={_password}";

                
                var content = new StringContent(formData, Encoding.UTF8, "application/x-www-form-urlencoded");

                cookieContainer.Add(baseAddress, new Cookie("lang",       $"{_lang}"));
                cookieContainer.Add(baseAddress, new Cookie("i_like_gogs", $"{_iLikeGogs}"));
                cookieContainer.Add(baseAddress, new Cookie("_csrf",      $"{_csrfToken}"));

                
                var result = await client.PostAsync("/user/login", content);

                
                result.EnsureSuccessStatusCode();

                
                string responseBody = await result.Content.ReadAsStringAsync();

                
                var statusCode = result.StatusCode;

                
                var reasonPhrase = result.ReasonPhrase;

                
                Debug.WriteLine($"Response Status Code: {statusCode}");
                Debug.WriteLine($"Response Reason Phrase: {reasonPhrase}");
                Debug.WriteLine($"Response Body: {responseBody}");
            }
            return false;
        }

        
        private string GetCookieValue(string cookie)
        {
            
            var value = cookie.Split(';').FirstOrDefault();
            if (value.Contains('='))
            {
                return value.Split('=')[1];
            }
            return string.Empty;
        }
    }
}
