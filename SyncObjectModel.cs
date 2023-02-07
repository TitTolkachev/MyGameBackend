namespace MyGameBackend
{
    public class SyncObjectModel
    {
        public double X { get; set; }
        public double Y { get; set; }
        public int Id { get; set; }
        public string Authority { get; set; }

        public SyncObjectModel(double x, double y, int id, string authority)
        {
            X = x;
            Y = y;
            Id = id;
            Authority = authority;
        }
    }
}