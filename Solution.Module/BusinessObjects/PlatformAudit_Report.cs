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
    /// Отчёт для площадок
    /// </summary>
    [NonPersistent]
    public class PlatformAudit_Report : BaseObject
    {
        #region Constructor
        public PlatformAudit_Report(Session session)
            : base(session)
        {
        }
        #endregion

        #region Fields
        private DateTime _dateTime;
        private Storage _storage;
        #endregion

        #region Properties
        /// <summary>
        /// Время, по которому происходит выборка
        /// </summary>
        [ModelDefault("EditFormat", "dd.MM.yyyy HH:mm:ss")]
        [ModelDefault("EditMask", "dd.MM.yyyy HH:mm:ss")]
        [ModelDefault("DisplayFormat", "{0: dd.MM.yyyy hh:mm:ss}")]
        public DateTime DateTime
        {
            get { return _dateTime; }
            set { SetPropertyValue(nameof(DateTime), ref _dateTime, value); }
        }

        /// <summary>
        /// Склад, по которому происходит выборка
        /// </summary>
        public Storage Storage
        {
            get { return _storage; }
            set { SetPropertyValue(nameof(Storage), ref _storage, value); }
        }
        #endregion

        #region OnPropertyChanged
        private void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region INotifyPropertyChanged members (see http://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged(v=vs.110).aspx)
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #endregion
    }
}