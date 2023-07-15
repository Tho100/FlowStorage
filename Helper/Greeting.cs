using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowSERVER1.Helper {
    public class Greeting {

        /// <summary>
        /// 
        /// Setup greeting label based on current time and translate 
        /// them into user default language (English on registration).
        /// 
        /// </summary>

        public static void buildGreetingLabel(String CurrentLang) {

            var form = HomePage.instance;
            var lab1 = form.lblGreetingText;

            DateTime now = DateTime.Now;

            var hours = now.Hour;
            string greeting = $"Good Night {Globals.custUsername}";

            if (hours >= 1 && hours <= 12) {
                if (CurrentLang == "US") {
                    greeting = "Good Morning, " + Globals.custUsername;
                }
                else if (CurrentLang == "MY") {
                    greeting = "Selemat Pagi, " + Globals.custUsername;
                }
                else if (CurrentLang == "GER") {
                    greeting = "Guten Morgen, " + Globals.custUsername;
                }
                else if (CurrentLang == "JAP") {
                    greeting = "おはよう " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "ESP") {
                    greeting = "Buen día " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "FRE") {
                    greeting = "Bonjour " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "POR") {
                    greeting = "Bom dia " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "CHI") {
                    greeting = "早上好 " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "RUS") {
                    greeting = "Доброе утро " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "DUT") {
                    greeting = "Goedemorgen " + Globals.custUsername + " :)";
                }

            }

            else if (hours >= 12 && hours <= 16) {
                if (CurrentLang == "US") {
                    greeting = "Good Afternoon, " + Globals.custUsername;
                }
                else if (CurrentLang == "MY") {
                    greeting = "Selamat Petang, " + Globals.custUsername;
                }
                else if (CurrentLang == "GER") {
                    greeting = "Guten Tag, " + Globals.custUsername;
                }
                else if (CurrentLang == "JAP") {
                    greeting = "こんにちは " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "ESP") {
                    greeting = "Buenas tardes " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "FRE") {
                    greeting = "Bon après-midi " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "POR") {
                    greeting = "Boa tarde " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "CHI") {
                    greeting = "下午好 " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "RUS") {
                    greeting = "Добрый день " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "DUT") {
                    greeting = "Goedemiddag " + Globals.custUsername + " :)";
                }
            }
            else if (hours >= 16 && hours <= 21) {
                if (hours == 20 || hours == 21) {
                    if (CurrentLang == "US") {
                        greeting = "Good Late Evening, " + Globals.custUsername;
                    }
                    else if (CurrentLang == "MY") {
                        greeting = "Selamat Lewat-Petang, " + Globals.custUsername;
                    }
                    else if (CurrentLang == "GER") {
                        greeting = "Guten späten Abend " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "JAP") {
                        greeting = "こんばんは " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "ESP") {
                        greeting = "buenas tardes " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "FRE") {
                        greeting = "bonne soirée " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "POR") {
                        greeting = "Boa noite " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "CHI") {
                        greeting = "晚上好 " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "RUS") {
                        greeting = "Добрый день " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "DUT") {
                        greeting = "Goedeavond " + Globals.custUsername + " :)";
                    }

                }
                else {
                    if (CurrentLang == "US") {
                        greeting = "Good Evening, " + Globals.custUsername;
                    }
                    else if (CurrentLang == "MY") {
                        greeting = "Selamat Petang, " + Globals.custUsername;
                    }
                    else if (CurrentLang == "GER") {
                        greeting = "Guten Abend, " + Globals.custUsername;
                    }
                    else if (CurrentLang == "JAP") {
                        greeting = "こんばんは " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "ESP") {
                        greeting = "Buenas terdes " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "FRE") {
                        greeting = "bonne soirée " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "POR") {
                        greeting = "Boa noite " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "CHI") {
                        greeting = "晚上好 " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "RUS") {
                        greeting = "Добрый вечер " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "DUT") {
                        greeting = "Goedeavond " + Globals.custUsername + " :)";
                    }
                }

            }
            else if (hours >= 21 && hours <= 24) {
                if (CurrentLang == "US") {
                    greeting = "Good Night, " + Globals.custUsername;
                }
                else if (CurrentLang == "MY") {
                    greeting = "Selamat Malam, " + Globals.custUsername;
                }
                else if (CurrentLang == "GER") {
                    greeting = "Guten Nacth, " + Globals.custUsername;
                }
                else if (CurrentLang == "JAP") {
                    greeting = "おやすみ " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "ESP") {
                    greeting = "Buenas noches " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "FRE") {
                    greeting = "bonne nuit " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "POR") {
                    greeting = "Boa noite " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "CHI") {
                    greeting = "晚安 " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "RUS") {
                    greeting = "Спокойной ночи " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "DUT") {
                    greeting = "Welterusten " + Globals.custUsername + " :)";
                }

            }

            lab1.Text = greeting;
        }

    }
}
