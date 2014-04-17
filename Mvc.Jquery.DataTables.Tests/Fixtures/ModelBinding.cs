using Mvc.JQuery.Datatables;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Mvc.JQuery.Datatables.Tests.TestingUtilities;

namespace Mvc.JQuery.Datatables.Tests
{
    [TestFixture]
    public class ModelBinding
    {
        [Test]
        public void TestDataTablesIModelBinder()
        {
            Mvc.JQuery.Datatables.Example.RegisterDatatablesModelBinder.Start();
            IModelBinder dataTablesBinder = ModelBinders.Binders.GetBinder(typeof(DataTablesParam), false);
            Assert.That(dataTablesBinder, Is.Not.Null, "DataTablesParam model binder not found in ModelBindersDictionary");

            var formCollection = new NameValueCollection 
            {
                { "sEcho", "1" },
                { "iColumns", "9" },
                { "sColumns", "" },
                { "iDisplayStart", "0" },
                { "iDisplayLength", "10" },
                { "mDataProp_0", "0" },
                { "mDataProp_1", "1" },
                { "mDataProp_2", "2" },
                { "mDataProp_3", "3" },
                { "mDataProp_4", "4" },
                { "mDataProp_5", "5" },
                { "mDataProp_6", "6" },
                { "mDataProp_7", "7" },
                { "mDataProp_8", "8" },
                { "sSearch", "" },
                { "bRegex", "false" },
                { "sSearch_0", "" },
                { "sSearch_1", "" },
                { "sSearch_2", "" },
                { "sSearch_3", "" },
                { "sSearch_4", "" },
                { "sSearch_5", "" },
                { "sSearch_6", "" },
                { "sSearch_7", "" },                
                { "sSearch_8", "" },
                { "bRegex_0", "false" },                
                { "bRegex_1", "false" },                
                { "bRegex_2", "false" },                
                { "bRegex_3", "false" },                
                { "bRegex_4", "false" },                
                { "bRegex_5", "false" },                
                { "bRegex_6", "false" },                
                { "bRegex_7", "false" },                
                { "bRegex_8", "false" },
                { "bSearchable_0", "true" },
                { "bSearchable_1", "true" },
                { "bSearchable_2", "false" },
                { "bSearchable_3", "true" },
                { "bSearchable_4", "true" },
                { "bSearchable_5", "true" },
                { "bSearchable_6", "true" },
                { "bSearchable_7", "true" },
                { "bSearchable_8", "true" },
                { "iSortCol_0", "0" },
                { "sSortDir_0", "asc" },
                { "iSortingCols", "1" },
                { "bSortable_0", "true" },
                { "bSortable_1", "true" },
                { "bSortable_2", "true" },
                { "bSortable_3", "false" },
                { "bSortable_4", "true" },
                { "bSortable_5", "true" },
                { "bSortable_6", "true" },
                { "bSortable_7", "true" },
                { "bSortable_8", "true" }
            };

            var res = SetupAndBind<DataTablesParam>(formCollection, dataTablesBinder);
            Assert.That(res.iColumns, Is.EqualTo(9), "iColumns");
            Assert.That(res.bEscapeRegex, Is.EqualTo(false),"bEscapeRegex");
            Assert.That(res.bEscapeRegexColumns, Is.EqualTo(Enumerable.Repeat(false, 9)), "bEscapeRegexColumns");
            Assert.That(res.bSearchable, Is.EqualTo(Enumerable.Repeat(true, 9).ReplaceAtIndex(false, 2)), "bSearchable");
            Assert.That(res.bSortable, Is.EqualTo(Enumerable.Repeat(true, 9).ReplaceAtIndex(false, 3)), "bSortable");
            Assert.That(res.iDisplayLength, Is.EqualTo(10), "iDisplayLength");
            Assert.That(res.iDisplayStart, Is.EqualTo(0), "iDisplayStart");
            Assert.That(res.iSortCol, Is.EqualTo(Enumerable.Repeat(0, 9)), "iSortCol");
            Assert.That(res.iSortingCols, Is.EqualTo(1), "iSortingCols");
            Assert.That(res.sEcho, Is.EqualTo(1), "sEcho");
            Assert.That(res.sSearch,Is.EqualTo(""),"sSearch");
            Assert.That(res.sSearchColumns, Is.EqualTo(Enumerable.Repeat("", 9)), "sSearchColumns");
            Assert.That(res.sSortDir, Is.EqualTo(Enumerable.Repeat<string>(null, 9).ReplaceAtIndex("asc", 0)), "sSortDir");
            Assert.That(res.bEscapeRegex, Is.EqualTo(false), "bEscapeRegex");
        }

        //http://www.jamie-dixon.co.uk/unit-testing/unit-testing-your-custom-model-binder/
        static TModel SetupAndBind<TModel>(NameValueCollection nameValueCollection, IModelBinder modelBinder)
            where TModel : class
        {
            var valueProvider = new NameValueCollectionValueProvider(nameValueCollection, null);
            var modelMetaData = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(TModel));
            var controllerContext = new ControllerContext();
            var bindingContext = new ModelBindingContext
            {
                ModelName = string.Empty,
                ValueProvider = valueProvider,
                ModelMetadata = modelMetaData,
            };

            return (TModel)modelBinder.BindModel(controllerContext, bindingContext);
        }
    }
}
