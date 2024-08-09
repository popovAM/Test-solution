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
using DevExpress.XtraEditors;
using Solution.Module.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Solution.Module.Controllers
{
    /// <summary>
    /// Контроллер для управления грузами
    /// </summary>
    public partial class CargoPicket_ListView : ViewController
    {
        #region Fields

        private Picket _picket;

        #endregion

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

            //Кнопка разгрузки пикета
            SimpleAction ClearPicket = new SimpleAction(this, "Clear Picket", PredefinedCategory.RecordEdit)
            {
                Caption = "Разгрузить пикет",
                ImageName = "MenuBar_Edit"
            };

            AddCargo.Execute += AddCargo_Execute;
            ClearPicket.Execute += ClearPicket_Execute;
        }

        #endregion

        #region Methods

        #region UpdateMasterObject
        /// <summary>
        /// Обновление родительского объекта
        /// </summary>
        /// <param name="masterObject"></param>
        private void UpdateMasterObject(object masterObject)
        {
            _picket = (Picket)masterObject;
        }
        #endregion

        #region OnMasterObjectChanged

        /// <summary>
        /// Обновление нашего объекта при изменении родительского объекта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMasterObjectChanged(object sender, EventArgs e) => UpdateMasterObject(((PropertyCollectionSource)sender).MasterObject);

        #endregion

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
            if (((DevExpress.ExpressApp.ListView)View).CollectionSource is PropertyCollectionSource collectionSource)
            {
                collectionSource.MasterObjectChanged += OnMasterObjectChanged;
                if (collectionSource.MasterObject != null)
                {
                    UpdateMasterObject(collectionSource.MasterObject);
                }
            }
        }

        #endregion

        #region OnDeactivated

        protected override void OnDeactivated()
        {
            if (((DevExpress.ExpressApp.ListView)View).CollectionSource is PropertyCollectionSource collectionSource)
            {
                collectionSource.MasterObjectChanged -= OnMasterObjectChanged;
            }
            base.OnDeactivated();
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
            addController.SaveOnAccept = false;
            addController.AcceptAction.Execute += (s, args) =>
            {
                bool IsPositive = IsPosiviteWeight(newCargoPicketRecord, context);

                if (context.IsModified && IsPositive)
                {
                    newCargoPicketRecord.CargoAuditTrails.Add(new CargoAuditTrail(((XPObjectSpace)context).Session)
                    {
                        OperationDateTime = DateTime.Now,
                        Weight = newCargoPicketRecord.Picket.Platform.Weight
                    });

                    context.CommitChanges();
                    context.Refresh();
                }
            };

            //Задаём параметры диалогового окна
            e.ShowViewParameters.CreatedView = view;
            e.ShowViewParameters.Context = TemplateContext.PopupWindow;
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.Controllers.Clear();
            e.ShowViewParameters.Controllers.Add(addController);
        }

        #endregion

        #region ClearPicket_Execute

        /// <summary>
        /// Очищение пикета от всех грузов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearPicket_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var message = XtraMessageBox.Show("Вы действительно хотите очистить пикет?", "Очищение пикета", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (message == DialogResult.Yes)
            {
                var thisPicket = _picket;
                var selectedCargoPickets = ((XPObjectSpace)ObjectSpace).Session.Query<CargoPicket>().Where(c => c.Picket == thisPicket && c.IsActive == true);
                foreach (var item in selectedCargoPickets)
                {
                    item.IsActive = false;
                }
                selectedCargoPickets.First().CargoAuditTrails.Add(new CargoAuditTrail(((XPObjectSpace)ObjectSpace).Session)
                {
                    OperationDateTime = DateTime.Now,
                    Weight = selectedCargoPickets.First().Picket.Platform.Weight
                });

                //Сохранение изменений
                if (ObjectSpace.IsModified)
                {
                    ObjectSpace.CommitChanges();
                    ObjectSpace.Refresh();
                }
            }
        }

        #endregion

        #region IsPositiveWeight

        private bool IsPosiviteWeight(CargoPicket currentObject, IObjectSpace context)
        {
            if (currentObject.Status == CargoPicket.OperationType.Outflow)
                currentObject.Weight *= (-1);

            decimal sumWeight = 0;
            var collectionSource = ((XPObjectSpace)context).Session.Query<CargoPicket>().Where(
                c => c.Picket == currentObject.Picket
                && c.Cargo == currentObject.Cargo
                && c.IsActive == true);

            if (collectionSource != null)
            {
                foreach (var item in collectionSource)
                {
                    sumWeight += item.Weight;
                }
            }
            sumWeight += currentObject.Weight;

            if (sumWeight < 0)
            {
                throw new UserFriendlyException("На площадке недостаточно груза.");
                return false;
            }

            else if (sumWeight == 0)
            {
                if (collectionSource != null)
                    foreach (var item in collectionSource)
                        item.IsActive = false;
                currentObject.IsActive = false;
            }

            return true;
        }


        #endregion

        #endregion


    }
}
