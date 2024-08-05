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
        /// <summary>
        /// Родительский объект Склад
        /// </summary>
        private Storage _storage;

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

        /// <summary>
        /// Создание площадки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreatePlatform_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            //Создание диалогового окна со списком пикетов, которые принадлежат текущему складу
            var collectionSource = new CollectionSource(ObjectSpace, typeof(Picket));
            collectionSource.Criteria["ThisStorage"] = CriteriaOperator.Parse($"[Storage].Oid == '{_storage.Oid}'");
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
                    while (currentObject.Pickets.Count != 0)
                    {
                        currentObject.Pickets[0].Platform = null;
                    }

                    string platformName = currentObject.Name;
                    int storageNumber = currentObject.Storage.Name;

                    //Удаление площадки и сохранение изменений
                    CreateNewRecord(currentObject, false);
                    ObjectSpace.Delete(currentObject);
                    ObjectSpace.SetModified(View.CurrentObject, View.ObjectTypeInfo.FindMember(nameof(Storage.Platforms)));
                    ObjectSpace.CommitChanges();

                }
            }
            //Если на площадке присутствует груз 
            else if (currentObject.Weight != 0)
                throw new UserFriendlyException("На платформе находится груз.");
        }

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
                    newPlatform.Pickets.Add(item);
                newPlatform.Name = selectedPickets[0].Number.ToString() + "-" + selectedPickets[selectedPickets.Count - 1].Number.ToString();

                //Сохраняем изменения
                CreateNewRecord(newPlatform, true);
                ObjectSpace.CommitChanges();

            }
            //Если список неправильный
            else
            {
                throw new UserFriendlyException("Площадка должна быть не занята и не разрывна");
            }
        }

        /// <summary>
        /// Создание новой записи в журнале изменений
        /// </summary>
        /// <param name="platformName"></param>
        /// <param name="storageNumber"></param>
        /// <param name="isCreated"></param>
        private void CreateNewRecord(Platform currentObject, bool isCreated)
        {
            //Создание новой записи и заполнение данных 
            var context = Application.CreateObjectSpace(typeof(PlatformAuditTrail));
            PlatformAuditTrail newRecord = new PlatformAuditTrail(((XPObjectSpace)context).Session)
            {
                TimeOperation = DateTime.Now,
                Platform = currentObject.Name,
                Storage = currentObject.Storage.Name
            };

            //Проверка статуса площадки (создана или расформирована)
            if (isCreated)
            {
                newRecord.Status = PlatformAuditTrail.PlatformStatus.Created;
            }
            else
            {
                newRecord.Status = PlatformAuditTrail.PlatformStatus.Deleted;
            }

            //Сохранение изменений
            context.CommitChanges();
            context.Refresh();
        }

        /// <summary>
        /// Обновление родительского объекта
        /// </summary>
        /// <param name="masterObject"></param>
        private void UpdateMasterObject(object masterObject)
        {
            _storage = (Storage)masterObject;
        }

        /// <summary>
        /// Обновление нашего объекта при изменении мастер-объекта
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

            if (((ListView)View).CollectionSource is PropertyCollectionSource collectionSource)
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
            if (((ListView)View).CollectionSource is PropertyCollectionSource collectionSource)
            {
                collectionSource.MasterObjectChanged -= OnMasterObjectChanged;
            }
            base.OnDeactivated();
        }
    }
}
