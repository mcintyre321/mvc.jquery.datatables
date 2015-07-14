turn IQueryables into datagrids
----------------------------------

> Install-Package Mvc.JQuery.Datatables

> Install-Package Mvc.JQuery.Datatables.Templates *


[Demo site](http://aspdatatables.azurewebsites.net/)
![Example](http://snag.gy/FQFdn.jpg)

 - turn any IQueryable into a live datagrid. Tested with:
   - Linq To Objects
   - Entity Framework
   - Lucene.Net.Linq
 - wraps the very comprehensive jquery datatables plugin. Supported features: 
   - Filtering (text, date or datetime range (with datepicker), number ranges, choose from dropdown, multiple values using checkboxes)
   - Sorting (configurable per column)
   - Paging (choose page size options, or fix them)
   - Customer column rendering 
   - Custom positioning of filters (e.g. you could move them above the table)
   - Localization
   - TableTools support (PDF/Excel export)
   - Attribute based configuration (optional)   
 - Can be run from dll 
 
*skip this if using EmbeddedResourceVirtualPathProvider


See the test page and example project for info on how to use

http://nuget.org/packages/Mvc.JQuery.Datatables

The code here is based on code from http://code.google.com/p/datatables-asp-mvc/

Note to users
-------------

This library has been developed on an as-needed basis, so not all configuration settings from datatables are implemented yet. If you need a setting to be added, please fork the project, update the code, and update the example page to include a usage of the new setting (where this makes sense) e.g. https://github.com/mcintyre321/mvc.jquery.datatables/commit/c70f9c1f51178386e84a73ccea4495343f815012

If you have a feature request, bug, or a patch, please could you add an example page on a fork demonstrating the problem or feature. Thanks!

[![Flattr this git repo](http://api.flattr.com/button/flattr-badge-large.png)](https://flattr.com/submit/auto?user_id=mcintyre321&url=https://github.com/mcintyre321/mvc.jquery.datatables&title=Mvc.JQuery.DataTables&language=&tags=github&category=software)

> If you have found this project useful, please consider contributing some documentation - it's the biggest weakness!
