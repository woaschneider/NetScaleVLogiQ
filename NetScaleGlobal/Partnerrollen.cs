namespace NetScaleGlobal
{
    public static class Partnerrollen
    {
        public static string GetRollenBezeichnung(string rollenKz)
        {
            string ret = "";

            switch (rollenKz)
            {
                case "AU":
                    ret = "Auftraggeber";
                    break;
                case "LI":
                    ret = "Lieferant";
                    break;
                case "SP":
                    ret = "Spedition";
                    break;
                case "FU":
                    ret = "Fuhrunternehmer";
                    break;
            }

            return ret;
        }

        //TODO:Das kann später in eine Tabelle
        public static bool IsRolleEdit(string rollenKz)
        {
            bool ret = false;

            switch (rollenKz)
            {
                case "AU":
                    ret = true;
                    break;
                case "LI":
                    ret = true;
                    break;
                case "SP":
                    ret = true;
                    break;
                case "FU":
                    ret = true;
                    break;
            }

            return ret;
        }

        public static string GetRollenByWiegeart(string wiegeart)
        {
            string ret = "";

            if (wiegeart == "A" | wiegeart == "R" | wiegeart == "P" | wiegeart == "G" | wiegeart == "F" |
                wiegeart == "W")
                ret = "AU";
            if (wiegeart == "E")
                ret = "LI";

            return ret;
        }
    }
}