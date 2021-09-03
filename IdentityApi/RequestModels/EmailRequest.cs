using System;
using System.Collections.Generic;

namespace IdentityApi.RequestModels
{
    public class EmailRequest
    {
        public List<string> TO { get; set; } = new();
        public string SUBJECT { get; set; }
        public string CONTENT { get; set; }
        public bool HTML { get; set; }
    }
}
