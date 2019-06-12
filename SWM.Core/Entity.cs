using System;
using System.Collections.Generic;
using System.Text;

namespace SWM.Core
{
    public class Entity<TPrimaryKey>
    {
        public TPrimaryKey Id { get; set; }
    }
}
