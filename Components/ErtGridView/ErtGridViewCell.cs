using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Test.Docking.Components
{
    public class ErtGridViewCell : DependencyObject
    {
        #region Fields



        #endregion

        #region Properties

        #endregion

        #region Dependency Properties
        
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(ErtGridViewCell), new PropertyMetadata(null));


        #endregion

        #region Methods

        public override string ToString()
        {
            return this.Content.ToString();
        }

        #endregion
    }
}
