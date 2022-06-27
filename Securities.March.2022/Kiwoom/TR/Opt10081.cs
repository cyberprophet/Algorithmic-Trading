using AxKHOpenAPILib;

using ShareInvest.Event;
using ShareInvest.Interface.OpenAPI;

namespace ShareInvest.Kiwoom
{
    class Opt10081 : Tr
    {
        public override event EventHandler<SecuritiesEventArgs>? Send;
        internal override TR? TR
        {
            get; set;
        }
        internal override AxKHOpenAPI? Ax
        {
            get; set;
        }
        internal override bool OnReceiveTrData(_DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            if (int.TryParse(e.sPrevNext, out int next))
            {
                var data = () =>
                {
                    var arr = TR?.Single is not null ? new string[TR.Single.Length] : null;

                    if (arr is not null && Ax is not null)
                        for (int i = 0; i < TR?.Single.Length; i++)
                            arr[i] = Ax.GetCommData(e.sTrCode, e.sRQName, 0, TR.Single[i]).Trim();

                    if (TR?.Multiple is not null)
                    {
                        var data = Ax?.GetCommDataEx(e.sTrCode, e.sRQName);

                        if (data is not null)
                        {
                            int x, y, lx = ((object[,])data).GetUpperBound(0), ly = ((object[,])data).GetUpperBound(1);
                            var catalog = new Queue<string[]>();

                            for (x = 0; x <= lx; x++)
                            {
                                var str = new string[ly + 1];

                                for (y = 0; y <= ly; y++)
                                    str[y] = ((string)((object[,])data)[x, y]).Trim();

                                if (string.IsNullOrEmpty(e.sRQName) is false &&
                                   (Array.Exists(e.sRQName.ToCharArray(), o => char.IsLetter(o)) || string.Compare(str[4], e.sRQName) < 0))
                                    catalog.Enqueue(str);
                            }
                            return (arr, catalog);
                        }
                    }
                    return (arr, null);
                };
                var tuple = data();

                if (tuple.catalog is not null && tuple.arr is not null)
                {
                    var queue = new Queue<Interface.Chart>();

                    while (tuple.catalog.TryDequeue(out string[]? item))
                        queue.Enqueue(new Interface.Chart
                        {
                            Code = tuple.arr[0],
                            Name = Ax?.GetMasterCodeName(tuple.arr[0]),
                            Current = item[1],
                            Volume = item[2],
                            Amount = item[3],
                            Date = item[4],
                            Start = item[5],
                            High = item[6],
                            Low = item[7],
                            Revise = item[8],
                            ReviseRate = item[9],
                            MainCategory = item[10],
                            SubCategory = item[11],
                            StockInfo = item[12],
                            ReviseEvent = item[13],
                            Close = item[14]
                        });
                    if (queue.Count > 0 && TR is not null)
                        Send?.Invoke(this, new SecuritiesEventArgs(TR, next, queue));
                }
                return next == 0;
            }
            return true;
        }
    }
}