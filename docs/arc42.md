# Arc42 — Documentation d'Architecture CanBankX

---

## 1. Introduction et Objectifs

### 1.1 Présentation du système

CanBankX est une plateforme bancaire commerciale canadienne destinée aux particuliers et aux PME. Elle permet la gestion complète des comptes bancaires (chèques et épargne), des paiements, des transferts, du crédit simplifié, et de la conformité réglementaire (FINTRAC, OSFI, LPRPDE).

### 1.2 Objectifs de qualité

| Priorité | Objectif | Scénario de mesure |
|----------|----------|--------------------|
| 1 | Disponibilité ≥ 95 % (microservices) | Monitoring uptime, health checks automatiques |
| 2 | Latence P95 ≤ 500 ms par opération | Tests de charge, métriques APM |
| 3 | Débit ≥ 600 ops/s | Tests de stress |
| 4 | Idempotence des paiements | Tests de rejeu, contrainte unique `idempotency_key` en DB |
| 5 | Auditabilité complète | Journaux immuables, domain events |
| 6 | Conformité réglementaire (KYC/AML) | Processus de vérification d'identité obligatoire |

### 1.3 Parties prenantes

| Partie prenante | Attentes |
|-----------------|----------|
| Clients (particuliers, PME) | Interface numérique fiable, transactions sécurisées |
| Back-Office bancaire | Compensation, rapprochement, clôture journalière |
| Conformité / Risque | Contrôles AML, KYC, surveillance des transactions |
| Régulateurs simulés (FINTRAC, OSFI) | Rapports réglementaires, traçabilité |
| Équipe de développement | Code maintenable, patterns DDD clairs |

---

## 2. Contraintes

### 2.1 Contraintes techniques

| Contrainte | Description |
|------------|-------------|
| Langage | C# / .NET 8 (conforme à la politique : Java, C#, Go, Rust, C++) |
| Base de données | PostgreSQL 16 (une instance dédiée par service) |
| Conteneurisation | Docker / Docker Compose obligatoire |
| API Gateway | nginx 1.27 avec Blazor WASM en frontend statique |
| Transport | HTTP interne uniquement (pas de message broker en phase 1) |

### 2.2 Contraintes organisationnelles

- Pas de dépendances directes entre projets de services (Users, Accounts, Transactions).
- La bibliothèque `Shared` est la seule dépendance compilée partagée.
- Chaque service possède sa propre base de données (Database per Service).

### 2.3 Contraintes réglementaires

- Chiffrement du NAS (SIN) au repos (AES-256).
- Hachage des mots de passe (BCrypt).
- Authentification MFA (TOTP / SMS OTP).
- Processus KYC obligatoire avant activation du compte.
- Journaux d'audit immuables (domain events).

---

## 3. Contexte et Périmètre

### 3.1 Contexte métier

```
┌─────────────────────────────────────────────────────────────┐
│                        CanBankX                             │
│                                                             │
│  ┌─────────┐  ┌──────────┐  ┌─────────────┐                 │
│  │  Users  │  │ Accounts │  │Transactions │                 │
│  └─────────┘  └──────────┘  └─────────────┘                 │
└─────────────────────────────────────────────────────────────┘
       │                │               │
       ▼                ▼               ▼
 Clients Web/Mobile  KYC/AML       Interac (simulé)
                     Régulateurs   Paiements (simulés)
```

### 3.2 Interfaces externes

| Interface | Direction | Protocole | Description |
|-----------|-----------|-----------|-------------|
| Client Web/Mobile | Entrante | HTTPS (nginx) | API REST JSON |
| KYC Provider (stub) | Sortante | HTTP (simulé) | Vérification d'identité |
| Interac (simulé) | Sortante | HTTP (simulé) | Transferts e-Transfer |
| FINTRAC/OSFI (simulé) | Sortante | — | Rapports réglementaires |

---

## 4. Solution Conceptuelle

### 4.1 Décision architecturale principale

