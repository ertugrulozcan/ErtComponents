using NHunspell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Eigen.Core.Utility
{
    public class SearchEngine
    {
        #region Fields

        private SearchTree tree;
        private HashSet<object> dataSource;
        private System.Collections.IEnumerable originalDataSource;
        private List<SpellFix> spellFixList = new List<SpellFix>();
        private bool useMemberKey;
        private string memberkey;

        #endregion

        #region Properties

        public SearchTree Tree
        {
            get
            {
                return tree;
            }

            private set
            {
                tree = value;
            }
        }

        public HashSet<object> DataSource
        {
            get
            {
                return dataSource;
            }

            private set
            {
                dataSource = value;
            }
        }

        public Hunspell SpellChecker { get; private set; }

        public List<SpellFix> SpellFixList
        {
            get
            {
                return spellFixList;
            }

            private set
            {
                spellFixList = value;
            }
        }

        public bool IsSpellCheckerActive { get; set; }

        public IEnumerable OriginalDataSource
        {
            get
            {
                return originalDataSource;
            }

            private set
            {
                if (this.originalDataSource != null && this.originalDataSource is System.Collections.Specialized.INotifyCollectionChanged)
                {
                    var collection = this.originalDataSource as System.Collections.Specialized.INotifyCollectionChanged;
                    collection.CollectionChanged -= Collection_CollectionChanged;
                }

                if (value is System.Collections.Specialized.INotifyCollectionChanged)
                {
                    var collection = value as System.Collections.Specialized.INotifyCollectionChanged;
                    collection.CollectionChanged += Collection_CollectionChanged;
                }

                originalDataSource = value;
            }
        }

        public string Memberkey
        {
            get
            {
                return memberkey;
            }

            private set
            {
                memberkey = value;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public SearchEngine(IEnumerable<ISearchable> dataSource)
        {
            this.Construct(dataSource, null);
        }

        /// <summary>
        /// Constructor 2
        /// </summary>
        public SearchEngine(IEnumerable<ISearchable> dataSource, string memberKey)
        {
            this.Construct(dataSource, memberKey);
        }

        /// <summary>
        /// Constructor 3
        /// </summary>
        public SearchEngine(IEnumerable<ISearchable> dataSource, bool isSpellCheckerActive)
        {
            this.IsSpellCheckerActive = isSpellCheckerActive;
            this.Construct(dataSource, null);
        }

        /// <summary>
        /// Constructor 4
        /// </summary>
        public SearchEngine(IEnumerable<ISearchable> dataSource, string memberKey, bool isSpellCheckerActive)
        {
            this.IsSpellCheckerActive = isSpellCheckerActive;
            this.Construct(dataSource, memberKey);
        }

        private void Construct(IEnumerable<ISearchable> dataSource, string memberKey)
        {
            this.OriginalDataSource = dataSource;

            this.DataSource = new HashSet<object>();
            HashSet<KeyValuePair<string, object>> dictionary = new HashSet<KeyValuePair<string, object>>();

            if (dataSource != null)
            {
                if (string.IsNullOrEmpty(memberKey))
                {
                    this.useMemberKey = false;
                    this.Memberkey = null;

                    foreach (var data in dataSource)
                    {
                        this.DataSource.Add(data);
                        dictionary.Add(new KeyValuePair<string, object>(data.SearchKey, data));
                    }
                }
                else
                {
                    this.useMemberKey = true;
                    this.Memberkey = memberKey;

                    foreach (var data in dataSource)
                    {
                        this.DataSource.Add(data);

                        var prop = GetPropValue(data, memberKey);
                        if (prop != null)
                            dictionary.Add(new KeyValuePair<string, object>(prop.ToString(), data));
                        else
                            dictionary.Add(new KeyValuePair<string, object>(data.SearchKey, data));
                    }
                }

                this.SplitWords(dictionary);

                try
                {
                    if (this.IsSpellCheckerActive)
                        this.SpellChecker = this.CreateSpellChecker(dictionary.Select(x => x.Key).ToList());
                }
                catch
                {
                    this.IsSpellCheckerActive = false;
                }
            }

            this.Tree = new SearchTree(dictionary);
        }

        public void Regenerate(IEnumerable<ISearchable> dataSource, string memberKey, bool isSpellCheckerActive)
        {
            this.Dispose();

            this.IsSpellCheckerActive = isSpellCheckerActive;
            this.Construct(dataSource, memberKey);
        }

        public void Regenerate(IEnumerable<ISearchable> dataSource, bool isSpellCheckerActive)
        {
            this.Dispose();

            this.IsSpellCheckerActive = isSpellCheckerActive;
            this.Construct(dataSource, null);
        }

        #endregion

        #region Methods

        public HashSet<object> Search(string searchKey, bool advancedSearch, CombineMode combineMode = CombineMode.Intersection)
        {
            this.SpellFixList.Clear();

            if (string.IsNullOrEmpty(searchKey.Trim(' ')))
                return this.DataSource;

            if (this.Tree != null)
            {
                if (!advancedSearch)
                {
                    Node node = this.Tree.FindNode(searchKey);

                    if (node != null)
                        return node.Objects;
                    else
                        return this.GetSpellCheckerSuggestions(searchKey);
                }
                else
                {
                    List<string> searchKeys = searchKey.Split(' ').Select(tag => tag.Trim()).Where(tag => !string.IsNullOrEmpty(tag)).ToList();
                    if (searchKeys.Count == 1)
                    {
                        Node node = this.Tree.FindNode(searchKey);

                        if (node != null)
                            return node.Objects;
                        else
                            return this.GetSpellCheckerSuggestions(searchKey);
                    }

                    List<Node> nodeList = new List<Node>();
                    foreach (string key in searchKeys)
                    {
                        Node found = this.Tree.FindNode(key);
                        if (found != null)
                            nodeList.Add(found);
                    }

                    if (nodeList.Count > 0)
                    {
                        return this.CombineResults(nodeList, combineMode);
                    }
                    else
                        return this.GetSpellCheckerSuggestions(searchKeys.First());
                }
            }
            else
                return new HashSet<object>();
        }

        private HashSet<object> CombineResults(List<Node> nodeList, CombineMode combineMode)
        {
            HashSet<object> combinedResults = new HashSet<object>();

            switch (combineMode)
            {
                case CombineMode.Combination:
                    {
                        foreach (Node node in nodeList)
                        {
                            foreach (object resultObject in node.Objects)
                            {
                                if (!combinedResults.Contains(resultObject))
                                {
                                    combinedResults.Add(resultObject);
                                }
                            }
                        }
                    }
                    break;
                case CombineMode.Intersection:
                    {
                        foreach (Node node in nodeList)
                        {
                            foreach (object resultObject in node.Objects)
                            {
                                if (!combinedResults.Contains(resultObject) && nodeList.All(x => x.Objects.Contains(resultObject)))
                                {
                                    combinedResults.Add(resultObject);
                                }
                            }
                        }
                    }
                    break;
                case CombineMode.Difference:
                    {
                        var combinationList = this.CombineResults(nodeList, CombineMode.Combination);
                        var intersectionList = this.CombineResults(nodeList, CombineMode.Intersection);
                        foreach (var item in combinationList)
                        {
                            if (!intersectionList.Contains(item))
                            {
                                combinedResults.Add(item);
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

            return combinedResults;
        }

        public enum CombineMode
        {
            Combination,
            Intersection,
            Difference
        }

        private HashSet<object> GetSpellCheckerSuggestions(string searchKey)
        {
            if (this.IsSpellCheckerActive && searchKey.Length > 1)
            {
                bool hasSuggestion = this.DidYouMean(searchKey);
                if (hasSuggestion)
                {
                    var dym = new HashSet<object>();
                    foreach (var spellfix in this.SpellFixList)
                        foreach (var suggestion in spellfix.SuggestionList)
                            dym.Add(spellfix);
                    return dym;
                }
                else
                    return new HashSet<object>();
            }
            else
                return new HashSet<object>();
        }

        private void SplitWords(HashSet<KeyValuePair<string, object>> dictionary)
        {
            HashSet<KeyValuePair<string, object>> newValuePairs = new HashSet<KeyValuePair<string, object>>();

            foreach (KeyValuePair<string, object> vp in dictionary)
            {
                List<string> words = vp.Key.Split(' ').Select(tag => tag.Trim()).Where(tag => !string.IsNullOrEmpty(tag)).ToList();
                if (words.Count > 1)
                {
                    /*
                    for (int i = 1; i < words.Length; i++)
                        newValuePairs.Add(new KeyValuePair<string, object>(words[i], vp.Value));
                    */

                    for (int i = 1; i < words.Count; i++)
                    {
                        string merged = string.Empty;
                        for (int j = i; j < words.Count; j++)
                            merged += words[j] + " ";
                        newValuePairs.Add(new KeyValuePair<string, object>(merged, vp.Value));
                    }
                }
                else if (words.Count == 1)
                    newValuePairs.Add(new KeyValuePair<string, object>(words[0], vp.Value));
            }

            foreach (var newVP in newValuePairs)
                dictionary.Add(newVP);
        }

        private IList<string> SplitWords(IList<string> dictionary)
        {
            List<string> splittedList = new List<string>();

            foreach (string sentence in dictionary)
            {
                List<string> words = sentence.Split(' ').Select(tag => tag.Trim()).Where(tag => !string.IsNullOrEmpty(tag)).ToList();
                if (words.Count > 1)
                {
                    for (int i = 1; i < words.Count; i++)
                    {
                        if (!splittedList.Any(x => x == words[i]))
                            splittedList.Add(words[i]);
                    }
                }
                else if (words.Count == 1)
                    if (!splittedList.Any(x => x == words[0]))
                        splittedList.Add(words[0]);
            }

            return splittedList;
        }

        public static object GetPropValue(object src, string propName)
        {
            var prop = src.GetType().GetProperty(propName);
            if (prop == null)
                return null;
            else
                return prop.GetValue(src, null);
        }

        private void Collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var addedItems = e.NewItems;
                if (addedItems != null && addedItems.Count > 0)
                {
                    this.AddItems(addedItems);
                }
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                var removedItems = e.OldItems;
                if (removedItems != null && removedItems.Count > 0)
                {
                    this.RemoveItems(removedItems);
                }
            }
        }

        private void AddItems(IList items)
        {
            HashSet<KeyValuePair<string, object>> dictionary = new HashSet<KeyValuePair<string, object>>();

            foreach (var item in items)
            {
                if (!this.DataSource.Contains(item))
                    this.DataSource.Add(item);

                dictionary.Add(this.ToKeyValuePair(item));
            }

            this.SplitWords(dictionary);

            if (this.IsSpellCheckerActive)
            {
                var spellDict = dictionary.Select(x => x.Key);
                foreach (var key in spellDict)
                    this.SpellChecker.Add(key);
            }

            this.Tree.AddItems(dictionary);
        }

        private void RemoveItems(IList items)
        {
            HashSet<KeyValuePair<string, object>> dictionary = new HashSet<KeyValuePair<string, object>>();

            foreach (var item in items)
            {
                if (this.DataSource.Contains(item))
                    this.DataSource.Remove(item);

                dictionary.Add(this.ToKeyValuePair(item));
            }

            this.SplitWords(dictionary);

            if (this.IsSpellCheckerActive)
            {
                var spellDict = dictionary.Select(x => x.Key);
                foreach (var key in spellDict)
                    this.SpellChecker.Remove(key);
            }

            this.Tree.RemoveItems(dictionary);
        }

        private KeyValuePair<string, object> ToKeyValuePair(object item)
        {
            if (this.useMemberKey)
            {
                var prop = GetPropValue(item, this.Memberkey);
                if (prop != null)
                    return new KeyValuePair<string, object>(prop.ToString(), item);
                else
                    return new KeyValuePair<string, object>(item.ToString(), item);
            }
            else
            {
                return new KeyValuePair<string, object>(item.ToString(), item);
            }
        }

        #endregion

        #region SpellChecker

        private Hunspell CreateSpellChecker(IList<string> dictionary)
        {
            dictionary = this.SplitWords(dictionary);

            string mergedDictionary = string.Join(Environment.NewLine, dictionary);
            mergedDictionary = string.Format("{0}{1}{2}", dictionary.Count, Environment.NewLine, mergedDictionary);

            var hunspell = new Hunspell(new byte[1], GetBytes(mergedDictionary));
            foreach (var word in dictionary)
            {
                hunspell.Add(word);
            }

            return hunspell;
        }

        private bool DidYouMean(string word)
        {
            if (this.SpellChecker != null && !SpellChecker.Spell(word))
            {
                var suggestions = SpellChecker.Suggest(word);
                if (suggestions.Count > 0)
                {
                    this.SpellFixList.Add(new SpellFix(word, suggestions));
                    return true;
                }
                else
                    return false;
            }
            else
            {
                this.SpellFixList = new List<SpellFix>();
                return false;
            }
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public class SpellFix
        {
            public string Original { get; set; }
            public List<string> SuggestionList { get; set; }

            public SpellFix(string original, List<string> suggestionList)
            {
                this.Original = original;
                this.SuggestionList = suggestionList;
            }

            public override string ToString()
            {
                if (this.SuggestionList.Count > 0)
                    return string.Format(SpellFixConfig.DidYouMeanMessage_, this.SuggestionList[0]);
                else
                    return string.Empty;
            }
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            this.OriginalDataSource = null;
            this.Tree.Destroy();
            this.DataSource.Clear();
            this.SpellFixList.Clear();
        }

        #endregion
    }

    public static class SpellFixConfig
    {
        public static string DidYouMeanMessage_ = "'{0}' mi demek istediniz?";

        public static readonly DependencyProperty DidYouMeanMessageProperty = DependencyProperty.RegisterAttached("DidYouMeanMessage", typeof(string), typeof(SpellFixConfig),
            new FrameworkPropertyMetadata(DidYouMeanMessage_,
                FrameworkPropertyMetadataOptions.AffectsRender,
                DidYouMeanMessageChanged));

        public static void SetDidYouMeanMessage(DependencyObject element, string value)
        {
            element.SetValue(DidYouMeanMessageProperty, value);
        }

        public static string GetDidYouMeanMessage(DependencyObject element)
        {
            return (string)element.GetValue(DidYouMeanMessageProperty);
        }

        private static void DidYouMeanMessageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            DidYouMeanMessage_ = args.NewValue.ToString();
        }
    }
}
