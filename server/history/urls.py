from django.conf.urls import url
from . import views


urlpatterns = [
    url(r'^add/$', views.add, name='add'), #получаем информацию о вопросе по id



    url(r'^test/$', views.test, name='test'), #получаем информацию о вопросе по id
    url(r'^addGroup/$', views.addGroup, name='addGroup'), #получаем информацию о вопросе по id
    url(r'^deleteGroup/$', views.deleteGroup, name='deleteGroup'), #получаем информацию о вопросе по id
    url(r'^getGroup/$', views.getGroup, name='getGroup'), #получаем информацию о вопросе по id
    url(r'^applyGroup/$', views.applyGroup, name='applyGroup'), #получаем информацию о вопросе по id
    url(r'^addQuery/$', views.addQuery, name='addQuery'), #получаем информацию о вопросе по id
    url(r'^deleteQuery/$', views.deleteQuery, name='deleteQuery'), #получаем информацию о вопросе по id
    url(r'^getQueries/$', views.getQueries, name='getQueries'), #получаем информацию о вопросе по id
    url(r'^getFilter/$', views.getFilter, name='getFilter'), #получаем информацию о вопросе по id
    url(r'^getHistory/$', views.getHistory, name='getHistory'), #получаем информацию о вопросе по id
]