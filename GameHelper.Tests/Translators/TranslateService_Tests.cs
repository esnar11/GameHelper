using System;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace GameHelper.Tests.Translators
{
    [Explicit]
    public class TranslateService_Tests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactory = new Mock<IHttpClientFactory>();

        [Test]
        public async Task Translate_Test()
        {
            _httpClientFactory
                .Setup(cf => cf.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient());

            throw new NotSupportedException();
        }
    }
}
