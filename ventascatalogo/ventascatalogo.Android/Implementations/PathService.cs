[assembly: Xamarin.Forms.Dependency(typeof(ventascatalogo.Droid.Implementations.PathService))]
namespace ventascatalogo.Droid.Implementations
{
    using Interfaces;
    using System;
    using System.IO;

    public class PathService : IPathService
    {
        public string GetDatabasePath()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Path.Combine(path, "VentasCatalogo.db3");
        }
    }
}