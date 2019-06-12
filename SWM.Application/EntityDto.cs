using System;
using System.Collections.Generic;
using System.Text;

namespace SWM.Application
{
    public class EntityDto<TPrimaryKey>
    {
        public TPrimaryKey Id { get; set; }
    }
}
