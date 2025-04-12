using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemindMe.Models.ResponseModels
{
    public class BackendClientResponseResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } // For error states
        public T? Data { get; set; }
    }
}
