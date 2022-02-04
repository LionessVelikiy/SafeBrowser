from django.db import models
from users.models import ChildModel, UserModel
# Create your models here.
    
class QueriesGroupModel(models.Model):
    class Meta:
        verbose_name = 'Группа запросов'
        verbose_name_plural = 'Группы запросов'

    
    title = models.CharField(max_length=300, verbose_name='Название', default='')
    age = models.CharField(max_length=300, verbose_name='Возраст', default='')
    type = models.CharField(max_length=300, verbose_name='Тип', default='')
    description = models.TextField(verbose_name="Описание",   default="",blank=True)
    user = models.ForeignKey(UserModel,blank=True, null=True,  on_delete=models.CASCADE)


    def __str__(self):
        return self.title


    def getJson(self):
        return  {
            'id':self.id,
            'title':self.title,
             'type':self.type,
             'age':self.age,
            'description':self.description,
            'user':self.user.getJson(),
        }

class QueriesModel(models.Model):
    class Meta:
        verbose_name = 'Запрос'
        verbose_name_plural = 'Запросы'
    QUERY_CHOICES = (
        ("URL", "Сайт"),
        ("WORD", "Фраза"),
        ("QUERY", "Запрос")
    )
    
    value = models.CharField(max_length=300, verbose_name='Значение', default='')
    type = models.CharField(max_length=100,verbose_name="Тип",   choices=QUERY_CHOICES, default="",blank=True)
    group =models.ForeignKey(QueriesGroupModel,blank=True, null=True,  on_delete=models.CASCADE)


    def __str__(self):
        return self.value
    
    def getFilterString(self):
        return  "~".join([self.value,self.type,str(self.id), str(self.group.type)])

    def getJson(self):
        return  {
            'id':self.id,
            'value':self.value,
            'type':self.type,
            'group':self.group.getJson(),
        }



class QueryGroupToChildModel(models.Model):
    class Meta:
        verbose_name = 'Связь'
        verbose_name_plural = 'Связи'

    child = models.ForeignKey(ChildModel,verbose_name='Пользователь',  on_delete=models.CASCADE)
    group = models.ForeignKey(QueriesGroupModel,verbose_name='Группа запросов',  on_delete=models.CASCADE)


    def getJson(self):
        return  self.group.getJson()




class HistoryModel(models.Model):
    class Meta:
        verbose_name = 'История'
        verbose_name_plural = 'История'

    
    url = models.CharField(max_length=300, verbose_name='URL', default='')
    typeQuery = models.TextField(verbose_name="Описание",default="",blank=True)
    user = models.ForeignKey(ChildModel,verbose_name='Пользователь',  on_delete=models.CASCADE)
    queries = models.ForeignKey(QueriesModel,verbose_name='Фильтр',blank=True, null=True,  on_delete=models.CASCADE)
    date = models.DateTimeField(verbose_name='Дата', auto_now=True)

    def __str__(self):
        return self.url


    def getJson(self):
        return  {
            'id':self.id,
            'date':self.date,
            'url':self.url,
            'typeQuery':self.typeQuery,
            'user':self.user.getJson(),
            'queries': self.queries.getJson() if self.queries is not None else None,
            #'queries': self.queries is None if  None else  self.queries.getJson(),
        }