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
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class Storage_ListView : ViewController
    {
        public Storage_ListView()
        {
            InitializeComponent();

            // Кнопка создания склада
            SimpleAction createStorage = new SimpleAction(this, "createStorageAction", PredefinedCategory.ObjectsCreation)
            {
                Caption = "Создать склад",
                ImageName = "MenuBar_New"
            };
            createStorage.Execute += createStorage_Execute;

            SimpleAction deleteStorage = new SimpleAction(this, "deleteStorageAction", PredefinedCategory.ObjectsCreation)
            {
                Caption = "Удалить склад",
                ImageName = "MenuBar_Delete"
            };
            deleteStorage.Execute += deleteStorage_Execute;
        }

        /// <summary>
        /// Создание склада
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createStorage_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            // Выборка склада по дате создания
            var lastStorage = ((XPObjectSpace)ObjectSpace).Session.Query<Storage>()
            .OrderByDescending(p => p.Name)
            .FirstOrDefault();

            // Создание пустого объекта 
            Storage newStorage = new Storage(((XPObjectSpace)ObjectSpace).Session);

            // Формирование названия объекта
            newStorage.Name = lastStorage != null ? lastStorage.Name + 1 : 1;

            //Сохранение изменений
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
        }

        /// <summary>
        /// Создание склада
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteStorage_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var collectionSource = new CollectionSource(ObjectSpace, typeof(Storage));
            var view = Application.CreateListView("Storage_ListView", collectionSource, false);
            
            // Выборка склада по дате создания
            var selectedStorage = view.SelectedObjects.OfType<Storage>().ToList();

            if (selectedStorage != null)
            {
                foreach (var storage in selectedStorage) {
                    bool isEmpty = true;
                    foreach (var picket in storage.Pickets)
                    {
                        if (picket.CargoPickets.Any())
                        {
                            isEmpty = false;
                            break;
                        }
                    }
                    if (isEmpty == true) ObjectSpace.Delete(storage);
                    else throw new UserFriendlyException("На пикетах находятся грузы.");
                }
            }
            //Сохранение изменений
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
        }
    }
}
