using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Module.Interfaces
{
    /// <summary>
    /// Активный объект
    /// </summary>
    interface IActive
    {
        /// <summary>
        /// Проверка на активность
        /// </summary>
        bool IsActive { get; set; }
    }
}
