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
using OfficeOpenXml.Style;
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
            //Кнопка создания отчета по карго аудиту
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

            var context = Application.CreateObjectSpace(typeof(CargoAuditTrailReport));
            var newReport = new CargoAuditTrailReport(((XPObjectSpace)context).Session);
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

        private void AddReport(CargoAuditTrailReport newReport, IObjectSpace context)
        {
            DateTime now = DateTime.Now;

            var cargoAuditTrails = ((XPObjectSpace)context).Session
                .Query<CargoAuditTrail>()
                .Where(p => p.OperationDateTime >= newReport.BeginDateTime
                && p.OperationDateTime <= newReport.EndDateTime
                && (newReport.Storage == null || p.CargoPicket.Picket.Storage.Name == newReport.Storage.Name)).OrderBy(p => p.OperationDateTime)
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

                    ws.Cells[1, 1, 6, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[1, 1, 6, 2].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[1, 1, 6, 2].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[1, 1, 6, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells[1, 1, 6, 2].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    ws.Cells[1, 1, 1, 2].Merge = true;
                    ws.Cells[1, 1, 100, 100].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    ws.Columns[1].Width = 22.44;
                    ws.Columns[2].Width = 17.9;
                    ws.Columns[3].Width = 19.5;

                    ws.Cells[1, 1].Value = "Наличие груза на площадке";

                    ws.Cells[2, 1].Value = "С:";
                    ws.Cells[2, 2].Value = newReport.BeginDateTime.ToString("d");

                    ws.Cells[3, 1].Value = "По:";
                    ws.Cells[3, 2].Value = newReport.EndDateTime.ToString("d");

                    ws.Cells[4, 1].Value = "Кем сформирован отчет:";
                    ws.Cells[4, 2].Value = SecuritySystem.CurrentUserName;

                    ws.Cells[5, 1].Value = "Дата и время создания:";
                    ws.Cells[5, 2].Value = DateTime.Now.ToString("G");
                    
                    var storage = newReport.Storage == null ? "Все" : newReport.Storage.Name.ToString();

                    ws.Cells[6, 1].Value = "Склад:";
                    ws.Cells[6, 2].Value = storage;

                    ws.Cells[8, 1].Value = "Платформа";
                    ws.Cells[8, 2].Value = "Вес";
                    ws.Cells[8, 3].Value = "Дата операции";

                    Row = 9;
                    Col = 1;

                    foreach (var cargoAuditTrail in cargoAuditTrails)
                    {
                        ws.Cells[Row, Col++].Value = cargoAuditTrail.Platform;
                        ws.Cells[Row, Col++].Value = cargoAuditTrail.Weight;
                        ws.Cells[Row, Col++].Value = cargoAuditTrail.OperationDateTime.ToString();

                        Row++;
                        Col = 1;
                    }
                    p.Save();
                }
            }
        }

        #endregion

    }
}
