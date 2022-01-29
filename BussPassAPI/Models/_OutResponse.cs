using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BussPassAPI.Models
{
    public class _OutResponseData
    {
        public int code { get; set; }
        public string message { get; set; }
        public List<object> data { get; set; }
    }
    public class _OutResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }
}