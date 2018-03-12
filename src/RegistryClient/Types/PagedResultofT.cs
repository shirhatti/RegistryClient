using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace RegistryClient
{
    public class PagedResult<T>
    {
        public IList<T> Results { get; set; }
        public Uri NextPagedResultUri { get; set; }
    }
}
