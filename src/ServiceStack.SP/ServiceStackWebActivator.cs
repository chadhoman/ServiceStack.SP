using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceStack.SP
{
    public class ServiceStackWebActivator
    {
        public static void Initialize()
        {
            ServiceStackAppHost.Start();
        }
    }
}
