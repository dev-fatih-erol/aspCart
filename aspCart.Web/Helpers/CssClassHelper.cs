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
    }
}