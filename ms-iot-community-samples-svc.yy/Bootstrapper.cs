namespace ms_iot_community_samples_svc
{
    using Autofac;
    using Nancy;
    using Nancy.Bootstrapper;
    using Nancy.Bootstrappers.Autofac;
    using Nancy.Conventions;
    using Nancy.Session;
    using Nancy.TinyIoc;

    public class Bootstrapper : AutofacNancyBootstrapper
    {
        // The bootstrapper enables you to reconfigure the composition of the framework,
        // by overriding the various methods and properties.
        // For more information https://github.com/NancyFx/Nancy/wiki/Bootstrapper
        // Nancy.Bootstrappers.Autofac

        ////https://github.com/nancyfx/nancy.bootstrappers.autofac
        ////http://stackoverflow.com/questions/17325840/registering-startup-class-in-nancy-using-autofac-bootstrapper

        protected override void ApplicationStartup(ILifetimeScope container, IPipelines pipelines)
        {
            StaticConfiguration.DisableErrorTraces = false;
            // No registrations should be performed in here, however you may
            // resolve things that are needed during application startup.
            CookieBasedSessions.Enable(pipelines, Nancy.Cryptography.CryptographyConfiguration.Default);
        }


        protected override void ConfigureApplicationContainer(ILifetimeScope existingContainer)
        {
            // Perform registration that should have an application lifetime
        }


        protected override void RequestStartup(ILifetimeScope container, IPipelines pipelines, NancyContext context)
        {
            // No registrations should be performed in here, however you may
            // resolve things that are needed during request startup.
        }

        protected override void ConfigureRequestContainer(ILifetimeScope container, NancyContext context)
        {
            // // Perform registrations that should have a request lifetime
            container.Update(builder =>
            {
                builder.RegisterType<IndexModule>();
            });
        }

        //public class CustomRootPathProvider : IRootPathProvider
        //{
        //    public string GetRootPath()
        //    {
        //        return @"C:\inetpub\wwwroot";

        //    }
        //}

        //protected override IRootPathProvider RootPathProvider
        //{
        //    get { return new CustomRootPathProvider(); }
        //}


        //https://github.com/NancyFx/Nancy/wiki/Managing-static-content
        ////protected override void ConfigureConventions(NancyConventions conventions)
       // {
            //    base.ConfigureConventions(conventions);
            //
            //Conventions.StaticContentsConventions.Add(
            //       Nancy.Conventions.StaticContentConventionBuilder.AddDirectory("/Json")
            //       );
            //
            //     nancyConventions.StaticContentsConventions.Add( 
            //           Nancy.Conventions.StaticContentConventionBuilder.AddFile("/Default.html", "Default.html") 
            //         ); 


            //    conventions.StaticContentsConventions.Add(
            //        StaticContentConventionBuilder.AddDirectory("assets", @"contentFolder/subFolder")
            //    );
            //}
        //}
    }
}