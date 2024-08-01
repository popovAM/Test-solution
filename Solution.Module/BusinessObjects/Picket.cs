﻿using DevExpress.Data.Filtering;
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
    /// <summary>
    /// Пикет
    /// </summary>
    [DefaultClassOptions]
    [DefaultProperty(nameof(Name))]
    public class Picket : BaseObject
    { 
        public Picket(Session session)
            : base(session)
        {
        }
        
        private Storage _storage;
        private Platform _platform;
        private bool _isFull;
        private int _number;

        /// <summary>
        /// Номер пикета, для формирования площадки
        /// </summary>
        [Index(1)]
        [VisibleInLookupListView(false)]
        public string Name
        {
            get 
            {
                return $"{Storage.Name}-({Number})";
            }
        }

        [Index(2)]
        [ModelDefault("AllowEdit", "false")]
        [VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(true)]
        public int Number
        {
            get { return _number; }
            set { SetPropertyValue(nameof(Number), ref _number, value); }
        }

        /// <summary>
        /// Склад, на котором находится пикет
        /// </summary>
        [Index(0)]
        [Association("Storage-Pickets")]
        [ImmediatePostData]
        [VisibleInListView(false), VisibleInLookupListView(true)]
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
        [Index(4)]
        [VisibleInListView(true), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public bool IsFull
        {
            get { return _isFull; }
            set { SetPropertyValue(nameof(IsFull), ref _isFull, value); }
        }

        /// <summary>
        /// Площадка, в которую включен пикет
        /// </summary>
        [Index(3)]
        [Association("Platform-Pickets")]
        [ModelDefault("AllowEdit", "false"), VisibleInLookupListView(true)]
        public Platform Platform
        {
            get { return _platform; }
            set { SetPropertyValue(nameof(Platform), ref _platform, value); }
        }

        ///<summary>
        /// Журнал расхода груза на площадке
        ///</summary>
        [Association("Picket-CargoPickets")]
        [VisibleInListView(false)]
        [ModelDefault("AllowEdit", "false")]
        public XPCollection<CargoPicket> CargoPickets
        {
            get { return GetCollection<CargoPicket>(nameof(CargoPickets)); }
        }
    }
}