﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceStack.SP
{
    [Obsolete]
    public class ServiceStackHandler : AApplicationStartHandler
    {
        protected override void OnStart()
        {
            ServiceStackAppHost.Start();
        }
    }
}
