# DataModelDevOpsExtractor - Test Suite

## 📁 Struttura del Progetto di Test

```
DataModelDevOpsExtractor.Tests/
├── Model/                          # Test per le entità di dominio
│   ├── DataModelTaskRowTests.cs
│   ├── EnumTests.cs
│   └── UploadStatusEntryTests.cs
├── Repository/                     # Test per i repository
│   └── DataModelRepositoryTests.cs
├── Service/                        # Test per i servizi
│   ├── MarkdownUtilitiesServiceTests.cs
│   └── PrimaryAttributeResolverServiceTests.cs
└── TestHelpers/                    # Utility per i test
    ├── FakeOrganizationService.cs
    └── TestDataBuilder.cs
```

## 🧪 Tipologie di Test

### 1. **Repository Tests**
- **DataModelRepositoryTests**: verifica operazioni CRUD su entità custom
  - Creazione e recupero tabelle
  - Creazione e recupero colonne
  - Gestione lookup
  - Validazione duplicati

### 2. **Service Tests**
- **PrimaryAttributeResolverServiceTests**: verifica logica risoluzione attributi primari
  - Identificazione colonne primarie
  - Validazione unicità
  - Gestione errori (duplicati)
  
- **MarkdownUtilitiesServiceTests**: verifica normalizzazione prefissi
  - Case sensitivity
  - Trim e formatting
  - Gestione caratteri speciali

### 3. **Model Tests**
- **EnumTests**: verifica correttezza valori enum
- **DataModelTaskRowTests**: verifica DTO
- **UploadStatusEntryTests**: verifica stato operazioni

## 🔧 Test Helpers

### FakeOrganizationService
Implementazione in-memory di `IOrganizationService` per test senza connessione a Dynamics 365:
- Supporta Create, Update, Delete, Retrieve, RetrieveMultiple
- Memorizza entità in dictionary
- Gestisce QueryExpression con condizioni semplici

### TestDataBuilder
Pattern Builder per creare dati di test:
- `CreateTableEntity()`: crea entità tabella
- `CreateColumnEntity()`: crea entità colonna
- `CreateDataModelTaskRow()`: crea DTO con valori di default
- `CreateMultipleRows()`: genera batch di righe

## ▶️ Eseguire i Test

### Visual Studio
1. Apri **Test Explorer** (Test → Test Explorer)
2. Click su "Run All Tests"

### Comando CLI
```bash
dotnet test DataModelDevOpsExtractor.Tests/DataModelDevOpsExtractor.Tests.csproj
```

### Con Coverage
```bash
dotnet test --collect:"Code Coverage"
```

## 📊 Coverage Obiettivi

- **Repository Layer**: >80%
- **Service Layer**: >75%
- **Model Layer**: >90%

## 🚀 Aggiungere Nuovi Test

1. Creare file nella cartella appropriata (Repository/Service/Model)
2. Nominare il file: `<NomeClasse>Tests.cs`
3. Usare attributo `[TestClass]` per la classe
4. Usare attributo `[TestMethod]` per ogni test
5. Seguire naming convention: `NomeMetodo_Scenario_RisultatoAtteso()`

### Esempio
```csharp
[TestMethod]
public void GetTableByName_WhenTableExists_ReturnsEntity()
{
    // Arrange
    var tableName = "contact";
    
    // Act
    var result = _repository.getTableByName(tableName);
    
    // Assert
    Assert.IsNotNull(result);
}
```

## 📝 Best Practices

1. **AAA Pattern**: Arrange → Act → Assert
2. **Un solo assert logico per test** (quando possibile)
3. **Nomi descrittivi**: il nome del test documenta il comportamento
4. **Test isolati**: ogni test deve essere indipendente
5. **Setup/Cleanup**: usare `[TestInitialize]` e `[TestCleanup]`

## ⚠️ Limitazioni Correnti

- DevOpsRepository non testato (richiede mock complessi di Azure DevOps API)
- EnvironmentRepository non testato (richiede mock di Metadata API)
- Test di integrazione end-to-end non presenti (richiedono ambiente D365 reale)

## 🔮 Test Futuri da Aggiungere

- [ ] DataModelService integration tests
- [ ] DevOpsDataModelParser tests (parsing HTML/Markdown)
- [ ] EnvironmentRepository unit tests (con mock di Execute)
- [ ] UploadProgressService tests
- [ ] E2E tests con database in-memory

## 📚 Riferimenti

- [MSTest Documentation](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-mstest)
- [Moq Documentation](https://github.com/moq/moq4)
- [Unit Testing Best Practices](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)
