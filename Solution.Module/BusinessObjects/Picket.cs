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
    public class Picket : BaseObject
    { 
        public Picket(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();            
        }

        private Storage _storage;
        private Platform _platform;
        private bool _isEmpty;
        private int _number;

        /// <summary>
        /// Номер пикета, для формирования площадки
        /// </summary>
        public int Number
        {
            get { return _number; }
            set { SetPropertyValue(nameof(Number), ref _number, value); }
        }

        /// <summary>
        /// Склад
        /// </summary>
        [Association("Storage-Pickets")]
        [VisibleInListView(false)]
        [ImmediatePostData]
        public Storage Storage
        {
            get { return _storage; }
            set
            {
                SetPropertyValue(nameof(Storage), ref _storage, value);
            }
        }

        /// <summary>
        /// Занят грузом или нет
        /// </summary>
        [VisibleInListView(false)]
        public bool IsEmpty
        {
            get { return _isEmpty; }
            set { SetPropertyValue(nameof(IsEmpty), ref _isEmpty, value); }
        }

        /// <summary>
        /// Площадка
        /// </summary>
        [Association("Platform-Pickets")]
        public Platform Platform
        {
            get { return _platform; }
            set { SetPropertyValue(nameof(Platform), ref _platform, value); }
        }
    }
}