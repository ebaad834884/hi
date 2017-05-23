using System.Web;
using System.Web.Optimization;
using System.Linq;
namespace NewSDTApplication
{
    public class BundleConfig
    {
        //Code Start-Hita - 12/05/2017 - US250/TA1680 
        private BundleConfig()
        {
        }
        //Code End-Hita - 12/05/2017 - US250/TA1680 

        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Content/js/bootstrap.min.js",
                     "~/Content/js/progressbar/bootstrap-progressbar.min.js", "~/Content/js/nicescroll/jquery.nicescroll.min.js", "~/Content/js/icheck/icheck.min.js", "~/Content/js/custom.js",
                      "~/Content/js/moment.min2.js", "~/Content/js/datepicker/daterangepicker.js", "~/Content/js/input_mask/jquery.inputmask.js", "~/Content/js/knob/jquery.knob.min.js", "~/Content/js/ion_range/ion.rangeSlider.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/datetime").Include(
                      "~/Scripts/moment*"
                      ));
            bundles.Add(new StyleBundle("~/Content/css").Include(
            "~/Content/css/bootstrap.min.css", "~/Content/fonts/css/font-awesome.css", "~/Content/css/animate.min.css", "~/Content/css/custom.css",
         "~/Content/css/icheck/flat/green.css", "~/Content/css/normalize.css", "~/Content/css/ion.rangeSlider.css",
          "~/Content/css/ion.rangeSlider.skinFlat.css", "~/Content/jqgrid/ui.jqgrid.css", "~/Content/jquery-ui.css"));


            bundles.Add(new StyleBundle("~/bundles/jqgrid").Include(
                      "~/Scripts/JQGrid/grid.locale-en.js", "~/Scripts/JQGrid/jquery.jqGrid.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/GoogleMapUtil").Include(
                    "~/Scripts/GoogleMapUtil.js"
                    ));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css","~/Content/site.css"));

            //, "~/Content/bootstrap-datetimepicker.css"));

            BundleTable.Bundles.All(b => { b.Transforms.Clear(); return true; });
            BundleTable.EnableOptimizations = true;
        }
    }
}