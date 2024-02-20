namespace TestMVC.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class ApiKeyAttribute : Attribute
    {
        private readonly string _apiRouteParam;
        private readonly bool _isPrimary;
        public ApiKeyAttribute(string apiRouteParam, bool isPrimary = false) {
            _apiRouteParam = apiRouteParam;
            _isPrimary = isPrimary;
        }
        public ApiKeyAttribute(bool isPrimary = true)
        {
            _apiRouteParam = string.Empty;
            _isPrimary = isPrimary;
        }

        public string ApiRouteParam
        {
            get { return _apiRouteParam; }
        }
        public bool IsPrimary
        {
            get { return _isPrimary; }
        }
    }
}
