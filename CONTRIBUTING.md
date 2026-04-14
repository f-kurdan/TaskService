# Руководство по изменениям

## Структура репозитория

- `src/` — исходный код по слоям: `Api`, `Application`, `Domain`, `Infrastructure`
- `tests/` — автоматические тесты
- `docs/` — документация и принятые решения
- Корневые `TaskService.sln`, `Dockerfile`, `docker-compose.yml`, `.gitlab-ci.yml` — сборка и поставка

Не размещайте исходный код и служебные файлы вне этих каталогов без отдельной договорённости.

## Целевая версия .NET

**net8.0**

## Зависимости

### NuGet-пакеты

- `Microsoft.AspNetCore.OpenApi`
- `Swashbuckle.AspNetCore`
- `Microsoft.EntityFrameworkCore`
- `Npgsql.EntityFrameworkCore.PostgreSQL`

### Зависимые сервисы

- **PostgreSQL** — основное хранилище данных. Убедитесь, что БД доступна и строка подключения корректна.

## Локальный запуск

1. Проверьте, что в `src/TaskService.Api/appsettings.json` заполнена секция `ConnectionStrings:DefaultConnection`.
2. Убедитесь, что PostgreSQL запущен и доступен на указанном хосте.
3. Примените миграции:

```bash
dotnet ef database update \
  --project src/TaskService.Infrastructure/TaskService.Infrastructure.csproj \
  --startup-project src/TaskService.Api/TaskService.csproj \
  --context AppDbContext
```

4. Запустите сервис:

```bash
dotnet run --project src/TaskService.Api/TaskService.csproj
```

### Базовые команды

```bash
dotnet restore TaskService.sln
dotnet build TaskService.sln
dotnet test TaskService.sln
```

## Миграции EF Core

**Создать новую миграцию:**

```bash
dotnet ef migrations add <MigrationName> \
  --project src/TaskService.Infrastructure/TaskService.Infrastructure.csproj \
  --startup-project src/TaskService.Api/TaskService.csproj \
  --context AppDbContext \
  --output-dir Persistence/Migrations
```

**Применить миграции:**

```bash
dotnet ef database update \
  --project src/TaskService.Infrastructure/TaskService.Infrastructure.csproj \
  --startup-project src/TaskService.Api/TaskService.csproj \
  --context AppDbContext
```

## Чек-лист для MR

- [ ] Проект собирается без ошибок (`dotnet build`)
- [ ] Все тесты проходят (`dotnet test`)
- [ ] Если изменена EF-модель — добавлена миграция и обновлён `ModelSnapshot`
- [ ] Swagger UI открывается корректно (`/swagger`)
- [ ] `swagger/v1/swagger.json` отдаётся без ошибок
- [ ] Подключение к PostgreSQL проверено локально
- [ ] Версия в `.csproj` обновлена при необходимости
- [ ] CI pipeline прошёл успешно
- [ ] Документация обновлена при необходимости
