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
using Solution.Module.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solution.Module.Controllers
{
    /// <summary>
    /// Контроллер для управления грузами
    /// </summary>
    public partial class CargoPicket_ListView : ViewController
    {
        public CargoPicket_ListView()
        {
            InitializeComponent();

            //Кнопка добавления груза на пикет
            SimpleAction AddCargo = new SimpleAction(this, "Add Cargo", PredefinedCategory.ObjectsCreation)
            {
                Caption = "Добавить груз на пикет",
                ImageName = "MenuBar_New"
            };

            //Кнопка изменения груза на пикете
            SimpleAction EditCargo = new SimpleAction(this, "Edit Cargo", PredefinedCategory.RecordEdit)
            {
                Caption = "Изменить",
                ImageName = "MenuBar_Edit"
            };

            

            AddCargo.Execute += AddCargo_Execute;
            EditCargo.Execute += EditCargo_Execute;
        }
        protected override void OnActivated()
        {
            base.OnActivated();

            //Создание контроллера для запрета открытия DetailView двойным кликом
            ListViewProcessCurrentObjectController targetController = Frame.GetController<ListViewProcessCurrentObjectController>();
            if (targetController != null)
            {
                targetController.ProcessCurrentObjectAction.Enabled["Нельзя"] = false;
            }
        }

        ///// <summary>
        ///// Удаление груза с пикета, если его вес равен нулю
        ///// </summary>
        //private bool DeleteCargoPicketEmpty()
        //{
        //    //Запрос на наличие груза с нулевым весом
        //    var session = ((XPObjectSpace)ObjectSpace).Session;
        //    var cargoPicketEmpty = session.Query<CargoPicket>().Where(p => p.Weight == 0).ToList();
        //
        //    //Проверяем, есть ли в листе данные
        //    if (cargoPicketEmpty.Any())
        //    {
        //        //Удаление этого груза
        //        foreach (var item in cargoPicketEmpty)
        //        {
        //            item.Picket.IsFull = false;
        //            ObjectSpace.Delete(item);
        //        }
        //
        //        //Сохранение изменений
        //        ObjectSpace.CommitChanges();
        //        ObjectSpace.Refresh();
        //        return true;
        //    }
        //    return false;
        //}

        /// <summary>
        /// Добавление груза на пикет
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddCargo_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            //Создаём диалоговое окно
            var context = Application.CreateObjectSpace(typeof(CargoPicket));
            CargoPicket newCargoPicket = new CargoPicket(((XPObjectSpace)context).Session);
            var view = Application.CreateDetailView(context, newCargoPicket);

            //Задаём поведение диалогового окна
            var addController = Application.CreateController<DialogController>();
            addController.AcceptAction.Caption = "Создать";
            addController.CancelAction.Caption = "Отменить";
            addController.SaveOnAccept = true;
            addController.AcceptAction.Execute += (s, args) =>
            {
                //Добавление записи в журнал изменений
                CreateNewRecord(newCargoPicket, true);
            };

            //Задаём параметры диалогового окна
            e.ShowViewParameters.CreatedView = view;
            e.ShowViewParameters.Context = TemplateContext.PopupWindow;
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.Controllers.Clear();
            e.ShowViewParameters.Controllers.Add(addController);
        }

        /// <summary>
        /// Изменение груза на пикете
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditCargo_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            //Выбираем текущий объект 
            var context = Application.CreateObjectSpace(typeof(CargoPicket));
            var currentObject = (CargoPicket)context.GetObject(View.CurrentObject);

            //Проверка на наличие текущего объекта
            if (currentObject != null)
            {
                //Создаём диалоговое окно
                var view = Application.CreateDetailView(context, currentObject);
                currentObject.PreviousWeight = currentObject.Weight;

                //Задаём поведение диалогового окна
                var addController = Application.CreateController<DialogController>();
                addController.AcceptAction.Caption = "Изменить";
                addController.CancelAction.Caption = "Отменить";
                addController.SaveOnAccept = true;
                addController.AcceptAction.Execute += (s, args) =>
                {
                    CreateNewRecord(currentObject, false);
                };

                //Задаём параметры диалогового окна
                e.ShowViewParameters.CreatedView = view;
                e.ShowViewParameters.Context = TemplateContext.PopupWindow;
                e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                e.ShowViewParameters.Controllers.Clear();
                e.ShowViewParameters.Controllers.Add(addController);
            }
        }
        /// <summary>
        /// Создание новой записи в журнале изменений
        /// </summary>
        /// <param name="cargoPicket"></param>
        private void CreateNewRecord(CargoPicket cargoPicket, bool isAddCargo)
        {
            //Создание новой записи
            var context = Application.CreateObjectSpace(typeof(CargoAuditTrail));
            CargoAuditTrail newRecord = new CargoAuditTrail(((XPObjectSpace)context).Session)
            {
                //Заполение полей новой записи
                OperationDateTime = DateTime.Now,
                Picket = cargoPicket.Picket.Name,
                Cargo = cargoPicket.Cargo.Name,
                Weight = cargoPicket.Weight
            };

            // Определение статуса
            if (isAddCargo)
            {
                newRecord.OperationType = CargoAuditTrail.CargoStatus.Добавление;
            }
            else
            {
                if (cargoPicket.Weight > cargoPicket.PreviousWeight)
                {
                    newRecord.OperationType = CargoAuditTrail.CargoStatus.Загрузка;
                }
                else if (cargoPicket.Weight < cargoPicket.PreviousWeight)
                {
                    newRecord.OperationType = CargoAuditTrail.CargoStatus.Выгрузка;
                }
                else if (cargoPicket.Weight == 0 && cargoPicket.PreviousWeight > 0)
                {
                    newRecord.OperationType = CargoAuditTrail.CargoStatus.Освобождение;
                }
            }

            //Сохранение изменений
            context.CommitChanges();
            context.Refresh();
        }
    }
}
