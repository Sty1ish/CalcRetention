using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BI.Core.Common.Analytics.Types
{
    // Daily Data Table (기록 단위 : Pid)
    public enum AnalyticsType
    {
        // AU / Retention
        UniquePids = 10,               // Daily Unique Pid (일일 접속 유저 Total)

        /// <summary>
        /// 국가 / 마켓 / PID / 첫 접속 여부 : 이렇게 저장하면 무슨일이 일어나지?
        /// 순수하게 한 행에 저장되는 데이터량이 25% 증가한다. (쿼리 타임 문제 있나?)
        /// 반대로 AU와 NRU를 나눠서 저장되는 경우, 행이 두배로 늘어난다.
        /// </summary>

        // User Gain
        NewResisterPids = 11,          // Daily Create Player id (최초 가입 유저)            > NRU 연산용.
        ReturnUserPids = 12,           // 당일 복귀 유저로 판별된 유저의 PID + 국가/마켓       > RAU 연산  // 얘는 지금 만들 필요 없음.

        // User Paid
        PaidUserDatas = 20,            // 유저의 결제 이력 (PID와 상품, 첫 결제 여부, 결제 내역 저장)    > NPU, PU, Revenue 연산 // ShopCustom 돌때 같이 검색함
    }
}