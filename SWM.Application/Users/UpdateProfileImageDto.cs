using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWM.Application.Users
{
    public class UpdateProfileImageDto : EntityDto<long>
    {
        public IFormFile Img { get; set; }
    }
}
