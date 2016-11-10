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
using System.Windows.Threading;

namespace Eigen.Infrastructure.Components
{
    /// <summary>
    /// Interaction logic for SearchBox.xaml
    /// </summary>
    public partial class SearchBox : UserControl, INotifyPropertyChanged
    {
        #region Constants

        #endregion

        #region Fields

        private ObservableRangeCollection<KeyValuePair<string, object>> searchResults;

        #endregion

        #region Properties

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

        public SearchEngine Engine
        {
            get { return (SearchEngine)GetValue(EngineProperty); }
            set { SetValue(EngineProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Engine.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EngineProperty =
            DependencyProperty.Register("Engine", typeof(SearchEngine), typeof(SearchBox), new PropertyMetadata(null, SearchEngineChangedCallback));

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
        public IEnumerable<ISearchable> DataSource
        {
            get { return (IEnumerable<ISearchable>)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(IEnumerable<ISearchable>), typeof(SearchBox), new PropertyMetadata(new ObservableCollection<ISearchable>(), DataSourceChangedCallback));

        public string SearchKeyPath
        {
            get { return (string)GetValue(SearchKeyPathProperty); }
            set { SetValue(SearchKeyPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SearchKeyPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchKeyPathProperty =
            DependencyProperty.Register("SearchKeyPath", typeof(string), typeof(SearchBox), new PropertyMetadata(null));

        public string ResultKeyPath
        {
            get { return (string)GetValue(ResultKeyPathProperty); }
            set { SetValue(ResultKeyPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ResultKeyPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ResultKeyPathProperty =
            DependencyProperty.Register("ResultKeyPath", typeof(string), typeof(SearchBox), new PropertyMetadata(null));

        public bool StaysOpenResults
        {
            get { return (bool)GetValue(StaysOpenResultsProperty); }
            set { SetValue(StaysOpenResultsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StaysOpenResults.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StaysOpenResultsProperty =
            DependencyProperty.Register("StaysOpenResults", typeof(bool), typeof(SearchBox), new PropertyMetadata(false));

        public bool IsResultsCombined
        {
            get { return (bool)GetValue(IsResultsCombinedProperty); }
            set { SetValue(IsResultsCombinedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsResultsCombined.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsResultsCombinedProperty =
            DependencyProperty.Register("IsResultsCombined", typeof(bool), typeof(SearchBox), new PropertyMetadata(true));

        public SearchEngine.CombineMode CombineMode
        {
            get { return (SearchEngine.CombineMode)GetValue(CombineModeProperty); }
            set { SetValue(CombineModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CombineMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CombineModeProperty =
            DependencyProperty.Register("CombineMode", typeof(SearchEngine.CombineMode), typeof(SearchBox), new PropertyMetadata(SearchEngine.CombineMode.Intersection));




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



        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(SearchBox), new PropertyMetadata(new CornerRadius(3)));



        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Caption.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(string), typeof(SearchBox), new PropertyMetadata("Search"));



        public bool StaysFocus
        {
            get { return (bool)GetValue(StaysFocusProperty); }
            set { SetValue(StaysFocusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StaysFocus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StaysFocusProperty =
            DependencyProperty.Register("StaysFocus", typeof(bool), typeof(SearchBox), new PropertyMetadata(false));


        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public SearchBox()
        {
            this.DataContext = this;

            this.SearchResults = new ObservableRangeCollection<KeyValuePair<string, object>>();

            InitializeComponent();

            //EventManager.RegisterClassHandler((typeof(FrameworkElement), FrameworkElement.MouseLeftButtonDownEvent, new RoutedEventHandler(this.SearchResultsListBox_MouseLeftButtonDown));

            this.SearchTextBox.PreviewGotKeyboardFocus += SearchTextBox_PreviewGotKeyboardFocus;
            this.SearchTextBox.PreviewLostKeyboardFocus += SearchTextBox_PreviewLostKeyboardFocus;
            this.SearchResultsListBox.PreviewGotKeyboardFocus += SearchResultsListBox_PreviewGotKeyboardFocus;
            this.SearchResultsListBox.PreviewLostKeyboardFocus += SearchResultsListBox_PreviewLostKeyboardFocus;
        }

        #endregion

        #region Methods

        private void Search(string text)
        {
            if (this.Engine == null)
                return;

            var resultDictionary = new ObservableRangeCollection<KeyValuePair<string, object>>();
            HashSet<object> results;

            if (text.Length > 0)
            {
                results = this.Engine.Search(text, this.IsResultsCombined, this.CombineMode);
            }
            else
            {
                if (this.StaysOpenResults)
                    results = this.Engine.Tree.Root.Objects;
                else
                    results = new HashSet<object>();
            }

            foreach (var obj in results)
            {
                if (!string.IsNullOrEmpty(this.ResultKeyPath))
                {
                    var prop = SearchEngine.GetPropValue(obj, this.ResultKeyPath);
                    if (prop != null)
                    {
                        resultDictionary.Add(new KeyValuePair<string, object>(prop.ToString(), obj));
                    }
                    else
                    {
                        resultDictionary.Add(new KeyValuePair<string, object>(obj.ToString(), obj));
                    }
                }
                else if (string.IsNullOrEmpty(this.ResultKeyPath) && !string.IsNullOrEmpty(this.SearchKeyPath))
                {
                    var prop = SearchEngine.GetPropValue(obj, this.SearchKeyPath);
                    if (prop != null)
                    {
                        resultDictionary.Add(new KeyValuePair<string, object>(prop.ToString(), obj));
                    }
                    else
                    {
                        resultDictionary.Add(new KeyValuePair<string, object>(obj.ToString(), obj));
                    }
                }
                else
                {
                    resultDictionary.Add(new KeyValuePair<string, object>(obj.ToString(), obj));
                }
            }

            this.SearchResults = resultDictionary;

            this.RaisePropertyChanged("SearchResultsListBox");
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.SetCaptionVisibility();
            this.Search(this.SearchTextBox.Text);
        }

        public void ForceFocus()
        {
            this.SearchTextBox.Focus();
            Keyboard.Focus(this.SearchTextBox);
        }

        public void SelectItem(KeyValuePair<string, object> selectedResult)
        {
            if (selectedResult.Value is SearchEngine.SpellFix)
            {
                var spellfix = (selectedResult.Value as SearchEngine.SpellFix);
                if (spellfix.SuggestionList.Count > 0)
                    this.SearchTextBox.Text = spellfix.SuggestionList.First();

                this.RaisePropertyChanged("Text");
                Keyboard.Focus(this.SearchTextBox);
                this.SearchTextBox.CaretIndex = this.SearchTextBox.Text.Length;
                return;
            }

            if (this.SelectionCommand != null && this.SelectionCommand.CanExecute(selectedResult.Value))
            {
                this.SelectionCommand.Execute(selectedResult.Value);
                this.RaisePropertyChanged("Text");

                if (this.StaysFocus)
                    this.ForceFocus();
            }

            this.SearchResults.Clear();

            this.SearchTextBox.Text = string.Empty;
        }

        private void SetCaptionVisibility()
        {
            if (this.CaptionTextBlock.IsFocused)
            {
                this.CaptionTextBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (this.SearchTextBox.Text.Length == 0)
                    this.CaptionTextBlock.Visibility = Visibility.Visible;
                else
                    this.CaptionTextBlock.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Callback Methods

        private static void SearchEngineChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as SearchBox;
        }

        private static void DataSourceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as SearchBox;

            if (self.Engine == null)
            {
                if (string.IsNullOrEmpty(self.SearchKeyPath))
                    self.Engine = new SearchEngine(self.DataSource, self.IsSpellCheckerActive);
                else
                    self.Engine = new SearchEngine(self.DataSource, self.SearchKeyPath, self.IsSpellCheckerActive);
            }
            else
            {
                if (string.IsNullOrEmpty(self.SearchKeyPath))
                    self.Engine.Regenerate(self.DataSource, self.IsSpellCheckerActive);
                else
                    self.Engine.Regenerate(self.DataSource, self.SearchKeyPath, self.IsSpellCheckerActive);
            }
        }

        #endregion

        #region Event Handlers

        private void SearchTextBox_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.SearchTextBox.PreviewKeyDown += SearchTextBox_PreviewKeyDown;
        }

        private void SearchResultsListBox_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.SearchTextBox.PreviewKeyDown -= SearchTextBox_PreviewKeyDown;
        }

        private void SearchResultsListBox_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.SearchResultsListBox.PreviewKeyDown += SearchResultsListBox_PreviewKeyDown;
        }

        private void SearchTextBox_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.SearchResultsListBox.PreviewKeyDown -= SearchResultsListBox_PreviewKeyDown;
        }

        private void SearchResultsListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.SearchResultsListBox == null)
                return;

            char c;
            char.TryParse(e.Key.ToString(), out c);

            if (e.Key == Key.Up && this.SearchResultsListBox.SelectedIndex == 0)
            {
                Keyboard.Focus(this.SearchTextBox);
                this.SearchResultsListBox.SelectedItem = null;
            }
            else if (e.Key == Key.Left)
            {
                e.Handled = true;

                // 5 geri
                int i = this.SearchResultsListBox.SelectedIndex;
                if (i >= 5)
                    this.SearchResultsListBox.SelectedIndex = i - 5;
                else
                    this.SearchResultsListBox.SelectedIndex = 0;

                var element = (this.SearchResultsListBox.ItemContainerGenerator.ContainerFromItem(this.SearchResultsListBox.SelectedItem) as FrameworkElement);
                if (element != null)
                {
                    element.Focus();
                    this.SearchResultsListBox.ScrollIntoView(this.SearchResultsListBox.SelectedItem);
                }
                else
                {
                    this.SearchResultsListBox.ScrollIntoView(this.SearchResultsListBox.SelectedItem);
                    element = (this.SearchResultsListBox.ItemContainerGenerator.ContainerFromItem(this.SearchResultsListBox.SelectedItem) as FrameworkElement);
                    if (element != null)
                        element.Focus();
                }
            }
            else if (e.Key == Key.Right)
            {
                e.Handled = true;

                // 5 ileri
                int i = this.SearchResultsListBox.SelectedIndex;
                if (i + 5 < this.SearchResultsListBox.Items.Count)
                    this.SearchResultsListBox.SelectedIndex = i + 5;
                else
                    this.SearchResultsListBox.SelectedIndex = this.SearchResultsListBox.Items.Count - 1;

                var element = (this.SearchResultsListBox.ItemContainerGenerator.ContainerFromItem(this.SearchResultsListBox.SelectedItem) as FrameworkElement);
                if (element != null)
                {
                    element.Focus();
                    this.SearchResultsListBox.ScrollIntoView(this.SearchResultsListBox.SelectedItem);
                }
                else
                {
                    this.SearchResultsListBox.ScrollIntoView(this.SearchResultsListBox.SelectedItem);
                    element = (this.SearchResultsListBox.ItemContainerGenerator.ContainerFromItem(this.SearchResultsListBox.SelectedItem) as FrameworkElement);
                    if (element != null)
                        element.Focus();
                }
            }
            else if ((e.Key != Key.Up && e.Key != Key.Down) && (char.IsLetterOrDigit(c) || char.IsPunctuation(c) || e.Key == Key.Back))
            {
                Keyboard.Focus(this.SearchTextBox);
                this.SearchResultsListBox.SelectedItem = null;
            }
        }

        private void SearchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Down || (e.Key == Key.Right && this.SearchTextBox.CaretIndex == this.SearchTextBox.Text.Length)) && this.SearchResultsListBox.HasItems)
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
                this.SelectItem(selectedResult);
            }
        }

        private void SearchResultsListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key & Key.Enter) == Key.Enter)
            {
                if (this.SearchResultsListBox.SelectedValue != null && this.SearchResultsListBox.SelectedValue is KeyValuePair<string, object>)
                    SelectItem((KeyValuePair<string, object>)this.SearchResultsListBox.SelectedValue);
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            this.SearchTextBox.Text = string.Empty;
            if (this.ClearCommand != null && this.ClearCommand.CanExecute(null))
                this.ClearCommand.Execute(null);
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            this.SetCaptionVisibility();
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.SetCaptionVisibility();
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

        #region Dispose

        ~SearchBox()
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                this.Dispose();
            }));
        }

        public void Dispose()
        {
            try
            {
                this.SearchTextBox.PreviewGotKeyboardFocus -= SearchTextBox_PreviewGotKeyboardFocus;
                this.SearchTextBox.PreviewLostKeyboardFocus -= SearchTextBox_PreviewLostKeyboardFocus;
                this.SearchResultsListBox.PreviewGotKeyboardFocus -= SearchResultsListBox_PreviewGotKeyboardFocus;
                this.SearchResultsListBox.PreviewLostKeyboardFocus -= SearchResultsListBox_PreviewLostKeyboardFocus;
                this.SearchTextBox.PreviewKeyDown -= SearchTextBox_PreviewKeyDown;
                this.SearchResultsListBox.PreviewKeyDown -= SearchResultsListBox_PreviewKeyDown;

                /*
                // SearchService yasadigi surece engine dispose edilmemeli
                if (this.Engine != null)
                    this.Engine.Dispose();
                */
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }
}
