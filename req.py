import sys
import requests

URL = str("https://swing.langara.bc.ca/prod/hzgkfcls.P_GetCrse")#?term_in=202310&sel_subj=dummy&sel_day=dummy&sel_schd=dummy&sel_insm=dummy&sel_camp=dummy&sel_levl=dummy&sel_sess=dummy&sel_instr=dummy&sel_ptrm=dummy&sel_attr=dummy&sel_dept=dummy&sel_subj=dummy&sel_crse=&sel_title=%25&sel_dept=%25&sel_ptrm=%25&sel_schd=%25&begin_hh=0&begin_mi=0&begin_ap=a&end_hh=0&end_mi=0&end_ap=a&sel_incl_restr=Y&sel_incl_preq=Y&SUB_BTN=Get+Courses")

Headers = dict({
       "Accept": "image/avif,image/webp,*/*",
       "Accept-Encoding": "gzip, deflate, br",
       "Accept-Language": "en-US,en;q=0.5",
       "Connection": "keep-alive",
       "Cookie":"__utma=7072176.677137216.1667324908.1671751024.1671761475.25; __utmz=7072176.1669747677.15.3.utmcsr=langara.ca|utmccn=(referral)|utmcmd=referral|utmcct=/",
       "Host":"swing.langara.bc.ca",
       "Referer":"https://swing.langara.bc.ca/prod/hzgkfcls.P_Sel_Crse_Search?term=202310",
       "Sec-Fetch-Dest": "image",
       "Sec-Fetch-Mode": "no-cors",
       "Sec-Fetch-Site": "same-origin"
    })

RequestBody = dict({
             
        })

def Main(args: list[str]):
    if (args[1] == "get"):
        print(requests.get("https://swing.langara.bc.ca/prod/hzgkfcls.P_GetCrse?term_in=202310&sel_subj=dummy&sel_day=dummy&sel_schd=dummy&sel_insm=dummy&sel_camp=dummy&sel_levl=dummy&sel_sess=dummy&sel_instr=dummy&sel_ptrm=dummy&sel_attr=dummy&sel_dept=dummy&sel_crse=&sel_title=%25&sel_dept=%25&sel_ptrm=%25&sel_schd=%25&begin_hh=0&begin_mi=0&begin_ap=a&end_hh=0&end_mi=0&end_ap=a&sel_incl_restr=Y&sel_incl_preq=Y&SUB_BTN=Get+Courses").content)
        return
    print(requests.post(URL, headers=Headers, data=RequestBody))

Main(sys.argv) 

