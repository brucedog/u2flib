using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Repositories;
using Repositories.Context;
using Services;
using Unity.Mvc4;

namespace DemoU2FSite
{
    public static class Bootstrapper
    {
        public static IUnityContainer Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            return container;
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();
  
            RegisterTypes(container);

            return container;
        }

        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<IMemeberShipService, MemeberShipService>();
            container.RegisterType<IDataContext, DataContext>();
            container.RegisterType<IUserRepository, UserRepository>();
        }
    }
}