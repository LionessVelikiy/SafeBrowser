# Generated by Django 3.2.5 on 2021-09-26 19:11

from django.db import migrations, models
import django.db.models.deletion


class Migration(migrations.Migration):

    dependencies = [
        ('users', '0003_childmodel_statusnet'),
        ('history', '0002_auto_20210926_1910'),
    ]

    operations = [
        migrations.AlterField(
            model_name='historymodel',
            name='queries',
            field=models.ForeignKey(blank=True, on_delete=django.db.models.deletion.CASCADE, to='users.usermodel', verbose_name='Фильтр'),
        ),
    ]