CanBankX adopte une **architecture microservices** avec :
- **Domain-Driven Design (DDD)** : Bounded contexts, agrégats, value objects, domain events.
- **CQRS avec MediatR** : Séparation stricte des commandes et des requêtes au sein de chaque service.
- **Architecture en couches** : Domain → Application → Infrastructure (dépendances dirigées vers l'intérieur).
- **Database per Service** : Isolation des données, PostgreSQL dédié par service.
- **Authentification partagée** : JWT (HS256) avec claims de `purpose`, middleware commun dans `Shared`.

### 4.2 Principes directeurs

1. **Séparation Domain ↔ Infrastructure** : Le domaine (entités, value objects, events, exceptions) n'a aucune dépendance sur les frameworks d'infrastructure (EF Core, ASP.NET Core).
2. **Dépendances dirigées vers l'intérieur** : Infrastructure dépend d'Application, Application dépend de Domain, Domain ne dépend de rien.
3. **Communication inter-services par HTTP** : Appels synchrones avec header d'authentification interne (`X-Internal-Secret`). Pas de couplage direct.
4. **Idempotence garantie** : Contrainte d'unicité sur `idempotency_key` en base de données et vérification applicative préalable.
5. **Domain Events** : Les effets de bord (appels inter-services, notifications) sont déclenchés par des domain events dispatché via MediatR après persistance.

### 4.3 Catalogue des patterns utilisés

| Pattern | Localisation | Justification |
|---------|-------------|---------------|
| Aggregate Root | `User`, `Account`, `Transaction` | Cohérence transactionnelle |
| Value Object | `Money`, `Email`, `Address`, etc. | Immuabilité, validation intrinsèque |
| Repository | `IUserRepository`, `IAccountRepository`, `ITransactionRepository` | Abstraction persistance |
| Unit of Work | `IUnitOfWork` (implémenté par DbContext) | Transaction DB atomique + dispatch events |
| CQRS | Commands / Queries via MediatR | Séparation lecture/écriture |
| Pipeline Behavior | `ValidationBehavior<TRequest, TResponse>` | Validation transversale |
| Domain Events | `IDomainEvent` + MediatR `INotificationHandler` | Découplage effets de bord |
| Global Exception Handler | `GlobalExceptionHandler` (IExceptionHandler) | Centralisation des erreurs HTTP |

---

## 5. Vues de Construction (Building Blocks)

### 5.1 Niveau 1 — Vue système

Voir Vue 4+1 Section 1 (Vue Logique) et Section 4 (Vue Déploiement).

### 5.2 Niveau 2 — Vue Service Users (référence)

Le service Users implémente la structure complète et sert de référence pour les autres services.

#### Couche Domain

- **`User` (Agrégat)** : Cycle de vie complet (PendingEmailVerification → PendingKyc → KycInProgress → Active). Expose des méthodes de domaine : `Register()`, `ConfirmEmail()`, `StartKyc()`, `ApproveKyc()`, `RejectKyc()`, `EnableTwoFactor()`, `SetInteracDepositAccount()`.
- **Value Objects** : `Email` (format + normalisation), `HashedPassword` (non vide), `FullName`, `Address` (province canadienne + code postal), `EncryptedSin` (contient le SIN chiffré), `PhoneNumber` (format E.164).
- **Interfaces de services** : `IPasswordHasher`, `IJwtService`, `ISinEncryptor`, `ITwoFactorService`, `IKycService`.

#### Couche Application

- **Commands** : `RegisterUserCommand`, `LoginUserCommand`, `SetupTwoFactorCommand`, `SetupInteracDepositCommand` — chacun avec Handler correspondant.
- **Validators** : `RegisterUserCommandValidator` (FluentValidation) injectée via `ValidationBehavior`.
- **Controller** : `UsersController` — routes publiques et protégées par JWT.

#### Couche Infrastructure

- **`UserDbContext`** : Implémente `IUnitOfWork`, dispatch les domain events avant `SaveChangesAsync`.
- **`UserConfiguration`** : Mapping EF Core (owned types pour Value Objects).
- **Services concrets** : `JwtService` (HS256), `BcryptPasswordHasher`, `AES256Encryptor`, `TwoFactorService` (stub SMS/TOTP), `KycProviderService` (stub).

---

## 6. Vues Runtime (Scénarios Dynamiques)

Voir Vue 4+1 Section 2 (Vue Processus) et Section 5 (Scénarios).

---

## 7. Concepts Transversaux

### 7.1 Sécurité

#### Authentification
- JWT HS256 avec claim `purpose` (`login` ou `verify-email`).
- `EndpointAuthMiddleware` intercepte toutes les requêtes et applique la politique selon les attributs (`[PublicEndpoint]`, `[LoginEndpoint]`, `[InternalEndpoint]`).
- Appels inter-services protégés par header partagé (`X-Internal-Secret`) avec comparaison en temps constant (`CryptographicOperations.FixedTimeEquals`).

#### Données sensibles
- Mots de passe : BCrypt (coût par défaut).
- NAS (SIN) : AES-256 CBC avec clé/IV configurés par variables d'environnement.
- JWT Secret : variable d'environnement obligatoire (valeur manquante = échec au démarrage).

### 7.2 Observabilité

- **Logs structurés** : `ILogger<T>` ASP.NET Core intégré. Chaque contrôleur et handler logue les opérations importantes (inscription, connexion, transfert, etc.).
- **Health Checks** : Endpoint `/health` (PublicEndpoint) avec vérification de la connexion PostgreSQL. Utilisé par Docker Compose pour les `depends_on: condition: service_healthy`.
- **Swagger/OpenAPI** : Activé via `AddSwaggerGen()` / `AddEndpointsApiExplorer()` dans chaque service.

### 7.3 Gestion des erreurs

Chaque service implémente un `GlobalExceptionHandler` (`IExceptionHandler`) qui :
- Mappe les exceptions de domaine vers des codes HTTP sémantiques (400, 401, 403, 404, 409, 422, 500).
- Retourne des `ProblemDetails` (RFC 7807) structurés.
- Masque les détails techniques pour les erreurs 500.
- Gère les erreurs de validation FluentValidation (`ValidationProblemDetails`).

### 7.4 Persistance

- **EF Core 9** avec migrations versionnées.
- Pattern Owned Entity pour les Value Objects (colonnes aplaties dans la même table).
- Les migrations sont appliquées au démarrage (`db.Database.MigrateAsync()`).
- Un seed de données est exécuté au démarrage du service Users (`UserDbContextSeed`).

### 7.5 Idempotence

- **Transactions** : Index unique sur `idempotency_key` en base + vérification applicative préalable dans `InitiateTransactionHandler`.
- **Comportement** : Si la transaction existe déjà, retour de la transaction existante avec flag `AlreadyExists=true` (HTTP 201).

---

## 8. Décisions Architecturales, Qualité et Risques

### 8.1 Décisions architecturales majeures

Voir le fichier `ADRs.md` pour les ADRs détaillées.

### 8.2 Objectifs de qualité et scénarios

| ID | Scénario | Stimulus | Réponse attendue | Mesure |
|----|----------|----------|-----------------|--------|
| Q1 | Disponibilité | Un service crashe | Redémarrage automatique (`restart: unless-stopped`) | < 30s recovery |
| Q2 | Performance | 600 requêtes simultanées | Latence P95 ≤ 500 ms | Tests de charge k6 |
| Q3 | Sécurité | Tentative d'accès sans JWT | 401 Unauthorized | Test d'intrusion |
| Q4 | Sécurité | Appel inter-services sans secret | 403 Forbidden | Test d'intrusion |
| Q5 | Intégrité | Rejeu d'un virement | Idempotence garantie, pas de double débit | Test de rejeu |
| Q6 | Auditabilité | Audit d'une transaction | Domain events persistés, logs structurés | Revue d'audit |

### 8.3 Risques techniques identifiés

| Risque | Probabilité | Impact | Mitigation |
|--------|-------------|--------|------------|
| Perte de cohérence inter-services (Transactions → Accounts) | Moyen | Élevé | Saga pattern (phase 2), logs de compensation |
| Absence de message broker (couplage synchrone) | Faible | Moyen | Architecture event-driven planifiée en phase 2 |
| Clé AES-256 statique (IV fixe) | Moyen | Élevé | Migrer vers IV aléatoire par entrée + HSM (phase 2) |
| JWT sans expiration courte sur tokens de production | Moyen | Élevé | Réduire TTL, implémenter refresh tokens |
| Stub KYC/2FA non opérationnel en production | Élevé | Élevé | Intégrer providers réels avant déploiement |
| Absence de rate limiting | Moyen | Moyen | Ajouter rate limiting nginx / middleware ASP.NET |
