namespace LandedMVC.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ApiRouteAttribute : Attribute
    {
        private string _route;

        public ApiRouteAttribute(string route)
        {
            _route = route;
        }
        public string Route
        {
            get { return _route; }
        }
    }
}
