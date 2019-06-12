using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ShareWithMe.Models
{
    public class ResponseModel
    {
        public ResponseModel(
            string message = "",
            HttpStatusCode code = HttpStatusCode.OK,
            object data = null,
            bool success = true,
            List<string> errors = null,
            bool unauth = false
            )
        {
            Message = message;
            Code = code;
            Data = data;
            Success = success;
            Errors = errors;
            UnAuthorized = unauth;
        }
        public string Message { get; set; } = "";
        public HttpStatusCode Code { get; set; } = HttpStatusCode.OK;
        public object Data { get; set; } = null;
        public bool Success { get; set; } = true;
        public List<string> Errors { get; set; } = null;
        public bool UnAuthorized { get; set; } = false;
    }
}
