using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
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
using Solution.Module.Interfaces;
using DevExpress.ExpressApp.SystemModule;

namespace Solution.Module.BusinessObjects
{
    /// <summary>
    /// Документ связей грузов и пикетов
    /// </summary>
    [DefaultClassOptions]
    [ListViewFilter("Only Active", "[IsActive] = true", true)]
    public class CargoPicket : BaseObject, IActive
    {
        #region Constructor
        public CargoPicket(Session session)
            : base(session)
        {
        }
        #endregion

        #region Fields
        private Cargo _cargo;
        private decimal _weight;
        private Picket _picket;
        private OperationType _status;
        private bool _isActive = true;
        #endregion

        #region Properties
        /// <summary>
        /// Груз
        /// </summary>
        [Index(1)]
        public Cargo Cargo
        {
            get { return _cargo; }
            set { SetPropertyValue(nameof(Cargo), ref _cargo, value); }
        }

        /// <summary>
        /// Вес груза
        /// </summary>
        [Index(2)]
        [ModelDefault("EditMask", "#,###,###,###,###.###;")]
        [ModelDefault("DisplayFormat", "{0:#,###,###,###,###.###}")]
        [DetailViewLayout(LayoutColumnPosition.Left)]
        [RuleRequiredField("RuleRequiredField for CargoPicket.Weight", DefaultContexts.Save, "Weight cannot be empty.", SkipNullOrEmptyValues = false)]
        public decimal Weight
        {
            get { return _weight; }
            set { SetPropertyValue(nameof(Weight), ref _weight, value); }
        }

        /// <summary>
        /// Пикет, на котором находится груз
        /// </summary>
        [Index(0)]
        [Association("Picket-CargoPickets")]
        [DataSourceCriteria("[Platform] is not null")]
        public Picket Picket
        {
            get { return _picket; }
            set { SetPropertyValue(nameof(Picket), ref _picket, value); }
        }

        /// <summary>
        /// Статус груза ()
        /// </summary>
        [VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public OperationType Status
        {
            get { return _status; }
            set { SetPropertyValue(nameof(Status), ref _status, value); }
        }

        /// <summary>
        /// Журнал изменений груза
        /// </summary>
        [Association("CargoPicket - CargoPicketAudits")]
        [VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public XPCollection<CargoAuditTrail> CargoAuditTrails
        {
            get { return GetCollection<CargoAuditTrail>(nameof(CargoAuditTrails)); }
        }
        #endregion

        #region Enums
        /// <summary>
        /// Статус операций
        /// </summary>
        public enum OperationType
        {
            Inflow,
            Outflow
        }

        [VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public bool IsActive
        {
            get { return _isActive; }
            set { SetPropertyValue(nameof(IsActive), ref _isActive, value); }
        }

        #endregion
    }
}