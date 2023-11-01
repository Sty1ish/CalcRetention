using LogTransfomer.Analytics;
using LogTransfomer.Analytics.Calculator;
using LogTransfomer.Analytics.Data;
using LogTransfomer.Database.GameLog.Types;

namespace LogTransfomer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /// config 
            /// database info
            var LogDBipAddress = "yourserverIP";
            var LogDBuserName = "youruserName";
            var LogDBpassword = "yourPasswd";
            var AnalyticsDBipAddress = "yourserverIP";
            var AnalyticsDBuserName = "youruserName";
            var AnalyticsDBpassword = "yourPasswd";
            var TargetDate = new DateTime(2023, 10, 20); // crontab에 물릴거면 Now.Date 사용. (연산 기준일)
            var AnalyticsDate = 60;

           
            // Retention Query Setting
            var startDate = TargetDate.AddDays(AnalyticsDate * -1); // AnalyticsDate만큼 이전 데이터를 조회 (데이터 조회 시작일)

            // Retention Calulator init + GetUniquePids
            var data = new Calculator_Retention(LogDBipAddress, LogDBuserName, LogDBpassword); 
            var uniquepid = data.GetUniquePids(startDate, AnalyticsDate);                      
            var Analytics_Retention = new Analytics_Retention(uniquepid);                      // Pid값 전달 > Analytics_Retention 객체 생성
            var countryInfo = Analytics_Retention.countryList();                               // 전달 받은 국가 리스트


            // Analytics Retention
            var retentionData = new List<AnalyData_RetentionData>();

            // Add Condition
            foreach (var targetMarket in Enum.GetValues(typeof(MarketType))) 
            {
                foreach (var targetCountry in countryInfo)
                {
                    // Rollup 결과 초기화 + 2중 For문 진입 후 필터 적용
                    Analytics_Retention.RollbackData(); 
                    Analytics_Retention.fillterData(targetCountry, (MarketType)targetMarket);

                    for (int i = 0; i <= AnalyticsDate; i++)
                    {
                        var queryDate = TargetDate.Date.AddDays(i * -1);

                        // is Rollup 처리 유무에 따라. 기준일 접속한 Pid값 기준 Rollup 실시
                        // Analytics_Retention.RollupData(QueryDate); // Rollup이 포함되면, Rollup / Fillter / Rollup이 모두 이 위치에 자리해야
                        
                        var retention = Analytics_Retention.SummariseRetention(queryDate); // 기준일의 데이터 확보

                        // 없는 날짜 연산시 null 반환 (저장하면 안되는 데이터)
                        if (retention == null) 
                        {
                            continue;
                        }
                        // null이 아닐때 저장.
                        retentionData.AddRange(retention);
                    }
                }
            }
            // Retention Uploader init
            var retention_uploadeer = new Analytics_Retention_Uploader(AnalyticsDBipAddress, AnalyticsDBuserName, AnalyticsDBpassword);

            // Save Retention Data
            retention_uploadeer.SaveDB_RetentionData(TargetDate, retentionData);








            ///////////////////////////////////////////////////////////////////////////
            /// C# 컨버터 제작 목적
            /// 목표 1. 지표가, 어떤 로그에서 발생하는지 문서 정리 위함
            /// 목표 2. 로그 -> 지표의 연산 과정을 정리하여 지표 오해 최소화
            /// 목표 3. "일자별" 지표 데이터 뿐만 아닌, "유저별" 데이터를 획득/분석하여 게임 개선을 위한 인사이트 도출
            ///
            ///
            ///
            /// 구현할 목록
            ///
            /// 추가 구현 1 (기존 구현분 / 단순)
            /// count / SUM 관련 지표 연산 필요
            /// ex) DAU, Rev, Purchaselist
            ///
            ///
            /// 추가 구현 2 (연산 심화)
            /// 구현1과 Retention Part를 이용해 C.C / Lness 연산과 같은 알고리즘 생성
            ///
            ///
            ///
            /// 추가구현 3 (유저단위 데이터 추출)
            /// 시작 ~ 종료일 기준으로 유저 명단 추출 후
            /// 특정 "이벤트/퍼널" 달성 유무, 레벨 변동 폭, 성장 단계, 아이템 소모량, 각종 게임 지표를 정의해, 최근 7일 접속 유무를 확인하는 코드 필요.
            /// Aha Moment 탐색을 위해 필수적
            /// UID 단위로 3일 연속 접속 : T, 5일 연속 접속 : T, 100 스테이지 통과 여부 : F ... 아이템 소모 20개 이상 : T... 를 뽑고
            /// 어떤 패턴을 달성 했을때, 잔존율이 70, 80, 90% 이상인지 확인하기 위함.
            /// 추출 가능한 수준인 경우 Python이 편리하나, 어느정도는 정제해서 나가야함.
            /// 변수 정의는 필수적 사항. 로그 정의서 보고 이벤트 정의 필요. / Int/float로 뽑아서 적재하고, 분석때 Python으로 범주화 하는게 좋을 것.
            ///////////////////////////////////////////////////////////////////////////




            /// 데이터 직렬화해서 뽑아보자. Value값 탐색.

            // 역직렬화 함수
            ///var deserialized = MemoryPackSerializer.Deserialize<GameLoglogData>(serialized);
            ///Console.WriteLine($"v1={deserialized.Val1}, v2={deserialized.Val2}");


            // memory pack EX
            //var tmpDate = new DateTime(2023, 8, 16); // AnalyticsDate만큼 이전 데이터를 조회
            //var tmpLoader = new Calulator_UserInfo(LogDBipAddress, LogDBuserName, LogDBpassword);
            //var gameLogDatas = tmpLoader.GetGameLogData(tmpDate);

            //Console.WriteLine(gameLogDatas);

            //foreach (var log in gameLogDatas) 
            //{
            //    if (log.LogType == LogType.Login) 
            //    {
            //        Console.WriteLine($"Log type: {log.LogType}, Pid: {log.Pid}");
            //        var logData = MemoryPackSerializer.Deserialize<LogData_Login>(log.LogData);
            //        Console.WriteLine($"val1: {logData.IsNew}, val2: {logData.UserId}");
            //    }
            //    
            //
            //    //var logData = MemoryPackSerializer.Deserialize<GameLoglogData>(log.byteLogData);
            //    // 
            //}



            /// 시작 byte가 11로 시작하는 애들
            /// 시작 byte가 139로 시작하는 애들 ㅣ 로그인/에피소드 시작+종료/커스텀상점 갱신/status 

        }
    }
}

