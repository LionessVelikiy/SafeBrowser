# Generated by Django 3.2.5 on 2021-09-27 18:43

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('history', '0007_auto_20210927_1138'),
    ]

    operations = [
        migrations.AddField(
            model_name='queriesgroupmodel',
            name='age',
            field=models.CharField(default='', max_length=300, verbose_name='Возраст'),
        ),
        migrations.AddField(
            model_name='queriesgroupmodel',
            name='type',
            field=models.CharField(default='', max_length=300, verbose_name='Тип'),
        ),
    ]
