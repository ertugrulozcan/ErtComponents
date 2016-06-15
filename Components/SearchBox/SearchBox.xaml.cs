using Eigen.Core.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace Eigen.Core.Components
{
    /// <summary>
    /// Interaction logic for SearchBox.xaml
    /// </summary>
    public partial class SearchBox : UserControl, INotifyPropertyChanged
    {
        #region Constants

        #endregion

        #region Fields

        private SearchEngine engine;
        private ObservableRangeCollection<KeyValuePair<string, object>> searchResults;

        public delegate void DataSourceChangeEvent();
        public event DataSourceChangeEvent DataSourceChanged;

        #endregion

        #region Properties

        public SearchEngine Engine
        {
            get
            {
                return engine;
            }

            private set
            {
                engine = value;
            }
        }

        public ObservableRangeCollection<KeyValuePair<string, object>> SearchResults
        {
            get
            {
                return searchResults;
            }

            set
            {
                searchResults = value;
                this.RaisePropertyChanged("SearchResults");
            }
        }

        #endregion

        #region Dependency Properties

        public ICommand SelectionCommand
        {
            get { return (ICommand)GetValue(SelectionCommandProperty); }
            set { SetValue(SelectionCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectionCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionCommandProperty =
            DependencyProperty.Register("SelectionCommand", typeof(ICommand), typeof(SearchBox), new PropertyMetadata(null));

        public ICommand ClearCommand
        {
            get { return (ICommand)GetValue(ClearCommandProperty); }
            set { SetValue(ClearCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ClearCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClearCommandProperty =
            DependencyProperty.Register("ClearCommand", typeof(ICommand), typeof(SearchBox), new PropertyMetadata(null));

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
            DependencyProperty.Register("DataSource", typeof(IEnumerable), typeof(SearchBox), new PropertyMetadata(new ObservableCollection<object>(), DataSourceChangedCallback));

        public string SearchKeyPath
        {
            get { return (string)GetValue(SearchKeyPathProperty); }
            set { SetValue(SearchKeyPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SearchKeyPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchKeyPathProperty =
            DependencyProperty.Register("SearchKeyPath", typeof(string), typeof(SearchBox), new PropertyMetadata(null));

        public bool StaysOpenResults
        {
            get { return (bool)GetValue(StaysOpenResultsProperty); }
            set { SetValue(StaysOpenResultsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StaysOpenResults.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StaysOpenResultsProperty =
            DependencyProperty.Register("StaysOpenResults", typeof(bool), typeof(SearchBox), new PropertyMetadata(false));
        

        public bool IsSpellCheckerActive
        {
            get { return (bool)GetValue(IsSpellCheckerActiveProperty); }
            set { SetValue(IsSpellCheckerActiveProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSpellCheckerActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSpellCheckerActiveProperty =
            DependencyProperty.Register("IsSpellCheckerActive", typeof(bool), typeof(SearchBox), new PropertyMetadata(true));



        public Brush SearchBoxBackground
        {
            get { return (Brush)GetValue(SearchBoxBackgroundProperty); }
            set { SetValue(SearchBoxBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SearchBoxBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchBoxBackgroundProperty =
            DependencyProperty.Register("SearchBoxBackground", typeof(Brush), typeof(SearchBox), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 39, 39, 39))));



        public Brush PopupBackgroundBrush
        {
            get { return (Brush)GetValue(PopupBackgroundBrushProperty); }
            set { SetValue(PopupBackgroundBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PopupBackgroundBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PopupBackgroundBrushProperty =
            DependencyProperty.Register("PopupBackgroundBrush", typeof(Brush), typeof(SearchBox), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 51, 51, 51))));



        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public SearchBox()
        {
            this.DataContext = this;
            this.DataSourceChanged += () => { };
            this.Loaded += SearchBox_Loaded;

            this.SearchResults = new ObservableRangeCollection<KeyValuePair<string, object>>();

            InitializeComponent();

            EventManager.RegisterClassHandler(typeof(FrameworkElement), FrameworkElement.MouseLeftButtonDownEvent, new RoutedEventHandler(this.SearchResultsListBox_MouseLeftButtonDown));

            this.SearchTextBox.PreviewGotKeyboardFocus += (s, e) => { this.SearchTextBox.PreviewKeyDown += SearchTextBox_PreviewKeyDown; };
            this.SearchTextBox.PreviewLostKeyboardFocus += (s, e) => { this.SearchTextBox.PreviewKeyDown -= SearchTextBox_PreviewKeyDown; };
            this.SearchResultsListBox.PreviewGotKeyboardFocus += (s, e) => { this.SearchResultsListBox.PreviewKeyDown += SearchResultsListBox_PreviewKeyDown; };
            this.SearchResultsListBox.PreviewLostKeyboardFocus += (s, e) => { this.SearchResultsListBox.PreviewKeyDown -= SearchResultsListBox_PreviewKeyDown; };
        }

        #endregion

        #region Methods

        /// <summary>
        /// DataSource güncellediğinde yapılacak extra işlemler;
        /// </summary>
        private void SearchBox_DataSourceChanged()
        {

        }

        private void SearchBox_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataSourceChanged += SearchBox_DataSourceChanged;
        }

        private void Search(string text)
        {
            if (this.Engine == null)
                return;

            var resultDictionary = new ObservableRangeCollection<KeyValuePair<string, object>>();
            HashSet<object> results;

            if (text.Length > 0)
            {
                results = this.Engine.Search(text);
            }
            else
            {
                if (this.StaysOpenResults)
                    results = this.Engine.Tree.Root.Objects;
                else
                    results = new HashSet<object>();
            }

            foreach (var obj in results)
                resultDictionary.Add(new KeyValuePair<string, object>(obj.ToString(), obj));

            this.SearchResults = resultDictionary;

            this.RaisePropertyChanged("SearchResultsListBox");
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.Search(this.SearchTextBox.Text);
        }

        #endregion

        #region Callback Methods

        private static void DataSourceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as SearchBox;

            if (self.SearchKeyPath == null)
                self.Engine = new SearchEngine(self.DataSource, self.IsSpellCheckerActive);
            else
                self.Engine = new SearchEngine(self.DataSource, self.SearchKeyPath, self.IsSpellCheckerActive);

            self.DataSourceChanged();
        }

        private void SearchResultsListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            char c;
            char.TryParse(e.Key.ToString(), out c);

            if (e.Key == Key.Up && this.SearchResultsListBox.SelectedIndex == 0)
            {
                Keyboard.Focus(this.SearchTextBox);
                this.SearchResultsListBox.SelectedItem = null;
            }
            else if ((e.Key != Key.Up && e.Key != Key.Down) && (char.IsLetterOrDigit(c) || e.Key == Key.Back))
            {
                Keyboard.Focus(this.SearchTextBox);
                this.SearchResultsListBox.SelectedItem = null;
            }
        }

        private void SearchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down && this.SearchResultsListBox.HasItems)
            {
                e.Handled = true;
                this.SearchResultsListBox.SelectedIndex = 0;
                (this.SearchResultsListBox.ItemContainerGenerator.ContainerFromItem(this.SearchResultsListBox.SelectedItem) as FrameworkElement).Focus();
            }
        }

        private void SearchResultsListBox_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            var dataContext = (sender as FrameworkElement).DataContext;
            if (dataContext is KeyValuePair<string, object>)
            {
                KeyValuePair<string, object> selectedResult = (KeyValuePair<string, object>)dataContext;
                if (this.SelectionCommand != null && this.SelectionCommand.CanExecute(selectedResult.Value))
                    this.SelectionCommand.Execute(selectedResult.Value);

                this.SearchResults.Clear();
            }
        }

        private void SearchResultsListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key & Key.Enter) == Key.Enter)
            {
                ItemSelected();
            }
        }

        public void ItemSelected()
        {
            if (this.SearchResultsListBox.SelectedValue != null)
            {
                KeyValuePair<string, object> selectedResult = (KeyValuePair<string, object>)this.SearchResultsListBox.SelectedValue;
                if (this.SelectionCommand != null && this.SelectionCommand.CanExecute(selectedResult.Value))
                {
                    this.SelectionCommand.Execute(selectedResult.Value);

                    this.RaisePropertyChanged("Text");
                    /*
                    if (selectedResult.Value is Infrastructure.Model.SymbolMenuItem)
                        this.Text = (selectedResult.Value as Infrastructure.Model.SymbolMenuItem).Name;
                    */
                }
            }

            this.SearchResults.Clear();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            this.SearchTextBox.Text = string.Empty;
            if (this.ClearCommand != null && this.ClearCommand.CanExecute(null))
                this.ClearCommand.Execute(null);
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {

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
