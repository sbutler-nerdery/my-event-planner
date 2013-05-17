using System.Data.Entity;
using Web.Data;
using Web.Data.Models;
using Web.Services;

[assembly: WebActivator.PreApplicationStartMethod(typeof(Web.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(Web.App_Start.NinjectWebCommon), "Stop")]

namespace Web.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
            
            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IEventService>().To<EventService>();
            kernel.Bind<INotificationService>().To<NotificationService>();
            kernel.Bind<IRepository<Event>>().To<EntityFrameworkRepository<Event>>().InRequestScope();
            kernel.Bind<IRepository<Person>>().To<EntityFrameworkRepository<Person>>().InRequestScope();
            kernel.Bind<IRepository<FoodItem>>().To<EntityFrameworkRepository<FoodItem>>().InRequestScope();
            kernel.Bind<IRepository<Game>>().To<EntityFrameworkRepository<Game>>().InRequestScope();
            kernel.Bind<IUserService>().To<UserService>();
            kernel.Bind<DbContext>().To<EventPlannerContext>().InRequestScope();
        }        
    }
}
