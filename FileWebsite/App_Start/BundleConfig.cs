using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace FileWebsite.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                 "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new StyleBundle("~/Content/MyTemplate/css").Include(
                 "~/Content/MyTemplate/css/materialize.css",
                 "~/Content/MyTemplate/css/materialize.min.css",
                 "~/Content/MyTemplate/css/style.css"));

            bundles.Add(new StyleBundle("~/Content/MyTemplate/js").Include(
                "~/Content/MyTemplate/js/materialize.js",
                "~/Content/MyTemplate/js/materialize.min.js",
                "~/Content/MyTemplate/js/init.js"));

            bundles.Add(new ScriptBundle("~/bundles/dropzone").Include(
                       "~/Scripts/dropzone.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                       "~/Content/bootstrap.css",
                       "~/Content/site.css",
                       "~/Content/basic.css",
                       "~/Content/dropzone.css"));
        }
    }
}