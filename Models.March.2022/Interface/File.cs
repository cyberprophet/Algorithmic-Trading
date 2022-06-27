namespace ShareInvest.Interface
{
    public struct File
    {
        public string Name
        {
            get; set;
        }
        public byte[] Data
        {
            get; set;
        }
        public long Ticks
        {
            get; set;
        }
    }
}