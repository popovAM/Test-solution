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
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;

namespace Solution.Module.BusinessObjects
{
    /// <summary>
    /// Площадка
    /// </summary>
    [DefaultClassOptions]
    public class Platform : Verification
    {
        #region Constructor
        public Platform(Session session)
            : base(session)
        {
        }
        #endregion

        #region Fields
        private string _name;
        private Storage _storage;
        #endregion

        #region Properties
        /// <summary>
        /// Название площадки
        /// </summary>
        [Index(0)]
        public string Name
        {
            get { return _name; }
            set { SetPropertyValue(nameof(Name), ref _name, value); }
        }

        [Association("Platform-Audits")]
        public XPCollection<PlatformAuditTrail> PlatformAudits
        {
            get { return GetCollection<PlatformAuditTrail>(nameof(PlatformAudits)); }
        }

        /// <summary>
        /// Склад, к которому относится площадка
        /// </summary>
        [Association("Storage-Platforms")]
        [VisibleInListView(false)]
        public Storage Storage
        {
            get { return _storage; }
            set { SetPropertyValue(nameof(Storage), ref _storage, value); }
        }

        /// <summary>
        /// Список пикетов
        /// </summary>
        [Association("Platforms-Pickets")]
        [VisibleInListView(false)]
        public XPCollection<Picket> Pickets
        {
            get
            {
                return GetCollection<Picket>(nameof(Pickets));
            }
        }

        /// <summary>
        /// Общий вес площадки
        /// </summary>
        [Index(1)]
        [ImmediatePostData]
        [ModelDefault("EditMask", "#,###,###,###,###.###")]
        [ModelDefault("DisplayFormat", "{0:#,###,###,###,###.###}")]
        public decimal Weight
        {
            get
            {
                return Pickets.Sum(p => p.CargoPickets.Where(c => c.IsActive == true).Sum(c => c.Weight));
            }
        }
        #endregion
    }
}