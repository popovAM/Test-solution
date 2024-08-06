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
    /// Документ связей грузов и пикетов
    /// </summary>
    [DefaultClassOptions]

    public class CargoPicket : Verification
    {
        public CargoPicket(Session session)
            : base(session)
        {
        }

        private Cargo _cargo;
        private decimal _weight;
        private Picket _picket;
        private decimal _previousWeight;
        private OperationType _status;

        /// <summary>
        /// Груз
        /// </summary>
        [Index(1)]
        public Cargo Cargo
        {
            get { return _cargo; }
            set { SetPropertyValue(nameof(Cargo), ref _cargo, value); }
        }

        /// <summary>
        /// Вес груза
        /// </summary>
        [Index(2)]
        [ModelDefault("EditMask", "#,###,###,###,###.###")]
        [ModelDefault("DisplayFormat", "{0:#,###,###,###,###.###}")]
        [DetailViewLayout(LayoutColumnPosition.Left)]
        [RuleRequiredField("RuleRequiredField for CargoPicket.Weight", DefaultContexts.Save, "Weight cannot be empty.", SkipNullOrEmptyValues = false)]
        [RuleValueComparison("", DefaultContexts.Save, ValueComparisonType.GreaterThanOrEqual, "0")]
        public decimal Weight
        {
            get { return _weight; }
            set { SetPropertyValue(nameof(Weight), ref _weight, value); }
        }

        /// <summary>
        /// Пикет, на котором находится груз
        /// </summary>
        [Index(0)]
        [Association("Picket-CargoPickets")]
        [DataSourceCriteria("[Platform] is not null")]
        public Picket Picket
        {
            get { return _picket; }
            set { SetPropertyValue(nameof(Picket), ref _picket, value); }
        }

        /// <summary>
        /// Свойство для хранения предыдущего веса
        /// </summary>
        [ModelDefault("AllowEdit", "False")]
        [VisibleInListView(false), VisibleInDetailView(false)]
        public decimal PreviousWeight
        {
            get { return _previousWeight; }
            set { SetPropertyValue(nameof(PreviousWeight), ref _previousWeight, value); }
        }

        public OperationType Status
        {
            get { return _status; }
            set { SetPropertyValue(nameof(Status), ref _status, value); }
        }

        public enum OperationType
        {
            inflow,
            outflow
        }

    }
}