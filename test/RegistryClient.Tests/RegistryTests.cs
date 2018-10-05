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
                    "shirhatti/registryclienttests",
                    new[] { "linux",
                            "nanoserver-1709",
                            "nanoserver-1803",
                            "nanoserver-sac2016",
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

        [Theory]
        [InlineData("shirhatti/registryclienttests", "linux", "sha256:1a6fd470b9ce10849be79e99529a88371dff60c60aab424c077007f6979b4812")]
        public async Task GetDigestFromTagTestAsync(string name, string reference, string expectedDigest)
        {
            var digest = await _registry.GetDigestFromTagAsync(name, reference);
            Assert.Equal(expectedDigest, digest);
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
