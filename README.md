# OrganizandoBolso API

Complete personal budgets and expenses management system built with .NET 8.0 following Clean Architecture.

## 🏗️ Architecture

The project follows Clean Architecture principles with the following layers:

- **Domain**: Domain models, interfaces and base entities
- **Application**: Business services and application logic
- **Repository**: Data access implementations with MongoDB
- **API**: REST controllers with Swagger

## 🚀 Technologies

- **.NET 8.0** - Main framework
- **MongoDB** - NoSQL database
- **Swagger** - API documentation
- **Docker** - Containerization
- **Clean Architecture** - Architectural pattern

## 📁 Project Structure

```
OrganizandoBolso/
├── OrganizandoBolso.Domain/          # Models and interfaces
├── OrganizandoBolso.Application/     # Business services
├── OrganizandoBolso.Repository/      # Data access
├── OrganizandoBolso.API/             # REST API
├── Dockerfile.API                    # API container
├── docker-compose.yml                # Services orchestration
├── mongo-init.js                     # MongoDB initialization script
└── README.md                         # This file
```

## 🛠️ Prerequisites

- .NET 8.0 SDK
- Docker and Docker Compose
- MongoDB (optional for local development)

## 🚀 How to Run

### 1. Clone the repository
```bash
git clone https://github.com/seu-usuario/organizandobolso.api.git
cd organizandobolso.api
```

### 2. Run with Docker Compose
```bash
docker-compose up -d
```

### 3. Access the application
- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000
- **MongoDB**: localhost:27017

### 4. Run locally (optional)
```bash
cd OrganizandoBolso.API
dotnet run
```

## 📊 Data Models

### Budget
- Name, icon
- Amount
- Representative color
- Audit properties

### Expense
- Budget id, name, amount
- Description
- Representative color
- Audit properties

### Log
- Tracks operations
- Change history
- User and IP information

## 🔧 Settings

### MongoDB
```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "OrganizandoBolso"
  }
}
```



## 📚 API Endpoints

### Budgets
- `GET /api/budget` - List all budgets
- `GET /api/budget/{id}` - Get by ID
- `POST /api/budget` - Create budget
- `PUT /api/budget/{id}` - Update budget
- `DELETE /api/budget/{id}` - Delete budget

### Expenses
- `GET /api/expense` - List all expenses
- `GET /api/expense/{id}` - Get by ID
- `POST /api/expense` - Create expense
- `PUT /api/expense/{id}` - Update expense
- `DELETE /api/expense/{id}` - Delete expense

## 🔒 Security

- Input validation on all endpoints
- Audit logs for traceability
- Data sanitization before persistence
- CORS configured for development

## 📈 Performance

- MongoDB connection pooling
- Response compression
- In-memory cache for frequent data
- Async/await for all I/O operations

## 🐳 Docker

### Build API image
```bash
docker build -f Dockerfile.API -t organizandobolso-api .
```

### Run services
```bash
docker-compose up -d
```

### Stop services
```bash
docker-compose down
```

## 🧪 Tests

To run tests:
```bash
dotnet test
```

## 📝 Logs

Logs are configured for different levels:
- **Development**: Debug, Info, Warning, Error
- **Production**: Info, Warning, Error

## 🌍 Environments

- **Development**: Local development settings
- **Production**: Production-optimized settings
- **Docker**: Container-specific settings

## 🤝 Contributing

1. Fork the project
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is under the MIT license. See [LICENSE](LICENSE) for more details.

## 📞 Support

For support and questions:
- Email: contato@organizandobolso.com
- Issues: [GitHub Issues](https://github.com/seu-usuario/organizandobolso.api/issues)

## 🔄 Changelog

- **v1.0.0**: Initial version with basic features
- Budgets and expenses support
- Full audit system
- REST API documented with Swagger
- Docker containerization
