using Newtonsoft.Json;
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
                    new[] { "latest",
                            "linux",
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

        public static IEnumerable<object[]> ManifestData
        {
            get
            {
                yield return new object[] {
                    "shirhatti/registryclienttests",
                    "linux",
                    new Manifest()
                    {
                        SchemaVersion = 2,
                        MediaType = "application/vnd.docker.distribution.manifest.v2+json",
                        Config = new Config()
                        {
                            MediaType = "application/vnd.docker.container.image.v1+json",
                            Size = 1510,
                            Digest = "sha256:4ab4c602aa5eed5528a6620ff18a1dc4faef0e1ab3a5eddeddb410714478c67f"
                        },
                        Layers = new Config[]
                        {
                            new Config()
                            {
                                MediaType = "application/vnd.docker.image.rootfs.diff.tar.gzip",
                                Size = 977,
                                Digest = "sha256:d1725b59e92d6462c6d688ef028979cc6bb150762db99d18dddc7fa54b82b0ce"
                            }
                        }
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(ManifestData))]
        public async Task GetManifestTest(string name, string reference, Manifest expectedManifest)
        {
            var manifest = await _registry.GetManifestAsync(name, reference);
            Assert.Equal(JsonConvert.SerializeObject(expectedManifest), JsonConvert.SerializeObject(manifest));
        }

        public static IEnumerable<object[]> ManifestListData
        {
            get
            {
                yield return new object[] {
                    "shirhatti/registryclienttests",
                    "latest",
                    new ManifestList()
                    {
                        SchemaVersion = 2,
                        MediaType = "application/vnd.docker.distribution.manifest.list.v2+json",
                        Manifests = new ManifestSummary[]
                        {
                            new ManifestSummary()
                            {
                                MediaType = "application/vnd.docker.distribution.manifest.v2+json",
                                Size = 524,
                                Digest = "sha256:1a6fd470b9ce10849be79e99529a88371dff60c60aab424c077007f6979b4812",
                                Platform = new Platform()
                                {
                                    Architecture = "amd64",
                                    Os = "linux"
                                }
                            },
                            new ManifestSummary()
                            {
                                MediaType = "application/vnd.docker.distribution.manifest.v2+json",
                                Size = 1357,
                                Digest = "sha256:2be344b1e25598202a3fb40486fa3ae3392314acef9d3defcede09bb002bcf63",
                                Platform = new Platform()
                                {
                                    Architecture = "amd64",
                                    Os = "windows",
                                    OsVersion = "10.0.16299.611"
                                }
                            },
                            new ManifestSummary()
                            {
                                MediaType = "application/vnd.docker.distribution.manifest.v2+json",
                                Size = 1357,
                                Digest = "sha256:670f0dc439e4669bc602cc571af9f72271ea3b39002416fe2398feb36a7ac20d",
                                Platform = new Platform()
                                {
                                    Architecture = "amd64",
                                    Os = "windows",
                                    OsVersion = "10.0.17134.228"
                                }
                            },
                            new ManifestSummary()
                            {
                                MediaType = "application/vnd.docker.distribution.manifest.v2+json",
                                Size = 1359,
                                Digest = "sha256:413f47f775b4553db0ce5de808b04cc4a0cb24e69ee38cfe78db1cda94532a33",
                                Platform = new Platform()
                                {
                                    Architecture = "amd64",
                                    Os = "windows",
                                    OsVersion = "10.0.14393.2430"
                                }
                            }
                        }
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(ManifestListData))]
        public async Task GetManifestListTest(string name, string reference, ManifestList expectedManifestList)
        {
            var manifestList = await _registry.GetManifestListAsync(name, reference);
            Assert.Equal(JsonConvert.SerializeObject(expectedManifestList), JsonConvert.SerializeObject(manifestList));
        }
    }
}
