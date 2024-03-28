using System.Web;
using System.Web.Optimization;

namespace PLL
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-1.12.4.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery2").Include(
                        "~/Scripts/jquery-ui-1.12.1.js",
                        "~/Scripts/DataTables/media/js/jquery.dataTables.min.js",

                        "~/Scripts/DataTables/extensions/Buttons/js/dataTables.buttons.min.js",
                        "~/Scripts/DataTables/extensions/Buttons/js/buttons.bootstrap.min.js",
                        "~/Scripts/DataTables/extensions/Buttons/js/buttons.colVis.min.js",
                        "~/Scripts/DataTables/extensions/Buttons/js/buttons.html5.min.js",
                        "~/Scripts/DataTables/extensions/FixedColumns/js/dataTables.fixedColumns.min.js",
                        "~/Scripts/DataTables/extensions/Buttons/js/jszip.min.js",

                        "~/Scripts/DataTables/extensions/Select/js/dataTables.select.min.js",
                        "~/Scripts/jquery-confirm/jquery-confirm.min.js",
                        "~/Scripts/jQuery.FileUpload/ex/tmpl.min.js",
                        "~/Scripts/jQuery.FileUpload/ex/load-image.all.min.js",
                        "~/Scripts/jQuery.FileUpload/ex/canvas-to-blob.min.js",
                        "~/Scripts/jQuery.FileUpload/ex/jquery.blueimp-gallery.min.js",
                        "~/Scripts/jquery.ui.widget.js",
                        "~/Scripts/jQuery.FileUpload/jquery.iframe-transport.js",
                        "~/Scripts/jQuery.FileUpload/jquery.fileupload.js",
                        "~/Scripts/jQuery.FileUpload/jquery.fileupload-process.js",
                        "~/Scripts/jQuery.FileUpload/jquery.fileupload-image.js",
                        "~/Scripts/jQuery.FileUpload/jquery.fileupload-audio.js",
                        "~/Scripts/jQuery.FileUpload/jquery.fileupload-video.js",
                        "~/Scripts/jQuery.FileUpload/jquery.fileupload-validate.js",
                        "~/Scripts/jQuery.FileUpload/jquery.fileupload-ui.js",
                        "~/Scripts/toastr/toastr.min.js",
                        "~/Scripts/autoNumeric/autoNumeric-min.js"));

            bundles.Add(new ScriptBundle("~/bundles/googleChart").Include(
                        "~/Scripts/GoogleChart/loader.js",
                        "~/Scripts/GoogleChart/jsapi.js"));

            bundles.Add(new ScriptBundle("~/bundles/quill").Include(
                        "~/Scripts/quill/quill.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/moment").Include(
                        "~/Scripts/momentjs/moment.js"));

            bundles.Add(new ScriptBundle("~/bundles/select2").Include(
                        "~/Scripts/select2/select2.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/blueimp-Gallerry2").Include(//Blueimp Gallery 2 
                        "~/Scripts/blueimp-gallery2/js/blueimp-gallery.js",
                        "~/Scripts/blueimp-gallery2/js/blueimp-gallery-video.js",
                        "~/Scripts/blueimp-gallery2/js/blueimp-gallery-indicator.js",
                        "~/Scripts/blueimp-gallery2/js/jquery.blueimp-gallery.js"));

            bundles.Add(new ScriptBundle("~/bundles/artworkcenter").Include(
                        "~/Scripts/ArtworkCenter/*.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/bootstrap.css"));

            bundles.Add(new StyleBundle("~/Content/jquery-confirm/css").Include(
                      "~/Content/jquery-confirm/jquery-confirm.min.css",
                      "~/Content/toastr/toastr.min.css"));

            bundles.Add(new StyleBundle("~/Content/jQuery.FileUpload/css/css").Include(
                      "~/Content/jQuery.FileUpload/css/jquery.fileupload.css",
                      "~/Content/jQuery.FileUpload/css/jquery.fileupload-ui.css",
                      "~/Content/blueimp-gallery2/css/blueimp-gallery.css",
                      "~/Content/blueimp-gallery2/css/blueimp-gallery-video.css",
                      "~/Content/blueimp-gallery2/css/blueimp-gallery-indicator.css"));

            bundles.Add(new StyleBundle("~/Content/DataTables/media/css/css").Include(
                        "~/Content/DataTables/media/css/jquery.dataTables.min.css",
                        "~/Content/DataTables/extensions/Buttons/css/buttons.bootstrap.min.css",
                        "~/Content/DataTables/extensions/FixedColumns/css/fixedColumns.dataTables.min.css",

                        "~/Content/DataTables/extensions/Select/css/select.dataTables.min.css"));

            bundles.Add(new StyleBundle("~/Content/quill/css").Include(
                        "~/Content/quill/quill.snow.css"));

            bundles.Add(new StyleBundle("~/Content/select2/css").Include(
                        "~/Content/select2/select2.min.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/*.css"));

            bundles.Add(new StyleBundle("~/Content/site").Include(
                        "~/Content/site.css"));


            //artwork page
            bundles.Add(new ScriptBundle("~/bundles/dashboard").Include(
                        "~/Scripts/table2excel.js",
                        "~/Scripts/ArtworkDashboard/Dashboard.js",
                       // "~/Scripts/ArtworkDashboard/DashboardInfoGroup.js",
                        "~/Scripts/ArtworkDashboard/DashboardIncomingArtwork.js",
                        "~/Scripts/ArtworkDashboard/DashboardIncomingArtworkForPG.js",
                        "~/Scripts/ArtworkDashboard/DashboardIncomingInbox.js",
                        "~/Scripts/ArtworkDashboard/DashboardIncomingInboxSentPP.js",
                        "~/Scripts/ArtworkDashboard/DashboardIncomingInboxCustomer.js",
                        "~/Scripts/ArtworkDashboard/DashboardIncomingInboxVendor.js",
                        "~/Scripts/ArtworkDashboard/DashboardIncomingMockup.js",
                        "~/Scripts/ArtworkDashboard/DashboardVendorPOReceived.js",
                        "~/Scripts/ArtworkDashboard/DashboardPPView.js",
                        "~/Scripts/ArtworkDashboard/DashboardSalesOrderNew.js",
                        "~/Scripts/ArtworkDashboard/DashboardSalesOrderRepeat.js",
                        "~/Scripts/ArtworkDashboard/DashboardIGridView2.js",
                        "~/Scripts/ArtworkDashboard/DashboardIGridInfoGroup.js"));

            bundles.Add(new ScriptBundle("~/bundles/checklistrequest").Include(
                        "~/Scripts/ArtworkMockupForm/CheckListRequestForm.js",
                        "~/Scripts/ArtworkMockupForm/CheckListRequestFormAttachment.js"));

            bundles.Add(new ScriptBundle("~/bundles/taskform").Include(
                        "~/Scripts/ArtworkMockupForm/TaskFormIndex.js",
                        "~/Scripts/ArtworkMockupForm/TaskFormCheckList.js",
                        "~/Scripts/ArtworkMockupForm/TaskFormPG.js",
                        "~/Scripts/ArtworkMockupForm/TaskFormAttachment.js",
                        "~/Scripts/ArtworkMockupForm/TaskFormAttachmentVersion.js",
                        "~/Scripts/ArtworkMockupForm/TaskFormHistory.js",
                        "~/Scripts/ArtworkMockupForm/TaskFormPriceTemplate.js",
                        "~/Scripts/ArtworkMockupForm/TaskFormPriceTemplateVendor.js",
                        "~/Scripts/ArtworkMockupForm/TaskFormVendor.js",
                        "~/Scripts/ArtworkMockupForm/TaskFormCustomer.js",
                        "~/Scripts/ArtworkMockupForm/TaskFormInternal.js"));

            bundles.Add(new ScriptBundle("~/bundles/reporttracking").Include(
                        "~/Scripts/ArtworkReport/ReportTracking.js"));

            bundles.Add(new ScriptBundle("~/bundles/taskformigrid").Include(
                          "~/Scripts/ArtworkForm/TaskFormIGridCreate.js",
                          "~/Scripts/ArtworkForm/TaskFormIGrid.js",
                          "~/Scripts/ArtworkForm/TaskFormIGridUpload.js"));



            bundles.Add(new ScriptBundle("~/bundles/reporttrackingv2").Include(
                        "~/Scripts/ArtworkReport/ReportTrackingV2.js"));

            bundles.Add(new ScriptBundle("~/bundles/reporttrackingv3").Include(
                    "~/Scripts/ArtworkReport/ReportTrackingV3.js"));

            bundles.Add(new ScriptBundle("~/bundles/reportoverview").Include(
                    "~/Scripts/ArtworkReport/ReportOverview.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportmatstatus").Include(
                    "~/Scripts/ArtworkReport/ReportMatStatus.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportoverallmaster").Include(
                    "~/Scripts/ArtworkReport/ReportOverAllMaster.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportimpactedmat_desc").Include(
                    "~/Scripts/ArtworkReport/ReportImpactedMat_Desc.js"));
            bundles.Add(new ScriptBundle("~/bundles/reportigridsummary").Include(
                    "~/Scripts/ArtworkReport/ReportIGridSummary.js"));

            bundles.Add(new ScriptBundle("~/bundles/reporttrackingigrid").Include(
                    "~/Scripts/ArtworkReport/ReportTrackingIGrid.js"));

            bundles.Add(new ScriptBundle("~/bundles/reportmasterdatachangelog").Include(
                  "~/Scripts/ArtworkReport/ReportMasterDataChangeLog.js"));

            bundles.Add(new ScriptBundle("~/bundles/reporthistory").Include(
                    "~/Scripts/ArtworkReport/ReportHistory.js"));

            bundles.Add(new ScriptBundle("~/bundles/reportoutstanding").Include(
                        "~/Scripts/ArtworkReport/ReportOutstanding.js"));

            bundles.Add(new ScriptBundle("~/bundles/reportendtoend").Include(
                        "~/Scripts/ArtworkReport/ReportEndToEnd.js"));

            bundles.Add(new ScriptBundle("~/bundles/reportendtoendv2").Include(
                        "~/Scripts/ArtworkReport/ReportEndToEndV2.js"));

            bundles.Add(new ScriptBundle("~/bundles/reportendtoendv3").Include(
                     "~/Scripts/ArtworkReport/ReportEndToEndV3.js"));

            bundles.Add(new ScriptBundle("~/bundles/reportwarehouse").Include(
                        "~/Scripts/ArtworkReport/ReportWarehouse.js"));

            bundles.Add(new ScriptBundle("~/bundles/reportwarehousev2").Include(
                     "~/Scripts/ArtworkReport/ReportWarehouseV2.js"));

            bundles.Add(new ScriptBundle("~/bundles/reportartworkmatcontrol").Include(
                        "~/Scripts/ArtworkReport/ReportArtworkMatControl.js"));


            bundles.Add(new ScriptBundle("~/bundles/reportartworkmatcontrolv2").Include(
                        "~/Scripts/ArtworkReport/ReportArtworkMatControlV2.js"));
            //inactive-http://idc-app-dv3-115.thaiunion.co.th:8089/Report/displayso/ 
            bundles.Add(new ScriptBundle("~/bundles/reportso").Include(
                        "~/Scripts/ArtworkReport/ReportSO.js"));

            bundles.Add(new ScriptBundle("~/bundles/reportkpi").Include(
                        "~/Scripts/ArtworkReport/ReportKPI.js"));

            bundles.Add(new ScriptBundle("~/bundles/reportsummary").Include(
                        "~/Scripts/ArtworkReport/ReportSummary.js"));

            bundles.Add(new ScriptBundle("~/bundles/reportvendorcustomercollaboration").Include(
                        "~/Scripts/ArtworkReport/reportvendorcustomercollaboration.js"));

            bundles.Add(new ScriptBundle("~/bundles/artworkupload").Include(
                        "~/Scripts/ArtworkForm/ArtworkUpload.js"));

            bundles.Add(new ScriptBundle("~/bundles/artworkrequest").Include(
                        "~/Scripts/ArtworkForm/ArtworkRequestForm.js"));

            bundles.Add(new ScriptBundle("~/bundles/taskformartwork").Include(
                        "~/Scripts/ArtworkForm/TaskFormIndex.js",
                        "~/Scripts/ArtworkForm/TaskFormSalesOrder.js",
                        "~/Scripts/ArtworkForm/TaskFormPA.js",
                        "~/Scripts/ArtworkMockupForm/TaskFormPG.js",
                        "~/Scripts/ArtworkForm/TaskFormPG.js",
                        "~/Scripts/ArtworkForm/TaskFormHistory.js",
                        "~/Scripts/ArtworkForm/TaskFormInternal.js",
                        "~/Scripts/ArtworkForm/TaskFormAttachment.js",
                        "~/Scripts/ArtworkForm/TaskFormAttachmentVersion.js",
                        "~/Scripts/ArtworkForm/TaskFormCustomer.js",
                        "~/Scripts/ArtworkForm/TaskFormPO.js",
                        "~/Scripts/ArtworkForm/TaskFormVendor.js"));

            bundles.Add(new ScriptBundle("~/bundles/artworksettingreassign").Include(
                        "~/Scripts/ArtworkSetting/Reassign.js"));

            bundles.Add(new ScriptBundle("~/bundles/picsetting").Include(
                      "~/Scripts/ArtworkSetting/PIC.js"));

            bundles.Add(new ScriptBundle("~/bundles/updatemaster").Include(
                      "~/Scripts/ArtworkSetting/updatemaster.js"));
            bundles.Add(new ScriptBundle("~/bundles/artworksettingreopen").Include(
                        "~/Scripts/ArtworkSetting/Reopen.js"));

            bundles.Add(new ScriptBundle("~/bundles/artworksettingrecall").Include(
                        "~/Scripts/ArtworkSetting/Recall.js"));

            bundles.Add(new ScriptBundle("~/bundles/artworksettingreassign2").Include(
                        "~/Scripts/ArtworkSetting/Reassign2.js"));
            //BundleTable.EnableOptimizations = true;
        }
    }
}

