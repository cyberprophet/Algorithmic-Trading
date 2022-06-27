namespace ShareInvest
{
    interface ICoreAPI<T>
    {
        event EventHandler<T> Send;
    }
}