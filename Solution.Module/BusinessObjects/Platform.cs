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
        public Platform(Session session)
            : base(session)
        {
        }

        private string _name;
        private Storage _storage;

        /// <summary>
        /// Название площадки
        /// </summary>
        [Index(0)]
        public string Name
        {
            get { return _name; }
            set { SetPropertyValue(nameof(Name), ref _name, value); }
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
                return Pickets.Sum(p => p.CargoPickets.Sum(c => c.Weight));
            }
        }
    }
}