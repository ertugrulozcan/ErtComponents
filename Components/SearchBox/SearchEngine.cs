using NHunspell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigen.Core.Utility
{
    public class SearchEngine
    {
        #region Fields

        private SearchTree tree;
        private HashSet<object> dataSource;
        private List<SpellFix> spellFixList = new List<SpellFix>();

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

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public SearchEngine(System.Collections.IEnumerable dataSource)
        {
            this.Construct(dataSource, null);
        }

        /// <summary>
        /// Constructor 2
        /// </summary>
        public SearchEngine(System.Collections.IEnumerable dataSource, string memberKey)
        {
            this.Construct(dataSource, memberKey);
        }

        /// <summary>
        /// Constructor 3
        /// </summary>
        public SearchEngine(System.Collections.IEnumerable dataSource, bool isSpellCheckerActive)
        {
            this.IsSpellCheckerActive = isSpellCheckerActive;
            this.Construct(dataSource, null);
        }

        /// <summary>
        /// Constructor 4
        /// </summary>
        public SearchEngine(System.Collections.IEnumerable dataSource, string memberKey, bool isSpellCheckerActive)
        {
            this.IsSpellCheckerActive = isSpellCheckerActive;
            this.Construct(dataSource, memberKey);
        }

        private void Construct(System.Collections.IEnumerable dataSource, string memberKey)
        {
            this.DataSource = new HashSet<object>();
            HashSet<KeyValuePair<string, object>> dictionary = new HashSet<KeyValuePair<string, object>>();

            if (dataSource != null)
            {
                if(string.IsNullOrEmpty(memberKey))
                {
                    foreach (var data in dataSource)
                    {
                        this.DataSource.Add(data);
                        dictionary.Add(new KeyValuePair<string, object>(data.ToString().ToUpper(), data));
                    }
                }
                else
                {
                    foreach (var data in dataSource)
                    {
                        this.DataSource.Add(data);

                        if (!string.IsNullOrEmpty(memberKey))
                        {
                            var prop = GetPropValue(data, memberKey);
                            if (prop != null)
                                dictionary.Add(new KeyValuePair<string, object>(prop.ToString(), data));
                            else
                                dictionary.Add(new KeyValuePair<string, object>(data.ToString(), data));
                        }
                        else
                            dictionary.Add(new KeyValuePair<string, object>(data.ToString(), data));
                    }
                }
                
                this.SplitWords(dictionary);

                if (this.IsSpellCheckerActive)
                    this.SpellChecker = this.CreateSpellChecker(dictionary.Select(x => x.Key).ToList());
            }

            this.Tree = new SearchTree(dictionary);
        }

        #endregion

        #region Methods

        public HashSet<object> Search(string searchKey)
        {
            this.SpellFixList.Clear();

            if (string.IsNullOrEmpty(searchKey.Trim(' ')))
                return this.DataSource;

            if (this.Tree != null)
            {
                Node node = this.Tree.FindNode(searchKey);

                if (node == null)
                {
                    if(this.IsSpellCheckerActive && searchKey.Length > 4)
                    {
                        bool hasSuggestion = this.DidYouMean(searchKey);
                        if (hasSuggestion)
                        {
                            var dym = new HashSet<object>();
                            foreach(var spellfix in this.SpellFixList)
                                foreach (var suggestion in spellfix.suggestionList)
                                    dym.Add(suggestion + " mi demek istediniz?");
                            return dym;
                        }
                        else
                            return new HashSet<object>();
                    }
                    else
                        return new HashSet<object>();
                }

                return node.Objects;
            }
            else
                return new HashSet<object>();
        }

        private void SplitWords(HashSet<KeyValuePair<string, object>> dictionary)
        {
            HashSet<KeyValuePair<string, object>> newValuePairs = new HashSet<KeyValuePair<string, object>>();

            foreach (KeyValuePair<string, object> vp in dictionary)
            {
                string[] words = vp.Key.Split(' ');
                if (words.Length > 1)
                {
                    /*
                    for (int i = 1; i < words.Length; i++)
                        newValuePairs.Add(new KeyValuePair<string, object>(words[i], vp.Value));
                    */

                    for (int i = 1; i < words.Length; i++)
                    {
                        string merged = string.Empty;
                        for (int j = i; j < words.Length; j++)
                            merged += words[j] + " ";
                        newValuePairs.Add(new KeyValuePair<string, object>(merged, vp.Value));
                    }
                }
            }

            foreach (var newVP in newValuePairs)
                dictionary.Add(newVP);
        }

        public static object GetPropValue(object src, string propName)
        {
            var prop = src.GetType().GetProperty(propName);
            if (prop == null)
                return null;
            else
                return prop.GetValue(src, null);
        }

        #endregion

        #region SpellChecker

        private Hunspell CreateSpellChecker(IList<string> dictionary)
        {
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
                if(suggestions.Count > 0)
                {
                    this.SpellFixList.Add(new SpellFix() { original = word, suggestionList = suggestions });
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

        public struct SpellFix
        {
            public string original;
            public List<string> suggestionList;
        }

        #endregion
    }
}
