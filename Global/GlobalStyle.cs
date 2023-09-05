using System.Collections.Generic;
using System.Drawing;

namespace FlowSERVER1 {
    public class GlobalStyle {

        // Date label
        static private readonly string DateLabelFontName = "Segoe UI Semibold";
        static private readonly float DateLabelFontSize = 9f;
        static public readonly Font DateLabelFont = new Font(DateLabelFontName, DateLabelFontSize, FontStyle.Bold);
        static public readonly Point DateLabelLoc = new Point(12, 194);

        // Title label
        static private readonly string TitleLabelFontName = "Segoe UI Semibold";
        static private readonly float TitleLabelFontSize = 11f;
        static public readonly Font TitleLabelFont = new Font(TitleLabelFontName, TitleLabelFontSize, FontStyle.Bold);
        static public readonly Point TitleLabelLoc = new Point(12, 166);

        // Panel
        static public readonly Color BorderColor = ColorTranslator.FromHtml("#212121");
        static public readonly Color DarkGrayColor = Color.DarkGray;
        static public readonly Color GainsboroColor = Color.Gainsboro;
        static public readonly Color TransparentColor = Color.Transparent;

        // Ps button tag label
        static private readonly string PsLabelTagFontName = "Segoe UI Semibold";
        static private readonly float PsLabelTagFontSize = 9f;
        static public readonly Point PsLabelTagLoc = new Point(29, 191);
        static public readonly Font PsLabelTagFont = new Font(PsLabelTagFontName, PsLabelTagFontSize, FontStyle.Bold);

        // Seperator button
        static public readonly Point PsSeperatorBut = new Point(84, 199);
        static public readonly Size PsSeperatorButSize = new Size(4, 4);

        // Garbage button
        static public readonly Color BorderColor2 = ColorTranslator.FromHtml("#232323");
        static public readonly Color FillColor = ColorTranslator.FromHtml("#4713BF");
        static public readonly Image GarbageImage = Globals.VerticalMenuImage;
        static public readonly Point GarbageButtonLoc = new Point(165, 188);
        static public readonly Point GarbageOffset = new Point(2, 0);

        static public readonly Color DarkPurpleColor = Color.FromArgb(255, 71, 19, 191);

        public static readonly Dictionary<string, Color> psBackgroundColorTag = new Dictionary<string, Color>
        {
            {"Gaming",Color.SteelBlue},
            {"Education",Color.Firebrick},
            {"Software",Color.MediumSeaGreen},
            {"Entertainment",Color.Orange},
            {"Random",Color.DimGray},
            {"Music",Color.Tomato},
            {"Data",Color.DarkTurquoise},
            {"Creativity",Color.BlueViolet},
        };

    }
}
