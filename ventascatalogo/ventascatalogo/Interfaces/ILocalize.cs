namespace ventascatalogo.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Globalization;

    public interface ILocalize
    {
        CultureInfo GetCurrentCultureInfo();
        void SetLocale(CultureInfo ci);
    }
}
