using System;

namespace PdnCodeLab
{
    // This is for working around an issue in PDN 5.1 whereby the effect will dispose the
    // IServiceProvider it was given when IEffectInfo2.CreateInstance() was called. This
    // wrapper prevents that from happening. I (Rick) plan on fixing this for 5.1.1, but
    // not yet sure when that will get released.
    internal sealed class ServiceProviderWrapper
        : IServiceProvider
    {
        private readonly IServiceProvider services;

        public ServiceProviderWrapper(IServiceProvider services)
        {
            this.services = services;
        }

        public object GetService(Type serviceType)
        {
            return this.services.GetService(serviceType);
        }
    }
}
