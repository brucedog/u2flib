using System.Web.Mvc;
using BaseLibrary;
using Services;
using Repositories;
using Microsoft.Practices.Unity;
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

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();    
            container.RegisterType<IDataContext, Repositories.Context.DataContext>();
            container.RegisterType<IMemeberShipService, MemeberShipService>();
            container.RegisterType<IUserRepository, UserRepository>();
            RegisterTypes(container);

          return container;
      }

      public static void RegisterTypes(IUnityContainer container)
    {
    
    }
  }
}