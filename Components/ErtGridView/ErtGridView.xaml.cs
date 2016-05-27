using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Test.Docking.Components
{
    /// <summary>
    /// Interaction logic for ErtGridView.xaml
    /// </summary>
    public partial class ErtGridView : UserControl
    {
        #region Fields

        public delegate void DataSourceChangeEvent();
        public event DataSourceChangeEvent DataSourceChanged;
        
        #endregion

        #region Properties

        public Visibility ShowHeadersVisibility
        {
            get { return this.ShowHeaders ? Visibility.Visible : Visibility.Collapsed; }
        }
        
        #endregion

        #region Dependency Properties

        /// <summary>
        /// Arkaplan rengi
        /// </summary>
        public Brush BackgroundColor
        {
            get { return (Brush)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OutlineBorderBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register("BackgroundColor", typeof(Brush), typeof(ErtGridView), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        /// <summary>
        /// En dış border rengi
        /// </summary>
        public Brush OutlineBorderBrush
        {
            get { return (Brush)GetValue(OutlineBorderBrushProperty); }
            set { SetValue(OutlineBorderBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OutlineBorderBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OutlineBorderBrushProperty =
            DependencyProperty.Register("OutlineBorderBrush", typeof(Brush), typeof(ErtGridView), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        /// <summary>
        /// En dış border kalınlığı
        /// </summary>
        public Thickness OutlineBorderThickness
        {
            get { return (Thickness)GetValue(OutlineBorderThicknessProperty); }
            set { SetValue(OutlineBorderThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OutlineBorderThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OutlineBorderThicknessProperty =
            DependencyProperty.Register("OutlineBorderThickness", typeof(Thickness), typeof(ErtGridView), new PropertyMetadata(new Thickness(0)));

        /// <summary>
        /// En dış border köşe yuvarlaklığı
        /// </summary>
        public CornerRadius OutlineCornerRadius
        {
            get { return (CornerRadius )GetValue(OutlineCornerRadiusProperty); }
            set { SetValue(OutlineCornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OutlineCornerRadiusProperty =
            DependencyProperty.Register("OutlineCornerRadiusProperty", typeof(CornerRadius), typeof(ErtGridView), new PropertyMetadata(new CornerRadius(0)));


        /// <summary>
        /// Başlık barı görünsün mü?
        /// </summary>
        public bool ShowHeaders
        {
            get { return (bool)GetValue(ShowHeadersProperty); }
            set { SetValue(ShowHeadersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowHeaders.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowHeadersProperty =
            DependencyProperty.Register("ShowHeaders", typeof(bool), typeof(ErtGridViewColumn), new PropertyMetadata(true));
        
        /// <summary>
        /// Başlık barı yüksekliği
        /// </summary>
        public GridLength HeaderRowHeight
        {
            get { return (GridLength)GetValue(HeaderRowHeightProperty); }
            set { SetValue(HeaderRowHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderRowHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderRowHeightProperty =
            DependencyProperty.Register("HeaderRowHeight", typeof(GridLength), typeof(ErtGridView), new PropertyMetadata(new GridLength(28)));


        /// <summary>
        /// Başlık barı arkaplan rengi
        /// </summary>
        public Brush HeaderRowBackground
        {
            get { return (Brush)GetValue(HeaderRowBackgroundProperty); }
            set { SetValue(HeaderRowBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderRowBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderRowBackgroundProperty =
            DependencyProperty.Register("HeaderRowBackground", typeof(Brush), typeof(ErtGridView), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));



        public Brush HeaderCellHoverBrush
        {
            get { return (Brush)GetValue(HeaderCellHoverBrushProperty); }
            set { SetValue(HeaderCellHoverBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderCellHoverBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderCellHoverBrushProperty =
            DependencyProperty.Register("HeaderCellHoverBrush", typeof(Brush), typeof(ErtGridView), new PropertyMetadata(new SolidColorBrush(Colors.WhiteSmoke)));



        /// <summary>
        /// Başlık barı yazı rengi
        /// </summary>
        public Brush HeaderRowForeground
        {
            get { return (Brush)GetValue(HeaderRowForegroundProperty); }
            set { SetValue(HeaderRowForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderRowForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderRowForegroundProperty =
            DependencyProperty.Register("HeaderRowForeground", typeof(Brush), typeof(ErtGridView), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
        

        public Brush HeaderGridLinesBrush
        {
            get { return (Brush)GetValue(HeaderGridLinesBrushProperty); }
            set { SetValue(HeaderGridLinesBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderGridLinesBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderGridLinesBrushProperty =
            DependencyProperty.Register("HeaderGridLinesBrush", typeof(Brush), typeof(ErtGridView), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
        
        /// <summary>
        /// Sütunlar
        /// </summary>
        public ObservableCollection<ErtGridViewColumn> Columns
        {
            get { return (ObservableCollection<ErtGridViewColumn>)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Columns.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(ObservableCollection<ErtGridViewColumn>), typeof(ErtGridView), new PropertyMetadata(new ObservableCollection<ErtGridViewColumn>()));



        public Brush ColumnHoverBrush
        {
            get { return (Brush)GetValue(ColumnHoverBrushProperty); }
            set { SetValue(ColumnHoverBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnHoverBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnHoverBrushProperty =
            DependencyProperty.Register("ColumnHoverBrush", typeof(Brush), typeof(ErtGridView), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(20, 0, 0, 0))));



        /// <summary>
        /// ItemsSource
        /// </summary>
        public IEnumerable DataSource
        {
            get { return (IEnumerable)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(IEnumerable), typeof(ErtGridView), new PropertyMetadata(new ObservableCollection<object>(), DataSourceChangedCallback));
        
        /// <summary>
        /// Satırlar
        /// </summary>
        public ObservableCollection<ErtGridViewRow> Rows
        {
            get { return (ObservableCollection<ErtGridViewRow>)GetValue(RowsProperty); }
            private set { SetValue(RowsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Rows.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register("Rows", typeof(ObservableCollection<ErtGridViewRow>), typeof(ErtGridView), new PropertyMetadata(new ObservableCollection<ErtGridViewRow>()));

        
        public ErtGridViewRow SelectedRow
        {
            get { return (ErtGridViewRow)GetValue(SelectedRowProperty); }
            set { SetValue(SelectedRowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedRow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedRowProperty =
            DependencyProperty.Register("SelectedRow", typeof(ErtGridViewRow), typeof(ErtGridView), new PropertyMetadata(null));
        

        /// <summary>
        /// Varsayılan tek satır yüksekliği
        /// </summary>
        public double RowHeight
        {
            get { return (double)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.Register("RowHeight", typeof(double), typeof(ErtGridView), new PropertyMetadata(28d));

        /// <summary>
        /// Bir hücrenin fare ile üzerine gelindiğinde alacağı renk
        /// </summary>
        public Brush CellHoverBrush
        {
            get { return (Brush)GetValue(CellHoverBrushProperty); }
            set { SetValue(CellHoverBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellHoverBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellHoverBrushProperty =
            DependencyProperty.Register("CellHoverBrush", typeof(Brush), typeof(ErtGridView), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 86, 86, 86))));

        /// <summary>
        /// Varsayılan satır arkaplanı
        /// </summary>
        public Brush RowBackground
        {
            get { return (Brush)GetValue(RowBackgroundProperty); }
            set { SetValue(RowBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowBackgroundProperty =
            DependencyProperty.Register("RowBackground", typeof(Brush), typeof(ErtGridView), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));
        
        public Brush SelectedRowBackground
        {
            get { return (Brush)GetValue(SelectedRowBackgroundProperty); }
            set { SetValue(SelectedRowBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedRowBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedRowBackgroundProperty =
            DependencyProperty.Register("SelectedRowBackground", typeof(Brush), typeof(ErtGridView), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));



        /// <summary>
        /// Alternatif satır arkaplanı (zebra görünümü için)
        /// </summary>
        public Brush AlternateRowBackground
        {
            get { return (Brush)GetValue(AlternateRowBackgroundProperty); }
            set { SetValue(AlternateRowBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AlternateRowBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AlternateRowBackgroundProperty =
            DependencyProperty.Register("AlternateRowBackground", typeof(Brush), typeof(ErtGridView), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));
        
        public int AlternationCount
        {
            get { return (int)GetValue(AlternationCountProperty); }
            set { SetValue(AlternationCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AlternationCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AlternationCountProperty =
            DependencyProperty.Register("AlternationCount", typeof(int), typeof(ErtGridView), new PropertyMetadata(1));



        /// <summary>
        /// Dikey çizgilerin rengi
        /// </summary>
        public Brush VerticalGridLinesBrush
        {
            get { return (Brush)GetValue(VerticalGridLinesBrushProperty); }
            set { SetValue(VerticalGridLinesBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalGridLinesBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalGridLinesBrushProperty =
            DependencyProperty.Register("VerticalGridLinesBrush", typeof(Brush), typeof(ErtGridView), new PropertyMetadata(new SolidColorBrush(Colors.Black)));



        public Brush HorizontalGridLinesBrush
        {
            get { return (Brush)GetValue(HorizontalGridLinesBrushProperty); }
            set { SetValue(HorizontalGridLinesBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalGridLinesBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalGridLinesBrushProperty =
            DependencyProperty.Register("HorizontalGridLinesBrush", typeof(Brush), typeof(ErtGridView), new PropertyMetadata(new SolidColorBrush(Colors.Black)));



        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ErtGridView()
        {
            this.DataSourceChanged += () => { };
            this.Loaded += ErtGridView_Loaded;

            InitializeComponent();
        }

        #endregion

        #region Methods

        private void ErtGridView_DataSourceChanged()
        {
            this.GenerateRows();
        }

        private void ErtGridView_Loaded(object sender, RoutedEventArgs e)
        {
            this.GenerateRows();
            this.DataSourceChanged += ErtGridView_DataSourceChanged;
        }

        /// <summary>
        /// Satır içeriği default yükseklik değerinden daha büyükse tüm satırı o yüksekliğe set eder
        /// </summary>
        private void RowBorder_Loaded(object sender, RoutedEventArgs e)
        {
            Border border = sender as Border;
            ErtGridViewRow row = border.DataContext as ErtGridViewRow;
            if (border.ActualHeight > row.Height)
                row.Height = border.ActualHeight;

            var binding = new Binding("Height");
            BindingOperations.SetBinding(border, Border.HeightProperty, binding);
        }

        private void OnCellClicked(object sender, MouseButtonEventArgs e)
        {
            Border rowBorder = sender as Border;
            ErtGridViewRow row = rowBorder.DataContext as ErtGridViewRow;

            if (this.SelectedRow != null)
                this.SelectedRow.IsSelected = false;
            row.IsSelected = true;

            this.SelectedRow = row;
        }

        /// <summary>
        /// Satırları oluşturur, alternate satırları ayarlar ve boyutlarını default'a set eder
        /// </summary>
        private void GenerateRows()
        {
            if (this.DataSource == null)
            {
                this.Rows = new ObservableCollection<ErtGridViewRow>();
                return;
            }

            ObservableCollection<ErtGridViewRow> rows = new ObservableCollection<ErtGridViewRow>();

            int validAlternationCount = 1;
            if (this.AlternationCount > 0)
                validAlternationCount = this.AlternationCount;

            bool alternateToggle = false;
            int altIndex = 1;

            foreach(var data in this.DataSource)
            {
                var row = new ErtGridViewRow(data) { IsAlternate = alternateToggle };
                
                if(altIndex >= validAlternationCount)
                {
                    alternateToggle = !alternateToggle;
                    altIndex = 0;
                }

                altIndex++;
                rows.Add(row);
            }

            this.Rows = rows;
        }

        #endregion

        #region Callback Methods

        private static void DataSourceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as ErtGridView;
            self.DataSourceChanged();
        }

        #endregion
    }
}
