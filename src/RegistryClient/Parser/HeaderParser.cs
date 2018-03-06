﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace RegistryClient.Parser
{
    static class HeaderParser
    {
        public static AuthenticationChallenge ParseBearerResponseChallenge (string header)
        {
            // TODO eliminate ugly regex
            Regex regex = new Regex("^(?:(?:[, ]+)?(?\'q\'\")?(?\'key\'[^=\"]*?)(?:\\k\'q\'(?\'-q\'))?=(?\'q\'\")?(?\'value\'(?:[^\"]|(?<=\\\\)\")*)(?:\\k\'q\'(?\'-q\'))?)*(?(q)(?!))$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(10));
            var match = regex.Match(header);
            //var keys = match.Groups["key"].Captures;
            var values = match.Groups["value"].Captures;
            var challenge = new AuthenticationChallenge()
            {
                Realm = new Uri(values[0].Value),
                Service = values[1].Value,
            };
            if (values.Count == 3)
            {
                challenge.Scope = values[2].Value;
            }
            return challenge;
        }
    }
}