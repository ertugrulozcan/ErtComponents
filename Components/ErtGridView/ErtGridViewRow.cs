using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Test.Docking.Components
{
    public class ErtGridViewRow : DependencyObject
    {
        #region Fields

        private bool isAlternate;
        private object data;

        #endregion

        #region Properties

        public bool IsAlternate
        {
            get
            {
                return isAlternate;
            }
            set
            {
                isAlternate = value;
            }
        }
        
        public object Data
        {
            get
            {
                return data;
            }

            set
            {
                data = value;
            }
        }

        #endregion

        #region Dependency Properties

        public double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }
        
        // Using a DependencyProperty as the backing store for Height.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(ErtGridViewRow), new PropertyMetadata(28d));
        

        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Background.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(ErtGridViewRow), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));
        

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(ErtGridViewRow), new PropertyMetadata(false));



        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        public ErtGridViewRow(object data)
        {
            this.Data = data;
        }

        #endregion
    }
}
