using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stocks.Domain
{
    public abstract class DomainBase
    {
        #region Fields

        private bool _isDirty;
        private bool _isMarkedForDeletion;

        #endregion

        #region Properties
        /// <summary> 
        /// Property indicating whether entity has changed since it 
        /// was retrieved from database. 
        /// </summary> 
        
        public bool IsDirty
        { 
            get { return _isDirty; } 
            set { _isDirty = value; } 
        }

        /// <summary> 
        /// Property that can be set to cause entity to be deleted 
        /// from database when persisted. 
        /// </summary>
        
        public bool IsMarkedForDeletion
        { 
            get { return _isMarkedForDeletion; }
            set { _isMarkedForDeletion = value; }
        }

        #endregion
    }
}
