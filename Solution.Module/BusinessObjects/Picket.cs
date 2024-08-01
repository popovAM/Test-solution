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
    /// <summary>
    /// Пикет
    /// </summary>
    [DefaultClassOptions]
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
        [Index(0)]
        public int Number
        {
            get { return _number; }
            set { SetPropertyValue(nameof(Number), ref _number, value); }
        }

        /// <summary>
        /// Склад, на котором находится пикет
        /// </summary>
        [Association("Storage-Pickets")]
        [ImmediatePostData]
        [Index(1)]
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
        public bool IsFull
        {
            get { return _isFull; }
            set { SetPropertyValue(nameof(IsFull), ref _isFull, value); }
        }

        /// <summary>
        /// Площадка, в которую включен пикет
        /// </summary>
        [Index(2)]
        [Association("Platform-Pickets")]
        public Platform Platform
        {
            get { return _platform; }
            set { SetPropertyValue(nameof(Platform), ref _platform, value); }
        }
    }
}