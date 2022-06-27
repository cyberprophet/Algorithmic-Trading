namespace ShareInvest.Interface
{
    public struct Chart
    {
        public string Code
        {
            get; set;
        }
        public string? Name
        {
            get; set;
        }
        public string Current
        {
            get; set;
        }
        public string Volume
        {
            get; set;
        }
        public string Amount
        {
            get; set;
        }
        public string Date
        {
            get; set;
        }
        public string Start
        {
            get; set;
        }
        public string High
        {
            get; set;
        }
        public string Low
        {
            get; set;
        }
        public string Revise
        {
            get; set;
        }
        public string ReviseRate
        {
            get; set;
        }
        public string MainCategory
        {
            get; set;
        }
        public string SubCategory
        {
            get; set;
        }
        public string StockInfo
        {
            set; get;
        }
        public string ReviseEvent
        {
            get; set;
        }
        public string Close
        {
            get; set;
        }
    }
}