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

namespace Eigen.Core.Components.Converters
{
    public class SelectedTextColorConverter : IMultiValueConverter
    {
        private readonly SolidColorBrush SELECTED_TEXT_COLOR = new SolidColorBrush(Colors.Chocolate);

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string fulltext = values[0].ToString();

            string fromTextBox = values[1].ToString();

            int coloredStart = fulltext.IndexOf(fromTextBox, StringComparison.CurrentCultureIgnoreCase);
            int coloredEnd = coloredStart + fromTextBox.Length;

            if (coloredStart == -1)
                return new TextBlock(new Run(fulltext));

            TextBlock textBlock = new TextBlock();

            // renksiz kısım
            textBlock.Inlines.Add(new Run(fulltext.Substring(0, coloredStart)));

            // renkli kısım
            Run coloredPart = new Run(fulltext.Substring(coloredStart, fromTextBox.Length));
            coloredPart.Foreground = SELECTED_TEXT_COLOR;
            textBlock.Inlines.Add(coloredPart);

            // renksiz kısım
            textBlock.Inlines.Add(new Run(fulltext.Substring(coloredEnd, fulltext.Length - coloredEnd)));

            /*
            string[] texts = fulltext.Split(' ');
            foreach (string text in texts)
            {
                if (text.StartsWith(fromTextBox, StringComparison.CurrentCultureIgnoreCase))
                {
                    // renkli kısım
                    //Run coloredPart = new Run(fromTextBox);
                    Run coloredPart = new Run(text.Substring(0, fromTextBox.Length));
                    coloredPart.Foreground = SELECTED_TEXT_COLOR;
                    textBlock.Inlines.Add(coloredPart);

                    // renksiz kısım
                    textBlock.Inlines.Add(new Run(text.Substring(fromTextBox.Length)));
                }
                else
                {
                    textBlock.Inlines.Add(new Run(text));
                }

                textBlock.Inlines.Add(" ");
            }
            */

            return textBlock;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
