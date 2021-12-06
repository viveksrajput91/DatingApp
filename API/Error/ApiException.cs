using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Error
{
    public class ApiException
    {
        public int _statusCode { get; set; }
        
        public string _message { get; set; }
        
        public string _details { get; set; }
        
        public ApiException(int statusCode,string message=null,string details=null)
        {
            _statusCode=statusCode;
            _message=message;
            _details=details;
        }
    }
}