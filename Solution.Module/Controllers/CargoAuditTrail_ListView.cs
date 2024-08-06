using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
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
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class CargoAuditTrail_ListView : ViewController
    {

        #region Constructor
        public CargoAuditTrail_ListView()
        {
            InitializeComponent();
            //Кнопка изменения груза на пикете
            SimpleAction Report = new SimpleAction(this, "Create Report", PredefinedCategory.ObjectsCreation)
            {
                Caption = "Создать отчет",
                ImageName = "MenuBar_New"
            };

            Report.Execute += Report_Execute;
        }
        #endregion

        #region Форма заполения параметров отчета
        private void Report_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            //Создаём диалоговое окно
            var context = Application.CreateObjectSpace(typeof(Report));
            Report newReport = new Report(((XPObjectSpace)context).Session);
            var view = Application.CreateDetailView(context, newReport);

            //Задаём поведение диалогового окна
            var addController = Application.CreateController<DialogController>();
            addController.AcceptAction.Caption = "Создать";
            addController.CancelAction.Caption = "Отменить";
            addController.SaveOnAccept = true;
            addController.AcceptAction.Execute += (s, args) =>
            {
                AddReport(newReport);
            };

            //Задаём параметры диалогового окна
            e.ShowViewParameters.CreatedView = view;
            e.ShowViewParameters.Context = TemplateContext.PopupWindow;
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.Controllers.Clear();
            e.ShowViewParameters.Controllers.Add(addController);
        }
        #endregion

        #region Protected Methods
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
        #endregion

        #region Создание отчета
        private void AddReport(Report newReport)
        {
            DateTime now = DateTime.Now;

            var session = ((XPObjectSpace)ObjectSpace).Session;

            var cargoAuditTrails = session.Query<CargoAuditTrail>()
                .Where(p => p.OperationDateTime >= newReport.BeginDateTime
                && p.OperationDateTime <= newReport.EndDateTime)
                .ToList();

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

                    foreach (var cargoAuditTrail in cargoAuditTrails)
                    {

                        ws.Cells[Row, Col++].Value = cargoAuditTrail.Picket;
                        ws.Cells[Row, Col++].Value = cargoAuditTrail.Cargo;
                        ws.Cells[Row, Col++].Value = cargoAuditTrail.Weight;
                        ws.Cells[Row, Col++].Value = cargoAuditTrail.OperationType;
                        ws.Cells[Row, Col++].Value = cargoAuditTrail.OperationDateTime;

                        Row++;
                        Col = 1;
                    }
                    p.Save();
                }
            }
            #endregion
        }
    }
}
