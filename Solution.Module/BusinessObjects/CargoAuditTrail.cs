using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
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
    [Appearance("ShowCreateDateTime", TargetItems = "CreateDateTime", Context = "ListView", Visibility = ViewItemVisibility.Show)]
    public class CargoAuditTrail : BaseObject
    {
        public CargoAuditTrail(Session session)
            : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        private decimal _weight;
        private string _picket;
        private string _cargo;
        private DateTime _timeOperation;
        private CargoStatus _operationType;

        /// <summary>
        /// Вес на определенный момент времени
        /// </summary>
        [Index(3)]
        public decimal Weight
        {
            get { return _weight; }
            set { SetPropertyValue(nameof(Weight), ref _weight, value); }
        }

        /// <summary>
        /// Площадка
        /// summary>
        [Index(1)]
        public string Picket
        {
            get { return _picket; }
            set { SetPropertyValue(nameof(Picket), ref _picket, value); }
        }

        /// <summary>
        /// Тип груза
        /// </summary>
        [Index(2)]
        public string Cargo
        {
            get { return _cargo; }
            set { SetPropertyValue(nameof(Cargo), ref _cargo, value); }
        }

        /// <summary>
        /// Время операции
        /// </summary>
        [Index(4)]
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
        /// Статус груза
        /// </summary>
        [Index(5)]
        public CargoStatus OperationType
        {
            get { return _operationType; }
            set { SetPropertyValue(nameof(OperationType), ref _operationType, value); }
        }
        [Flags]
        public enum CargoStatus
        {
            Загрузка = 0b_0000_0000,
            Выгрузка = 0b_0000_0001,
            Освобождение = 0b_0000_0010,
            Добавление = 0b_0000_0100
        }
    }
}