using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using OfficeOpenXml;
using Solution.Module.BusinessObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Solution.Module.Controllers
{
        public partial class PlatformAuditTrail_Report : ViewController
    {
        public PlatformAuditTrail_Report()
        {
            InitializeComponent();
            SimpleAction Report = new SimpleAction(this, "Create ProReport", PredefinedCategory.ObjectsCreation)
            {
                Caption = "Создать отчет",
                ImageName = "Menubar_Report"
            };

            Report.Execute += Report_Execute;

        }

        private void Report_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var context = Application.CreateObjectSpace(typeof(PlatformAuditTrail_Report));
            var newReport = new PlatformAudit_Report(((XPObjectSpace)context).Session);
            var view = Application.CreateDetailView(context, newReport);

            //Задаём поведение диалогового окна
            var addController = Application.CreateController<DialogController>();
            addController.AcceptAction.Caption = "Создать";
            addController.CancelAction.Caption = "Отменить";
            addController.SaveOnAccept = true;
            addController.AcceptAction.Execute += (s, args) =>
            {
                AddReport(newReport, context);
            };

            //Задаём параметры диалогового окна
            e.ShowViewParameters.CreatedView = view;
            e.ShowViewParameters.Context = TemplateContext.PopupWindow;
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.Controllers.Clear();
            e.ShowViewParameters.Controllers.Add(addController);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }

        private void AddReport(PlatformAudit_Report newReport, IObjectSpace context)
        {
            DateTime now = DateTime.Now;
            
            var session = ((XPObjectSpace)context).Session;

            var platformAuditTrailsCreated = session.Query<PlatformAuditTrail>()
                .Where(p => p.TimeOperation <= newReport.DateTime
                && p.Status == PlatformAuditTrail.PlatformStatus.Created);
            var platformAuditTrailsDeleted = session.Query<PlatformAuditTrail>()
                .Where(p => p.TimeOperation <= newReport.DateTime
                && p.Status == PlatformAuditTrail.PlatformStatus.Deleted);

            var platformAuditTrails = platformAuditTrailsCreated.Except(platformAuditTrailsDeleted).ToList();

            string fileName = $"Report_{now.Hour}--{now.Minute}_{now.Day}.{now.Month}.{now.Year}.xlsx";
            string relativePath = Path.Combine("Reports", fileName);
            string fullPath = Path.GetFullPath(relativePath);

            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            using (var source = File.Create(fullPath))
            {
                using (var p = new ExcelPackage(source))
                {
                    var ws = p.Workbook.Worksheets.Add("List");

                    int Row = 1, Col = 1;

                    ws.Cells[Row, 10].Value = newReport.DateTime.ToString("G");                    
                    foreach (var platformAuditTrail in platformAuditTrails)
                    {
                        ws.Cells[Row, Col++].Value = platformAuditTrail.PlatformName;
                        ws.Cells[Row, Col++].Value = platformAuditTrail.Storage;
                        ws.Cells[Row, Col++].Value = platformAuditTrail.TimeOperation.ToString();

                        Row++;
                        Col = 1;
                    }
                    p.Save();
                }
            }

        }
    }
}
