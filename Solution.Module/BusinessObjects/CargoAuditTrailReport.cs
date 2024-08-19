using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Xpo;
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
    [NonPersistent]
    [Appearance("EnabledEndDateTime", TargetItems = "EndDateTime", Context = "DetailView", Enabled = false , Criteria = "[BeginDateTime] is null")]
    public class CargoAuditTrailReport : BaseObject
    {
        #region Fields
        private DateTime _beginDateTime;
        private DateTime _endDateTime;
        private Storage _storage;
        //private DateTime DateTimeNow = DateTime.Now;
        #endregion

        #region Properties

        [ImmediatePostData]
        [RuleRequiredField(DefaultContexts.Save)]
        public DateTime BeginDateTime
        {
            get
            {
                return _beginDateTime;
            }
            set
            {
                SetPropertyValue(nameof(BeginDateTime), ref _beginDateTime, value);
            }
        }

        [ImmediatePostData]
        public DateTime EndDateTime
        {
            get
            {
                return _endDateTime;
            }
            set
            {
                SetPropertyValue(nameof(EndDateTime), ref _endDateTime, value);
            }
        }

        public Storage Storage
        {
            get 
            {
                return _storage;
            }
            set
            {
                SetPropertyValue(nameof(Storage), ref _storage, value);
            }
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
        #endregion

        #region Constructor
        public CargoAuditTrailReport(Session session) : base(session)
        {
        }
        #endregion

        #region INotifyPropertyChanged members (see http://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged(v=vs.110).aspx)
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}