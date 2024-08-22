using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Solution.Module.BusinessObjects
{
    /// <summary>
    /// Базовый класс с проверкой активности
    /// </summary>
    [DefaultClassOptions]
    [ListViewFilter("Only Active", "[IsActive] = true", true)]
    public class Verification : BaseObject
    {
        #region Constructor
        public Verification(Session session)
            : base(session)
        {
        }
        #endregion

        #region Fields
        private bool _isActive = true;
        #endregion

        #region Propeties
        /// <summary>
        /// Проверка на то, является ли объект активным
        /// </summary>
        [VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public bool IsActive
        {
            get { return _isActive; }
            set { SetPropertyValue(nameof(IsActive), ref _isActive, value); }
        }
        #endregion
    }
}