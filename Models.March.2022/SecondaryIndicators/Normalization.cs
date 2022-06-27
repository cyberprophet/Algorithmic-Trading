namespace ShareInvest.SecondaryIndicators
{
    public class Normalization
    {
        public Normalization(dynamic max, dynamic min)
        {
            denominator = max - min;
            this.min = min;
        }
        public double Normalize(int arg) => (arg - min) / denominator;
        public double Normalize(uint arg) => (arg - min) / denominator;
        public double Normalize(long arg) => (arg - min) / denominator;
        public double Normalize(ulong arg) => (arg - min) / denominator;
        public double Normalize(double arg) => (arg - min) / denominator;
        public double Normalize(float arg) => (arg - min) / denominator;
        public double Normalize(decimal arg) => (arg - min) / denominator;
        readonly dynamic min;
        readonly double denominator;
    }
}