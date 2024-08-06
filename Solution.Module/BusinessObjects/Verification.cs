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
    [DefaultClassOptions]
    [ListViewFilter("Only Active", "[IsActive] = true")]
    public class Verification : BaseObject
    { 
        public Verification(Session session)
            : base(session)
        {
        }

        private bool _isActive = true;

        /// <summary>
        /// Проверка на то, является ли объект активным
        /// </summary>
        //[VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        [VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(true)]
        public bool IsActive
        {
            get { return _isActive; }
            set { SetPropertyValue(nameof(IsActive), ref _isActive, value); }
        }
    }
}