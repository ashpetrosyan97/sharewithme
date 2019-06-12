using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using SWM.Core.Files;

namespace SWM.Application.Files
{
    public class UploadFileDto
    {
        public IFormFile File { get; set; }
        public string Name { get; set; }
        public long ParentId { get; set; }
        public string Uid { get; set; }
    }
}
