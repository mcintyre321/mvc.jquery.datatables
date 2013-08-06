using System.Reflection;
using System.Linq;
namespace Mvc.JQuery.Datatables.Example.App_Code
{
    public class RegisterVirtualPathProvider
    {
        public static void AppInitialize()
        {
            System.Web.Hosting.HostingEnvironment.RegisterVirtualPathProvider(new EmbeddedResourceVirtualPathProvider.Vpp()
            {
				{typeof(Mvc.JQuery.Datatables.DataTableConfigVm).Assembly, @"..\Mvc.JQuery.Datatables"} 
            });
        }
    }
}