using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solution.Module.BusinessObjects;
using DevExpress.XtraEditors;
using System.Windows.Forms;
using ListView = DevExpress.ExpressApp.ListView;
using DevExpress.ExpressApp.Xpo;

namespace Solution.Module.Controllers
{
    /// <summary>
    /// Контроллер для управления площадками
    /// </summary>
    public partial class Platform_ListView : ViewController
    {
        #region Fields

        /// <summary>
        /// Родительский объект Склад
        /// </summary>
        private Storage _storage; 

        #endregion

        #region Constructor

        public Platform_ListView()
        {
            InitializeComponent();

            // Кнопка создания площадки
            SimpleAction createPlatform = new SimpleAction(this, "CreatePlatform", PredefinedCategory.ObjectsCreation)
            {
                Caption = "Создать платформу",
                ImageName = "MenuBar_New"
            };
            createPlatform.Execute += CreatePlatform_Execute;

            // Кнопка удаления площадки
            SimpleAction deletePlatform = new SimpleAction(this, "DeletePlatform", PredefinedCategory.ObjectsCreation)
            {
                Caption = "Удалить платформу",
                ImageName = "Action_Delete"
            };
            deletePlatform.Execute += DeletePlatform_Execute;
        }

        #endregion

        #region Methods

        #region CreatePlatform_Execute

        /// <summary>
        /// Создание площадки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreatePlatform_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            //Создание диалогового окна со списком пикетов, которые принадлежат текущему складу
            var collectionSource = new CollectionSource(ObjectSpace, typeof(Picket));
            collectionSource.Criteria["ThisStorage"] = CriteriaOperator.Parse($"[Storage].Oid == '{_storage.Oid}' AND [IsActive] = true");
            var view = Application.CreateListView("Picket_ListView", collectionSource, false);
            view.Caption = "Добавить пикеты";

            //Описание действий и внешнего вида кнопок в диалоговом окне
            var addController = Application.CreateController<DialogController>();
            addController.AcceptAction.Caption = "Добавить";
            addController.CancelAction.Caption = "Отменить";
            addController.SaveOnAccept = false;
            addController.AcceptAction.Execute += (s, args) =>
            {
                AddPicket(view);
            };

            //Описание параметров диалогового окна
            e.ShowViewParameters.CreatedView = view;
            e.ShowViewParameters.Context = TemplateContext.PopupWindow;
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.Controllers.Clear();
            e.ShowViewParameters.Controllers.Add(addController);
        }

        #endregion

        #region DeletePlatform_Execute

        /// <summary>
        /// Удаление платформы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeletePlatform_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            //Выбор нужной площадки
            Platform currentObject = (Platform)ObjectSpace.GetObject(View.CurrentObject);

            //Проверка на наличие площадки и отсутствие груза на ней 
            if (currentObject != null && currentObject.Weight == 0)
            {
                //Диалоговое окно подтверждения
                var message = XtraMessageBox.Show("Вы действительно хотите удалить платформу?", "Удалить платформу", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (message == DialogResult.Yes)
                {
                    //Удаление связи площадки со всеми пикетами
                    foreach (var item in currentObject.Pickets)
                    {
                        item.NotActivePlatforms.Add(currentObject);
                        item.Platform = null;
                    }

                    //Удаление площадки и сохранение изменений
                    currentObject.IsActive = false;
                    currentObject.PlatformAudits.Add(new PlatformAuditTrail(((XPObjectSpace)ObjectSpace).Session)
                    {
                        TimeOperation = DateTime.Now,
                        Status = PlatformAuditTrail.PlatformStatus.Deleted
                    });
                }
            }
            //Если на площадке присутствует груз 
            else if (currentObject.Weight != 0)
                throw new UserFriendlyException("На платформе находится груз.");

            //Сохранение изменений
            if (ObjectSpace.IsModified)
            {
                ObjectSpace.CommitChanges();
                ObjectSpace.Refresh();
            }
        }

        #endregion

        #region AddPicket

        /// <summary>
        /// Добавление пикета на площадку
        /// </summary>
        /// <param name="view"></param>
        private void AddPicket(ListView view)
        {
            //Создаем список выбранных пикетов
            var selectedPickets = view.SelectedObjects.OfType<Picket>().ToList();

            bool isCorrect = true;

            //Проверка на правильность списка (пикеты не разрываются и не находятся на других площадках)
            for (int i = 0; i < selectedPickets.Count; i++)
            {
                if (selectedPickets[i].Platform != null || i != 0 && selectedPickets[i].Number != selectedPickets[i - 1].Number + 1)
                {
                    isCorrect = false;
                }
            }

            //Если список правильный
            if (isCorrect)
            {
                //Создаем новую площадку и заполняем поля
                Platform newPlatform = new Platform(((XPObjectSpace)ObjectSpace).Session);
                newPlatform.Storage = _storage;
                foreach (var item in selectedPickets)
                {
                    item.Platform = newPlatform;
                    newPlatform.Pickets.Add(item);
                }
                newPlatform.Name = selectedPickets[0].Number.ToString() + "-" + selectedPickets[selectedPickets.Count - 1].Number.ToString();

                newPlatform.PlatformAudits.Add(new PlatformAuditTrail(((XPObjectSpace)ObjectSpace).Session)
                {
                    TimeOperation = DateTime.Now,
                    Status = PlatformAuditTrail.PlatformStatus.Created
                });
            }
            //Если список неправильный
            else
            {
                throw new UserFriendlyException("Площадка должна быть не занята и не разрывна");
            }

            //Сохранение изменений
            if (ObjectSpace.IsModified)
            {
                ObjectSpace.CommitChanges();
                ObjectSpace.Refresh();
            }
        }

        #endregion

        #region UpdateMasterObject

        /// <summary>
        /// Обновление родительского объекта
        /// </summary>
        /// <param name="masterObject"></param>
        private void UpdateMasterObject(object masterObject)
        {
            _storage = (Storage)masterObject;
        }

        #endregion

        #region OnMasterObjectChanged

        /// <summary>
        /// Обновление нашего объекта при изменении мастер-объекта
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

            if (((ListView)View).CollectionSource is PropertyCollectionSource collectionSource)
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
            if (((ListView)View).CollectionSource is PropertyCollectionSource collectionSource)
            {
                collectionSource.MasterObjectChanged -= OnMasterObjectChanged;
            }
            base.OnDeactivated();
        }

        #endregion
    }
    #endregion
}
