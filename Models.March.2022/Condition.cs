using ShareInvest.Interface;

using System.Diagnostics;

namespace ShareInvest
{
    public static class Condition
    {
        public static IEnumerable<Models.CompanyOverview>? CompanyOverview
        {
            get; set;
        }
        public static API API => new(Url);
        public static API Dart => new("https://opendart.fss.or.kr");
        public static API Kakao => new("https://dapi.kakao.com");
        public static bool IsDebug
        {
            get; private set;
        }
        public static uint Visitant
        {
            get; set;
        }
        public static uint Count
        {
            get; set;
        }
        public static string Url => IsDebug ? "https://localhost:44343" : "https://coreapi.shareinvest.net";
        [Conditional("DEBUG")]
        public static void SetDebug()
        {
            IsDebug = true;
            Debug.WriteLine(nameof(SetDebug));
        }
        public static Operation? GetOperation(string arg)
        {
            var index = char.IsDigit(arg[0]) && int.TryParse(arg, out int digit) ? digit : char.TryParse(arg, out char sub) ? sub : char.MinValue;

            if (Enum.IsDefined(typeof(Operation), index))
                return (Operation)index;

            return null;
        }
        public static string GetStockName(IEnumerable<Models.OpenAPI.Account> accounts, string code)
        {
            foreach (var acc in accounts)
                if (acc.Balances.Any(o => code.Equals(o.Code)) &&
                    acc.Balances.First(o => code.Equals(o.Code)).Name is string name)
                    return name;

            return code;
        }
        public static string GetInnerHTML(string? sign, string day) => sign switch
        {
            "2" => string.Concat("<span class=\"oi oi-caret-top oi-padding\">", "</span>", day),
            "5" => string.Concat("<span class=\"oi oi-caret-bottom oi-padding\">", "</span>", day),
            "1" => string.Concat("<span class=\"oi oi-arrow-thick-top oi-padding\">", "</span>", day),
            "4" => string.Concat("<span class=\"oi oi-arrow-thick-bottom oi-padding\">", "</span>", day),
            _ => string.Empty
        };
        public static string GetIcon(string? sign) => Path.Combine(Url, "images", "pin", string.Concat("pin_", sign switch
        {
            "2" or "1" => "red",
            "5" or "4" => "blue",
            _ => "black"
        },
            ".png"));
        public static Dictionary<int, string> Error => new()
        {
            {
                0,
                "정상처리"
            },
            {
                -1,
                "미접속상태"
            },
            {
                -10,
                "실패"
            },
            {
                -11,
                "조건번호 없음"
            },
            {
                -12,
                "조건번호와 조건식 불일치"
            },
            {
                -13,
                "조건검색 조회요청 초과"
            },
            {
                -22,
                "전문 처리 실패"
            },
            {
                -0x64,
                "사용자정보교환 실패"
            },
            {
                -101,
                "서버 접속 실패"
            },
            {
                -102,
                "버전처리 실패"
            },
            {
                -103,
                "개인방화벽 실패"
            },
            {
                -104,
                "메모리 보호실패"
            },
            {
                -105,
                "함수입력값 오류"
            },
            {
                -0x6A,
                "통신연결 종료"
            },
            {
                -107,
                "보안모듈 오류"
            },
            {
                -108,
                "공인인증 로그인 필요"
            },
            {
                -0xC8,
                "시세조회 과부하"
            },
            {
                -201,
                "전문작성 초기화 실패"
            },
            {
                -0xCA,
                "전문작성 입력값 오류"
            },
            {
                -203,
                "데이터 없음"
            },
            {
                -204,
                "조회가능한 종목수 초과.(한번에 조회 가능한 종목개수는 최대 100종목)"
            },
            {
                -205,
                "데이터 수신 실패"
            },
            {
                -206,
                "조회가능한 FID수 초과.(한번에 조회 가능한 FID개수는 최대 100개)"
            },
            {
                -207,
                "실시간 해제오류"
            },
            {
                -209,
                "시세조회 제한"
            },
            {
                -0xD3,
                "시세조회 과부하"
            },
            {
                -300,
                "입력값 오류"
            },
            {
                -301,
                "계좌비밀번호 없음"
            },
            {
                -302,
                "타인계좌 사용오류"
            },
            {
                -303,
                "주문가격이 주문착오 금액기준 초과."
            },
            {
                -304,
                "주문가격이 주문착오 금액기준 초과."
            },
            {
                -305,
                "주문수량이 총발행주수의 1% 초과오류"
            },
            {
                -306,
                "주문수량은 총발행주수의 3% 초과오류"
            },
            {
                -307,
                "주문전송 실패"
            },
            {
                -308,
                "주문전송 과부하"
            },
            {
                -309,
                "주문수량 300계약 초과"
            },
            {
                -310,
                "주문수량 500계약 초과"
            },
            {
                -311,
                "주문전송제한 과부하"
            },
            {
                -340,
                "계좌정보 없음"
            },
            {
                -500,
                "종목코드 없음"
            }
        };
    }
}