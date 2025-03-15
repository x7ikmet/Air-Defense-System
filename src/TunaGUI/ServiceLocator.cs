using Microsoft.Extensions.DependencyInjection;
using System;

namespace TunaGUI
{
    public static class ServiceLocator
    {
        public static T GetService<T>() where T : class
        {
            if (App.Services == null)
                throw new InvalidOperationException("Service provider is not initialized");
                
            return App.Services.GetRequiredService<T>();
        }
        
        public static object GetService(Type serviceType)
        {
            if (App.Services == null)
                throw new InvalidOperationException("Service provider is not initialized");
                
            return App.Services.GetRequiredService(serviceType);
        }
    }
}
