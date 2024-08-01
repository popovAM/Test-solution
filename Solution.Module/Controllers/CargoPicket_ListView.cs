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
        private Picket _picket;
        public CargoPicket_ListView()
        {
            InitializeComponent();

            //Кнопка добавления груза на пикет
            SimpleAction AddCargo = new SimpleAction(this, "Add Cargo", PredefinedCategory.ObjectsCreation)
            {
                Caption = "Добавить груз на пикет",
                ImageName = "MenuBar_New"
            };

            AddCargo.Execute += AddCargo_Execute;
        }

        /// <summary>
        /// Обновление родительского объекта
        /// </summary>
        /// <param name="masterObject"></param>
        private void UpdateMasterObject(object masterObject)
        {
            _picket = (Picket)masterObject;
        }

        /// <summary>
        /// Обновление нашего объекта при изменении родительского объекта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMasterObjectChanged(object sender, EventArgs e) => UpdateMasterObject(((PropertyCollectionSource)sender).MasterObject);

        protected override void OnActivated()
        {
            base.OnActivated();

            //Создание контроллера для запрета открытия DetailView двойным кликом
            ListViewProcessCurrentObjectController targetController = Frame.GetController<ListViewProcessCurrentObjectController>();
            if (targetController != null)
            {
                targetController.ProcessCurrentObjectAction.Enabled["Нельзя"] = false;
            }

            if (((DevExpress.ExpressApp.ListView)View).CollectionSource is PropertyCollectionSource collectionSource)
            {
                collectionSource.MasterObjectChanged += OnMasterObjectChanged;
                if (collectionSource.MasterObject != null)
                {
                    UpdateMasterObject(collectionSource.MasterObject);
                }
            }
        }

        protected override void OnDeactivated()
        {
            if (((DevExpress.ExpressApp.ListView)View).CollectionSource is PropertyCollectionSource collectionSource)
            {
                collectionSource.MasterObjectChanged -= OnMasterObjectChanged;
            }
            base.OnDeactivated();
        }

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
            };

            //Задаём параметры диалогового окна
            e.ShowViewParameters.CreatedView = view;
            e.ShowViewParameters.Context = TemplateContext.PopupWindow;
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.Controllers.Clear();
            e.ShowViewParameters.Controllers.Add(addController);
        }
    }
}
