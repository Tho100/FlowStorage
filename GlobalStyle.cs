using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowSERVER1 {
    public class GlobalStyle {

        // Date label
        static public readonly string DateLabelFontName = "Segoe UI Semibold";
        static public readonly float DateLabelFontSize = 9f;
        static public readonly Font DateLabelFont = new Font(DateLabelFontName, DateLabelFontSize, FontStyle.Bold);

        // Title label
        static public readonly string TitleLabelFontName = "Segoe UI Semibold";
        static public readonly float TitleLabelFontSize = 11f;
        static public readonly Font TitleLabelFont = new Font(TitleLabelFontName, TitleLabelFontSize, FontStyle.Bold);

        // Panel
        static public readonly Color BorderColor = ColorTranslator.FromHtml("#212121");
        static public readonly Color DarkGrayColor = Color.DarkGray;
        static public readonly Color GainsboroColor = Color.Gainsboro;
        static public readonly Color TransparentColor = Color.Transparent;
        static public readonly Point TitleLabelLoc = new Point(12, 166);
        static public readonly Point DateLabelLoc = new Point(12, 192);

        // Garbage button
        static public readonly Color BorderColor2 = ColorTranslator.FromHtml("#232323");
        static public readonly Color FillColor = ColorTranslator.FromHtml("#4713BF");
        static public readonly Image GarbageImage = FlowSERVER1.Properties.Resources.icons8_menu_vertical_30;
        static public readonly Point GarbageButtonLoc = new Point(165, 188);
        static public readonly Point GarbageOffset = new Point(2, 0);

    }
}
