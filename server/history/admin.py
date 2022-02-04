from django.contrib import admin

# Register your models here.
from history.models import QueriesModel, HistoryModel, QueriesGroupModel, QueryGroupToChildModel


class HistoryAdmin(admin.ModelAdmin):
    list_display = ('__str__',)
# admin.site.register(QueryGroupToChildModel,HistoryAdmin)
# admin.site.register(QueriesGroupModel,HistoryAdmin)
# admin.site.register(HistoryModel,HistoryAdmin)
# admin.site.register(QueriesModel,HistoryAdmin)