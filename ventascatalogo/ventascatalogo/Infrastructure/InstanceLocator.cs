namespace ventascatalogo.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ViewModels;

    public class InstanceLocator
    {
        public MainViewModel Main { get; set; }

        #region Constructor
        public InstanceLocator()
        {
            this.Main = new MainViewModel();
        }
        #endregion
    }
}
