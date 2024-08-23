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
    /// Склад
    /// </summary>
    [DefaultClassOptions]
    [DefaultProperty(nameof(Name))]
    [ListViewFilter("Only Active", "[IsActive] = true", true)]
    public class Storage : BaseObject, IActive
    {
        #region Constructor
        public Storage(Session session)
            : base(session)
        {
        }
        #endregion

        #region Fields
        private int _name;
        private bool _isActive = true;
        #endregion

        #region Properties
        /// <summary>
        /// Название склада
        /// </summary>
        [ModelDefault("AllowEdit", "false")]
        public int Name
        {
            get { return _name; }
            set { SetPropertyValue(nameof(Name), ref _name, value); }
        }

        /// <summary>
        /// Коллекция пикетов склада
        /// </summary>
        [DevExpress.Xpo.Aggregated, Association("Storage-Pickets")]
        public XPCollection<Picket> Pickets
        {
            get { return GetCollection<Picket>(nameof(Pickets)); }
        }

        /// <summary>
        /// Коллекция площадок склада
        /// </summary>
        [DevExpress.Xpo.Aggregated, Association("Storage-Platforms")]
        public XPCollection<Platform> Platforms
        {
            get { return GetCollection<Platform>(nameof(Platforms)); }
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