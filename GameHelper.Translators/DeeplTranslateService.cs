using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DeepL;
using GameHelper.Interfaces;

namespace GameHelper.Translators
{
    public class DeeplTranslateService : ITranslateService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DeeplTranslateService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<string> Translate(string value, CancellationToken cancellationToken = default)
        {
            //using var deepLClient = new DeepLClient("v2|8231d3a2-a81f-4ae2-a651-7b1fc4916555|aa6f42a67dbea54a7027970794d9452c");
            //var res = await deepLClient.TranslateAsync(new[] {value}, Language.English, cancellationToken: cancellationToken);
            //var array = res.ToArray();
            //return array.Select(a => a.Text).First();
            
            using var client = _httpClientFactory.CreateClient(nameof(DeeplTranslateService));
            //const string requestUri = "https://dict.deepl.com/english-russian/search?ajax=1&source=english&onlyDictEntries=1&translator=dnsof7h3k2lgh3gda&delay=300&jsStatus=0&kind=full&eventkind=keyup&forleftside=true";
            const string requestUri = "https://www2.deepl.com/jsonrpc";
            using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new StringContent(CreateBody(value), Encoding.UTF8, "application/json")
            };
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        private static string CreateBody(string value)
        {
            return $"{{\r\n    \"jsonrpc\": \"2.0\",\r\n    \"method\": \"LMT_handle_jobs\",\r\n    \"params\": {{\r\n        \"jobs\": [{{\r\n                \"kind\": \"default\",\r\n                \"raw_en_sentence\": \"{value}\",\r\n                \"raw_en_context_before\": [],\r\n                \"raw_en_context_after\": [],\r\n                \"preferred_num_beams\": 4,\r\n                \"quality\": \"fast\"\r\n            }}\r\n        ],\r\n        \"lang\": {{\r\n            \"user_preferred_langs\": [\"RU\", \"EN\"],\r\n            \"source_lang_user_selected\": \"EN\",\r\n            \"target_lang\": \"RU\"\r\n        }},\r\n        \"priority\": -1,\r\n        \"commonJobParams\": {{}}\r\n    }}\r\n}}";
            //return $"{{\r\n    \"jsonrpc\": \"2.0\",\r\n    \"method\": \"LMT_split_into_sentences\",\r\n    \"params\": {{\r\n        \"texts\": [\"{value}\"],\r\n        \"lang\": {{\r\n            \"lang_user_selected\": \"EN\",\r\n            \"user_preferred_langs\": [\"RU\", \"EN\"]\r\n        }}\r\n    }},\r\n    \"id\": 93930028\r\n}}\r\n";
        }
    }
}
