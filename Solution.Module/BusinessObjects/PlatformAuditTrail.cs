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
    /// Журнал изменений площадок
    /// </summary>
    [DefaultClassOptions]
    public class PlatformAuditTrail : BaseObject
    {
        #region Constructor
        public PlatformAuditTrail(Session session)
            : base(session)
        {
        }
        #endregion

        #region Fields
        private DateTime _timeOperation;
        private PlatformStatus _status;
        private Platform _platform;
        #endregion

        #region Properties
        /// <summary>
        /// Площадка (головной объект)
        /// </summary>
        [Index(1)]
        [Association("Platform-Audits")]
        public Platform Platform
        {
            get { return _platform; }
            set { SetPropertyValue(nameof(Platform), ref _platform, value); }
        }

        /// <summary>
        /// Время операции
        /// </summary>
        [Index(3)]
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
        [Index(0)]
        public int? Storage
        {
            get { return _platform?.Storage?.Name; }
        }

        /// <summary>
        /// Статус площадки
        /// </summary>
        [Index(2)]
        public PlatformStatus Status
        {
            get { return _status; }
            set { SetPropertyValue(nameof(PlatformStatus), ref _status, value); }
        }
        #endregion

        #region Enum
        /// <summary>
        /// Статус платформы
        /// </summary>
        [Flags]
        public enum PlatformStatus
        {
            Created = 0b_0000_0000,
            Deleted = 0b_0000_0001
        }
        #endregion
    }
}