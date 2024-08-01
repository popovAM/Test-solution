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

namespace Solution.Module.BusinessObjects
{
    [DefaultClassOptions]
    public class Storage : BaseObject
    { 
        public Storage(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();            
        }

        private int _name;

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
        /// Коллекция пикетов склада
        /// </summary>
        [DevExpress.Xpo.Aggregated, Association("Storage-Platforms")]
        public XPCollection<Platform> Platforms
        {
            get { return GetCollection<Platform>(nameof(Platforms)); }
        }
    }
}