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
    public class Report : BaseObject
    { 
        #region Fields
        private string _fileName;
        private DateTime _beginDateTime;
        private DateTime _endDateTime;
        private DateTime _creationDateTime;
        private string _user;
        #endregion

        #region Constructor
        public Report(Session session)
            : base(session)
        {
        }
        #endregion

        #region Events
        public override void AfterConstruction()
        {
            base.AfterConstruction();

            _creationDateTime = DateTime.Now;

            // Заполняем поле User названием пользователя
            _user = SecuritySystem.CurrentUserName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Название файла
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set { SetPropertyValue(nameof(FileName), ref _fileName, value); }
        }

        /// <summary>
        /// Дата и время начала 
        /// </summary>
        public DateTime BeginDateTime
        {
            get { return _beginDateTime; }
            set { SetPropertyValue(nameof(BeginDateTime), ref _beginDateTime, value); }
        }
        
        /// <summary>
        /// Дата и время конца
        /// </summary>
        public DateTime EndDateTime
        {
            get { return _endDateTime; }
            set { SetPropertyValue(nameof(EndDateTime), ref _endDateTime, value); }
        }

        /// <summary>
        /// Дата создания отчета
        /// </summary>
        [Browsable(false)]
        public DateTime CreationDateTime
        {
            get { return _creationDateTime; }
            set { SetPropertyValue(nameof(CreationDateTime), ref _creationDateTime, value); }
        }

        /// <summary>
        /// Пользователь, создавший отчет
        /// </summary>
        [Browsable(false)]
        public string User 
        {   
            get { return _user;}
            set { SetPropertyValue(nameof(User), ref _user, value); } 
        }
        #endregion
    }
}