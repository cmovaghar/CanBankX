# Critères d'Acceptation — CanBankX

---

## CA-01 : Dépendances Dirigées (Inward Dependencies)

### Description

Les dépendances entre couches au sein de chaque service doivent être **strictement dirigées vers l'intérieur** : Infrastructure → Application → Domain. Le Domain ne doit avoir aucune dépendance sur des frameworks d'infrastructure.

### Critères de vérification

| ID | Critère | Vérification | Statut |
|----|---------|-------------|--------|
| CA-01-01 | Le projet `Domain` ne référence ni EF Core, ni ASP.NET Core, ni MediatR (sauf `IDomainEvent : INotification`) | Revue du `.csproj` Domain | Conforme |
| CA-01-02 | Les interfaces de repositories (`IUserRepository`, `IAccountRepository`, `ITransactionRepository`) sont définies dans `Domain` | Namespace `*.Domain.Repositories` | Conforme |
| CA-01-03 | Les interfaces de services domaine (`IPasswordHasher`, `IJwtService`, etc.) sont définies dans `Domain` | Namespace `*.Domain.Services` | Conforme |
| CA-01-04 | Les implémentations concrètes (EF Core, BCrypt, JWT) sont dans `Infrastructure` | Namespace `*.Infrastructure.Services` | Conforme |
| CA-01-05 | Les exceptions de domaine n'héritent pas d'exceptions HTTP | Classe `DomainValidationException : Exception` | Conforme |
| CA-01-06 | Aucun service ne référence directement le projet d'un autre service | Solution `.sln` — références projet | Conforme |

### Exemple de conformité

```csharp
// CORRECT : Interface dans Domain
namespace Users.Domain.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> FindByIdAsync(Guid id, ...);
    }
}

// CORRECT : Implémentation dans Infrastructure
namespace Users.Infrastructure.Repositories
{
    public class UserRepository(UserDbContext context) : IUserRepository { ... }
}

// INCORRECT (non présent) : EF Core dans Domain
namespace Users.Domain.Entities
{
    [Table("users")] // Interdit
    public class User { ... }
}
```

---

## CA-02 : Séparation Domain ↔ Infrastructure

### Description

Le modèle de domaine (entités, value objects, domain events) doit être pur : pas d'annotations d'infrastructure, pas de constructeurs publics exposant la persistence, isolation via le pattern Repository.

### Critères de vérification

| ID | Critère | Vérification | Statut |
|----|---------|-------------|--------|
| CA-02-01 | Les entités n'ont pas de constructeurs publics (création via méthodes statiques de fabrique) | `User.Register()`, `Account.Open()`, `Transaction.Initiate()` | Conforme |
| CA-02-02 | Les propriétés des entités sont en lecture seule (`private set`) | Inspection des agrégats | Conforme |
| CA-02-03 | Les Value Objects sont immuables et valident à l'instanciation | `Money`, `Email`, `Address`, etc. | Conforme |
| CA-02-04 | Le mapping EF Core est externalisé dans des classes `IEntityTypeConfiguration` | `UserConfiguration`, `AccountConfiguration`, `TransactionConfiguration` | Conforme |
| CA-02-05 | Les agrégats exposent des méthodes de domaine (pas de setters directs) | `Account.Debit()`, `User.ConfirmEmail()`, etc. | Conforme |
| CA-02-06 | Le dispatch des domain events est dans `SaveEntitiesAsync()` (Infrastructure), pas dans les handlers | `UserDbContext.SaveEntitiesAsync()` | Conforme |

### Exemple de conformité

```csharp
// CORRECT : Méthode de fabrique statique + constructeur privé
public class Account : Entity, IAggregateRoot
{
    private Account() { } // Pour EF Core uniquement

    public static Account Open(Guid userId, AccountType type, Currency currency)
    {
        var account = new Account(Guid.NewGuid(), userId, type, Money.Of(100, currency));
        account.AddDomainEvent(new AccountOpenedDomainEvent(account));
        return account;
    }
}

// CORRECT : Value Object immuable avec validation
public class Money : ValueObject
{
    public decimal Amount { get; }  // Pas de setter
    public Currency Currency { get; }

    public Money(decimal amount, Currency currency)
    {
        if (amount < 0) throw new DomainValidationException(...);
        Amount = Math.Round(amount, 2, MidpointRounding.AwayFromZero);
    }
}
```

---

## CA-03 : Routes Cohérentes et Conventions REST

### Description

Toutes les routes API doivent suivre des conventions REST cohérentes. Les routes publiques, protégées et internes doivent être clairement distinguées et documentées.

### Critères de vérification

