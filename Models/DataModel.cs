namespace LandedMVC.Models
{
    public class DataModel<D> where D : class
    {
        public D[]? Data { get; set; }
    }
}
