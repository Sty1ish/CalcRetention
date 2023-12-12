import random
import pymysql
import datetime
import numpy as np
from tqdm import tqdm

db = pymysql.connect(
    host="10.0.60.10",
    user="devel",
    password="tjqj@@##123",
    database="redash"
)

cursor = db.cursor()

'''
sql = "SELECT * FROM TBL_DailyKPI"
cursor.execute(sql)
result = cursor.fetchall()
for row in result:
    print(row)
'''

product_list = {'Product A' : 1.99, 
                'Product B' : 2.99, 
                'Product C' : 4.99, 
                'Product D' : 7.99, 
                'Product E' : 9.99, 
                'Product F' : 29.99, 
                'Product G' : 49.99}



def make_uid():
    UID = ''.join(random.sample(['A','B','C','D','E','F','G','H','I','J','K'], 6))
    return UID

def make_user_data(UID):
    country = np.random.choice(['KR','JP','CN','US'], 1, p = [0.5,0.25,0.15,0.1], replace = False)[0]
    market = random.sample([0,1,2], 1)[0]
    return UID, country, market

def write_sql(table, date, country, market, uid):
    sql = f"INSERT INTO {table} (logdate, CountryCode, MarketType, PID) VALUES (%s, %s, %s, %s)"
    cursor.execute(sql, (date, country, market, uid))
    db.commit()


def write_sql_market(date, country, market, uid, Product, Price, ShopType, PurchaseCount):
    sql = f"INSERT INTO TBL_PUBLISH_PurchasePids (logdate, CountryCode, MarketType, PID, Product, Price, ShopType, PurchaseCount) VALUES (%s, %s, %s, %s, %s, %s, %s, %s)"
    cursor.execute(sql, (date, country, market, uid, Product, Price, ShopType, PurchaseCount))
    db.commit()


DATE_RANGE = 45
total_uid = list(set([make_uid() for _ in range(25000)]))
total_user = list(set([make_user_data(uid) for uid in total_uid]))
start_user = random.sample(total_user, round(len(total_user)*0.3))
add_user = [i for i in total_user if i not in start_user]
START_DATE = datetime.datetime(2023,7,20)
current_user = [i for i in start_user]
purchase_cnt = {}



# 유저 행동 패턴 확률적 제작
for i in tqdm(range(DATE_RANGE)):
    TARGET_DATE = START_DATE + datetime.timedelta(days=i)
    
    temp_add_user = random.sample(add_user, round(len(add_user) * 0.05))
    add_user = [i for i in add_user if i not in temp_add_user]
    
    
    temp_return_user = random.sample(add_user, round(len(add_user) * 0.025))
    add_user = [i for i in add_user if i not in temp_return_user]
    
    # NRU
    for uid, country, market in temp_add_user:
        write_sql('TBL_PUBLISH_NewResisterPids', TARGET_DATE, country, market, uid)
        
    # RAU
    for uid, country, market in temp_return_user:
        write_sql('TBL_PUBLISH_ReturnUserPids', TARGET_DATE, country, market, uid)
        
    
    # 이탈 반영
    temp_dropout_user = random.sample(current_user, round(len(add_user) * 0.06))
    current_user.extend(temp_add_user)
    current_user.extend(temp_return_user)
    current_user = [i for i in current_user if i not in temp_dropout_user]
    
    
    # 해당 유저의 35% 일일 접속을 유지 가정.
    temp_current_user = random.sample(current_user, round(len(current_user) * 0.35))
    
    
    # DAU
    for uid, country, market in temp_current_user:
        write_sql('TBL_PUBLISH_UniquePids', TARGET_DATE, country, market, uid)
        
        
    # 확률적 구매
    for uid, country, market in temp_current_user:
        
        # 하루 3회 반복구매 테스트.
        for _ in range(3):
            # 10%구매 확률에 들었을 때
            if np.random.choice([0,1], 1,  p = [0.9,0.1], replace = False)[0]:
                # 구매 상품
                product = np.random.choice(list(product_list.keys()), 1, p = [0.3,0.275,0.2,0.1,0.075,0.025,0.025], replace = False)[0]
                price = product_list[product]
                shoptype = np.random.choice(['DefaultShop', 'PopupShop', 'EventShop'], 1, p = [0.5,0.25,0.25], replace = False)[0]
                
                if uid in purchase_cnt.keys():
                    purchase_cnt[uid] += 1
                else:
                    purchase_cnt[uid] = 1
            
                purchase_count = purchase_cnt[uid]
                
                write_sql_market(TARGET_DATE, country, market, uid, product, price, shoptype, purchase_count)
            
