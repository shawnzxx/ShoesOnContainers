﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebMvc.Infrastructure
{
    public interface IHttpClient
    {
        Task<string> GetStringAsync(string uri);

        Task<HttpResponseMessage> PostAsync<T>(string uri, T item);

        Task<HttpResponseMessage> PutAsync<T>(string uri, T item);

        Task<HttpResponseMessage> DeleteAsync(string uri);
    }
}
