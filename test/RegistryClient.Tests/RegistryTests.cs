using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace RegistryClient.Tests
{
    public class RegistryTests
    {
        private Registry _registry;
        public RegistryTests()
        {
            _registry = new Registry();
        }

        [Fact]
        public async Task GetApiVersionTest()
        {
            var apiVersion = await _registry.GetApiVersionAsync();
            Assert.Equal(ApiVersion.v2, apiVersion);
        }

        public static IEnumerable<object[]> TagData
        {
            get
            {
                yield return new object[] {
                    "library/hello-world",
                    new[] { "latest",
                            "linux",
                            "nanoserver-1709",
                            "nanoserver-sac2016",
                            "nanoserver",
                            "nanoserver1709"
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(TagData))]
        public async Task GetTagsTest(string name, string[] expectedTags)
        {
            var tags = await _registry.GetTagsAsync(name);
            for(var i = 0; i < tags.Count; i++)
            {
                Assert.Equal(expectedTags[i], tags[i]);
            }
        }

        //TODO Test GetManifestAsync(string name, string reference);
        [Fact]
        public void GetManifestTest()
        {

        }
        //TODO Test GetManifestListAsync(string name, string reference);
        [Fact]
        public void GetManifestListTest()
        {

        }
    }
}
