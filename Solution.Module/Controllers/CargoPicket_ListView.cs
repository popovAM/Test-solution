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
        #region Constructor

        public CargoPicket_ListView()
        {
            InitializeComponent();

            //Кнопка добавления груза на пикет
            SimpleAction AddCargo = new SimpleAction(this, "Add Cargo", PredefinedCategory.ObjectsCreation)
            {
                Caption = "Загрузить на пикет",
                ImageName = "MenuBar_New"
            };

            ////Кнопка разгрузки пикета
            //SimpleAction OutflowCargo = new SimpleAction(this, "Outflow Cargo", PredefinedCategory.RecordEdit)
            //{
            //    Caption = "Разгрузить пикет",
            //    ImageName = "MenuBar_Edit"
            //};

            AddCargo.Execute += AddCargo_Execute;
            //OutflowCargo.Execute += OutflowCargo_Execute;
        }

        #endregion

        #region Methods

        #region OnActivated

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

        #endregion

        #region AddCargo_Execute

        /// <summary>
        /// Загрузка на пикет
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddCargo_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            //Создаём диалоговое окно
            var context = Application.CreateObjectSpace(typeof(CargoPicket));
            CargoPicket newCargoPicketRecord = new CargoPicket(((XPObjectSpace)context).Session);
            var view = Application.CreateDetailView(context, newCargoPicketRecord);

            //Задаём поведение диалогового окна
            var addController = Application.CreateController<DialogController>();
            addController.AcceptAction.Caption = "Сохранить";
            addController.CancelAction.Caption = "Отменить";
            addController.SaveOnAccept = true;
            addController.AcceptAction.Execute += (s, args) =>
            {
            };

            //Задаём параметры диалогового окна
            e.ShowViewParameters.CreatedView = view;
            e.ShowViewParameters.Context = TemplateContext.PopupWindow;
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.Controllers.Clear();
            e.ShowViewParameters.Controllers.Add(addController);
        }

        #endregion

        //#region OutflowCargo_Execute
        ///// <summary>
        ///// Выгрузка пикета
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void OutflowCargo_Execute(object sender, SimpleActionExecuteEventArgs e)
        //{
        //    //Создаём диалоговое окно
        //    var context = Application.CreateObjectSpace(typeof(CargoPicket));
        //    CargoPicket newCargoPicketRecord = new CargoPicket(((XPObjectSpace)context).Session);
        //    var view = Application.CreateDetailView(context, newCargoPicketRecord);

        //    //Задаём поведение диалогового окна
        //    var addController = Application.CreateController<DialogController>();
        //    addController.AcceptAction.Caption = "Сохранить";
        //    addController.CancelAction.Caption = "Отменить";
        //    addController.SaveOnAccept = true;
        //    addController.AcceptAction.Execute += (s, args) =>
        //    {
        //        newCargoPicketRecord.Weight *= (-1);
        //    };

        //    //Задаём параметры диалогового окна
        //    e.ShowViewParameters.CreatedView = view;
        //    e.ShowViewParameters.Context = TemplateContext.PopupWindow;
        //    e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
        //    e.ShowViewParameters.Controllers.Clear();
        //    e.ShowViewParameters.Controllers.Add(addController);
        //}

        //#endregion

        #endregion

    }
}
