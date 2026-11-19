# ComputerStore

1. В phpMyAdmin Open Server откройте вкладку SQL и выполните `database.sql`.
2. Откройте `ComputerStore.csproj` в Visual Studio и восстановите NuGet-пакеты.
3. Если Open Server использует не `root` без пароля на `127.0.0.1:3306`, замените только строку `ConnectionString` в `Db.cs`.
4. Соберите и запустите проект.

Приложение поддерживает CRUD категорий и товаров, историю продаж, контроль остатка и транзакционное оформление продажи.
