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
    public class PlatformAuditTrail : BaseObject
    { 
        public PlatformAuditTrail(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        private DateTime _timeOperation;
        private PlatformStatus _status;
        private int _storage;
        private string _platform;


        /// <summary>
        /// Платформа (головной объект)
        /// </summary>        
        public string Platform
        {
            get { return _platform; }
            set { SetPropertyValue(nameof(Platform), ref _platform, value); }
        }

        /// <summary>
        /// Время операции
        /// </summary>
        //[Index()]
        [ModelDefault("AllowEdit", "False")]
        [ModelDefault("DisplayFormat", "{0:dd.MM.yyyy HH:mm:ss}")]
        [ModelDefault("EditFormat", "dd.MM.yyyy HH:mm:ss")]
        [ModelDefault("EditMask", "dd.MM.yyyy HH:mm:ss")]
        public DateTime TimeOperation
        {
            get { return _timeOperation; }
            set { SetPropertyValue(nameof(TimeOperation), ref _timeOperation, value); }
        }

        /// <summary>
        /// Склад
        /// </summary>
        public int Storage
        {
            get { return _storage; }
            set { SetPropertyValue(nameof(Storage), ref _storage, value); }
        }

        /// <summary>
        /// Статус платформы
        /// </summary>
        public PlatformStatus Status
        {
            get { return _status; }
            set { SetPropertyValue(nameof(PlatformStatus), ref _status, value); }
        } 

        /// <summary>
        /// Статус платформы
        /// </summary>
        [Flags]
        public enum PlatformStatus
        {
            Created = 0b_0000_0000,
            Deleted = 0b_0000_0001
        }
    }
}