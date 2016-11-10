using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace Eigen.Infrastructure.Converter
{
    public class SelectedTextColorConverter : IMultiValueConverter
    {
        private readonly SolidColorBrush SELECTED_TEXT_COLOR = new SolidColorBrush(Colors.Chocolate);

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            TextBlock textBlock = new TextBlock();
            string[] words = values[0].ToString().Split(' ');
            List<string> inputWords = values[1].ToString().Split(' ').Select(tag => tag.Trim()).Where(tag => !string.IsNullOrEmpty(tag)).ToList();

            foreach (string word in words)
            {
                Run run = new Run(word + " ");
                bool found = false;
                foreach (string inputPart in inputWords)
                {
                    if (word.StartsWith(inputPart, StringComparison.CurrentCultureIgnoreCase))
                    {
                        int start = word.IndexOf(inputPart, StringComparison.CurrentCultureIgnoreCase);
                        int end = start + inputPart.Length;

                        // renksiz kısım
                        textBlock.Inlines.Add(new Run(word.Substring(0, start)));

                        // renkli kısım
                        Run coloredPart = new Run(word.Substring(start, inputPart.Length));
                        coloredPart.Foreground = SELECTED_TEXT_COLOR;
                        textBlock.Inlines.Add(coloredPart);

                        // renksiz kısım
                        textBlock.Inlines.Add(new Run(word.Substring(end, word.Length - end) + " "));

                        found = true;
                        break;
                    }
                }

                if (!found)
                    textBlock.Inlines.Add(run);
            }

            return textBlock;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
