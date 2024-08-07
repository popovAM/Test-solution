using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
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
    /// Справочник груза
    /// </summary>
    [DefaultClassOptions]
    [Appearance("NotEnabledFields", TargetItems = "*", Context = "DetailView", Enabled = false, Criteria = "!IsNewObject(this)")]
    public class Cargo : BaseObject
    {
        #region Constructor
        public Cargo(Session session)
            : base(session)
        {
        }
        #endregion

        #region Fields
        private Type _cargoType;
        private string _name;
        #endregion

        #region Properties
        /// <summary>
        /// Название груза
        /// </summary>
        [Index(0)]
        [RuleRequiredField(DefaultContexts.Save)]
        public string Name
        {
            get { return _name; }
            set { SetPropertyValue(nameof(Name), ref _name, value); }
        }
        
        /// <summary>
        /// Тип груза
        /// </summary>
        [Index(1)]
        public Type CargoType
        {
            get { return _cargoType; }
            set { SetPropertyValue(nameof(CargoType), ref _cargoType, value); }
        }
        #endregion

        #region Enums
        /// <summary>
        /// Набор типов грузов
        /// </summary>
        public enum Type
        {
            Undefined,
            Gaseous,
            Bulk,
            Liquid
        }
        #endregion
    }
}