namespace aspCart.Web.Helpers
{
    public class CssClassHelper
    {
        public string GetType1(int index)
        {
            string cssClass;

            if (index == 2)
            {
                cssClass = "remove-divider-xl remove-divider-md-lg";
            }
            else if (index == 3)
            {
                cssClass = "remove-divider-wd";
            }
            else if (index == 5)
            {
                cssClass = "remove-divider-xl remove-divider-md-lg";
            }
            else if (index == 6)
            {
                cssClass = "d-md-none d-wd-block";
            }
            else if (index == 7)
            {
                cssClass = "remove-divider d-md-none d-wd-block";
            }
            else
            {
                cssClass = string.Empty;
            }

            return cssClass;
        }

        public string GetType2(int index)
        {
            string cssClass;

            if (index == 2)
            {
                cssClass = "remove-divider-md-lg";
            }
            else if (index == 3)
            {
                cssClass = "remove-divider-xl";
            }
            else if (index == 4)
            {
                cssClass = "d-xl-none d-wd-block";
            }
            else if (index == 5)
            {
                cssClass = "d-xl-none d-wd-block remove-divider-wd remove-divider-md-lg";
            }
            else
            {
                cssClass = string.Empty;
            }

            return cssClass;
        }

        public string GetType3(int index)
        {
            string cssClass;

            if (index == 3)
            {
                cssClass = "remove-divider-md-lg";
            }
            else if (index == 4)
            {
                cssClass = "remove-divider-xl";
            }
            else if (index == 5)
            {
                cssClass = "remove-divider-wd";
            }
            else if (index == 7)
            {
                cssClass = "remove-divider-md-lg";
            }
            else if (index == 9)
            {
                cssClass = "remove-divider-xl";
            }
            else if (index == 10)
            {
                cssClass = "d-xl-none d-wd-block";
            }
            else if (index == 11)
            {
                cssClass = "d-xl-none d-wd-block remove-divider-wd remove-divider-md-lg";
            }
            else
            {
                cssClass = string.Empty;
            }

            return cssClass;
        }

        public string GetType4(int index)
        {
            int four = 4;
            int five = 5;
            int eight = 8;
            int ten = 10;
            int twelve = 12;
            int fifteen = 15;
            int sixteen = 16;
            int twenty = 20;

            if (((index - four) % 20) == 0 || index == four)
            {
                return "remove-divider-md-lg remove-divider-xl";
            }

            if (((index - five) % 20) == 0 || index == five)
            {
                return "remove-divider-wd";
            }

            if (((index - eight) % 20) == 0 || index == eight)
            {
                return "remove-divider-md-lg remove-divider-xl";
            }

            if (((index - ten) % 20) == 0 || index == ten)
            {
                return "remove-divider-wd";
            }

            if (((index - twelve) % 20) == 0 || index == twelve)
            {
                return "remove-divider-md-lg remove-divider-xl";
            }

            if (((index - fifteen) % 20) == 0 || index == fifteen)
            {
                return "remove-divider-wd";
            }

            if (((index - sixteen) % 20) == 0 || index == sixteen)
            {
                return "remove-divider-md-lg remove-divider-xl";
            }

            if (((index - twenty) % 20) == 0 || index == twenty)
            {
                return "remove-divider";
            }

            return string.Empty;
        }
    }
}