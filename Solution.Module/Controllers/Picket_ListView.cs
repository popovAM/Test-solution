using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using System;
using System.Linq;
using Solution.Module.BusinessObjects;
using System.Windows.Forms;

namespace Solution.Module.Controllers
{
    public partial class Picket_ListView : ViewController
    {
        private Storage _storage;
        public Picket_ListView()
        {
            InitializeComponent();

            // Кнопка создания пикета
            SimpleAction createPicket = new SimpleAction(this, "CreatePicketAction", PredefinedCategory.ObjectsCreation)
            {
                Caption = "Создать пикет",
                ImageName = "MenuBar_New"
            };

            // Кнопка удаления последнего пикета
            SimpleAction deletePicket = new SimpleAction(this, "DeletePicketAction", PredefinedCategory.ObjectsCreation)
            {
                Caption = "Удалить последний пикет",
                ImageName = "Action_Delete",
            };

            createPicket.Execute += CreatePicket_Execute;
            deletePicket.Execute += DeletePicket_Execute;
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
        /// Обновление нашего объекта при изменении родительского объекта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMasterObjectChanged(object sender, EventArgs e) => UpdateMasterObject(((PropertyCollectionSource)sender).MasterObject);

        protected override void OnActivated()
        {
            base.OnActivated();
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
        /// Создание пикета
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreatePicket_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var thisStorage = _storage;

            //Проверка на наличие текущего склада
            if (thisStorage != null)
            {
                //Создание нового пикета
                Picket newPicket = new Picket(((XPObjectSpace)ObjectSpace).Session);

                // Делаем выборку по пикетам, находим последний созданный
                var lastPicket = thisStorage.Pickets.Where(p => p.IsActive == true)
                .OrderByDescending(p => p.Number)
                .FirstOrDefault();
                newPicket.Storage = thisStorage;

                // Задаем номер пикета, исходя из запроса
                newPicket.Number = lastPicket != null ? lastPicket.Number + 1 : 1;

                //Сохранение изменений
                ObjectSpace.SetModified(View.CurrentObject, View.ObjectTypeInfo.FindMember(nameof(Storage.Pickets)));
                ObjectSpace.CommitChanges();
            }
        }

        /// <summary>
        /// Удаление пикета
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeletePicket_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var thisStorage = _storage;

            //Проверка на наличие текущего склада 
            if (thisStorage != null)
            {
                //Выборка последнего пикета по дате создания
                var lastPicket = thisStorage.Pickets.Where(p => p.IsActive == true)
                    .OrderByDescending(p => p.Number)
                    .FirstOrDefault();

                //Проверка на наличие последнего пикета
                if (lastPicket != null)
                {
                    // Удаление пикета, если он без площадки
                    if (lastPicket.Platform == null)
                    {
                        lastPicket.IsActive = false;
                        ObjectSpace.SetModified(View.CurrentObject, View.ObjectTypeInfo.FindMember(nameof(Storage.Pickets)));
                        ObjectSpace.CommitChanges();
                    }
                    else
                    {
                        // Окно об ошибке, если пикет включен в площадку
                        var result = XtraMessageBox.Show(
                            "Пикет находится на площадке или на нём присутствует груз.",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Question
                        );
                    }
                }
            }
        }
    }
}
