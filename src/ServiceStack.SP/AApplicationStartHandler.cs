using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ServiceStack.SP
{
    public abstract class AApplicationStartHandler : IHttpModule
    {

        private readonly static object _mutex = new object();
        private static bool _initialized = false;

        protected abstract void OnStart();

        public void Init(HttpApplication application)
        {
            if (!_initialized)
                lock (_mutex)
                    if (!_initialized)
                        Application_Start();

        }

        private void Application_Start()
        {
            _initialized = true;
            OnStart();
        }

        public void Dispose() { }
    }
}
