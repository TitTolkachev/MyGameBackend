namespace MyGameBackend
{
    public class ProjectileModel
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Vx { get; set; }
        public double Vy { get; set; }

        public ProjectileModel(int id, int ownerId, double x, double y, double vx, double vy)
        {
            Id = id; 
            OwnerId = ownerId; 
            X = x; 
            Y = y; 
            Vx = vx;
            Vy = vy;
        }
    }
}