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
    /// Документ связей грузов и пикетов
    /// </summary>
    [DefaultClassOptions]
    
    public class CargoPicket : BaseObject
    {
        public CargoPicket(Session session)
            : base(session)
        {
        }
        
        private Cargo _cargo;
        private decimal _weight;
        private Picket _picket;

        /// <summary>
        /// Груз
        /// </summary>
        public Cargo Cargo
        {
            get { return _cargo; }
            set { SetPropertyValue(nameof(Cargo), ref _cargo, value); }
        }

        /// <summary>
        /// Вес груза
        /// </summary>
        [ModelDefault("EditMask", "#,###,###,###,###.###")]
        [ModelDefault("DisplayFormat", "{0:#,###,###,###,###.###}")]
        public decimal Weight
        {
            get { return _weight; }
            set { SetPropertyValue(nameof(Weight), ref _weight, value); }
        }

        /// <summary>
        /// Пикет, на котором находится груз
        /// </summary>
        [Association("Picket-CargoPickets")]
        public Picket Picket
        {
            get { return _picket; }
            set { SetPropertyValue(nameof(Picket), ref _picket, value); }
        }
    }
}