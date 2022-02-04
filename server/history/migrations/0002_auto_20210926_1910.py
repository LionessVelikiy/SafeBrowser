# Generated by Django 3.2.5 on 2021-09-26 19:10

from django.db import migrations, models
import django.db.models.deletion


class Migration(migrations.Migration):

    dependencies = [
        ('users', '0003_childmodel_statusnet'),
        ('history', '0001_initial'),
    ]

    operations = [
        migrations.CreateModel(
            name='HistoryModel',
            fields=[
                ('id', models.BigAutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('url', models.CharField(default='', max_length=300, verbose_name='Имя')),
                ('status', models.TextField(blank=True, default='', verbose_name='Описание')),
                ('queries', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, to='users.usermodel', verbose_name='Фильтр')),
                ('user', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, to='users.childmodel', verbose_name='Пользователь')),
            ],
            options={
                'verbose_name': 'История',
                'verbose_name_plural': 'История',
            },
        ),
        migrations.CreateModel(
            name='QueriesModel',
            fields=[
                ('id', models.BigAutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('value', models.CharField(default='', max_length=300, verbose_name='Значение')),
                ('typeQuery', models.TextField(blank=True, choices=[('URL', 'Сайт'), ('WORD', 'Фраза'), ('QUERY', 'Запрос')], default='', verbose_name='Тип')),
                ('user', models.FileField(blank=True, upload_to='images/users/', verbose_name='Фото')),
            ],
            options={
                'verbose_name': 'Запрос',
                'verbose_name_plural': 'Запросы',
            },
        ),
        migrations.DeleteModel(
            name='SupportModel',
        ),
    ]