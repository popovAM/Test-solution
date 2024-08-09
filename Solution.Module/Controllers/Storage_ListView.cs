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
    public partial class Storage_ListView : ViewController
    {
        #region Constructor

        public Storage_ListView()
        {
            InitializeComponent();

            // Кнопка создания склада
            SimpleAction createStorage = new SimpleAction(this, "createStorageAction", PredefinedCategory.ObjectsCreation)
            {
                Caption = "Создать склад",
                ImageName = "MenuBar_New"
            };
            createStorage.Execute += CreateStorage_Execute;

            SimpleAction deleteStorage = new SimpleAction(this, "deleteStorageAction", PredefinedCategory.ObjectsCreation)
            {
                Caption = "Удалить склад",
                ImageName = "MenuBar_Delete"
            };
            deleteStorage.Execute += DeleteStorage_Execute;
        }

        #endregion

        #region CreateStorage_Execute

        /// <summary>
        /// Создание склада
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateStorage_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            // Выборка склада по дате создания
            var lastStorage = ((XPObjectSpace)ObjectSpace).Session.Query<Storage>().Where(p => p.IsActive == true)
            .OrderByDescending(p => p.Name)
            .FirstOrDefault();

            // Создание пустого объекта 
            Storage newStorage = new Storage(((XPObjectSpace)ObjectSpace).Session);

            // Формирование названия объекта
            newStorage.Name = lastStorage != null ? lastStorage.Name + 1 : 1;

            //Сохранение изменений
            if (ObjectSpace.IsModified)
            {
                ObjectSpace.CommitChanges();
                ObjectSpace.Refresh();
            }
        }

        #endregion

        #region DeleteStorage_Execute

        private void DeleteStorage_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            Storage selectedStorage = (Storage)((XPObjectSpace)ObjectSpace).GetObject(View.CurrentObject);

            bool IsCorrect = true;

            foreach (var item in selectedStorage.Platforms)
            {
                if (item.IsActive == true && item.Weight != 0)
                    IsCorrect = false;
            }
            if (IsCorrect == true)
            {
                foreach (var item in selectedStorage.Platforms)
                {
                    item.IsActive = false;
                }
                foreach (var item in selectedStorage.Pickets)
                {
                    item.IsActive = false;
                }
                selectedStorage.IsActive = false;
            }

            //Сохранение изменений
            if (ObjectSpace.IsModified)
            {
                ObjectSpace.CommitChanges();
                ObjectSpace.Refresh();
            }
        }

        #endregion

    }
}
