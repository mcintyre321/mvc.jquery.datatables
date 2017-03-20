using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.FileProviders;
using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Mvc.JQuery.DataTables;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConfigureServicesExtensions
    {
        public static IServiceCollection AddMvcJQueryDataTables(this IServiceCollection services)
        {

            var dataTablesViewModelType = typeof(Mvc.JQuery.DataTables.DataTableConfigVm).GetTypeInfo();
            var settings = new Mvc.JQuery.DataTables.Settings
            {
                FileProvider = new EmbeddedFileProvider(dataTablesViewModelType.Assembly,
                                                        dataTablesViewModelType.Namespace + ".Common"),
            };
            services.AddSingleton(settings);
            services.Configure<RazorViewEngineOptions>(s => s.FileProviders.Add(settings.FileProvider));
            services.AddMvc(options => { options.UseHtmlEncodeModelBinding(); });

            return services;
        }

        public static void UseHtmlEncodeModelBinding(this MvcOptions opts)
        {
            var binderToFind = opts.ModelBinderProviders.FirstOrDefault(x => x.GetType() == typeof(DataTablesModelBinderProvider));

            if (binderToFind != null) return;

            opts.ModelBinderProviders.Insert(0, new DataTablesModelBinderProvider());
        }
    }
}

namespace Microsoft.AspNetCore.Builder
{
    using Microsoft.Extensions.DependencyInjection;

    public static class MvcJQueryDataTablesExtensions
    {
        public static IApplicationBuilder UseMvcJQueryDataTables(this IApplicationBuilder app)
        {
            var settings = app.ApplicationServices.GetService<global::Mvc.JQuery.DataTables.Settings>();
            if(settings == null)
            {
                throw new InvalidOperationException("Unable to find the required services. Please add all the required services by calling 'IServiceCollection.{}' inside the call to 'ConfigureServices(...)' in the application startup code.");
            } 

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = settings.FileProvider,
            });
            return app;
        }
    }
}