| ID | Critère | Vérification | Statut |
|----|---------|-------------|--------|
| CA-03-01 | Routes préfixées par `/api/{service}` | nginx.conf + contrôleurs | Conforme |
| CA-03-02 | Méthodes HTTP sémantiques (POST pour création, GET pour lecture) | Contrôleurs | Conforme |
| CA-03-03 | Endpoints publics marqués `[PublicEndpoint]` | `POST /register`, `POST /login`, `GET /health` | Conforme |
| CA-03-04 | Endpoints protégés marqués `[LoginEndpoint(JwtPurposes.Login)]` | `POST /api/accounts`, `GET /api/accounts`, `POST /api/transactions` | Conforme |
| CA-03-05 | Endpoints internes marqués `[InternalEndpoint]` | `POST /api/accounts/transfer`, `POST /api/accounts/verify-ownership` | Conforme |
| CA-03-06 | Réponses de création retournent HTTP 201 | `CreatedAtAction(...)` | Conforme |
| CA-03-07 | Enums sérialisés en string dans l'API | `JsonStringEnumConverter` dans tous les services | Conforme |

### Tableau des routes publiques

| Méthode | Route | Auth | Description |
|---------|-------|------|-------------|
| `POST` | `/api/users/register` | Public | Inscription client |
| `POST` | `/api/users/login` | Public | Authentification |
| `GET` | `/api/users/health` | Public | Health check Users |
| `POST` | `/api/users/setup-interac` | JWT Login | Configuration compte Interac |
| `GET` | `/api/accounts` | JWT Login | Liste des comptes |
| `POST` | `/api/accounts` | JWT Login | Ouverture de compte |
| `GET` | `/api/accounts/health` | Public | Health check Accounts |
| `POST` | `/api/transactions` | JWT Login | Initiation d'une transaction |
| `GET` | `/api/transactions/health` | Public | Health check Transactions |

### Tableau des routes internes (inter-services)

| Méthode | Route | Header requis | Description |
|---------|-------|--------------|-------------|
| `POST` | `/api/accounts/transfer` | `X-Internal-Secret` | Transfert de fonds |
| `POST` | `/api/accounts/verify-ownership` | `X-Internal-Secret` | Vérification de propriété |
| `POST` | `/api/users/verify-ownership` | `X-Internal-Secret` | Récupération compte Interac |

---

## CA-04 : Swagger/OpenAPI Publié

### Description

Chaque service doit exposer une documentation Swagger/OpenAPI accessible, permettant aux développeurs et aux intégrateurs de découvrir et tester l'API.

### Critères de vérification

| ID | Critère | Vérification | Statut |
|----|---------|-------------|--------|
| CA-04-01 | `AddSwaggerGen()` configuré dans chaque service | `Program.cs` de chaque service | Conforme |
| CA-04-02 | `AddEndpointsApiExplorer()` activé | `Program.cs` de chaque service | Conforme |
| CA-04-03 | Contrôleurs annotés `[Produces("application/json")]` | Tous les contrôleurs | Conforme |
| CA-04-04 | Enums lisibles (string) dans la documentation | `JsonStringEnumConverter` | Conforme |
| CA-04-05 | Swagger UI accessible en développement | `UseSwagger()` / `UseSwaggerUI()` (à activer selon l'environnement) | Non configuré explicitement en Production |

---

## CA-05 : Contraintes de Sécurité

| ID | Critère | Vérification | Statut |
|----|---------|-------------|--------|
| CA-05-01 | Mots de passe hachés avec BCrypt | `BcryptPasswordHasher` | Conforme |
| CA-05-02 | SIN chiffré avec AES-256 | `AES256Encryptor` | Conforme |
| CA-05-03 | JWT signé HS256 avec secret configurable | `JwtService` | Conforme |
| CA-05-04 | Comparaison du secret interne en temps constant | `CryptographicOperations.FixedTimeEquals` | Conforme |
| CA-05-05 | Erreurs 500 ne divulguent pas les détails techniques | `GlobalExceptionHandler` | Conforme |
| CA-05-06 | Authentification par objet de valeur `purpose` dans le JWT | Claim `purpose` + `EndpointAuthMiddleware` | Conforme |
| CA-05-07 | Variables sensibles via variables d'environnement | `docker-compose.yml` + `.env` | Conforme |

---

## CA-06 : Contraintes de Disponibilité et Déploiement

| ID | Critère | Vérification | Statut |
|----|---------|-------------|--------|
| CA-06-01 | Health check `/health` sur chaque service | `MapHealthChecks("/health")` | Conforme |
| CA-06-02 | Gateway ne démarre qu'après services healthy | `condition: service_healthy` | Conforme |
| CA-06-03 | Redémarrage automatique des conteneurs | `restart: unless-stopped` | Conforme |
| CA-06-04 | Migrations DB appliquées au démarrage | `db.Database.MigrateAsync()` | Conforme |
| CA-06-05 | Réseau interne isolé (services non exposés directement) | `networks: internal (internal: true)` | Conforme |
| CA-06-06 | Base de données persistante par service | Volumes Docker nommés | Conforme |

---

## Résumé de conformité

| Critère d'acceptation | Conformité |
|-----------------------|-----------|
| CA-01 : Dépendances dirigées | Conforme |
| CA-02 : Séparation Domain ↔ Infrastructure | Conforme |
| CA-03 : Routes cohérentes | Conforme |
| CA-04 : Swagger publié | Partiel (activer en Development) |
| CA-05 : Contraintes de sécurité | Conforme |
| CA-06 : Disponibilité et déploiement | Conforme |
