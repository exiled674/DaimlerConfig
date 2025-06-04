using Microsoft.Maui.Storage;

namespace DaimlerConfig.Services
{
    public class NavigationStateService
    {
        private const string LAST_ROUTE_KEY = "LastRoute";
        private string _currentRoute = "/";

        public string CurrentRoute => _currentRoute;

        public void SetCurrentRoute(string route)
        {
            _currentRoute = route;
            // Route sofort in Preferences speichern
            Preferences.Default.Set(LAST_ROUTE_KEY, route);
        }

        public string GetLastSavedRoute()
        {
            return Preferences.Default.Get(LAST_ROUTE_KEY, "/");
        }

        public void ClearSavedRoute()
        {
            Preferences.Default.Remove(LAST_ROUTE_KEY);
        }
    }
}