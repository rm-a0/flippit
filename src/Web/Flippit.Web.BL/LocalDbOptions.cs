using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flippit.Web.BL
{
    public record LocalDbOptions
    {
        public bool IsLocalDbEnabled { get; set; }
    }
}
