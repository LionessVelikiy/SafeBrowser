# Generated by Django 3.2.5 on 2021-09-26 19:58

from django.db import migrations, models
import django.db.models.deletion


class Migration(migrations.Migration):

    dependencies = [
        ('history', '0005_auto_20210926_1936'),
    ]

    operations = [
        migrations.AlterField(
            model_name='historymodel',
            name='queries',
            field=models.ForeignKey(blank=True, null=True, on_delete=django.db.models.deletion.CASCADE, to='history.queriesmodel', verbose_name='Фильтр'),
        ),
    ]
