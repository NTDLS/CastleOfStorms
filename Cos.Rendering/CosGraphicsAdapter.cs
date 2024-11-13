namespace Cos.Rendering
{
    public class CosGraphicsAdapter
    {
        public int DeviceId { get; set; }
        public string Description { get; set; }

        public double VideoMemoryMb { get; set; }

        public CosGraphicsAdapter(int deviceId, string description)
        {
            DeviceId = deviceId;
            Description = description;
        }

        public override string ToString()
        {
            return Description;
        }
    }
}
