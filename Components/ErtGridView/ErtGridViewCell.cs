using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Test.Docking.Components
{
    public class ErtGridViewCell : ContentControl, INotifyPropertyChanged
    {
        #region Fields

        private bool isSelected;

        #endregion

        #region Properties

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }

            set
            {
                isSelected = value;
                this.RaisePropertyChanged("IsSelected");
            }
        }

        #endregion

        #region Dependency Properties



        #endregion

        #region Methods

        public override string ToString()
        {
            return this.Content.ToString();
        }

        #endregion

        #region RaisePropertyChanged

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
