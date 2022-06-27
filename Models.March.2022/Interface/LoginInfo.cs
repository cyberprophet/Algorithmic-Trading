namespace ShareInvest.Interface
{
    public struct LoginInfo
    {
        public string Key
        {
            get; set;
        }
        public int ACCOUNT_CNT
        {
            get; set;
        }
        public string[] ACCLIST
        {
            get; set;
        }
        public string USER_ID
        {
            get; set;
        }
        public string USER_NAME
        {
            get; set;
        }
        public bool GetServerGubun
        {
            get; set;
        }
    }
}