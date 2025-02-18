﻿using DevExpress.Data.Filtering;
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
    public partial class PlatformAuditTrail_Report : ViewController
    {

        #region Constructor
        public PlatformAuditTrail_Report()
        {
            InitializeComponent();
            //Кнопка создания отчета по платформе
            SimpleAction Report = new SimpleAction(this, "Create Report", PredefinedCategory.ObjectsCreation)
            {
                Caption = "Создать отчет"
            };

            Report.Execute += Report_Execute;

        }
        #endregion


        #region Форма заполения параметров отчета
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
        #endregion


        #region Создание отчета
        private void AddReport(PlatformAudit_Report newReport, IObjectSpace context)
        {
            DateTime now = DateTime.Now;
            
            // Выбор записей по параметрам
            var session = ((XPObjectSpace)context).Session;

            var platformAuditTrails = 
                session.Query<PlatformAuditTrail>()
                       .Where(p => p.TimeOperation <= newReport.DateTime
                                               && p.Status == PlatformAuditTrail.PlatformStatus.Created)
                       .Select(s => s.Platform)
                       .Where(w => !w.PlatformAudits
                                                .Any(a => a.Status == PlatformAuditTrail.PlatformStatus.Deleted))
                       .ToList();

            var oids = platformAuditTrails.Select(s => s.Oid).ToList();
            var audits = session.Query<PlatformAuditTrail>().Where(w => oids.Contains(w.Platform.Oid)).ToList();

            audits = audits.Where(p => newReport.Storage == null || p.Platform.Storage.Name == newReport.Storage.Name).OrderBy(p => p.TimeOperation)
                .ToList();

            //Настраиваем файл
            string fileName = $"Report_{now.Hour}--{now.Minute}_{now.Day}.{now.Month}.{now.Year}.xlsx";
            string relativePath = Path.Combine("Reports", fileName);
            string fullPath = Path.GetFullPath(relativePath);

            //Создаем директорий
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            using (var source = File.Create(fullPath))
            {
                using (var p = new ExcelPackage(source))
                {
                    var ws = p.Workbook.Worksheets.Add("List");

                    int Row = 1, Col = 1;

                    //Стиль границ ячеек в шапке
                    ws.Cells[2, 1, 5, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[2, 1, 5, 2].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[2, 1, 5, 2].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[2, 1, 5, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    //Стиль границ шапки
                    ws.Cells[2, 1, 5, 2].Style.Border.BorderAround(ExcelBorderStyle.Thick);

                    //Объединение ячеек
                    ws.Cells[1, 1, 1, 2].Merge = true;

                    //Выравнивание текста по центру внутри ячеек
                    ws.Cells[1, 1, 100, 100].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //Ширина колонок
                    ws.Columns[1].Width = 32;
                    ws.Columns[2].Width = 15;
                    ws.Columns[3].Width = 18;

                    ws.Cells[1, 1].Value = "Платформы на складе";

                    ws.Cells[2, 1].Value = "Дата";
                    ws.Cells[2, 2].Value = newReport.DateTime.ToString("g");

                    ws.Cells[3, 1].Value = "Кем сформирован отчет:";
                    ws.Cells[3, 2].Value = SecuritySystem.CurrentUserName;

                    ws.Cells[4, 1].Value = "Дата и время формирования отчета:";
                    ws.Cells[4, 2].Value = DateTime.Now.ToString("g");

                    var storage = newReport.Storage == null ? "Все" : newReport.Storage.Name.ToString();
                    ws.Cells[5, 1].Value = "Склад:";
                    ws.Cells[5, 2].Value = storage;

                    Row = 8;
                    Col = 1;

                    //Заполнение файла
                    if (newReport.Storage == null)
                    {
                        ws.Cells[7, 1].Value = "Склад";
                        ws.Cells[7, 2].Value = "Платформа";

                        foreach (var platformAuditTrail in audits)
                        {
                            ws.Cells[Row, Col++].Value = platformAuditTrail.Storage;
                            ws.Cells[Row, Col++].Value = platformAuditTrail.Platform.Name;

                            Row++;
                            Col = 1;
                        }
                    }

                    else
                    {
                        ws.Cells[7, 1].Value = "Платформа";
                        foreach (var platformAuditTrail in audits)
                        { 
                            ws.Cells[Row, Col++].Value = platformAuditTrail.Platform.Name;

                            Row++;
                            Col = 1;
                        }
                    }
                    p.Save();
                }
            }
        }
        #endregion
    }
}
