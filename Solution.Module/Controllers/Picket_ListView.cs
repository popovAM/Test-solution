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

        #region Fields

        private Storage _storage;

        #endregion

        #region Constructor

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

        #endregion

        #region Methods

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

        #region CreatePicket_Execute

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
            }

            //Сохранение изменений
            if (ObjectSpace.IsModified)
            {
                ObjectSpace.CommitChanges();
                ObjectSpace.Refresh();
            }
        }
        #endregion

        #region DeletePicket_Execute
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

                //Сохранение изменений
                if (ObjectSpace.IsModified)
                {
                    ObjectSpace.CommitChanges();
                    ObjectSpace.Refresh();
                }
            }
        } 
        #endregion

        #endregion

    }
}
