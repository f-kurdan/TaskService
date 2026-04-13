# Руководство по изменениям

## Структура репозитория
* `src/` - исходный код приложения по слоям `API`, `Application`, `Domain`, `Infrastructure`.
* `docs/` - документация по сервису и принятым решениям.
* `tests/` - автоматические тесты и материалы для проверки.
* корневые `UIEngine.sln`, `Dockerfile`, `docker-compose.yml`, `.gitlab-ci.yml` используются для сборки и поставки.

Не размещайте исходный код и служебные файлы вне этих каталогов без отдельной договорённости.

## Целевая версия .NET
* **net8.0**

## Зависимости
### NuGet-пакеты
* **Microsoft.AspNetCore.Authentication.JwtBearer**
* **Swashbuckle.AspNetCore**
* **Microsoft.EntityFrameworkCore**
* **Npgsql.EntityFrameworkCore.PostgreSQL**
* **PIT.Infrastructure**
* **Serilog.AspNetCore**
* **DynamicDirectories.Client**
* **PIT.HybridCache**
* **PIT.Tarantool**
* **TarantoolClient**

### Зависимые сервисы
* `PostgreSQL` или основное хранилище: проверьте доступность и корректность строки подключения там, где это применимо.
* `AuthService`: проверьте настройки аутентификации и авторизации, если сервис защищён через JWT.
* `CardService`: проверьте доступность внешнего API, если сервис зависит от карточных API.
* `Tarantool`: проверьте доступность кэша или хранилища, если сервис его использует.
* `PIT.Infrastructure`: предоставляет логирование, телеметрию, health checks и общую API-инфраструктуру.

## Локальный запуск
* Проверьте, что в `src/UIEngine.API/appsettings.json` корректно заполнены следующие секции: `ConnectionStrings`, `CardService`, `Infrastructure`, `AuthService`, `Tarantool`, `Caching`, `SidebarConfigStorage`.
* Если вы проверяете локальную инициализацию ролевых sidebar-конфигов, заполните секцию `SidebarConfigInitialization` в `src/UIEngine.API/appsettings.Development.json`.
* Убедитесь, что сервис собирается и запускается локально без ошибок.
* При необходимости поднимите внешние зависимости локально или укажите общую тестовую среду.
* Команда запуска: `dotnet run --project src/UIEngine.API/UIEngine.API.csproj`

### Базовые команды
* `dotnet restore UIEngine.sln`
* `dotnet build UIEngine.sln`
* `dotnet run --project src/UIEngine.API/UIEngine.API.csproj`

## Чек-лист для MR
* Проект собирается без ошибок.
* Проверена совместимость зависимых сервисов и пакетов.
* Сервис запускается локально без ошибок.
* Проверено подключение к базе данных или хранилищу там, где это применимо.
* Проверена интеграция с `AuthService`.
* Проверена интеграция с `CardService`.
* Проверена доступность `Tarantool` там, где это применимо.
* `OpenAPI` / `Swagger` открывается корректно, если сервис публикует HTTP API.
* Версия в `.csproj` обновлена при необходимости.
* CI pipeline прошёл успешно.
* Документация обновлена при необходимости.
