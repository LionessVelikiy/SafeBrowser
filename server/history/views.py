import json

from django.http import JsonResponse
from django.http.response import HttpResponse
from django.shortcuts import render
import secure
# Create your views here.
from django.views.decorators.csrf import csrf_exempt
from cryptography.fernet import Fernet
from history.models import HistoryModel, QueryGroupToChildModel, QueriesModel, QueriesGroupModel
from users.views import getUserIdByToken
from users.models import UserModel, ChildModel





#ИЗ БРАУЗЕРА , ДОБАВЛЯЕТ ЗАПИСЬ В ИСТОРИЮ
@csrf_exempt
def addHistory(request):
    post = request.GET

    children = ChildModel.objects.get(
        number = post["user"]
    )

    HistoryModel.objects.create(
        url = post["url"] ,
        typeQuery = post["typeQuery"] ,
        queries_id = post["filter"] if not post["filter"] == "-"  else "",
        user_id=children.id,

    )

    return  JsonResponse({'status':True})


#ЛК ВЫВОДИТ ИСТОРИЮ 
@csrf_exempt
def getHistory(request):
    post = request.GET
    user = getUserIdByToken(request)
    userIds = []


    childrens = ChildModel.objects.filter(
        owner_id = user["id"]
    )


    for u in childrens:
        userIds.append(u.id)
    history= []
    historyQS = HistoryModel.objects.filter(
        user_id__in = userIds
    ).order_by("-id")

    for h in historyQS:
        history.append(h.getJson())

    return  JsonResponse(history, safe=False)




#список групп
@csrf_exempt
def getGroup(request):

    user = getUserIdByToken(request)

    groups = []
    groupsQS = QueriesGroupModel.objects.filter(
        user_id__in = [user["id"],1]
    )


    for t in groupsQS:
        tmp = t.getJson()
        tmp["canEdit"] = t.user.id == user["id"]
        groups.append(tmp)

    return  JsonResponse(groups, safe=False)


#создать группу
@csrf_exempt
def addGroup(request):
    post = json.loads(request.body)
    user = getUserIdByToken(request)


    group = QueriesGroupModel.objects.create(
        title = post["title"],
        description = post["description"],
        type = post["type"] ,
        age = post["age"] ,
        user_id = user["id"]
    )
    groupJson = group.getJson()
    groupJson["canEdit"] = user["id"] == group.user.id
    return  JsonResponse(groupJson, safe=False)


#удалить группу
@csrf_exempt
def deleteGroup(request):
    post = json.loads(request.body)
    user = getUserIdByToken(request)


    QueriesGroupModel.objects.filter(
        id= post["id"]
    ).delete()


    return  JsonResponse({
        "status":True,
        "id":post["id"]
    }, safe=False)



#список запросов по группе
@csrf_exempt
def getQueries(request):
    post = request.GET
    user = getUserIdByToken(request)



    queries = []
    queriesQS = QueriesModel.objects.filter(
       group_id = post["id"]
    )


    for q in queriesQS:
        queries.append(q.getJson())


    return  JsonResponse(queries, safe=False)


#создать запрос
@csrf_exempt
def addQuery(request):
    post = json.loads(request.body)

    query = QueriesModel.objects.create(
        value = post["value"],
        type = post["type"],
        group_id =post["group"]
    )


    return  JsonResponse(query.getJson(), safe=False)

#удалить запрос
@csrf_exempt
def deleteQuery(request):
    post = json.loads(request.body)

    QueriesModel.objects.filter(
        id = post["id"]
    ).delete()


    return  JsonResponse({
        "status":True,
        "id":post["id"]
    }, safe=False)



#применить группу к пользователю
@csrf_exempt
def applyGroup(request):
    post = json.loads(request.body)

    if post['status']:

        QueryGroupToChildModel.objects.create(
            child_id=post['child'],
            group_id=post['group']
        )

    else: 
        QueryGroupToChildModel.objects.filter(
            child_id=post['child'],
            group_id=post['group']
        ).delete()

    return  JsonResponse({
        "status":True
    }, safe=False)

#из браузера
@csrf_exempt
def getFilter(request):
    post = request.GET

    type = post["type"]
    number = post["number"]
    
    child = ChildModel.objects.get(number = number)
    groups = QueryGroupToChildModel.objects.filter(child_id = child.id) 
    group_ids = []
    for item in groups:
        #if item.group.type.lower() == type.lower():
            group_ids.append(item.group.id)
    
    result = []
    querys = QueriesModel.objects.filter(group_id__in = group_ids)
    for q in querys:
        result.append(q.getFilterString())
    

    return HttpResponse( "|".join(result) )
