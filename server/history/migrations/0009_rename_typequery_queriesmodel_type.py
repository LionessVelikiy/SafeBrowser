# Generated by Django 3.2.5 on 2021-09-28 20:54

from django.db import migrations


class Migration(migrations.Migration):

    dependencies = [
        ('history', '0008_auto_20210927_1843'),
    ]

    operations = [
        migrations.RenameField(
            model_name='queriesmodel',
            old_name='typeQuery',
            new_name='type',
        ),
    ]
