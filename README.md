# TrainingAssistant

Веб-платформа для персонального планирования тренировок и питания. По анкетным данным пользователя система формирует согласованный семидневный план: тренировки подбирает ML-сервис, питание рассчитывает backend. Поддерживаются дневник, учёт прогресса, экспорт плана в PDF и автоматическое продление недели.

## Состав репозитория

| Каталог | Назначение | Стек |
|---------|------------|------|
| `TrainingAssistant.Api` | REST API, бизнес-логика, БД, фоновое продление плана | ASP.NET Core 8, EF Core, PostgreSQL |
| `TrainingAssistant.Web` | Одностраничный веб-интерфейс | React, TypeScript |
| `TrainingAssistant.Ml` | Классификация типа программы и сборка тренировок | Python, FastAPI, scikit-learn |

## Архитектура

```
Браузер  →  Web (React)  →  API (ASP.NET Core)  →  PostgreSQL
                                    ↓
                              ML (FastAPI)
```

Пользователь работает только с web-клиентом. API хранит профиль, планы и прогресс, вызывает ML-сервис при генерации тренировок и сам рассчитывает меню. Фоновая служба на backend периодически создаёт следующую неделю после истечения текущего периода.

## Требования

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) 18+ и npm
- [Python](https://www.python.org/) 3.10+
- PostgreSQL 14+ (в `appsettings.json` по умолчанию порт **5433**)

## Быстрый старт

### 1. База данных

Создайте БД `training_assistant` и при необходимости поправьте строку подключения в  
`TrainingAssistant.Api/TrainingAssistant.Api/appsettings.json`.

Применить миграции:

```bash
cd TrainingAssistant.Api
dotnet run --project TrainingAssistant.Migrator
```

### 2. ML-сервис

```bash
cd TrainingAssistant.Ml
python -m venv .venv
.venv\Scripts\activate          # Windows
# source .venv/bin/activate     # Linux / macOS
pip install -r requirements.txt
uvicorn main:app --reload --port 8000
```

Модель по умолчанию: `artifacts/program_classifier.pkl`.  
Переобучение (при необходимости):

```bash
python -m ml.generate_dataset
python -m ml.train
```

Swagger ML-сервиса: http://localhost:8000/docs

### 3. Backend API

```bash
cd TrainingAssistant.Api
dotnet run --project TrainingAssistant.Api
```

- API: http://localhost:5031  
- Swagger: http://localhost:5031/swagger  

Ключ ML-сервиса в `appsettings.json` (`MlService:ApiKey`) должен совпадать с `API_KEY` в ML (по умолчанию `dev-ml-key`).

### 4. Web-клиент

```bash
cd TrainingAssistant.Web/training-assistant-web
npm install
npm start
```

Интерфейс: http://localhost:3000  

Адрес API задаётся переменной `REACT_APP_API_URL` (по умолчанию `http://localhost:5031`).

## Порядок запуска для разработки

1. PostgreSQL  
2. `TrainingAssistant.Ml` (порт 8000)  
3. `TrainingAssistant.Api` (порт 5031)  
4. `TrainingAssistant.Web` (порт 3000)

## Структура solution (API)

```
TrainingAssistant.Api/
├── TrainingAssistant.Api/          # REST-контроллеры
├── TrainingAssistant.Application/  # интерфейсы, DTO
├── TrainingAssistant.Domain/       # сущности и перечисления
├── TrainingAssistant.Infrastructure/ # EF Core, сервисы, ML-клиент
└── TrainingAssistant.Migrator/     # применение миграций и сидинг
```

## Конфигурация

| Параметр | Файл | Описание |
|----------|------|----------|
| PostgreSQL | `TrainingAssistant.Api/.../appsettings.json` | `ConnectionStrings:DefaultConnection` |
| JWT | там же | `Jwt:Key`, срок действия токена |
| ML-сервис | там же | `MlService:BaseUrl`, `ApiKey` |
| Автопродление недели | там же | `WeekRenewal:Enabled`, `IntervalMinutes` |
| URL API для фронта | `.env` в `training-assistant-web` | `REACT_APP_API_URL` |

