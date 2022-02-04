from django.contrib import admin

# Register your models here.
from users.models import UserModel, ChildModel


class UsersAdmin(admin.ModelAdmin):
    list_display = ('__str__',)


admin.site.register(UserModel,UsersAdmin)
admin.site.register(ChildModel,UsersAdmin)