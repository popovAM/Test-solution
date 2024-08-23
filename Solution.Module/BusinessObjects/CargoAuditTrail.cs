using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using static Solution.Module.BusinessObjects.CargoPicket;

namespace Solution.Module.BusinessObjects
{
    /// <summary>
    /// Журнал изменений грузов
    /// </summary>
    [DefaultClassOptions]
    [Appearance("ShowCreateDateTime", TargetItems = "CreateDateTime", Context = "ListView", Visibility = ViewItemVisibility.Show)]
    public class CargoAuditTrail : BaseObject
    {
        #region Constructor
        public CargoAuditTrail(Session session)
            : base(session)
        {
        }
        #endregion

        #region AfterConstruction
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _operationDateTime = DateTime.Now;
        }
        #endregion

        #region Fields
        private DateTime _operationDateTime;
        private CargoPicket _cargoPicket;
        private decimal _weight;
        #endregion

        #region Properties
        /// <summary>
        /// Вес на определенный момент времени
        /// </summary>
        [Index(3)]
        [ModelDefault("EditMask", "#,###,###,###,###.###")]
        [ModelDefault("DisplayFormat", "{0:#,###,###,###,###.###}")]
        [ModelDefault("AllowEdit", "false")]
        public decimal Weight
        {
            get { return _weight; }
            set { SetPropertyValue(nameof(Weight), ref _weight, value); }
        }

        /// <summary>
        /// Площадка
        /// summary>
        [Index(1)]
        public string Platform
        {
            get { return CargoPicket?.Picket?.Platform?.Name; }
        }

        /// <summary>
        /// Склад
        /// </summary>
        [Index(0)]
        public int? Storage
        {
            get { return CargoPicket?.Picket?.Storage?.Name; }
        }

        /// <summary>
        /// Время операции
        /// </summary>
        [Index(2)]
        [ModelDefault("AllowEdit", "False")]
        [ModelDefault("DisplayFormat", "{0:dd.MM.yyyy HH:mm:ss}")]
        [ModelDefault("EditFormat", "dd.MM.yyyy HH:mm:ss")]
        [ModelDefault("EditMask", "dd.MM.yyyy HH:mm:ss")]
        public DateTime OperationDateTime
        {
            get { return _operationDateTime; }
            set { SetPropertyValue(nameof(OperationDateTime), ref _operationDateTime, value); }
        }

        /// <summary>
        /// Груз на пикете, по которому создаются записи
        /// </summary>
        [Association("CargoPicket - CargoPicketAudits")]
        [VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public CargoPicket CargoPicket
        {
            get { return _cargoPicket; }
            set { SetPropertyValue(nameof(CargoPicket), ref _cargoPicket, value); }
        }

        #endregion
    }
}